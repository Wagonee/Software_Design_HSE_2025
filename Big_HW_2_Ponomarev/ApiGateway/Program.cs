using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var reverseProxyConfig = builder.Configuration.GetSection("ReverseProxy");
builder.Services.AddReverseProxy()
    .LoadFromConfig(reverseProxyConfig);

builder.Services.AddHttpClient("swagger_downloader")
    .AddStandardResilienceHandler();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("aggregated-v1", new OpenApiInfo
    {
        Title = "API Gateway (Aggregated Services)",
        Version = "v1",
        Description = "Aggregated API documentation from FileStoringService and FileAnalysisService, accessed via API Gateway."
    });
    options.DocInclusionPredicate((docName, apiDesc) => false);
});

var app = builder.Build();

app.MapReverseProxy();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(opt => {});
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/custom/aggregated-v1/swagger.json", "Aggregated API V1");
        options.RoutePrefix = "swagger";
    });

    app.MapGet("/swagger/custom/aggregated-v1/swagger.json", async (
        IConfiguration config,
        IHttpClientFactory httpClientFactory,
        ILoggerFactory loggerFactory) =>
    {
        var logger = loggerFactory.CreateLogger("SwaggerAggregatorEndpoint");
        var combinedOpenApiDoc = new OpenApiDocument
        {
            Info = new OpenApiInfo { Title = "Aggregated API via Gateway", Version = "v1" },
            Paths = new OpenApiPaths(),
            Components = new OpenApiComponents { Schemas = new Dictionary<string, OpenApiSchema>() },
            Servers = new List<OpenApiServer> { new OpenApiServer { Url = "/" } }
        };

        var swaggerEndpoints = config.GetSection("SwaggerEndpoints").Get<List<SwaggerEndpointConfig>>();
        var httpClient = httpClientFactory.CreateClient("swagger_downloader");

        if (swaggerEndpoints == null || !swaggerEndpoints.Any())
        {
            logger.LogWarning("No Swagger endpoints configured in appsettings.json 'SwaggerEndpoints' section.");
            return Results.NotFound("No backend Swagger endpoints configured.");
        }

        foreach (var endpoint in swaggerEndpoints)
        {
            if (string.IsNullOrEmpty(endpoint.Url) || string.IsNullOrEmpty(endpoint.Key) ||
                string.IsNullOrEmpty(endpoint.GatewayPathPrefix) || string.IsNullOrEmpty(endpoint.ServicePathPrefixToReplace))
            {
                logger.LogWarning("Invalid Swagger endpoint configuration (missing Url, Key, GatewayPathPrefix, or ServicePathPrefixToReplace): {@EndpointConfig}", endpoint);
                continue;
            }

            try
            {
                logger.LogInformation("Fetching Swagger from {ServiceKey} at {ServiceUrl}", endpoint.Key, endpoint.Url);
                var response = await httpClient.GetAsync(endpoint.Url);
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    logger.LogWarning("Failed to fetch Swagger JSON from {ServiceUrl} for service {ServiceKey}. Status: {StatusCode}. Body: {ErrorBody}",
                        endpoint.Url, endpoint.Key, response.StatusCode, errorBody);
                    continue;
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var reader = new OpenApiStreamReader();
                var diagnostic = new OpenApiDiagnostic();
                var serviceDoc = reader.Read(stream, out diagnostic);

                if (diagnostic.Errors.Any())
                {
                    logger.LogWarning("Errors parsing Swagger JSON from {ServiceUrl} for service {ServiceKey}: {Errors}",
                        endpoint.Url, endpoint.Key, string.Join("; ", diagnostic.Errors.Select(e => $"{e.Message} ({e.Pointer})")));
                    continue;
                }

                if (serviceDoc != null)
                {
                    var schemaMapping = new Dictionary<string, string>();
                    if (serviceDoc.Components?.Schemas != null)
                    {
                        foreach (var schemaEntry in serviceDoc.Components.Schemas)
                        {
                            var originalSchemaName = schemaEntry.Key;
                            var newSchemaName = $"{endpoint.Key}_{originalSchemaName}";
                            if (!combinedOpenApiDoc.Components.Schemas.ContainsKey(newSchemaName))
                            {
                                combinedOpenApiDoc.Components.Schemas.Add(newSchemaName, schemaEntry.Value);
                                schemaMapping[originalSchemaName] = newSchemaName;
                            }
                            else
                            {
                                logger.LogWarning("Schema name conflict after prefixing: {NewSchemaName} for service {ServiceKey}", newSchemaName, endpoint.Key);
                            }
                        }
                    }

                    if (serviceDoc.Paths != null)
                    {
                        foreach (var pathEntry in serviceDoc.Paths)
                        {
                            var originalPathKey = pathEntry.Key; 
                            var transformedPathItem = pathEntry.Value;

                            string finalGatewayPath;
                            if (originalPathKey.StartsWith(endpoint.ServicePathPrefixToReplace))
                            {
                                var relativePath = originalPathKey.Substring(endpoint.ServicePathPrefixToReplace.Length);
                                finalGatewayPath = $"{endpoint.GatewayPathPrefix.TrimEnd('/')}{relativePath}";
                            }
                            else
                            {
                                finalGatewayPath = $"{endpoint.GatewayPathPrefix.TrimEnd('/')}{originalPathKey}";
                                logger.LogWarning("Path {OriginalPathKey} from service {ServiceKey} did not start with configured ServicePathPrefixToReplace '{ServicePathPrefix}'. Resulting gateway path: {FinalGatewayPath}",
                                    originalPathKey, endpoint.Key, endpoint.ServicePathPrefixToReplace, finalGatewayPath);
                            }

                            foreach (var operation in transformedPathItem.Operations.Values)
                            {
                                if (operation.Parameters != null)
                                {
                                    foreach (var parameter in operation.Parameters)
                                    {
                                        if (parameter.Schema?.Reference?.Id != null && schemaMapping.TryGetValue(parameter.Schema.Reference.Id, out var newRefNameParam))
                                        {
                                            parameter.Schema.Reference = new OpenApiReference { Id = newRefNameParam, Type = ReferenceType.Schema };
                                        }
                                    }
                                }
                                if (operation.RequestBody?.Content != null)
                                {
                                    foreach (var content in operation.RequestBody.Content.Values)
                                    {
                                        if (content.Schema?.Reference?.Id != null && schemaMapping.TryGetValue(content.Schema.Reference.Id, out var newRefNameReq))
                                        {
                                            content.Schema.Reference = new OpenApiReference { Id = newRefNameReq, Type = ReferenceType.Schema };
                                        }
                                    }
                                }
                                if (operation.Responses != null)
                                {
                                    foreach (var responseValue in operation.Responses.Values)
                                    {
                                        if (responseValue?.Content != null)
                                        {
                                            foreach (var content in responseValue.Content.Values)
                                            {
                                                if (content.Schema?.Reference?.Id != null && schemaMapping.TryGetValue(content.Schema.Reference.Id, out var newRefNameResp))
                                                {
                                                    content.Schema.Reference = new OpenApiReference { Id = newRefNameResp, Type = ReferenceType.Schema };
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (!combinedOpenApiDoc.Paths.ContainsKey(finalGatewayPath))
                            {
                                combinedOpenApiDoc.Paths.Add(finalGatewayPath, transformedPathItem);
                                logger.LogDebug("Added path to aggregated doc: {FinalGatewayPath} (from original {OriginalPathKey} of service {ServiceKey})", finalGatewayPath, originalPathKey, endpoint.Key);
                            }
                            else
                            {
                                logger.LogWarning("Path conflict or duplicate in aggregated doc: {FinalGatewayPath} from {ServiceKey}", finalGatewayPath, endpoint.Key);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing Swagger for service {ServiceKey} from {ServiceUrl}", endpoint.Key, endpoint.Url);
            }
        }

        var outputStream = new MemoryStream();
        var streamWriter = new StreamWriter(outputStream, new UTF8Encoding(false)); 
        var writer = new Microsoft.OpenApi.Writers.OpenApiJsonWriter(streamWriter);
        combinedOpenApiDoc.SerializeAsV3(writer);
        streamWriter.Flush();
        outputStream.Position = 0;
        var resultJson = await new StreamReader(outputStream).ReadToEndAsync();
        return Results.Content(resultJson, "application/json; charset=utf-8");

    }).WithName("GetAggregatedSwaggerJson").Produces<string>();
}

app.Run();

public class SwaggerEndpointConfig
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string GatewayPathPrefix { get; set; } = string.Empty; 
    public string ServicePathPrefixToReplace { get; set; } = string.Empty;  
}
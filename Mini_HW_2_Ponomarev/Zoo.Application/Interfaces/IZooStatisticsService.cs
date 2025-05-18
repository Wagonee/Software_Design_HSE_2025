using Zoo.Application.DTOs;

namespace Zoo.Application.Interfaces;

public interface IZooStatisticsService
{
    Task<ZooStatisticsDto> GetStatisticsAsync();
}
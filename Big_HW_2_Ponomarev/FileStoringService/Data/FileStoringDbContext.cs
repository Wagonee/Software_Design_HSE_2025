using FileStoringService.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStoringService.Data
{
    public class FileStoringDbContext : DbContext
    {
        public FileStoringDbContext(DbContextOptions<FileStoringDbContext> options)
            : base(options)
        {
        }

        public DbSet<StoredFile> StoredFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StoredFile>()
                .HasIndex(sf => sf.Hash)
                .IsUnique();
        }
    }
}
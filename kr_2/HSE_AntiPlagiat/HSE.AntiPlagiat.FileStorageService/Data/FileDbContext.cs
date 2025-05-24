using Microsoft.EntityFrameworkCore;
/// <summary>
/// Контекст базы данных для хранения информации о файлах.
/// </summary>
public class FileDbContext : DbContext
{
    public FileDbContext(DbContextOptions<FileDbContext> options) : base(options) { }

    public DbSet<FileInfo> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FileInfo>()
            .HasIndex(f => f.Hash);
    }
}
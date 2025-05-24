using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
/// <summary>
/// Контекст базы данных для анализа текстов и результатов сравнения.
/// </summary>
public class AnalysisDbContext : DbContext
{
    public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : base(options) { }

    public DbSet<TextAnalysisResult> AnalysisResults { get; set; }
    public DbSet<SimilarityResult> SimilarityResults { get; set; }
}
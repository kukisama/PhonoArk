using Microsoft.EntityFrameworkCore;

namespace PhonoArk.Data;

public class AppDbContext : DbContext
{
    public DbSet<Models.FavoritePhoneme> FavoritePhonemes { get; set; }
    public DbSet<Models.ExamResult> ExamResults { get; set; }
    public DbSet<Models.AppSettings> Settings { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Models.FavoritePhoneme>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PhonemeSymbol).IsRequired();
            entity.Property(e => e.GroupName).HasDefaultValue("Default");
        });

        modelBuilder.Entity<Models.ExamResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExamScope).HasDefaultValue("All");
        });

        modelBuilder.Entity<Models.AppSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}

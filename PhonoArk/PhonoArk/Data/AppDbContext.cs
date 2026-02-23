using Microsoft.EntityFrameworkCore;

namespace PhonoArk.Data;

public class AppDbContext : DbContext
{
    public DbSet<Models.FavoritePhoneme> FavoritePhonemes { get; set; }
    public DbSet<Models.ExamResult> ExamResults { get; set; }
    public DbSet<Models.ExamQuestionAttempt> ExamQuestionAttempts { get; set; }
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

        modelBuilder.Entity<Models.ExamQuestionAttempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PhonemeSymbol).IsRequired();
            entity.Property(e => e.CorrectWord).IsRequired();
            entity.Property(e => e.CorrectIpa).IsRequired();
            entity.Property(e => e.UserWord).IsRequired();
            entity.Property(e => e.UserIpa).IsRequired();
            entity.HasIndex(e => e.ExamResultId);
            entity.HasIndex(e => e.IsCorrect);
            entity.HasIndex(e => e.PhonemeSymbol);
        });

        modelBuilder.Entity<Models.AppSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}

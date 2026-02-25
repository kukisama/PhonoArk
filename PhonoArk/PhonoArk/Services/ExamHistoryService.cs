using Microsoft.EntityFrameworkCore;
using PhonoArk.Data;
using PhonoArk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhonoArk.Services;

public class ExamHistoryService
{
    public event Action? HistoryChanged;

    public sealed class PhonemeErrorStat
    {
        public string PhonemeSymbol { get; init; } = string.Empty;
        public int ErrorCount { get; init; }
    }

    private readonly DbContextOptions<AppDbContext> _dbOptions;

    public ExamHistoryService(DbContextOptions<AppDbContext> dbOptions)
    {
        _dbOptions = dbOptions;
        using var context = CreateContext();
        EnsureQuestionAttemptsTable(context);
    }

    private AppDbContext CreateContext() => new AppDbContext(_dbOptions);

    public async Task<List<ExamResult>> GetAllResultsAsync()
    {
        using var context = CreateContext();
        return await context.ExamResults
            .OrderByDescending(e => e.ExamDate)
            .ToListAsync();
    }

    public async Task<ExamResult?> GetResultByIdAsync(int id)
    {
        using var context = CreateContext();
        return await context.ExamResults.FindAsync(id);
    }

    public async Task SaveResultAsync(ExamResult result)
    {
        using var context = CreateContext();
        context.ExamResults.Add(result);
        await context.SaveChangesAsync();
        HistoryChanged?.Invoke();
    }

    public async Task SaveResultWithAttemptsAsync(ExamResult result, IReadOnlyList<ExamQuestion> questions)
    {
        using var context = CreateContext();
        context.ExamResults.Add(result);
        await context.SaveChangesAsync();

        var attempts = questions
            .Select((question, index) => new ExamQuestionAttempt
            {
                ExamResultId = result.Id,
                QuestionOrder = index + 1,
                ExamDate = result.ExamDate,
                PhonemeSymbol = question.Phoneme.Symbol,
                CorrectWord = question.CorrectAnswer.Word,
                CorrectIpa = question.CorrectAnswer.IpaTranscription,
                UserWord = question.UserAnswer?.Word ?? string.Empty,
                UserIpa = question.UserAnswer?.IpaTranscription ?? string.Empty,
                IsCorrect = question.IsCorrect
            })
            .ToList();

        if (attempts.Count > 0)
        {
            context.ExamQuestionAttempts.AddRange(attempts);
            await context.SaveChangesAsync();
        }

        HistoryChanged?.Invoke();
    }

    public async Task DeleteResultAsync(int id)
    {
        using var context = CreateContext();
        var result = await context.ExamResults.FindAsync(id);
        if (result != null)
        {
            context.ExamResults.Remove(result);
            await context.SaveChangesAsync();
            HistoryChanged?.Invoke();
        }
    }

    public async Task ClearAllResultsAsync()
    {
        using var context = CreateContext();
        context.ExamQuestionAttempts.RemoveRange(context.ExamQuestionAttempts);
        context.ExamResults.RemoveRange(context.ExamResults);
        await context.SaveChangesAsync();
        HistoryChanged?.Invoke();
    }

    public async Task<double> GetAverageScoreAsync()
    {
        using var context = CreateContext();
        var results = await context.ExamResults.ToListAsync();
        return results.Count > 0 ? results.Average(r => r.Score) : 0;
    }

    public async Task<List<ExamQuestionAttempt>> GetAllQuestionAttemptsAsync()
    {
        using var context = CreateContext();
        return await context.ExamQuestionAttempts
            .OrderByDescending(a => a.ExamDate)
            .ThenBy(a => a.QuestionOrder)
            .ToListAsync();
    }

    public async Task<List<ExamQuestionAttempt>> GetAllQuestionAttemptsSummaryAsync()
    {
        using var context = CreateContext();
        return await context.ExamQuestionAttempts
            .OrderByDescending(a => a.ExamDate)
            .ThenBy(a => a.QuestionOrder)
            .Select(a => new ExamQuestionAttempt
            {
                Id = a.Id,
                ExamResultId = a.ExamResultId,
                QuestionOrder = a.QuestionOrder,
                ExamDate = a.ExamDate,
                PhonemeSymbol = a.PhonemeSymbol,
                IsCorrect = a.IsCorrect
            })
            .ToListAsync();
    }

    public async Task<List<int>> GetExamResultIdPageAsync(int skip, int take)
    {
        using var context = CreateContext();
        return await context.ExamResults
            .OrderByDescending(r => r.Id)
            .ThenByDescending(r => r.ExamDate)
            .Skip(skip)
            .Take(take)
            .Select(r => r.Id)
            .ToListAsync();
    }

    public async Task<List<ExamQuestionAttempt>> GetQuestionAttemptsSummaryByResultIdsAsync(
        IReadOnlyCollection<int> examResultIds,
        bool wrongOnly)
    {
        if (examResultIds.Count == 0)
        {
            return new List<ExamQuestionAttempt>();
        }

        using var context = CreateContext();
        var query = context.ExamQuestionAttempts
            .Where(a => examResultIds.Contains(a.ExamResultId));

        if (wrongOnly)
        {
            query = query.Where(a => !a.IsCorrect);
        }

        return await query
            .OrderByDescending(a => a.ExamDate)
            .ThenBy(a => a.QuestionOrder)
            .Select(a => new ExamQuestionAttempt
            {
                Id = a.Id,
                ExamResultId = a.ExamResultId,
                QuestionOrder = a.QuestionOrder,
                ExamDate = a.ExamDate,
                PhonemeSymbol = a.PhonemeSymbol,
                IsCorrect = a.IsCorrect
            })
            .ToListAsync();
    }

    public async Task<(int TotalAttempts, int WrongAttempts)> GetAttemptMetricsAsync()
    {
        using var context = CreateContext();
        var total = await context.ExamQuestionAttempts.CountAsync();
        var wrong = await context.ExamQuestionAttempts.CountAsync(a => !a.IsCorrect);
        return (total, wrong);
    }

    public async Task<ExamQuestionAttempt?> GetQuestionAttemptDetailByIdAsync(int id)
    {
        using var context = CreateContext();
        return await context.ExamQuestionAttempts
            .Where(a => a.Id == id)
            .Select(a => new ExamQuestionAttempt
            {
                Id = a.Id,
                ExamResultId = a.ExamResultId,
                QuestionOrder = a.QuestionOrder,
                ExamDate = a.ExamDate,
                PhonemeSymbol = a.PhonemeSymbol,
                CorrectWord = a.CorrectWord,
                CorrectIpa = a.CorrectIpa,
                UserWord = a.UserWord,
                UserIpa = a.UserIpa,
                IsCorrect = a.IsCorrect
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<PhonemeErrorStat>> GetPhonemeErrorStatsAsync()
    {
        using var context = CreateContext();
        return await context.ExamQuestionAttempts
            .Where(a => !a.IsCorrect)
            .GroupBy(a => a.PhonemeSymbol)
            .Select(group => new PhonemeErrorStat
            {
                PhonemeSymbol = group.Key,
                ErrorCount = group.Count()
            })
            .OrderByDescending(s => s.ErrorCount)
            .ThenBy(s => s.PhonemeSymbol)
            .ToListAsync();
    }

    private static void EnsureQuestionAttemptsTable(AppDbContext context)
    {
        context.Database.ExecuteSqlRaw(
            @"CREATE TABLE IF NOT EXISTS ExamQuestionAttempts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ExamResultId INTEGER NOT NULL,
                QuestionOrder INTEGER NOT NULL,
                ExamDate TEXT NOT NULL,
                PhonemeSymbol TEXT NOT NULL,
                CorrectWord TEXT NOT NULL,
                CorrectIpa TEXT NOT NULL,
                UserWord TEXT NOT NULL,
                UserIpa TEXT NOT NULL,
                IsCorrect INTEGER NOT NULL
            );");

        context.Database.ExecuteSqlRaw(
            "CREATE INDEX IF NOT EXISTS IX_ExamQuestionAttempts_ExamResultId ON ExamQuestionAttempts (ExamResultId);");
        context.Database.ExecuteSqlRaw(
            "CREATE INDEX IF NOT EXISTS IX_ExamQuestionAttempts_IsCorrect ON ExamQuestionAttempts (IsCorrect);");
        context.Database.ExecuteSqlRaw(
            "CREATE INDEX IF NOT EXISTS IX_ExamQuestionAttempts_PhonemeSymbol ON ExamQuestionAttempts (PhonemeSymbol);");
    }
}

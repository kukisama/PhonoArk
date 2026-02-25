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

    private readonly AppDbContext _context;

    public ExamHistoryService(AppDbContext context)
    {
        _context = context;
        EnsureQuestionAttemptsTable();
    }

    public async Task<List<ExamResult>> GetAllResultsAsync()
    {
        return await _context.ExamResults
            .OrderByDescending(e => e.ExamDate)
            .ToListAsync();
    }

    public async Task<ExamResult?> GetResultByIdAsync(int id)
    {
        return await _context.ExamResults.FindAsync(id);
    }

    public async Task SaveResultAsync(ExamResult result)
    {
        _context.ExamResults.Add(result);
        await _context.SaveChangesAsync();
        HistoryChanged?.Invoke();
    }

    public async Task SaveResultWithAttemptsAsync(ExamResult result, IReadOnlyList<ExamQuestion> questions)
    {
        _context.ExamResults.Add(result);
        await _context.SaveChangesAsync();

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
            _context.ExamQuestionAttempts.AddRange(attempts);
            await _context.SaveChangesAsync();
        }

        HistoryChanged?.Invoke();
    }

    public async Task DeleteResultAsync(int id)
    {
        var result = await _context.ExamResults.FindAsync(id);
        if (result != null)
        {
            _context.ExamResults.Remove(result);
            await _context.SaveChangesAsync();
            HistoryChanged?.Invoke();
        }
    }

    public async Task ClearAllResultsAsync()
    {
        _context.ExamQuestionAttempts.RemoveRange(_context.ExamQuestionAttempts);
        _context.ExamResults.RemoveRange(_context.ExamResults);
        await _context.SaveChangesAsync();
        HistoryChanged?.Invoke();
    }

    public async Task<double> GetAverageScoreAsync()
    {
        var results = await _context.ExamResults.ToListAsync();
        return results.Count > 0 ? results.Average(r => r.Score) : 0;
    }

    public async Task<List<ExamQuestionAttempt>> GetAllQuestionAttemptsAsync()
    {
        return await _context.ExamQuestionAttempts
            .OrderByDescending(a => a.ExamDate)
            .ThenBy(a => a.QuestionOrder)
            .ToListAsync();
    }

    public async Task<List<ExamQuestionAttempt>> GetAllQuestionAttemptsSummaryAsync()
    {
        return await _context.ExamQuestionAttempts
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
        return await _context.ExamResults
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

        var query = _context.ExamQuestionAttempts
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
        var total = await _context.ExamQuestionAttempts.CountAsync();
        var wrong = await _context.ExamQuestionAttempts.CountAsync(a => !a.IsCorrect);
        return (total, wrong);
    }

    public async Task<ExamQuestionAttempt?> GetQuestionAttemptDetailByIdAsync(int id)
    {
        return await _context.ExamQuestionAttempts
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
        return await _context.ExamQuestionAttempts
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

    private void EnsureQuestionAttemptsTable()
    {
        _context.Database.ExecuteSqlRaw(
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

        _context.Database.ExecuteSqlRaw(
            "CREATE INDEX IF NOT EXISTS IX_ExamQuestionAttempts_ExamResultId ON ExamQuestionAttempts (ExamResultId);");
        _context.Database.ExecuteSqlRaw(
            "CREATE INDEX IF NOT EXISTS IX_ExamQuestionAttempts_IsCorrect ON ExamQuestionAttempts (IsCorrect);");
        _context.Database.ExecuteSqlRaw(
            "CREATE INDEX IF NOT EXISTS IX_ExamQuestionAttempts_PhonemeSymbol ON ExamQuestionAttempts (PhonemeSymbol);");
    }
}

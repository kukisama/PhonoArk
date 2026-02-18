using Microsoft.EntityFrameworkCore;
using PhonoArk.Data;
using PhonoArk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhonoArk.Services;

public class ExamHistoryService
{
    private readonly AppDbContext _context;

    public ExamHistoryService(AppDbContext context)
    {
        _context = context;
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
    }

    public async Task DeleteResultAsync(int id)
    {
        var result = await _context.ExamResults.FindAsync(id);
        if (result != null)
        {
            _context.ExamResults.Remove(result);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearAllResultsAsync()
    {
        _context.ExamResults.RemoveRange(_context.ExamResults);
        await _context.SaveChangesAsync();
    }

    public async Task<double> GetAverageScoreAsync()
    {
        var results = await _context.ExamResults.ToListAsync();
        return results.Count > 0 ? results.Average(r => r.Score) : 0;
    }
}

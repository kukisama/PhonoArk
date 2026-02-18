using Microsoft.EntityFrameworkCore;
using PhonoArk.Data;
using PhonoArk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhonoArk.Services;

public class FavoriteService
{
    private readonly AppDbContext _context;

    public FavoriteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FavoritePhoneme>> GetAllFavoritesAsync()
    {
        return await _context.FavoritePhonemes.ToListAsync();
    }

    public async Task<List<FavoritePhoneme>> GetFavoritesByGroupAsync(string groupName)
    {
        return await _context.FavoritePhonemes
            .Where(f => f.GroupName == groupName)
            .ToListAsync();
    }

    public async Task<List<string>> GetAllGroupsAsync()
    {
        return await _context.FavoritePhonemes
            .Select(f => f.GroupName)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> IsFavoriteAsync(string phonemeSymbol)
    {
        return await _context.FavoritePhonemes
            .AnyAsync(f => f.PhonemeSymbol == phonemeSymbol);
    }

    public async Task AddFavoriteAsync(string phonemeSymbol, string groupName = "Default")
    {
        if (await IsFavoriteAsync(phonemeSymbol))
            return;

        var favorite = new FavoritePhoneme
        {
            PhonemeSymbol = phonemeSymbol,
            GroupName = groupName
        };

        _context.FavoritePhonemes.Add(favorite);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFavoriteAsync(string phonemeSymbol)
    {
        var favorites = await _context.FavoritePhonemes
            .Where(f => f.PhonemeSymbol == phonemeSymbol)
            .ToListAsync();

        _context.FavoritePhonemes.RemoveRange(favorites);
        await _context.SaveChangesAsync();
    }

    public async Task ClearAllFavoritesAsync()
    {
        _context.FavoritePhonemes.RemoveRange(_context.FavoritePhonemes);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateFavoriteGroupAsync(int favoriteId, string newGroupName)
    {
        var favorite = await _context.FavoritePhonemes.FindAsync(favoriteId);
        if (favorite != null)
        {
            favorite.GroupName = newGroupName;
            await _context.SaveChangesAsync();
        }
    }
}

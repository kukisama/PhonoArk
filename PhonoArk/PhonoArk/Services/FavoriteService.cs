using Microsoft.EntityFrameworkCore;
using PhonoArk.Data;
using PhonoArk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhonoArk.Services;

public class FavoriteService
{
    private readonly DbContextOptions<AppDbContext> _dbOptions;

    public FavoriteService(DbContextOptions<AppDbContext> dbOptions)
    {
        _dbOptions = dbOptions;
    }

    private AppDbContext CreateContext() => new AppDbContext(_dbOptions);

    public async Task<List<FavoritePhoneme>> GetAllFavoritesAsync()
    {
        using var context = CreateContext();
        return await context.FavoritePhonemes.ToListAsync();
    }

    public async Task<List<FavoritePhoneme>> GetFavoritesByGroupAsync(string groupName)
    {
        using var context = CreateContext();
        return await context.FavoritePhonemes
            .Where(f => f.GroupName == groupName)
            .ToListAsync();
    }

    public async Task<List<string>> GetAllGroupsAsync()
    {
        using var context = CreateContext();
        return await context.FavoritePhonemes
            .Select(f => f.GroupName)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> IsFavoriteAsync(string phonemeSymbol)
    {
        using var context = CreateContext();
        return await context.FavoritePhonemes
            .AnyAsync(f => f.PhonemeSymbol == phonemeSymbol);
    }

    public async Task AddFavoriteAsync(string phonemeSymbol, string groupName = "Default")
    {
        using var context = CreateContext();
        var exists = await context.FavoritePhonemes
            .AnyAsync(f => f.PhonemeSymbol == phonemeSymbol);
        if (exists)
            return;

        var favorite = new FavoritePhoneme
        {
            PhonemeSymbol = phonemeSymbol,
            GroupName = groupName
        };

        context.FavoritePhonemes.Add(favorite);
        await context.SaveChangesAsync();
    }

    public async Task RemoveFavoriteAsync(string phonemeSymbol)
    {
        using var context = CreateContext();
        var favorites = await context.FavoritePhonemes
            .Where(f => f.PhonemeSymbol == phonemeSymbol)
            .ToListAsync();

        context.FavoritePhonemes.RemoveRange(favorites);
        await context.SaveChangesAsync();
    }

    public async Task ClearAllFavoritesAsync()
    {
        using var context = CreateContext();
        context.FavoritePhonemes.RemoveRange(context.FavoritePhonemes);
        await context.SaveChangesAsync();
    }

    public async Task UpdateFavoriteGroupAsync(int favoriteId, string newGroupName)
    {
        using var context = CreateContext();
        var favorite = await context.FavoritePhonemes.FindAsync(favoriteId);
        if (favorite != null)
        {
            favorite.GroupName = newGroupName;
            await context.SaveChangesAsync();
        }
    }
}

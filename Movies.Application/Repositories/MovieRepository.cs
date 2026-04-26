using Microsoft.EntityFrameworkCore;
using Movies.Application.Contexts;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly MovieContext _context;

    public MovieRepository(MovieContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(Movie movie, IEnumerable<string> genres)
    {
        var movieEntity = await _context.Movies.AddAsync(movie);

        if (movieEntity == null)
        {
            return false;
        }

        foreach (var genreName in genres)
        {
            var genre = new Genre
            {
                Id = Guid.NewGuid(),
                Name = genreName,
                MovieId = movie.Id
            };

            _context.Genres.Add(genre);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        return await _context.Movies.FindAsync(id);
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        // Slug is derived; load candidates (acceptable for current scale) or add persisted Slug for indexes later.
        var list = await _context.Movies.AsNoTracking().ToListAsync();
        return list.FirstOrDefault(m => string.Equals(m.Slug, slug, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        return await _context.Movies.AsNoTracking().ToListAsync();
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        _context.Movies.Update(movie);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        var existing = await _context.Movies.FindAsync(id);
        if (existing is null)
        {
            return false;
        }

        _context.Movies.Remove(existing);
        return await _context.SaveChangesAsync() > 0;
    }
}

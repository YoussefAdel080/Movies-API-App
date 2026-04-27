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
        await _context.Movies.AddAsync(movie);

        foreach (var genreName in genres)
        {
            _context.Genres.Add(new Genre
            {
                Id = Guid.NewGuid(),
                Name = genreName,
                MovieId = movie.Id
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<MovieWithGenres?> GetByIdAsync(Guid id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
            return null;

        var genres = await GetGenresForMovie(movie.Id);

        return new MovieWithGenres
        {
            Movie = movie,
            Genres = genres
        };
    }

    public async Task<MovieWithGenres?> GetBySlugAsync(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return null;

        var movies = await _context.Movies.AsNoTracking().ToListAsync();

        var movie = movies.FirstOrDefault(m =>
            string.Equals(m.Slug, slug, StringComparison.OrdinalIgnoreCase));

        if (movie == null)
            return null;

        var genres = await GetGenresForMovie(movie.Id);

        return new MovieWithGenres
        {
            Movie = movie,
            Genres = genres
        };
    }

    public async Task<IEnumerable<MovieWithGenres>> GetAllAsync()
    {
        var movies = await _context.Movies.AsNoTracking().ToListAsync();
        var genres = await _context.Genres.AsNoTracking().ToListAsync();

        var lookup = genres
            .GroupBy(g => g.MovieId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Name).ToList()
            );

        return movies.Select(movie => new MovieWithGenres
        {
            Movie = movie,
            Genres = lookup.GetValueOrDefault(movie.Id, new List<string>())
        });
    }

    public async Task<MovieWithGenres?> UpdateAsync(Movie movie, IEnumerable<string> genres)
    {
        var existing = await _context.Movies.FindAsync(movie.Id);

        if (existing == null)
            return null;

        // update movie fields
        existing.Title = movie.Title;
        existing.YearOfRelease = movie.YearOfRelease;

        // remove old genres
        var oldGenres = _context.Genres.Where(g => g.MovieId == movie.Id);
        _context.Genres.RemoveRange(oldGenres);

        // add new genres
        foreach (var genreName in genres)
        {
            _context.Genres.Add(new Genre
            {
                Id = Guid.NewGuid(),
                Name = genreName,
                MovieId = movie.Id
            });
        }

        await _context.SaveChangesAsync();

        return new MovieWithGenres
        {
            Movie = existing,
            Genres = genres.ToList()
        };
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
            return false;

        var genres = _context.Genres.Where(g => g.MovieId == id);

        _context.Genres.RemoveRange(genres);
        _context.Movies.Remove(movie);

        return await _context.SaveChangesAsync() > 0;
    }

    private async Task<List<string>> GetGenresForMovie(Guid movieId)
    {
        return await _context.Genres
            .Where(g => g.MovieId == movieId)
            .Select(g => g.Name)
            .ToListAsync();
    }
}
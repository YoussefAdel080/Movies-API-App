using Microsoft.EntityFrameworkCore;
using Movies.Application.Contexts;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly MovieContext _context;
    private readonly IRatingRepository _ratingRepository;

    public MovieRepository(MovieContext context, IRatingRepository ratingRepository)
    {
        _context = context;
        _ratingRepository = ratingRepository;
    }

    public async Task<bool> CreateAsync(Movie movie, IEnumerable<string> genres, CancellationToken token = default)
    {
        await _context.Movies.AddAsync(movie, token);

        foreach (var genreName in genres)
        {
            _context.Genres.Add(new Genre
            {
                Id = Guid.NewGuid(),
                Name = genreName,
                MovieId = movie.Id
            });
        }

        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<MovieWithGenresAndRating?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        var movie = await _context.Movies.FindAsync(id, token);

        if (movie == null)
            return null;

        var genres = await GetGenresForMovie(movie.Id, token);

        var ratingResults = await _ratingRepository.GetRatingAsync(id, userId, token);

        return new MovieWithGenresAndRating
        {
            Movie = movie,
            Genres = genres,
            Rating = ratingResults.Item1,
            UserRating = ratingResults.Item2
        };
    }

    public async Task<MovieWithGenresAndRating?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return null;

        var movies = await _context.Movies.AsNoTracking().ToListAsync(token);

        var movie = movies.FirstOrDefault(m =>
            string.Equals(m.Slug, slug, StringComparison.OrdinalIgnoreCase));

        if (movie == null)
            return null;

        var genres = await GetGenresForMovie(movie.Id, token);

        var ratingResults = await _ratingRepository.GetRatingAsync(movie.Id, userId, token); ;

        return new MovieWithGenresAndRating
        {
            Movie = movie,
            Genres = genres,
            Rating = ratingResults.Item1,
            UserRating = ratingResults.Item2,
        };
    }

    public async Task<IEnumerable<MovieWithGenresAndRating>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
    {
        var result = await _context.Movies
        .AsNoTracking()
        .Where(m =>
            (string.IsNullOrWhiteSpace(options.Title) ||
             m.Title.ToLower().Contains(options.Title.ToLower())) &&

            (!options.YearOfRelease.HasValue ||
             m.YearOfRelease == options.YearOfRelease.Value)
        )
        .Select(m => new MovieWithGenresAndRating
        {
            Movie = m,

            Genres = _context.Genres
                .Where(g => g.MovieId == m.Id)
                .Select(g => g.Name)
                .Distinct()
                .ToList(),

            Rating = _context.Ratings
                .Where(r => r.MovieId == m.Id)
                .Select(r => (float?)r.RatingValue)
                .Average() != null
                    ? (float)Math.Round(
                        _context.Ratings
                            .Where(r => r.MovieId == m.Id)
                            .Average(r => (float)r.RatingValue), 1)
                    : null,

            UserRating = options.UserId == null
                ? null
                : _context.Ratings
                    .Where(r => r.MovieId == m.Id && r.UserId == options.UserId)
                    .Select(r => (int?)r.RatingValue)
                    .FirstOrDefault()
        })
        .ToListAsync(token);

        return result;
    }

    public async Task<MovieWithGenresAndRating?> UpdateAsync(Movie movie, IEnumerable<string> genres, Guid? userId = default, CancellationToken token = default)
    {
        var existing = await _context.Movies.FindAsync(movie.Id, token);

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

        var oldRatings = await _ratingRepository.GetRatingAsync(movie.Id, userId, token); ;

        await _context.SaveChangesAsync(token);

        return new MovieWithGenresAndRating
        {
            Movie = existing,
            Genres = genres.ToList(),
            Rating = oldRatings.Item1,
            UserRating = oldRatings.Item2,

        };
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        var movie = await _context.Movies.FindAsync(id, token);

        if (movie == null)
            return false;

        var genres = _context.Genres.Where(g => g.MovieId == id);

        _context.Genres.RemoveRange(genres);
        _context.Movies.Remove(movie);

        return await _context.SaveChangesAsync(token) > 0;
    }

    private async Task<List<string>> GetGenresForMovie(Guid movieId, CancellationToken token = default)
    {
        return await _context.Genres
            .Where(g => g.MovieId == movieId)
            .Select(g => g.Name)
            .ToListAsync(token);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        var movieExiste = await _context.Movies.FindAsync(id, token);
        return movieExiste != null;
    }
}
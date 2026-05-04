using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie, IEnumerable<string> genres, CancellationToken token = default);

    Task<MovieWithGenresAndRating?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default);

    Task<MovieWithGenresAndRating?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default);

    Task<IEnumerable<MovieWithGenresAndRating>> GetAllAsync(Guid? userId = default, CancellationToken token = default);

    Task<MovieWithGenresAndRating?> UpdateAsync(Movie movie, IEnumerable<string> genres, Guid? userId = default, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);
}
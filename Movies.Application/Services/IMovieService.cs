using Movies.Application.Models;

namespace Movies.Application.Services
{
    public interface IMovieService
    {
        Task<bool> CreateAsync(Movie movie, IEnumerable<string> genres, CancellationToken token = default);

        Task<MovieWithGenres?> GetByIdAsync(Guid id, CancellationToken token = default);

        Task<MovieWithGenres?> GetBySlugAsync(string slug, CancellationToken token = default);

        Task<IEnumerable<MovieWithGenres>> GetAllAsync(CancellationToken token = default);

        Task<MovieWithGenres?> UpdateAsync(Movie movie, IEnumerable<string> genres, CancellationToken token = default);

        Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
    }
}

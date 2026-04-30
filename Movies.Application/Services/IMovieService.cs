using Movies.Application.Models;

namespace Movies.Application.Services
{
    public interface IMovieService
    {
        Task<bool> CreateAsync(Movie movie, IEnumerable<string> genres);

        Task<MovieWithGenres?> GetByIdAsync(Guid id);

        Task<MovieWithGenres?> GetBySlugAsync(string slug);

        Task<IEnumerable<MovieWithGenres>> GetAllAsync();

        Task<MovieWithGenres?> UpdateAsync(Movie movie, IEnumerable<string> genres);

        Task<bool> DeleteByIdAsync(Guid id);
    }
}

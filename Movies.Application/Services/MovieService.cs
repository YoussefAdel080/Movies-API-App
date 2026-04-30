using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieService _movieRepository;

        public MovieService(IMovieService movieRepository) { 
            _movieRepository = movieRepository;
        }
        public Task<bool> CreateAsync(Movie movie, IEnumerable<string> genres)
        {
            return _movieRepository.CreateAsync(movie, genres);
        }

        public Task<bool> DeleteByIdAsync(Guid id)
        {
            return _movieRepository.DeleteByIdAsync(id);
        }

        public Task<IEnumerable<MovieWithGenres>> GetAllAsync()
        {
            return _movieRepository.GetAllAsync();
        }

        public Task<MovieWithGenres?> GetByIdAsync(Guid id)
        {
            return _movieRepository.GetByIdAsync(id);
        }

        public Task<MovieWithGenres?> GetBySlugAsync(string slug)
        {
            return _movieRepository.GetBySlugAsync(slug);
        }

        public async Task<MovieWithGenres?> UpdateAsync(Movie movie, IEnumerable<string> genres)
        {
            var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id);
            if (!movieExists) {
                return null;
            };
            var movieWithGenres = await _movieRepository.UpdateAsync(movie, genres);
            return movieWithGenres;
        }

    }
}

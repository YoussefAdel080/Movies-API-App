using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Validators;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly MovieValidator _movieValidator;

        public MovieService(IMovieRepository movieRepository, MovieValidator movieValidator) { 
            _movieRepository = movieRepository;
            _movieValidator = movieValidator;
        }
        public async Task<bool> CreateAsync(Movie movie, IEnumerable<string> genres, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: token);
            return await _movieRepository.CreateAsync(movie, genres, token);
        }

        public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            return _movieRepository.DeleteByIdAsync(id, token);
        }

        public Task<IEnumerable<MovieWithGenres>> GetAllAsync(CancellationToken token = default)
        {
            return _movieRepository.GetAllAsync(token);
        }

        public Task<MovieWithGenres?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return _movieRepository.GetByIdAsync(id, token);
        }

        public Task<MovieWithGenres?> GetBySlugAsync(string slug, CancellationToken token = default)
        {
            return _movieRepository.GetBySlugAsync(slug, token);
        }

        public async Task<MovieWithGenres?> UpdateAsync(Movie movie, IEnumerable<string> genres, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: token);
            var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, token);
            if (!movieExists) {
                return null;
            };
            var movieWithGenres = await _movieRepository.UpdateAsync(movie, genres, token);
            return movieWithGenres;
        }

    }
}

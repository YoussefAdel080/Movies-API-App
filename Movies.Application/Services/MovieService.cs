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
        public async Task<bool> CreateAsync(Movie movie, IEnumerable<string> genres)
        {
            await _movieValidator.ValidateAndThrowAsync(movie);
            return await _movieRepository.CreateAsync(movie, genres);
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
            await _movieValidator.ValidateAndThrowAsync(movie);
            var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id);
            if (!movieExists) {
                return null;
            };
            var movieWithGenres = await _movieRepository.UpdateAsync(movie, genres);
            return movieWithGenres;
        }

    }
}

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
        private readonly IValidator<GetAllMoviesOptions> _getAllMoviesOptionsValidator;

        public MovieService(IMovieRepository movieRepository, MovieValidator movieValidator, IValidator<GetAllMoviesOptions> getAllMoviesOptionsValidator)
        {
            _movieRepository = movieRepository;
            _movieValidator = movieValidator;
            _getAllMoviesOptionsValidator = getAllMoviesOptionsValidator;
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

        public async Task<IEnumerable<MovieWithGenresAndRating>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
        {
            await _getAllMoviesOptionsValidator.ValidateAndThrowAsync(options, cancellationToken: token);

            return await _movieRepository.GetAllAsync(options, token);
        }

        public Task<MovieWithGenresAndRating?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
        {
            return _movieRepository.GetByIdAsync(id, userId, token);
        }

        public Task<MovieWithGenresAndRating?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
        {
            return _movieRepository.GetBySlugAsync(slug, userId, token);
        }

        public async Task<MovieWithGenresAndRating?> UpdateAsync(Movie movie, IEnumerable<string> genres, Guid? userId = default, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: token);
            var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, token);
            if (!movieExists) {
                return null;
            };
            var movieWithGenres = await _movieRepository.UpdateAsync(movie, genres, userId, token);
            return movieWithGenres;
        }

        public Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default)
        {
            return _movieRepository.GetCountAsync(title, yearOfRelease, token);
        }

    }
}

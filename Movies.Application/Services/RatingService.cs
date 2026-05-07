using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Contexts;
using Movies.Application.Models;
using Movies.Application.Repositories;
using System.Reflection.Metadata.Ecma335;

namespace Movies.Application.Services
{
    public class RatingService: IRatingService
    {
        private readonly MovieContext _context;
        private readonly IRatingRepository _ratingRepository;
        private readonly IMovieRepository _movieRepository;

        public RatingService(MovieContext context, IRatingRepository ratingRepository, IMovieRepository movieRepository)
        {
            _context = context;
            _ratingRepository = ratingRepository;
            _movieRepository = movieRepository;
        }

        public async Task<(float?, int?)> GetRatingAsync(Guid movieId, Guid? userId, CancellationToken token = default)
        {
            return await _ratingRepository.GetRatingAsync(movieId, userId, token);
        }

        public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default)
        {
            if(rating <= 0 || rating < 5)
            {
                throw new ValidationException(new []
                {
                    new ValidationFailure
                    (
                        "Rating",
                        "Rating Must Be Between 1 And 5"
                    )
                });
            }

            var movieExists = await _movieRepository.ExistsByIdAsync(movieId, token);

            if (!movieExists) 
            {
                return false;
            }

            return await _ratingRepository.RateMovieAsync(movieId, userId, rating, token);
        }

        public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token)
        {
            return await _ratingRepository.DeleteRatingAsync(movieId, userId, token);
        }

        public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
        {
            return await _ratingRepository.GetRatingsForUserAsync(userId, token);
        }
    }
}

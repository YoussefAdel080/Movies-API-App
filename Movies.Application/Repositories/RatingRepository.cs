using Microsoft.EntityFrameworkCore;
using Movies.Application.Contexts;
using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public class RatingRepository: IRatingRepository
    {
        private MovieContext _context;

        public RatingRepository(MovieContext context) { 
            _context = context;
        }

        public async Task<(float?, int?)> GetRatingAsync(Guid movieId, Guid? userId, CancellationToken token = default)
        {
            var result = await _context.Ratings
            .Where(r => r.MovieId == movieId)
            .GroupBy(r => r.MovieId)
            .Select(g => new
            {
                AvgRating = (float?)Math.Round(g.Average(x => x.RatingValue), 1),

                UserRating = userId == null
                    ? null
                    : g.Where(x => x.UserId == userId)
                        .Select(x => (int?)x.RatingValue)
                        .FirstOrDefault()
            })
            .FirstOrDefaultAsync(token);

            return (result?.AvgRating, result?.UserRating);
        }

        public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default)
        {
            await _context.Ratings.AddAsync(
                new Rating 
                {
                    MovieId = movieId,
                    UserId = userId,
                    RatingValue = rating
                },token);
            await _context.SaveChangesAsync(token);
            return true;
        }
    }
}

namespace Movies.Application.Services
{
    public interface IRatingService
    {
        Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default);
        Task<(float?, int?)> GetRatingAsync(Guid movieId, Guid? userId, CancellationToken token = default);
    }
}

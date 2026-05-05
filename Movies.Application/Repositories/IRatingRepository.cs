namespace Movies.Application.Repositories
{
    public interface IRatingRepository
    {
        Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default);
        Task<(float?, int?)> GetRatingAsync(Guid movieId, Guid ?userId, CancellationToken token = default);
        Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default);
    }
}

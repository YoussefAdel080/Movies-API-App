namespace Movies.Application.Models
{
    public class Rating
    {
        public Guid UserId { get; set; }
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public int RatingValue { get; set; }
    }
}

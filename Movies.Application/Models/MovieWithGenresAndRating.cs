namespace Movies.Application.Models
{
    public class MovieWithGenresAndRating
    {
        public Movie Movie { get; set; }
        public List<string> Genres { get; set; } = new();
        public required float? Rating { get; init; }
        public required int? UserRating { get; init; }
    }
}

namespace Movies.Application.Models
{
    public class MovieWithGenres
    {
        public Movie Movie { get; set; }
        public List<string> Genres { get; set; } = new();
    }
}

namespace Movies.Application.Models
{
    public class Genre
    {

        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required Guid MovieId { get; init; }
    }
}

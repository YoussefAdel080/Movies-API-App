using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Mapping
{
    public static class ContractMapping
    {
        public static Movie MapToMovie(this CreateMovieRequest request) {
            return new Movie
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
            };
        }
        public static Movie MapToMovie(this UpdateMovieRequest request, Guid id) {
            return new Movie
            {
                Id = id,
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
            };
        }

        public static MovieResponse MapToResponse(this Movie request, IEnumerable<string> genres, float? rating, int? userRating) {
            return new MovieResponse
            {
                Id = request.Id,
                Slug = request.Slug,
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                Genres = genres,
                Rating = rating,
                UserRating = userRating
            };
        }

        public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings)
        {
            return ratings.Select(r => 
                new MovieRatingResponse {
                        Rating = r.Rating,
                        Slug = r.Slug,
                        MovieId = r.MovieId,
                    }
                );
        }
    }
}

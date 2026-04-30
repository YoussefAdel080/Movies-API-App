using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators
{
    public class MovieValidator : AbstractValidator<Movie>
    {
        private IMovieRepository _movieRepository;
        public MovieValidator()
        {
            RuleFor(m => m.Id)
                .NotEmpty();

            RuleFor(m => m.Title)
                .NotEmpty();

            RuleFor(m => m.YearOfRelease)
                .LessThanOrEqualTo(DateTime.UtcNow.Year);

            RuleFor(m => m.Slug)
                .MustAsync(ValidateSlug)
                .WithMessage("This movie already exists in the system");

        }

        private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token = default)
        {
            var existingMovie = await _movieRepository.GetBySlugAsync(slug);
            if(existingMovie is not null)
            {
                return existingMovie.Movie.Id == movie.Id;
            }
            return existingMovie is null;
        }
    }
}

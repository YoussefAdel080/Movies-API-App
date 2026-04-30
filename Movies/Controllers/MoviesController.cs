using Microsoft.AspNetCore.Mvc;
using Movies;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Movies.Mapping;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet($"{ApiEndpoints.Movies.GetAll}")]
    public async Task<IActionResult> GetAll()
    {
        var movies = await _movieService.GetAllAsync();

        var response = new MoviesResponse
        {
            Items = movies.Select(m =>
                m.Movie.MapToResponse(m.Genres))
        };

        return Ok(response);
    }

    [HttpGet($"{ApiEndpoints.Movies.Get}")]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id)
            : await _movieService.GetBySlugAsync(idOrSlug);

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(movie.Movie.MapToResponse(movie.Genres));
    }

    [HttpPost($"{ApiEndpoints.Movies.Create}")]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.MapToMovie();

        await _movieService.CreateAsync(movie, request.Genres);

        // fetch genres again for response
        var created = await _movieService.GetByIdAsync(movie.Id);

        return CreatedAtAction(
            nameof(Get),
            new { idOrSlug = movie.Id },
            created!.Movie.MapToResponse(created.Genres)
        );
    }

    [HttpPut($"{ApiEndpoints.Movies.Update}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateMovieRequest request)
    {
        var movie = request.MapToMovie(id);

        var updated = await _movieService.UpdateAsync(movie, request.Genres);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(movie.MapToResponse(request.Genres));
    }

    [HttpDelete($"{ApiEndpoints.Movies.Delete}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _movieService.DeleteByIdAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
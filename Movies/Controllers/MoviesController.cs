using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies;
using Movies.Application.Services;
using Movies.Auth;
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

    [AllowAnonymous]
    [HttpGet($"{ApiEndpoints.Movies.GetAll}")]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var movies = await _movieService.GetAllAsync(userId, token);

        var response = new MoviesResponse
        {
            Items = movies.Select(m => m.Movie.MapToResponse(m.Genres, m.Rating, m.UserRating))
        };

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet($"{ApiEndpoints.Movies.Get}")]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, userId, token)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, token);

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(movie.Movie.MapToResponse(movie.Genres, movie.Rating, movie.UserRating));
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost($"{ApiEndpoints.Movies.Create}")]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie();

        await _movieService.CreateAsync(movie, request.Genres, token);

        // fetch genres again for response
        var created = await _movieService.GetByIdAsync(movie.Id, userId, token);

        return CreatedAtAction(
            nameof(Get),
            new { idOrSlug = movie.Id },
            created!.Movie.MapToResponse(created.Genres, created.Rating, created.UserRating)
        );
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut($"{ApiEndpoints.Movies.Update}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateMovieRequest request,
        CancellationToken token = default)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie(id);

        var updated = await _movieService.UpdateAsync(movie, request.Genres, userId, token);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(movie.MapToResponse(request.Genres, updated.Rating, updated.UserRating));
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete($"{ApiEndpoints.Movies.Delete}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await _movieService.DeleteByIdAsync(id, token);

        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
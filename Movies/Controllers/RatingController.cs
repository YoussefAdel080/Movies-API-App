using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Services;
using Movies.Auth;
using Movies.Contracts.Requests;

namespace Movies.Controllers
{
    public class RatingController: ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [Authorize]
        [HttpPut(ApiEndpoints.Movies.Rate)]
        public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken token = default)
        {
            var userId = HttpContext.GetUserId();

            var result = await _ratingService.RateMovieAsync(id, userId!.Value, request.Rating, token);

            return result ? Ok() : NotFound();
        }

        [Authorize]
        [HttpPut(ApiEndpoints.Movies.DeleteRating)]
        public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token = default)
        {
            var userId = HttpContext.GetUserId();

            var result = await _ratingService.DeleteRatingAsync(id, userId!.Value, token);

            return result ? Ok() : NotFound();
        }
    }
}

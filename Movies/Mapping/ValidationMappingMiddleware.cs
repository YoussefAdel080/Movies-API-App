using FluentValidation;
using Movies.Contracts.Responses;

namespace Movies.Mapping
{
    public class ValidationMappingMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMappingMiddleware(RequestDelegate next) 
        { 
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try 
            {

            } 
            catch(ValidationException ex)
            {
                context.Response.StatusCode = 400;
                var validationFailureRseponse = new ValidationFailureResponse
                {
                    Errors = ex.Errors.Select(x => 
                    new ValidationResponse
                    {
                        PropertyName = x.PropertyName,
                        Message = x.ErrorMessage
                    })
                };

                await context.Response.WriteAsJsonAsync(validationFailureRseponse);
            }
        }
    }
}

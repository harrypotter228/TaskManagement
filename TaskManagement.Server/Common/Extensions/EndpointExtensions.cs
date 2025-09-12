using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Server.Common.Extensions
{
    public static class EndpointExtensions
    {
        public static IResult CreateValidationProblem(string key, string message)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [key] = new[] { message }
            });
        }

        public static IResult CreateValidationProblem(Dictionary<string, string[]> errors)
        {
            return Results.ValidationProblem(errors);
        }

        public static IResult CreateNotFoundResult(string message)
        {
            return Results.NotFound(new { message });
        }

        public static IResult CreateBadRequestResult(string message)
        {
            return Results.BadRequest(new { message });
        }
    }
}

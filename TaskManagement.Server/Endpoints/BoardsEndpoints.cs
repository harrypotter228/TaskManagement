using TaskManagement.Server.Contracts.Dtos;
using TaskManagement.Server.Services;

namespace TaskManagement.Server.Endpoints
{
    public static class BoardsEndpoints
    {
        public static IEndpointRouteBuilder MapBoardEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/boards");

            // -------------------------
            // GET /api/v1/boards  (Load list boards for user select 1)
            // -------------------------

            group.MapGet("", (
                IBoardService boardService
            ) =>
            {
                var result = boardService.GetAll();
                return Results.Ok(result);
            })
            .WithName("Boards_GetAll")
            .Produces<List<BoardDto>>(StatusCodes.Status200OK);

            return app;
        }
    }
}

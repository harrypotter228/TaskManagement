using TaskManagement.Server.Common.Exceptions;
using TaskManagement.Server.Contracts.Dtos;
using TaskManagement.Server.Contracts.Requests;
using TaskManagement.Server.Contracts.Responses;
using TaskManagement.Server.Contracts.DtoEnums;
using TaskManagement.Server.Domains.Interfaces.Repositories;
using TaskManagement.Server.Domains.Enums;

namespace TaskManagement.Server.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository;
        
        public BoardService(IBoardRepository boards)
        {
            _boardRepository = boards;
        }

        public async Task<List<BoardDto>> GetAll()
        {
            var result = _boardRepository.GetAll()
                    .Select(b => new BoardDto(
                        b.Id,
                        b.Name))
                    .ToList();
            return await Task.FromResult(result);
        }


        public async Task<UpdateBoardStatusesResponse> UpdateStatusesAsync(Guid boardId, UpdateBoardStatusesRequest request)
        {
            var board = _boardRepository.Get(boardId);
            if (board == null)
                throw new NotFoundException("Board not found.");

            if (!TryBuildStatuses(request, out var statusSet))
                throw new ValidationException("At least one valid status is required.", 
                    new Dictionary<string, string[]> { ["statuses"] = new[] { "At least one valid status is required." } });

            board.SetStatuses(statusSet);
            return new UpdateBoardStatusesResponse(board.Id, request.Statuses);
        }

        public async Task<bool> BoardExistsAsync(Guid boardId)
        {
            return _boardRepository.Get(boardId) != null;
        }

        private static bool TryBuildStatuses(UpdateBoardStatusesRequest req, out List<TaskStatusEnum> result)
        {
            result = new();
            foreach (var c in req.Statuses)
                if (Enum.IsDefined(typeof(TaskStatusDtoEnum), c)) 
                    result.Add(MapTaskStatusEnum(c));
            result = result.Distinct().ToList();
            return result.Count > 0;
        }

        private static TaskStatusEnum MapTaskStatusEnum(TaskStatusDtoEnum status) =>
            status switch
            {
                TaskStatusDtoEnum.ToDo => TaskStatusEnum.ToDo,
                TaskStatusDtoEnum.InProgress => TaskStatusEnum.InProgress,
                TaskStatusDtoEnum.Done => TaskStatusEnum.Done,
                _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unhandled status value: {status}")
            };
    }
}
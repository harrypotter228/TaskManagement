using TaskManagement.Server.Contracts.Dtos;
using TaskManagement.Server.Contracts.Requests;
using TaskManagement.Server.Contracts.Responses;
using TaskManagement.Server.Domains.Enums;

namespace TaskManagement.Server.Services
{
    public interface IBoardService
    {
        Task<List<BoardDto>> GetAll();
        Task<UpdateBoardStatusesResponse> UpdateStatusesAsync(Guid boardId, UpdateBoardStatusesRequest request);
        Task<bool> BoardExistsAsync(Guid boardId);
    }
}

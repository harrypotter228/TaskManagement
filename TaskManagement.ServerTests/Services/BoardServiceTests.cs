using FluentAssertions;
using Moq;
using TaskManagement.Server.Common.Exceptions;
using TaskManagement.Server.Contracts.Dtos;
using TaskManagement.Server.Contracts.Requests;
using TaskManagement.Server.Contracts.Responses;
using TaskManagement.Server.Contracts.DtoEnums;
using TaskManagement.Server.Domains.Interfaces.Repositories;
using TaskManagement.Server.Domains.Enums;
using TaskManagement.Server.Services;
using TaskManagement.Server.Domains.Entities;
using Xunit;

namespace TaskManagement.Server.Tests.Services;

public class BoardServiceTests
{
    private static Board MakeBoard(string name)
    {
        return new Board(name);
    }

    [Fact]
    public async Task GetAll_Should_Map_To_Dto()
    {
        var b1 = MakeBoard("Board A");
        var b2 = MakeBoard("Board B");

        var repo = new Mock<IBoardRepository>();
        repo.Setup(r => r.GetAll()).Returns(new[] { b1, b2 });

        var svc = new BoardService(repo.Object);
        var result = await svc.GetAll();

        result.Should().BeOfType<List<BoardDto>>();
        result.Should().HaveCount(2);
        result.Select(x => x.Name).Should().ContainInOrder("Board A", "Board B");
        result.Select(x => x.Id).Should().BeEquivalentTo(new[] { b1.Id, b2.Id });
    }

    [Fact]
    public async Task UpdateStatusesAsync_Should_Map_And_Set_On_Board()
    {
        var board = MakeBoard("Board X");
        var repo = new Mock<IBoardRepository>();
        repo.Setup(r => r.Get(board.Id)).Returns(board);

        var svc = new BoardService(repo.Object);

        var req = new UpdateBoardStatusesRequest
        {
            Statuses = new[] { TaskStatusDtoEnum.InProgress, TaskStatusDtoEnum.Done, TaskStatusDtoEnum.InProgress }
        };

        var resp = await svc.UpdateStatusesAsync(board.Id, req);

        resp.Should().BeOfType<UpdateBoardStatusesResponse>();
        resp.BoardId.Should().Be(board.Id);
        resp.Statuses.Should().BeEquivalentTo(req.Statuses);

        board.Statuses.Should().BeEquivalentTo(new[] { TaskStatusEnum.InProgress, TaskStatusEnum.Done });
    }

    [Fact]
    public async Task UpdateStatusesAsync_Should_Throw_NotFound_When_Board_Missing()
    {
        var repo = new Mock<IBoardRepository>();
        repo.Setup(r => r.Get(It.IsAny<Guid>())).Returns((Board?)null);

        var svc = new BoardService(repo.Object);
        var req = new UpdateBoardStatusesRequest { Statuses = new[] { TaskStatusDtoEnum.ToDo } };

        var act = async () => await svc.UpdateStatusesAsync(Guid.NewGuid(), req);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Board not found.");
    }

    [Fact]
    public async Task UpdateStatusesAsync_Should_Throw_Validation_When_Empty_Or_Invalid()
    {
        var board = MakeBoard("B");
        var repo = new Mock<IBoardRepository>();
        repo.Setup(r => r.Get(board.Id)).Returns(board);

        var svc = new BoardService(repo.Object);

        var req = new UpdateBoardStatusesRequest { Statuses = Array.Empty<TaskStatusDtoEnum>() };

        var act = async () => await svc.UpdateStatusesAsync(board.Id, req);
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("At least one valid status is required.*");
    }

    [Fact]
    public async Task BoardExistsAsync_Should_Return_True_When_Found()
    {
        var board = MakeBoard("B");
        var repo = new Mock<IBoardRepository>();
        repo.Setup(r => r.Get(board.Id)).Returns(board);

        var svc = new BoardService(repo.Object);
        var exists = await svc.BoardExistsAsync(board.Id);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task BoardExistsAsync_Should_Return_False_When_NotFound()
    {
        var repo = new Mock<IBoardRepository>();
        repo.Setup(r => r.Get(It.IsAny<Guid>())).Returns((Board?)null);

        var svc = new BoardService(repo.Object);
        var exists = await svc.BoardExistsAsync(Guid.NewGuid());

        exists.Should().BeFalse();
    }
}

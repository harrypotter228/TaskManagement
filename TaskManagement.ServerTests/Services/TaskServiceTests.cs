using FluentAssertions;
using Moq;
using TaskManagement.Server.Common.Exceptions;
using TaskManagement.Server.Contracts.Dtos;
using TaskManagement.Server.Contracts.DtoEnums;
using TaskManagement.Server.Contracts.Requests;
using TaskManagement.Server.Contracts.Responses;
using TaskManagement.Server.Domains.Entities;
using TaskManagement.Server.Domains.Enums;
using TaskManagement.Server.Domains.Interfaces.Repositories;
using TaskManagement.Server.Services;
using Xunit;

namespace TaskManagement.Server.Tests.Services;

public class TaskServiceTests
{
    private static TaskItem MakeTask(string name, TaskStatusEnum s = TaskStatusEnum.ToDo, string? desc = null, DateOnly? dl = null)
        => new(name, desc, dl, s);

    private static (Mock<ITaskItemRepository> tasks, Mock<IBoardTaskRepository> links, Mock<IFavoriteRepository> favs) Mocks()
        => (new Mock<ITaskItemRepository>(MockBehavior.Strict),
            new Mock<IBoardTaskRepository>(MockBehavior.Strict),
            new Mock<IFavoriteRepository>(MockBehavior.Strict));

    [Fact]
    public async Task CreateTaskAsync_Should_Create_And_Link()
    {
        var (tasks, links, favs) = Mocks();
        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var boardId = Guid.NewGuid();

        tasks.Setup(x => x.Create(It.IsAny<TaskItem>()))
             .Returns<TaskItem>(t => t);
        links.Setup(x => x.Add(boardId, It.IsAny<Guid>()));

        var req = new CreateTaskRequest { Name = " Task A ", Description = "D", Deadline = "2025-01-31" };
        var dto = await svc.CreateTaskAsync(boardId, req);

        dto.Should().BeOfType<TaskDto>();
        dto.Name.Should().Be("Task A");
        dto.Status.Should().Be(TaskStatusDtoEnum.ToDo);

        tasks.Verify(x => x.Create(It.IsAny<TaskItem>()), Times.Once);
        links.Verify(x => x.Add(boardId, dto.Id), Times.Once);
        favs.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateTaskAsync_Should_Throw_When_Invalid_Name()
    {
        var (tasks, links, favs) = Mocks();
        var svc = new TaskService(tasks.Object, links.Object, favs.Object);

        var act = async () => await svc.CreateTaskAsync(Guid.NewGuid(), new CreateTaskRequest { Name = "  " });
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateTaskAsync_Should_Throw_When_Invalid_Deadline()
    {
        var (tasks, links, favs) = Mocks();
        var svc = new TaskService(tasks.Object, links.Object, favs.Object);

        var act = async () => await svc.CreateTaskAsync(Guid.NewGuid(), new CreateTaskRequest { Name = "A", Deadline = "31/01/2025" });
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GetTaskByIdAsync_Should_Return_Null_When_Not_In_Board()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid(); var t = Guid.NewGuid();
        links.Setup(x => x.Exists(b, t)).Returns(false);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var result = await svc.GetTaskByIdAsync(b, t, null);

        result.Should().BeNull();
        tasks.VerifyNoOtherCalls();
        favs.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetTaskByIdAsync_Should_Return_Dto_With_Favorite()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid(); var u = Guid.NewGuid();
        var ti = MakeTask("X", TaskStatusEnum.InProgress);
        links.Setup(x => x.Exists(b, ti.Id)).Returns(true);
        tasks.Setup(x => x.Get(ti.Id)).Returns(ti);
        favs.Setup(x => x.IsFavorite(u, ti.Id)).Returns(true);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var dto = await svc.GetTaskByIdAsync(b, ti.Id, u);

        dto.Should().NotBeNull();
        dto!.IsFavorite.Should().BeTrue();
        dto.Status.Should().Be(TaskStatusDtoEnum.InProgress);
    }

    [Fact]
    public async Task GetTasksAsync_Should_Filter_And_Sort_Fav_Then_Name()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid(); var u = Guid.NewGuid();
        var t1 = MakeTask("Bravo", TaskStatusEnum.ToDo);     // fav
        var t2 = MakeTask("Alpha", TaskStatusEnum.ToDo);
        var t3 = MakeTask("Charlie", TaskStatusEnum.Done);

        links.Setup(x => x.GetTaskIds(b)).Returns(new[] { t1.Id, t2.Id, t3.Id });
        tasks.Setup(x => x.Get(t1.Id)).Returns(t1);
        tasks.Setup(x => x.Get(t2.Id)).Returns(t2);
        tasks.Setup(x => x.Get(t3.Id)).Returns(t3);
        favs.Setup(x => x.IsFavorite(u, t1.Id)).Returns(true);
        favs.Setup(x => x.IsFavorite(u, t2.Id)).Returns(false);
        favs.Setup(x => x.IsFavorite(u, t3.Id)).Returns(false);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);

        var list = await svc.GetTasksAsync(b, u, TaskStatusEnum.ToDo, null);
        list.Select(x => x.Name).Should().ContainInOrder("Bravo", "Alpha");

        var search = await svc.GetTasksAsync(b, u, null, "ha");
        search.Select(x => x.Name).Should().BeEquivalentTo(new[] { "Alpha", "Charlie" }, o => o.WithoutStrictOrdering());
    }

    [Fact]
    public async Task GetTasksSortedByColumnAsync_Should_Return_Only_Status_And_Sort()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid(); var u = Guid.NewGuid();
        var a = MakeTask("B", TaskStatusEnum.InProgress);
        var btask = MakeTask("A", TaskStatusEnum.InProgress);
        var c = MakeTask("C", TaskStatusEnum.Done);

        links.Setup(x => x.GetTaskIds(b)).Returns(new[] { a.Id, btask.Id, c.Id });
        tasks.Setup(x => x.Get(a.Id)).Returns(a);
        tasks.Setup(x => x.Get(btask.Id)).Returns(btask);
        tasks.Setup(x => x.Get(c.Id)).Returns(c);
        favs.Setup(x => x.IsFavorite(u, a.Id)).Returns(true);
        favs.Setup(x => x.IsFavorite(u, btask.Id)).Returns(false);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var list = await svc.GetTasksSortedByColumnAsync(b, u, TaskStatusDtoEnum.InProgress);

        list.Select(x => x.Name).Should().ContainInOrder("B", "A");
        list.Should().OnlyContain(x => x.Status == TaskStatusDtoEnum.InProgress);
    }

    [Fact]
    public async Task UpdateTaskAsync_Should_Update_Fields_And_Status()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid(); var u = Guid.NewGuid();
        var t = MakeTask("Old", TaskStatusEnum.ToDo, "d0");
        links.Setup(x => x.Exists(b, t.Id)).Returns(true);
        tasks.Setup(x => x.Get(t.Id)).Returns(t);
        tasks.Setup(x => x.Update(It.IsAny<TaskItem>()));
        favs.Setup(x => x.IsFavorite(u, t.Id)).Returns(true);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var req = new UpdateTaskRequest { Name = "New", Description = "d1", Deadline = "2025-02-01", Status = TaskStatusEnum.Done };

        var dto = await svc.UpdateTaskAsync(b, t.Id, req, u);

        dto.Name.Should().Be("New");
        dto.Description.Should().Be("d1");
        dto.Deadline.Should().Be("2025-02-01");
        dto.Status.Should().Be(TaskStatusDtoEnum.Done);
        dto.IsFavorite.Should().BeTrue();
        tasks.Verify(x => x.Update(It.Is<TaskItem>(ti => ti.Name == "New" && ti.Status == TaskStatusEnum.Done)), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_Should_Throw_When_NotInBoard()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid(); var t = Guid.NewGuid();
        links.Setup(x => x.Exists(b, t)).Returns(false);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var act = async () => await svc.UpdateTaskAsync(b, t, new UpdateTaskRequest { Name = "A" });
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateTaskAsync_Should_Throw_When_Task_NotFound()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid(); var t = Guid.NewGuid();
        links.Setup(x => x.Exists(b, t)).Returns(true);
        tasks.Setup(x => x.Get(t)).Returns((TaskItem?)null);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var act = async () => await svc.UpdateTaskAsync(b, t, new UpdateTaskRequest { Name = "A" });
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateTaskAsync_Should_Throw_When_Invalid_Name()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid();
        var ti = MakeTask("X");
        links.Setup(x => x.Exists(b, ti.Id)).Returns(true);
        tasks.Setup(x => x.Get(ti.Id)).Returns(ti);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var act = async () => await svc.UpdateTaskAsync(b, ti.Id, new UpdateTaskRequest { Name = "  " });
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_Should_Change_When_Different()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid(); var u = Guid.NewGuid();
        var ti = MakeTask("X", TaskStatusEnum.ToDo);
        links.Setup(x => x.Exists(b, ti.Id)).Returns(true);
        tasks.Setup(x => x.Get(ti.Id)).Returns(ti);
        tasks.Setup(x => x.Update(It.IsAny<TaskItem>()));
        favs.Setup(x => x.IsFavorite(u, ti.Id)).Returns(false);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var dto = await svc.UpdateTaskStatusAsync(b, ti.Id, new ChangeTaskStatusRequest { Status = TaskStatusDtoEnum.InProgress }, u);

        dto.Status.Should().Be(TaskStatusDtoEnum.InProgress);
        tasks.Verify(x => x.Update(It.Is<TaskItem>(x => x.Status == TaskStatusEnum.InProgress)), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_Should_Not_Update_When_Same()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid();
        var ti = MakeTask("X", TaskStatusEnum.Done);
        links.Setup(x => x.Exists(b, ti.Id)).Returns(true);
        tasks.Setup(x => x.Get(ti.Id)).Returns(ti);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var dto = await svc.UpdateTaskStatusAsync(b, ti.Id, new ChangeTaskStatusRequest { Status = TaskStatusDtoEnum.Done }, null);

        dto.Status.Should().Be(TaskStatusDtoEnum.Done);
        tasks.Verify(x => x.Update(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_Should_Throw_When_No_Status()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid();
        var ti = MakeTask("X");
        links.Setup(x => x.Exists(b, ti.Id)).Returns(true);
        tasks.Setup(x => x.Get(ti.Id)).Returns(ti);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var act = async () => await svc.UpdateTaskStatusAsync(b, ti.Id, new ChangeTaskStatusRequest { Status = null }, null);
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task DeleteTaskAsync_Should_Remove_From_All_Boards_And_Delete()
    {
        var (tasks, links, favs) = Mocks();
        var t = MakeTask("X");
        var b = Guid.NewGuid();
        var b2 = Guid.NewGuid();
        links.Setup(x => x.Exists(b, t.Id)).Returns(true);
        tasks.Setup(x => x.Get(t.Id)).Returns(t);
        links.Setup(x => x.GetBoardIds(t.Id)).Returns(new[] { b, b2 });
        links.Setup(x => x.Remove(b, t.Id));
        links.Setup(x => x.Remove(b2, t.Id));
        tasks.Setup(x => x.Delete(t.Id));

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        await svc.DeleteTaskAsync(b, t.Id);

        links.Verify(x => x.Remove(b, t.Id), Times.Once);
        links.Verify(x => x.Remove(b2, t.Id), Times.Once);
        tasks.Verify(x => x.Delete(t.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_Should_Throw_When_NotInBoard()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid(); var t = Guid.NewGuid();
        links.Setup(x => x.Exists(b, t)).Returns(false);

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var act = async () => await svc.DeleteTaskAsync(b, t);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteTasksBulkAsync_Should_Return_Removed_And_NotFound()
    {
        var (tasks, links, favs) = Mocks();
        var b = Guid.NewGuid();
        var t1 = Guid.NewGuid();
        var t2 = Guid.NewGuid();
        var t3 = Guid.NewGuid();

        links.Setup(x => x.Exists(b, t1)).Returns(true);
        links.Setup(x => x.Exists(b, t2)).Returns(false);
        links.Setup(x => x.Exists(b, t3)).Returns(true);
        links.Setup(x => x.Remove(b, t1));
        links.Setup(x => x.Remove(b, t3));

        var svc = new TaskService(tasks.Object, links.Object, favs.Object);
        var resp = await svc.DeleteTasksBulkAsync(b, new DeleteTasksRequest { TaskIds = new[] { t1, t2, t3 } });

        resp.Should().BeOfType<DeleteTasksResponse>();
        resp.Removed.Should().BeEquivalentTo(new[] { t1, t3 });
        resp.NotFound.Should().BeEquivalentTo(new[] { t2 });
    }

    [Fact]
    public async Task DeleteTasksBulkAsync_Should_Throw_When_Empty()
    {
        var (tasks, links, favs) = Mocks();
        var svc = new TaskService(tasks.Object, links.Object, favs.Object);

        var act = async () => await svc.DeleteTasksBulkAsync(Guid.NewGuid(), new DeleteTasksRequest { TaskIds = Array.Empty<Guid>() });
        await act.Should().ThrowAsync<ValidationException>();
    }
}

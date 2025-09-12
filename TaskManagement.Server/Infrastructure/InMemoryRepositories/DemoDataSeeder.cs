using TaskManagement.Server.Domains.Entities;
using TaskManagement.Server.Domains.Interfaces.Repositories;

namespace TaskManagement.Server.Infrastructure.InMemoryRepositories
{
    public sealed class DemoDataSeeder(
    IBoardRepository boardRepository) : IDataSeeder
    {
        public Task SeedAsync(CancellationToken ct = default)
        {
            var boardList = boardRepository.GetAll();

            if(!boardList.Any())
            {
                boardRepository.Create(new Board("Development Board"));
                boardRepository.Create(new Board("QA Board"));
                boardRepository.Create(new Board("Staging Board"));
                boardRepository.Create(new Board("Sprint 125 Board"));
            }

            return Task.CompletedTask;
        }
    }
}

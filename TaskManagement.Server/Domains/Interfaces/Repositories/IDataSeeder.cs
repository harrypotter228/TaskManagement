namespace TaskManagement.Server.Domains.Interfaces.Repositories
{
    public interface IDataSeeder
    {
        Task SeedAsync(CancellationToken ct = default);
    }
}

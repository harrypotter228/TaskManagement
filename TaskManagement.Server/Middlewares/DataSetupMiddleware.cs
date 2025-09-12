using System.Data;
using TaskManagement.Server.Domains.Interfaces.Repositories;

namespace TaskManagement.Server.Middlewares
{
    public sealed class DataSetupMiddleware
    {
        private static int _initialized;
        private readonly RequestDelegate _next;

        public DataSetupMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IDataSeeder seeder)
        {
            if (Interlocked.Exchange(ref _initialized, 1) == 0)
            {
                try
                {
                    await seeder.SeedAsync(context.RequestAborted);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Seeding error: {ex}");
                    throw;
                }
            }

            await _next(context);
        }
    }
}

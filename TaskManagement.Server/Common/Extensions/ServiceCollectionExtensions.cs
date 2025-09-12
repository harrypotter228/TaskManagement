using TaskManagement.Server.Domains.Interfaces.Repositories;
using TaskManagement.Server.Infrastructure.InMemoryRepositories;
using TaskManagement.Server.Services;

namespace TaskManagement.Server.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<ITaskItemRepository, TaskItemRepository>();
            services.AddSingleton<IBoardRepository, BoardRepository>();
            services.AddSingleton<IBoardTaskRepository, BoardTaskRepository>();
            services.AddSingleton<IFavoriteRepository, FavoriteRepository>();
            services.AddSingleton<IDataSeeder, DemoDataSeeder>();
            
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IBoardService, BoardService>();
            services.AddSingleton<ITaskService, TaskService>();
            services.AddSingleton<ITaskAttachmentService, TaskAttachmentService>();
            services.AddSingleton<IFavoriteService, FavoriteService>();

            return services;
        }
    }
}

namespace TaskManagement.Server.Middlewares
{
    public static class DataSetupMiddlewareExtensions
    {
        public static IApplicationBuilder UseDataSetup(this IApplicationBuilder app)
            => app.UseMiddleware<DataSetupMiddleware>();
    }
}

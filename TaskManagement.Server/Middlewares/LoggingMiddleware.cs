using System.Diagnostics;

namespace TaskManagement.Server.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            // Pre-logic
            Console.WriteLine($"[REQ] {context.Request.Method} {context.Request.Path}");

            await _next(context); // IMPORTANT: call next

            // Post-logic
            sw.Stop();
            Console.WriteLine($"[RES] {context.Response.StatusCode} in {sw.ElapsedMilliseconds} ms");
        }
    }
}

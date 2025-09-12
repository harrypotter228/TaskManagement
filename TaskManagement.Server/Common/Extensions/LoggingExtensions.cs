using Microsoft.Extensions.Logging;

namespace TaskManagement.Server.Common.Extensions
{
    public static class LoggingExtensions
    {
        public static void LogOperationStart(this ILogger logger, string operation, object? parameters = null)
        {
            logger.LogInformation("Starting {Operation} with parameters: {@Parameters}", operation, parameters);
        }

        public static void LogOperationSuccess(this ILogger logger, string operation, object? result = null)
        {
            logger.LogInformation("Successfully completed {Operation} with result: {@Result}", operation, result);
        }

        public static void LogOperationError(this ILogger logger, string operation, Exception exception, object? parameters = null)
        {
            logger.LogError(exception, "Error occurred during {Operation} with parameters: {@Parameters}", operation, parameters);
        }

        public static void LogValidationError(this ILogger logger, string operation, Dictionary<string, string[]> errors, object? parameters = null)
        {
            logger.LogWarning("Validation failed for {Operation} with parameters: {@Parameters}. Errors: {@Errors}", 
                operation, parameters, errors);
        }
    }
}

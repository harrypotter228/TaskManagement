namespace TaskManagement.Server.Common.Configuration
{
    public class AppSettings
    {
        public FileSettings File { get; set; } = new();
        public SwaggerSettings Swagger { get; set; } = new();
    }

    public class FileSettings
    {
        public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10 MB
        public int MaxFileNameLength { get; set; } = 120;
        public string UploadsPath { get; set; } = "uploads";
        public string[] AllowedImageMimeTypes { get; set; } = 
        {
            "image/png",
            "image/jpeg",
            "image/jpg", 
            "image/gif",
            "image/webp"
        };
    }

    public class SwaggerSettings
    {
        public string Title { get; set; } = "TaskManagement.Server";
        public string Version { get; set; } = "v1";
        public string Description { get; set; } = "Task Management API";
    }
}

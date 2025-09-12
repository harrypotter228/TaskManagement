namespace TaskManagement.Server.Common.Constants
{
    public static class FileConstants
    {
        public static readonly HashSet<string> AllowedImageMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/png",
            "image/jpeg", 
            "image/jpg",
            "image/gif",
            "image/webp"
        };

        public const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
        public const int MaxFileNameLength = 120;
        public const string UploadsPath = "uploads";
    }
}

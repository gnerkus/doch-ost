namespace Dochost.Server.Jobs
{
    public record UploadJob
    {
        public required Guid Id { get; init; }
        public required string OwnerId { get; init; }
        public required MemoryStream FormFile { get; init; }
        public required string FileExt { get; init; }
        public required string FilePath { get; init; }
        public required string PreviewPath { get; init; }
    }
}
namespace Core.Contracts
{
    public record DocumentInfoDto
    {
        public Guid Id { get; init; }
        public required string FileName { get; init; }
        public required string FileExt { get; init; }
        public int DownloadCount { get; init; }
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
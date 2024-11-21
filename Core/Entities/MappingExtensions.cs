using Core.Contracts;

namespace Core.Entities
{
    public static class MappingExtensions
    {
        public static DocumentInfoDto ToDto(this DocumentInfo documentInfo)
        {
            if (documentInfo != null)
                return new DocumentInfoDto
                {
                    Id = documentInfo.Id,
                    FileName = documentInfo.DisplayName,
                    FileExt = documentInfo.FileExt,
                    DownloadCount = documentInfo.DownloadCount,
                    CreatedAt = documentInfo.CreatedAt,
                    UpdatedAt = documentInfo.UpdatedAt
                };

            return null;
        }
    }
}
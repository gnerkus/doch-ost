using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class DocumentInfo
    {
        public Guid Id { get; init; }

        [Required(ErrorMessage = "Document must have a file path")]
        [MaxLength(60, ErrorMessage = "Maximum length for file name was exceeded")]
        public string? FileName { get; init; }
        
        [MaxLength(60, ErrorMessage = "Maximum length for file name was exceeded")]
        public string? PreviewUrl { get; init; }

        [Required(ErrorMessage = "Document must have a file name")]
        [MaxLength(60, ErrorMessage = "Maximum length for file name was exceeded")]
        public string? DisplayName { get; init; }

        [Required(ErrorMessage = "Document must have a file extension")]
        [MaxLength(10, ErrorMessage = "Maximum length for file extension was exceeded")]
        public string? FileExt { get; init; }

        public int DownloadCount { get; set; }
        
        public Guid JobId { get; init; }

        [MaxLength(20, ErrorMessage = "Maximum length for upload status was exceeded")]
        public string UploadStatus { get; set; } = "pending";
        
        [MaxLength(20, ErrorMessage = "Maximum length for preview status was exceeded")]
        public string PreviewStatus { get; set; } = "pending";

        [ForeignKey(nameof(ApplicationUser))] public string? OwnerId { get; set; }
        public ApplicationUser? Owner { get; init; }

        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class DocumentInfo
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Document must have a file name")]
        [MaxLength(60, ErrorMessage = "Maximum length for file name was exceeded")]
        public string? FileName { get; set; }
        
        [Required(ErrorMessage = "Document must have a file extension")]
        [MaxLength(10, ErrorMessage = "Maximum length for file extension was exceeded")]
        public string? FileExt { get; set; }

        public int DownloadCount { get; set; }
        
        [ForeignKey(nameof(ApplicationUser))] public string? OwnerId { get; set; }
        public ApplicationUser? Owner { get; set; }
        
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; }
    }
}
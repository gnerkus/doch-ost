﻿using Microsoft.AspNetCore.Authorization;

namespace Dochost.Server.Endpoints
{
    public static class DocumentEndpoints
    {
        private static readonly string[] PermittedExtensions = [".txt", ".pdf", ".doc", ".docx", ".xlsx", ".jpg", ".png"];
        
        [Authorize]
        private static async Task<IResult> UploadFileAsync(IFormFile formFile, IConfiguration 
            config) {
            if (formFile.Length <= 0) return TypedResults.BadRequest("Invalid file.");
            
            var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !PermittedExtensions.Contains(ext))
            {
                return TypedResults.BadRequest("File type not permitted");
            }

            var fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            var size = formFile.Length;

            if (size > fileSizeLimit)
            {
                return TypedResults.UnprocessableEntity("File too large");
            }
            
            var filePath = Path.GetTempFileName();

            await using var stream = File.Create(filePath);
            await formFile.CopyToAsync(stream);

            return TypedResults.Ok(new { FilePath = filePath, size });
        }

        [Authorize]
        private static async Task<IResult> DownloadFile(string filename)
        {
            var filePath = Path.Combine(Path.GetTempPath(), "Dochost", "Uploads",
                filename);
            if (!File.Exists(filePath))
            {
                return TypedResults.NotFound("File not found");
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            return TypedResults.File(fileBytes, "application/octet-stream", filename);
        }
        public static void RegisterDocumentEndpoints(this WebApplication app)
        {
            var documentGroup = app.MapGroup("/documents");
            documentGroup.MapPost("/upload", UploadFileAsync);
            documentGroup.MapGet("/download/{filename}", DownloadFile);
        }
    }
}
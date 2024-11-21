using Microsoft.AspNetCore.Authorization;

namespace Dochost.Server.Endpoints
{
    public static class DocumentEndpoints
    {
        [Authorize]
        private static async Task<IResult> UploadFile(IFormFile file) {
            if (file.Length <= 0) return TypedResults.BadRequest("Invalid file.");
            var filePath = Path.Combine(Path.GetTempPath(), "Dochost", "Uploads",
                file.FileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return TypedResults.Ok(new { FilePath = filePath });
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
            documentGroup.MapPost("/upload", UploadFile);
            documentGroup.MapGet("/download/{filename}", DownloadFile);
        }
    }
}
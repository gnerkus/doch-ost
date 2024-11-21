using System.Security.Claims;
using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Dochost.Server.Endpoints
{
    public static class DocumentEndpoints
    {
        private static readonly string[] PermittedExtensions =
            [".txt", ".pdf", ".doc", ".docx", ".xlsx", ".jpg", ".png"];

        [Authorize]
        private static async Task<IResult> UploadFileAsync(List<IFormFile> formFiles, IConfiguration
                config, IDocumentInfoRepository documentInfoRepository, ClaimsPrincipal user,
            UserManager<ApplicationUser> userManager)
        {
            var ownerId = userManager.GetUserId(user);
            if (string.IsNullOrEmpty(ownerId)) return TypedResults.Unauthorized();

            foreach (var formFile in formFiles)
            {
                if (formFile.Length <= 0) return TypedResults.BadRequest("Invalid file.");

                var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || !PermittedExtensions.Contains(ext))
                    return TypedResults.BadRequest("File type not permitted");
                
                var fileSizeLimit = config.GetValue<long>("FileSizeLimit");
                var size = formFile.Length;

                if (size > fileSizeLimit) return TypedResults.UnprocessableEntity("File too large");

                var filePath = Path.GetTempFileName();

                await using var stream = File.Create(filePath);
                await formFile.CopyToAsync(stream);
                
                documentInfoRepository.CreateDocument(ownerId, new DocumentInfo
                {
                    DisplayName = formFile.FileName,
                    FileName = filePath,
                    FileExt = ext,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }
            
            await documentInfoRepository.SaveAsync();

            return TypedResults.Ok();
        }

        [Authorize]
        private static async Task<IResult> DownloadFile(Guid fileId, IDocumentInfoRepository documentInfoRepository, ClaimsPrincipal user,
            UserManager<ApplicationUser> userManager)
        {
            var ownerId = userManager.GetUserId(user);
            if (string.IsNullOrEmpty(ownerId)) return TypedResults.Unauthorized();

            var documentInfo = await documentInfoRepository.GetDocumentAsync(ownerId, fileId,
                false);

            if (documentInfo == null)
                return TypedResults.NotFound(fileId);

            var filePath = documentInfo.FileName;
            if (!File.Exists(filePath)) return TypedResults.NotFound("File not found");

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            return TypedResults.File(fileBytes, "application/octet-stream", documentInfo.DisplayName);
        }

        [Authorize]
        private static async Task<IResult> GetDocumentInfosAsync(
            IDocumentInfoRepository documentInfoRepository, ClaimsPrincipal user,
            UserManager<ApplicationUser> userManager)
        {
            var ownerId = userManager.GetUserId(user);
            if (string.IsNullOrEmpty(ownerId)) return TypedResults.Unauthorized();

            var documentInfos = await documentInfoRepository.GetAllDocumentsAsync(ownerId, false);
            return TypedResults.Ok(documentInfos.Select(info => info.ToDto()));
        }

        public static void RegisterDocumentEndpoints(this WebApplication app)
        {
            var documentGroup = app.MapGroup("/documents");
            documentGroup.MapGet("/", GetDocumentInfosAsync);
            documentGroup.MapPost("/upload", UploadFileAsync).DisableAntiforgery();
            documentGroup.MapGet("/download/{fileId:guid}", DownloadFile);
        }
    }
}
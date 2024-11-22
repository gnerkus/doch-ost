using System.Security.Claims;
using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dochost.Server.Endpoints
{
    public static class DocumentEndpoints
    {
        private static readonly string[] PermittedExtensions =
            [".txt", ".pdf", ".doc", ".docx", ".xlsx", ".jpg", ".png"];

        [Authorize]
        private static async Task<IResult> UploadFileAsync(IFormFileCollection formFiles, 
        IConfiguration
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

            var memory = new MemoryStream();
            await using var stream = new FileStream(filePath, FileMode.Open);
            await stream.CopyToAsync(memory);
            memory.Position = 0;

            var contentType = documentInfo.FileExt switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                "" => "application/octet-stream",
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return TypedResults.File(memory, contentType, documentInfo.DisplayName);
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
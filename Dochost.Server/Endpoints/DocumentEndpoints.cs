using System.Globalization;
using System.Security.Claims;
using Core.Contracts;
using Core.Entities;
using Dochost.Encryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dochost.Server.Endpoints
{
    public static class DocumentEndpoints
    {
        private static readonly string[] PermittedExtensions =
            [".txt", ".pdf", ".doc", ".docx", ".xlsx", ".jpg", ".png", ".jpeg"];

        private const int ExpirationDurationMs = 1000 * 60 * 5; // 5 minutes

        [Authorize]
        private static async Task<IResult> UploadFileAsync(IFormFileCollection formFiles,
            IConfiguration
                config, IDocumentInfoRepository documentInfoRepository, ClaimsPrincipal user,
            UserManager<ApplicationUser> userManager, IPreviewManager previewManager)
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

                if (size > fileSizeLimit)
                    return TypedResults.UnprocessableEntity("File too large");

                var filePath = Path.GetTempFileName();

                await using (var stream = File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                var previewUrl = Path.GetTempFileName();

                switch (ext)
                {
                    case ".pdf":
                        previewManager.PdfPreviewGenerator.GetSinglePagePreview(previewUrl,
                            filePath, 1);
                        break;
                    case ".txt":
                    case ".doc":
                    case ".docx":
                        previewManager.WordPreviewGenerator.GetSinglePagePreview(previewUrl,
                            filePath, 1);
                        break;
                    case ".xls":
                    case ".xlsx":
                        previewManager.SpreadsheetPreviewGenerator.GetSinglePagePreview
                            (previewUrl, filePath, 1);
                        break;
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        previewUrl = filePath;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                documentInfoRepository.CreateDocument(ownerId, new DocumentInfo
                {
                    DisplayName = formFile.FileName,
                    FileName = filePath,
                    FileExt = ext,
                    PreviewUrl = previewUrl,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            await documentInfoRepository.SaveAsync();

            return TypedResults.Ok();
        }

        [Authorize]
        private static async Task<IResult> DownloadFileAsync(Guid fileId,
            IDocumentInfoRepository documentInfoRepository, ClaimsPrincipal user,
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
                ".jpeg" => "image/jpeg",
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" =>
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "" => "application/octet-stream",
                _ => throw new ArgumentOutOfRangeException()
            };

            return TypedResults.File(memory, contentType, documentInfo.DisplayName,
                documentInfo.UpdatedAt);
        }

        [Authorize]
        private static async Task<IResult> GetSharedLinkAsync(Guid fileId, ClaimsPrincipal user,
            UserManager<ApplicationUser> userManager)
        {
            var ownerId = userManager.GetUserId(user);
            if (string.IsNullOrEmpty(ownerId)) return TypedResults.Unauthorized();

            var secret = Environment.GetEnvironmentVariable("DCH_SECRET");
            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("Missing configuration");
            }

            var base64String = await TokenGenerator.EncryptUserFile(ownerId, fileId, secret);
            return TypedResults.Ok(base64String);
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

        [AllowAnonymous]
        private static async Task<IResult> DownloadSharedFileAsync([FromQuery] string share,
            IDocumentInfoRepository documentInfoRepository)
        {
            var secret = Environment.GetEnvironmentVariable("DCH_SECRET");
            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("Missing configuration");
            }

            var decryptedResult = await TokenGenerator.DecryptUserFile(share, secret);
            var stringDate = DateTime.ParseExact(decryptedResult.DateString,
                "yyyy-MM-dd HH:mm:ss,fff", CultureInfo
                    .InvariantCulture);
            var duration = DateTime.Now.Subtract(stringDate);
            if (duration.Milliseconds > ExpirationDurationMs)
            {
                return TypedResults.NotFound();
            }

            var documentInfo = await documentInfoRepository.GetDocumentAsync(decryptedResult
                    .UserId, new Guid(decryptedResult.FileId),
                false);

            if (documentInfo == null)
                return TypedResults.NotFound();

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
                ".jpeg" => "image/jpeg",
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" =>
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "" => "application/octet-stream",
                _ => throw new ArgumentOutOfRangeException()
            };

            return Results.File(filePath, contentType, documentInfo.DisplayName);
        }

        [Authorize]
        private static async Task<IResult> GetPreviewAsync(Guid fileId,
            IDocumentInfoRepository documentInfoRepository, ClaimsPrincipal user,
            UserManager<ApplicationUser> userManager)
        {
            var ownerId = userManager.GetUserId(user);
            if (string.IsNullOrEmpty(ownerId)) return TypedResults.Unauthorized();

            var documentInfo = await documentInfoRepository.GetDocumentAsync(ownerId, fileId,
                false);

            if (documentInfo == null)
                return TypedResults.NotFound(fileId);

            var filePath = documentInfo.PreviewUrl;
            if (!File.Exists(filePath)) return TypedResults.NotFound("File not found");

            var memory = new MemoryStream();
            await using var stream = new FileStream(filePath, FileMode.Open);
            await stream.CopyToAsync(memory);
            memory.Position = 0;

            return TypedResults.File(memory, "image/png", documentInfo.DisplayName);
        }

        public static void RegisterDocumentEndpoints(this WebApplication app)
        {
            var documentGroup = app.MapGroup("/documents");
            documentGroup.MapGet("/", GetDocumentInfosAsync);
            documentGroup.MapPost("/upload", UploadFileAsync).DisableAntiforgery();
            documentGroup.MapGet("/{fileId:guid}/download", DownloadFileAsync);
            documentGroup.MapGet("/{fileId:guid}/preview", GetPreviewAsync);
            documentGroup.MapGet("/{fileId:guid}/share", GetSharedLinkAsync);
            documentGroup.MapGet("/file", DownloadSharedFileAsync);
        }
    }
}
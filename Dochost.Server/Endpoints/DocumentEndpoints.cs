using System.Globalization;
using System.Security.Claims;
using Core.Contracts;
using Core.Entities;
using Dochost.Encryption;
using Dochost.Server.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dochost.Server.Endpoints
{
    public static class DocumentEndpoints
    {
        private static readonly string[] PermittedExtensions =
            [".txt", ".pdf", ".doc", ".docx", ".xlsx", ".jpg", ".png", ".jpeg", ".xls"];

        private static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png"];
        private static readonly string[] DocumentExtensions = [".txt", ".doc", ".docx"];

        [Authorize]
        private static async Task<IResult> UploadFileAsync(IFormFileCollection formFiles,
            IConfiguration
                config, IDocumentInfoRepository documentInfoRepository, ClaimsPrincipal user,
            UserManager<ApplicationUser> userManager, IJobQueue
                jobQueue)
        {
            var ownerId = userManager.GetUserId(user);
            if (string.IsNullOrEmpty(ownerId)) return TypedResults.Unauthorized();

            var jobs = new List<UploadJob>();

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
                var previewUrl = Path.GetTempFileName();

                if (DocumentExtensions.Contains(ext))
                {
                    previewUrl = $"{Path.ChangeExtension(previewUrl, null)}.png";
                }
                
                var jobId = Guid.NewGuid();

                var clonedFormFile = new MemoryStream();
                await formFile.CopyToAsync(clonedFormFile);
                clonedFormFile.Position = 0;

                var uploadJob = new UploadJob
                {
                    Id = jobId,
                    FilePath = filePath,
                    PreviewPath = previewUrl,
                    FormFile = clonedFormFile,
                    FileExt = ext,
                    OwnerId = ownerId
                };

                jobs.Add(uploadJob);

                documentInfoRepository.CreateDocument(ownerId, new DocumentInfo
                {
                    DisplayName = formFile.FileName,
                    FileName = filePath,
                    FileExt = ext,
                    PreviewUrl = ImageExtensions.Contains(ext) ? filePath : previewUrl,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    JobId = jobId,
                    UploadStatus = "queued",
                    PreviewStatus = "queued"
                });
            }

            await documentInfoRepository.SaveAsync();

            // enqueue jobs here
            foreach (var job in jobs) await jobQueue.EnqueueAsync(job);

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
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
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
            if (string.IsNullOrEmpty(secret)) throw new Exception("Missing configuration");

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
            return TypedResults.Ok(documentInfos.Select(info => info.ToDto()).OrderByDescending
                (info => info.CreatedAt));
        }

        [AllowAnonymous]
        private static async Task<IResult> DownloadSharedFileAsync([FromQuery] string share,
            IDocumentInfoRepository documentInfoRepository, IConfiguration
                config)
        {
            var secret = Environment.GetEnvironmentVariable("DCH_SECRET");
            if (string.IsNullOrEmpty(secret)) throw new Exception("Missing configuration");

            var decryptedResult = await TokenGenerator.DecryptUserFile(share, secret);
            var stringDate = DateTime.ParseExact(decryptedResult.DateString,
                "yyyy-MM-dd HH:mm:ss,fff", CultureInfo
                    .InvariantCulture);
            var duration = DateTime.Now.Subtract(stringDate);
            var expirationDuration = config.GetValue<long>("ExpirationDurationMs");
            if (duration.TotalMilliseconds > expirationDuration) return TypedResults.NotFound();

            var documentInfo = await documentInfoRepository.GetDocumentAsync(decryptedResult
                    .UserId, new Guid(decryptedResult.FileId),
                true);

            if (documentInfo == null)
                return TypedResults.NotFound();

            var filePath = documentInfo.FileName;
            if (!File.Exists(filePath)) return TypedResults.NotFound("File not found");

            var memory = new MemoryStream();
            await using var stream = new FileStream(filePath, FileMode.Open);
            await stream.CopyToAsync(memory);
            memory.Position = 0;

            documentInfo.DownloadCount += 1;
            await documentInfoRepository.SaveAsync();

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
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
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
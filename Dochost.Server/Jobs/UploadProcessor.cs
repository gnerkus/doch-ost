using Core.Contracts;

namespace Dochost.Server.Jobs
{
    public class UploadProcessor(
        IJobQueue jobQueue,
        IPreviewManager previewManager,
        IServiceProvider serviceProvider)
        : BackgroundService
    {
        public IJobQueue JobQueue { get; } = jobQueue;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceProvider.CreateScope();
            var infoRepository =
                scope.ServiceProvider.GetRequiredService<IDocumentInfoRepository>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var uploadJob = await JobQueue.DequeueAsync(stoppingToken);
                var documentInfo = await infoRepository.GetByJobId(uploadJob
                    .OwnerId, uploadJob.Id, true);

                try
                {
                    // 1. tell repository is job is processing
                    if (documentInfo == null)
                        throw new Exception("Missing job");

                    documentInfo.UploadStatus = "processing";
                    documentInfo.PreviewStatus = "processing";
                    await infoRepository.SaveAsync();

                    // 2. save file to storage
                    await using (var stream = File.Create(uploadJob.FilePath))
                    {
                        await uploadJob.FormFile.CopyToAsync(stream, stoppingToken);
                    }

                    // 3. update the upload status
                    documentInfo.UploadStatus = "completed";
                    await infoRepository.SaveAsync();

                    // 4. generate previews
                    switch (uploadJob.FileExt)
                    {
                        case ".pdf":
                            previewManager.PdfPreviewGenerator.GetSinglePagePreview(
                                uploadJob.PreviewPath,
                                uploadJob.FilePath, 1);
                            break;
                        case ".txt":
                        case ".doc":
                        case ".docx":
                            var previewUrl =
                                $"{Path.ChangeExtension(uploadJob.PreviewPath, null)}.png";
                            previewManager.WordPreviewGenerator.GetSinglePagePreview(previewUrl,
                                uploadJob.FilePath, 1);
                            break;
                        case ".xls":
                        case ".xlsx":
                            previewManager.SpreadsheetPreviewGenerator.GetSinglePagePreview
                                (uploadJob.PreviewPath, uploadJob.FilePath, 1);
                            break;
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    // 5. update preview status
                    documentInfo.PreviewStatus = "completed";
                    await infoRepository.SaveAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    documentInfo.PreviewStatus = "failed";
                    await infoRepository.SaveAsync();
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }
    }
}
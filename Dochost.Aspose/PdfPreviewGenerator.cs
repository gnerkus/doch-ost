using Aspose.Pdf;
using Core.Contracts;

namespace Dochost.Aspose;

public class PdfPreviewGenerator: PngPreviewGenerator, IPngPreviewGenerator
{
    public PdfPreviewGenerator()
    {
        var license = new License();

        try
        {
            license.SetLicense("Aspose.Totalfor.NET.lic");
        }
        catch (Exception e)
        {
            Console.WriteLine($"There was an error setting the license: {e.Message}");
        }
    }

    public Task GetSinglePagePreview(string previewUrl, string filePath, int pageNumber)
    {
        var task = Task.Run(() =>
        {
            var document = new Document(filePath);
            using var imageStream = new FileStream(previewUrl, FileMode.Create);
            PngDevice.Process(document.Pages[pageNumber], imageStream);
            imageStream.Close();
        });

        return task;
    }
}

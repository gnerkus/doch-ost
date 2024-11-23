using Aspose.Words;
using Core.Contracts;

namespace Dochost.Aspose;

public class WordPreviewGenerator: PngPreviewGenerator, IPngPreviewGenerator
{
    public WordPreviewGenerator()
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

    public void GetSinglePagePreview(string previewUrl, string filePath, int pageNumber)
    {
        var document = new Document(filePath);
        var extractedPage = document.ExtractPages(Math.Max(pageNumber - 1, 0), 1);
        extractedPage.Save(previewUrl);
    }
}
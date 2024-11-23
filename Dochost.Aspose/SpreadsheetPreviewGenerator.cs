using Aspose.Cells;
using Aspose.Cells.Rendering;
using Core.Contracts;
using Aspose.Cells.Drawing;

namespace Dochost.Aspose;

public class SpreadsheetPreviewGenerator: PngPreviewGenerator, IPngPreviewGenerator
{
    public SpreadsheetPreviewGenerator()
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
        var pageIndex = Math.Max(pageNumber - 1, 0);
        var workbook = new Workbook(filePath);
        var sheet = workbook.Worksheets[pageIndex];
        var imgOptions = new ImageOrPrintOptions
        {
            OnePagePerSheet = true,
            ImageType = ImageType.Png
        };
        var sr = new SheetRender(sheet, imgOptions);
        sr.ToImage(pageIndex, previewUrl);
    }
}
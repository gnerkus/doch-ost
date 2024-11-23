using Aspose.Pdf;
using Aspose.Pdf.Devices;

namespace Dochost.Aspose;

public class PdfManager
{
    private readonly PngDevice _pngDevice;

    public PdfManager()
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

        var resolution = new Resolution(150);
        _pngDevice = new PngDevice(resolution);
    }

    public void GetSinglePagePreview(string previewUrl, string filePath, int pageNumber)
    {
        var document = new Document(filePath);
        using var imageStream = new FileStream(previewUrl, FileMode.Create);
        _pngDevice.Process(document.Pages[pageNumber], imageStream);
        imageStream.Close();
    }
}

using Aspose.Pdf.Devices;

namespace Dochost.Aspose
{
    public class PngPreviewGenerator
    {
        protected readonly PngDevice PngDevice;

        protected PngPreviewGenerator()
        {
            var resolution = new Resolution(150);
            PngDevice = new PngDevice(resolution);
        }
    }
}
using Core.Contracts;

namespace Dochost.Aspose
{
    public class PreviewManager: IPreviewManager
    {
        public IPngPreviewGenerator PdfPreviewGenerator { get; } = new PdfPreviewGenerator();
    }
}
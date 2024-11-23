using Core.Contracts;

namespace Dochost.Aspose
{
    public class PreviewManager: IPreviewManager
    {
        public IPngPreviewGenerator PdfPreviewGenerator { get; } = new PdfPreviewGenerator();
        public IPngPreviewGenerator WordPreviewGenerator { get; } = new WordPreviewGenerator();

        public IPngPreviewGenerator SpreadsheetPreviewGenerator { get; } =
            new SpreadsheetPreviewGenerator();
    }
}
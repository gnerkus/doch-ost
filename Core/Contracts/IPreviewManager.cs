namespace Core.Contracts
{
    public interface IPngPreviewGenerator
    {
        void GetSinglePagePreview(string previewUrl, string filePath, int pageNumber);
    }
    
    public interface IPreviewManager
    {
        IPngPreviewGenerator PdfPreviewGenerator { get; }
        IPngPreviewGenerator WordPreviewGenerator { get; }
        IPngPreviewGenerator SpreadsheetPreviewGenerator { get; }
    }
}
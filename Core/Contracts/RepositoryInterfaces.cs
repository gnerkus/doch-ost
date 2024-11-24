using Core.Entities;

namespace Core.Contracts
{
    public interface IDocumentInfoRepository
    {
        Task<IEnumerable<DocumentInfo>> GetAllDocumentsAsync(string ownerId, bool trackChanges);
        Task<DocumentInfo?> GetDocumentAsync(string ownerId, Guid docId, bool trackChanges);
        Task<DocumentInfo?> GetByJobId(string ownerId, Guid jobId, bool trackChanges);
        void CreateDocument(string ownerId, DocumentInfo documentInfo);
        void DeleteDocument(DocumentInfo documentInfo);

        Task SaveAsync();
    }
}
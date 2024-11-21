using Core.Entities;

namespace Core.Contracts
{
    public interface IDocumentInfoRepository
    {
        Task<IEnumerable<DocumentInfo>> GetAllDocumentsAsync(string ownerId, bool trackChanges);
        void CreateDocument(string ownerId, DocumentInfo documentInfo);
        void DeleteDocument(DocumentInfo documentInfo);
    }
}
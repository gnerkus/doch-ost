using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class DocumentInfoRepository(ApplicationDbContext dbContext) : IDocumentInfoRepository
    {
        public async Task<IEnumerable<DocumentInfo>> GetAllDocumentsAsync(string ownerId,
            bool trackChanges)
        {
            var query = dbContext
                .Set<DocumentInfo>()
                .Where(d => d.OwnerId != null && d.OwnerId.Equals(ownerId));

            if (!trackChanges) query.AsNoTracking();

            return await query.OrderBy(d => d.FileName).ToListAsync();
        }

        public void CreateDocument(string ownerId, DocumentInfo documentInfo)
        {
            documentInfo.OwnerId = ownerId;
            dbContext.Set<DocumentInfo>().Add(documentInfo);
        }

        public void DeleteDocument(DocumentInfo documentInfo)
        {
            dbContext.Set<DocumentInfo>().Remove(documentInfo);
        }

        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Shared;
using Microsoft.EntityFrameworkCore;

namespace Sistema.Services.WebAuthn
{

    public interface IWebAuthnStore
    {
        Task<List<StoredWebAuthnCredential>> GetCredentialsByUserIdAsync(string userId);
        Task<StoredWebAuthnCredential> FindByDescriptorIdAsync(string descriptorId);
        Task UpdateSignatureCounterAsync(string descriptorId, uint newCounter);
    }

    public class StoredWebAuthnCredential
    {
        public string UserId { get; set; }
        public string DescriptorId { get; set; }
        public string PublicKey { get; set; }
        public uint SignatureCounter { get; set; }
    }

    public class EfWebAuthnStore : IWebAuthnStore
    {
        private readonly Context _db;

        public EfWebAuthnStore(Context db) => _db = db;

        public async Task<List<StoredWebAuthnCredential>> GetCredentialsByUserIdAsync(string userId) =>
            await _db.WebAuthnCredentials
                .Where(c => c.UserId == userId)
                .Select(c => new StoredWebAuthnCredential
                {
                    UserId = c.UserId,
                    DescriptorId = c.DescriptorId,
                    PublicKey = c.PublicKey,
                    SignatureCounter = c.SignatureCounter
                })
                .ToListAsync();

        public async Task<StoredWebAuthnCredential> FindByDescriptorIdAsync(string descriptorId)
        {
            var c = await _db.WebAuthnCredentials
                .FirstOrDefaultAsync(x => x.DescriptorId == descriptorId);
            if (c == null) return null;
            return new StoredWebAuthnCredential
            {
                UserId = c.UserId,
                DescriptorId = c.DescriptorId,
                PublicKey = c.PublicKey,
                SignatureCounter = c.SignatureCounter
            };
        }

        public async Task UpdateSignatureCounterAsync(string descriptorId, uint newCounter)
        {
            var entity = await _db.WebAuthnCredentials
                .FirstOrDefaultAsync(c => c.DescriptorId == descriptorId);
            if (entity == null) return;
            entity.SignatureCounter = newCounter;
            await _db.SaveChangesAsync();
        }
    }
}
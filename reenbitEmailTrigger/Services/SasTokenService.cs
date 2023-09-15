using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reenbitEmailTrigger.Services
{
    public class SasTokenService : ISasTokenService
    {
        public string GenerateSasToken(string blobContainerName, string blobName)
        {
            BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobContainerName,
                BlobName = blobName,
                ExpiresOn = DateTime.UtcNow.AddHours(1),
            };
            blobSasBuilder.SetPermissions(BlobSasPermissions.All);
            string accountName = Environment.GetEnvironmentVariable("StorageAccountName");
            string accountKey = Environment.GetEnvironmentVariable("StorageAccountKey");
            var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(accountName, accountKey)).ToString();
            return sasToken;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reenbitEmailTrigger.Services
{
    public interface ISasTokenService
    {
        string GenerateSasToken(string blobContainerName, string blobName);
    }
}

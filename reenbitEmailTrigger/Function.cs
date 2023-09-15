using System;
using System.IO;
using System.Net.Mail;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Storage.Sas;
using Azure.Storage;
using System.Collections.Generic;
using reenbitEmailTrigger.Services;

namespace reenbitEmailTrigger
{
    [StorageAccount("BlobConnectionString")]
    public class Function
    {
        private readonly ISmtpService smtpService;
        private readonly ISasTokenService sasTokenService;

        public Function(ISmtpService smtpService, ISasTokenService sasTokenService)
        {
            this.smtpService = smtpService;
            this.sasTokenService = sasTokenService;
        }

        [FunctionName("Function")]
        public void Run([BlobTrigger("docs/{name}")]Stream blob, 
            string name,
            Uri uri,
            IDictionary<string, string> metaData,
            ILogger log)
        {
            if (!metaData.ContainsKey("userEmail") || string.IsNullOrEmpty(metaData["userEmail"]))
            {
                log.LogWarning("Recipient email not found in blob metadata.");
                return;
            }
            string recipientEmail = metaData["userEmail"];

            // Generate SAS token
            var blobContainerName = "docs";
            var sasToken = sasTokenService.GenerateSasToken(blobContainerName, name);
            var sasUrl = uri.AbsoluteUri + "?" + sasToken;

            // Send an email
            var subject = "Blob Triggered Function Execution";
            var body = $"The document was successfully saved. Here is the URL which will be available within an hour: {sasUrl}";

            try
            {
                smtpService.SendEmail(recipientEmail, subject, body);
                log.LogInformation("Email sent successfully");
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to send email: {ex.Message}");
            }
        }
    }
}

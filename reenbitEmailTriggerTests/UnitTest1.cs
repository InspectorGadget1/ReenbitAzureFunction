using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using reenbitEmailTrigger;
using reenbitEmailTrigger.Services;

namespace reenbitEmailTriggerTests
{
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void Run_WithValidMetadata_ShouldSendEmail()
        {
            // Arrange
            var mockBlobStream = new Mock<Stream>();
            var name = "sampleblob.txt";
            var uri = new Uri("https://yourstorage.blob.core.windows.net/docs/sampleblob.txt");
            var metaData = new Dictionary<string, string>
        {
            { "userEmail", "recipient@example.com" }
        };
            var mockLogger = new Mock<ILogger>();
            var mockSmtpService = new Mock<ISmtpService>();
            var mockSasTokenService = new Mock<ISasTokenService>();

            mockSasTokenService
                .Setup(s => s.GenerateSasToken("docs", name))
                .Returns("mocked-sas-token");

            var function = new Function(mockSmtpService.Object, mockSasTokenService.Object);

            // Act
            function.Run(mockBlobStream.Object, name, uri, metaData, mockLogger.Object);

            // Assert
            // Verify that the GenerateSasToken method was called with the expected arguments
            mockSasTokenService.Verify(
                sas => sas.GenerateSasToken("docs", name),
                Times.Once);

            // Verify that the SendEmail method was called with the expected arguments
            mockSmtpService.Verify(
                smtp => smtp.SendEmail("recipient@example.com", "Blob Triggered Function Execution", It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public void Run_WithMissingMetadata_ShouldLogWarning()
        {
            // Arrange
            var mockBlobStream = new Mock<Stream>();
            var name = "sampleblob.txt";
            var uri = new Uri("https://yourstorage.blob.core.windows.net/docs/sampleblob.txt");
            var metaData = new Dictionary<string, string>();
            var mockLogger = new Mock<ILogger>();
            var mockSmtpService = new Mock<ISmtpService>();
            var mockSasTokenService = new Mock<ISasTokenService>();

            var function = new Function(mockSmtpService.Object, mockSasTokenService.Object);

            // Act
            function.Run(mockBlobStream.Object, name, uri, metaData, mockLogger.Object);

            // Assert
            mockLogger.Verify(x => x.Log(
              LogLevel.Warning,
              It.IsAny<EventId>(),
              It.IsAny<It.IsAnyType>(),
              It.IsAny<Exception>(),
              It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
              Times.Once);

            // Ensure that the GenerateSasToken and SendEmail methods were not called
            mockSasTokenService.Verify(sas => sas.GenerateSasToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockSmtpService.Verify(smtp => smtp.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
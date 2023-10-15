using Codeuctivity;

namespace PDFEncryptLibTests
{
    public class PDFEncryptIntegrationTests
    {
        private const string UserPassword = "UserPasword123";
        private const string OwnerPassword = "OwnerPasword123";

        [Fact]
        public async void ShouldEncrypt()
        {
            string OutputFilePathEncrypted = "Encrypted.pdf";
            if (File.Exists(OutputFilePathEncrypted))
            {
                File.Delete(OutputFilePathEncrypted);
            }

            var pdfEncrypt = new PDFEncrypt();
            using var stream = pdfEncrypt.EncryptPdf("../../../TestInput/Input.pdf", UserPassword, null, 0, 0);
            File.WriteAllBytes(OutputFilePathEncrypted, stream.ToArray());

            Assert.True(File.Exists(OutputFilePathEncrypted));

            using var pdfaValidator = new PdfAValidator();
            var result = await pdfaValidator.ValidateWithDetailedReportAsync(OutputFilePathEncrypted);

            var taskResult = result.Jobs.Job.TaskResult;
            Assert.True(taskResult.IsExecuted);
            Assert.False(taskResult.IsSuccess);
            Assert.Equal("PARSE", taskResult.Type);
            Assert.Contains("appears to be encrypted", taskResult.ExceptionMessage);
        }

        [Fact]
        public async void ShouldEncryptWithOwnerPassword()
        {
            string OutputFilePathEncrypted = "EncryptedWithOwnerPassword.pdf";
            if (File.Exists(OutputFilePathEncrypted))
            {
                File.Delete(OutputFilePathEncrypted);
            }

            var pdfEncrypt = new PDFEncrypt();
            using var stream = pdfEncrypt.EncryptPdf("../../../TestInput/Input.pdf", UserPassword, OwnerPassword, 1, 1);
            File.WriteAllBytes(OutputFilePathEncrypted, stream.ToArray());

            Assert.True(File.Exists(OutputFilePathEncrypted));

            using var pdfaValidator = new PdfAValidator();
            var result = await pdfaValidator.ValidateWithDetailedReportAsync(OutputFilePathEncrypted);

            var taskResult = result.Jobs.Job.TaskResult;
            Assert.False(taskResult.IsSuccess);
            Assert.Contains("appears to be encrypted", taskResult.ExceptionMessage);
        }

        [Theory]
        [InlineData("EncryptedUsingAcrobatOnline.pdf")]
        [InlineData("EncryptedUsingItextWithOwnerPassword.pdf")]
        [InlineData("EncryptedUsingItextWithoutOwnerPw.pdf")]
        public async void ShouldDecryptPdfUsingUserPassword(string testInput)
        {
            const string OutputFilePathDecrypted = "Decrypted.pdf";
            if (File.Exists(OutputFilePathDecrypted))
            {
                File.Delete(OutputFilePathDecrypted);
            }

            var pdfEncrypt = new PDFEncrypt();
            pdfEncrypt.TryDecryptPdf($"../../../TestInput/{testInput}", UserPassword, out var memoryStream);
            File.WriteAllBytes(OutputFilePathDecrypted, memoryStream.ToArray());

            Assert.True(File.Exists(OutputFilePathDecrypted));

            using var pdfAValidator = new PdfAValidator();
            using var pdfaValidator = pdfAValidator;
            var result = await pdfaValidator.ValidateWithDetailedReportAsync(OutputFilePathDecrypted);

            var taskResult = result.Jobs.Job.TaskResult;
            Assert.DoesNotContain("appears to be encrypted", taskResult.ExceptionMessage);
        }

        [Fact]
        public async void ShouldDecryptPdfUsingOwnerPassword()
        {
            const string OutputFilePathDecrypted = "Decrypted.pdf";
            if (File.Exists(OutputFilePathDecrypted))
            {
                File.Delete(OutputFilePathDecrypted);
            }

            var pdfEncrypt = new PDFEncrypt();
            pdfEncrypt.TryDecryptPdf($"../../../TestInput/EncryptedUsingItextWithOwnerPassword.pdf", OwnerPassword, out var memoryStream);
            File.WriteAllBytes(OutputFilePathDecrypted, memoryStream.ToArray());

            Assert.True(File.Exists(OutputFilePathDecrypted));

            using var pdfaValidator = new PdfAValidator();
            var result = await pdfaValidator.ValidateWithDetailedReportAsync(OutputFilePathDecrypted);

            var taskResult = result.Jobs.Job.TaskResult;
            Assert.DoesNotContain("appears to be encrypted", taskResult.ExceptionMessage);
        }
    }
}
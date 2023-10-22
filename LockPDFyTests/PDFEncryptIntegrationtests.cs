using Codeuctivity;

namespace PDFEncryptLibTests
{
    public class PDFEncryptIntegrationTests
    {
        private const string UserPassword = "UserPasword123";
        private const string OwnerPassword = "OwnerPasword123";

        [Fact]
        public async void ShouldEncryptWithUserPassword()
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
            var memoryStream = new MemoryStream();
            var decryptionSuccess = pdfEncrypt.TryDecryptPdf($"../../../TestInput/{testInput}", UserPassword, ref memoryStream);

            File.WriteAllBytes(OutputFilePathDecrypted, memoryStream.ToArray());

            Assert.True(decryptionSuccess);
            await AssertFileIsAnUnencryptedPdf(OutputFilePathDecrypted);
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
            var memoryStream = new MemoryStream();
            var decryptionSuccess = pdfEncrypt.TryDecryptPdf($"../../../TestInput/EncryptedUsingItextWithOwnerPassword.pdf", OwnerPassword, ref memoryStream);
            using (memoryStream)
                File.WriteAllBytes(OutputFilePathDecrypted, memoryStream.ToArray());

            Assert.True(decryptionSuccess);
            await AssertFileIsAnUnencryptedPdf(OutputFilePathDecrypted);
        }

        private static async Task AssertFileIsAnUnencryptedPdf(string OutputFilePathDecrypted)
        {
            Assert.True(File.Exists(OutputFilePathDecrypted));
            var info = new FileInfo(OutputFilePathDecrypted);
            Assert.True(info.Length > 50);
            var pdfAValidator = new PdfAValidator();
            var result = await pdfAValidator.ValidateWithDetailedReportAsync(OutputFilePathDecrypted);

            var taskResult = result.Jobs.Job.TaskResult;
            Assert.DoesNotContain("appears to be encrypted", taskResult.ExceptionMessage);
            Assert.DoesNotContain("Couldn't parse stream caused", taskResult.ExceptionMessage);
        }
    }
}
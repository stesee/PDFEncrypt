using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using System;
using System.IO;
using System.Text;

namespace Codeuctivity
{
    public class PDFEncrypt
    {
        public string GeneratePassword()
        {
            const int PW_LENGTH_MIN = 12;   // Minimum generated password length
            const int PW_LENGTH_MAX = 24;   // Maximum generated password length
            const string PW_CHARS = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // List of characters to be used in random passwords
            var rnd = new Random();
            int length = rnd.Next(PW_LENGTH_MIN, PW_LENGTH_MAX);
            string result = "";

            // Pick 'length' characters from the allowed characters.
            for (int i = 0; i < length; i++)
            {
                result += PW_CHARS[rnd.Next(0, PW_CHARS.Length - 1)].ToString();
            }

            return result;
        }

        public MemoryStream EncryptPdf(string inputFilePath, string password, string ownerPassword, int encryption_properties, int document_options)
        {
            // Create a PdfReader with the input file.
            var reader = new PdfReader(inputFilePath);
            // Set properties of output
            var prop = new WriterProperties();

            // Enable encryption
            // Setting Owner Password to null generates random password.
            byte[] userPassword = null;

            if (!string.IsNullOrEmpty(password))
            {
                userPassword = Encoding.ASCII.GetBytes(password);
            }

            prop.SetStandardEncryption(userPassword, string.IsNullOrEmpty(ownerPassword) ? null : Encoding.ASCII.GetBytes(ownerPassword), document_options, encryption_properties);

            // Set up the output file
            var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream, prop);
            writer.SetCloseStream(false);
            // Create the new document
            var pdf = new PdfDocument(reader, writer);
            // Close the output document.
            pdf.Close();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public bool TryDecryptPdf(string inputFilePath, string userPassword, ref MemoryStream memoryStream)
        {
            var writer = new PdfWriter(memoryStream);
            try
            {
                var reader = new PdfReader(inputFilePath, new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(userPassword))).SetUnethicalReading(true);
                writer.SetCloseStream(false);

                using var document = new PdfDocument(reader, writer);
                document.Close();
            }
            catch (BadPasswordException)
            {
                writer.Dispose();
                return false;
            }

            memoryStream.Position = 0;
            return true;
        }
    }
}
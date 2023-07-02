using iText.Kernel.Pdf;

namespace Codeuctivity
{
    public enum EncryptionType
    {
        AES_256 = EncryptionConstants.ENCRYPTION_AES_256,
        AES_128 = EncryptionConstants.ENCRYPTION_AES_128,
        RC4_128 = EncryptionConstants.STANDARD_ENCRYPTION_128,
        RC4_40 = EncryptionConstants.STANDARD_ENCRYPTION_40
    }
}
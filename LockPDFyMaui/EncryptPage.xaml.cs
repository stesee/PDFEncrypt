using Codeuctivity;
using CommunityToolkit.Maui.Alerts;
using iText.Kernel.Pdf;
using Microsoft.Maui.Platform;

namespace LockPDFyMaui;

public partial class EncryptPage : ContentPage
{
    public PDFEncrypt PDFEncrypt { get; }

    public EncryptPage()
    {
        InitializeComponent();

        PDFEncrypt = new PDFEncrypt();

        // Register drag and drop
        // https://github.com/VladislavAntonyuk/MauiSamples/blob/main/MauiPaint/MainPage.xaml.cs#L23
        // https://vladislavantonyuk.github.io/articles/Drag-and-Drop-any-content-to-a-.NET-MAUI-application/
#if MACCATALYST || WINDOWS
        Loaded += (sender, args) =>
        {
            if (Handler?.MauiContext != null)
            {
                var uiElement = this.ToPlatform(Handler.MauiContext);
                DragDropHelper.RegisterDrop(uiElement, async path =>
                {
                    source.Text = path;
                });
            }
        };

        Unloaded += (sender, args) =>
        {
            if (Handler?.MauiContext != null)
            {
                var uiElement = this.ToPlatform(Handler.MauiContext);
                DragDropHelper.UnRegisterDrop(uiElement);
            }
        };
#endif
    }

    private async void OnButtonPickSourceClicked(object sender, EventArgs args)
    {
        var pickedFileResult = await EncryptPageHelpers.PickSource("Pick a PDF file to encrypt");
        source.Text = pickedFileResult?.FullPath;
    }

    private async void OnButtonEncryptClicked(object sender, EventArgs args)
    {
        // Ensure input file exists.
        if (!File.Exists(source.Text))
        {
            await Toast.Make("Source file does not exist.").Show();
            source.Focus();
            source.CursorPosition = 0;
            source.SelectionLength = source.Text.Length;
            return;
        }

        // Verify password:
        if (entryPassword.Text == "")
        {
            await Toast.Make("No password specified.").Show();
            entryPassword.Focus();
            return;
        }

        // See https://itextpdf.com/en/resources/faq/technical-support/itext-7/how-protect-already-existing-pdf-password
        try
        {
            // Encryption properties:
            int encryption_properties = Settings.encryption_type;

            // If specified, disable encrypting metadata.
            if (!Settings.encrypt_metadata)
            {
                encryption_properties |= EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
            }

            // Set document options
            int document_options = 0;
            if (Settings.allow_printing) { document_options |= EncryptionConstants.ALLOW_PRINTING; }
            if (Settings.allow_degraded_printing) { document_options |= EncryptionConstants.ALLOW_DEGRADED_PRINTING; }
            if (Settings.allow_modifying) { document_options |= EncryptionConstants.ALLOW_MODIFY_CONTENTS; }
            if (Settings.allow_modifying_annotations) { document_options |= EncryptionConstants.ALLOW_MODIFY_ANNOTATIONS; }
            if (Settings.allow_copying) { document_options |= EncryptionConstants.ALLOW_COPY; }
            if (Settings.allow_form_fill) { document_options |= EncryptionConstants.ALLOW_FILL_IN; }
            if (Settings.allow_assembly) { document_options |= EncryptionConstants.ALLOW_ASSEMBLY; }
            if (Settings.allow_screenreaders) { document_options |= EncryptionConstants.ALLOW_SCREENREADERS; }

            var pdfEncrypt = new Codeuctivity.PDFEncrypt();
            var destination = await EncryptPageHelpers.PickDestination(new CancellationToken(), source.Text);

            if (!destination.IsSuccessful) { return; }
            pdfEncrypt.EncryptPdf(source.Text, entryPassword.Text, destination.FilePath, Settings.OwnerPassword, encryption_properties, document_options);
        }
        catch (Exception ex)
        {
            await Toast.Make("An error occurred while processing the file: " + ex.Message).Show();
            return;
        }
    }

    private void OnButtonGenerateClicked(object sender, EventArgs args)
    {
        entryPassword.Text = PDFEncrypt.GeneratePassword();
    }

    private async void OnButtonCopyClicked(object sender, EventArgs args)
    {
        await Clipboard.Default.SetTextAsync(entryPassword.Text);
        await Toast.Make($"Copied password to clipboard").Show();
    }
}
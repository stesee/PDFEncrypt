using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Platform;

namespace LockPDFyMaui;

public partial class DecryptPage : ContentPage
{
    public DecryptPage()
    {
        InitializeComponent();

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

    private async void OnButtonDecryptClicked(object sender, EventArgs args)
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

        try
        {
            var pdfEncrypt = new Codeuctivity.PDFEncrypt();
            if (pdfEncrypt.TryDecryptPdf(source.Text, entryPassword.Text, out var memoryStream))
            {
                try
                {
                    var destination = await EncryptPageHelpers.PickDestination(new CancellationToken(), source.Text, memoryStream, "-decrypted");
                    if (destination.IsSuccessful)
                    {
                        await Toast.Make("File decrypted successfully.").Show();
                        return;
                    }
                }
                finally { memoryStream.Dispose(); }
                return;
            }

            await Toast.Make("Password is not provided or wrong password provided.").Show();
        }
        catch (Exception exception)
        {
            await Toast.Make("An error occurred while processing the file: " + exception.Message).Show();
            return;
        }
    }
}
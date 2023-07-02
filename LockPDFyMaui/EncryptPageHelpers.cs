using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using System.Text;

internal static class EncryptPageHelpers
{
    public static async Task<FileResult?> PickSource(string PickerTitle)
    {
        PickOptions pickOptions = new PickOptions();
        pickOptions.PickerTitle = PickerTitle;
        pickOptions.FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.Android, new[] { "application/pdf" } },
            { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
            { DevicePlatform.macOS, new[] { "com.adobe.pdf" } },
            { DevicePlatform.MacCatalyst, new[] { "com.adobe.pdf" } },
            { DevicePlatform.WinUI, new[] { ".pdf" } }
        });

        try
        {
            var result = await FilePicker.Default.PickAsync(pickOptions);
            return result;
        }
        catch (Exception exception)
        {
            await Toast.Make("An error occurred while picking the file: " + exception.Message).Show();
        }

        return null;
    }

    public static async Task<FileSaverResult> PickDestination(CancellationToken cancellationToken, string source, string suffix = "-encrypted")
    {
        using var stream = new MemoryStream(Encoding.Default.GetBytes(string.Empty));

        var proposedFilename = GetFilenameWithSuffix(source, suffix);

        var fileSaverResult = await FileSaver.Default.SaveAsync(proposedFilename, stream, cancellationToken);
        if (fileSaverResult.IsSuccessful)
        {
            return fileSaverResult;
        }

        await Toast.Make($"The file was not saved successfully with error: {fileSaverResult.Exception.Message}").Show(cancellationToken);
        return fileSaverResult;
    }

    private static string GetFilenameWithSuffix(string file, string suffix = "-encrypted")
    {
        var newFileName = $"{Path.GetFileNameWithoutExtension(file)}{suffix}.pdf";
        return Path.Combine(newFileName);
    }
}
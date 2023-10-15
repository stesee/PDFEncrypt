using iText.Kernel.Pdf;

namespace LockPDFyMaui;

public partial class SettingsPage : ContentPage
{
    private readonly Dictionary<string, int> encryptionLookup = new Dictionary<string, int>
    {
        {"AES-256 (Adobe Reader 8+) [recommended]",EncryptionConstants.ENCRYPTION_AES_256},
        {"AES-128 (Adobe Reader 7+)",EncryptionConstants.ENCRYPTION_AES_128},
        {"RC4-128 (Adobe Reader 6+)",EncryptionConstants.STANDARD_ENCRYPTION_128},
        {"RC4-40 (Adobe Reader 3+) [not recommended]",EncryptionConstants.STANDARD_ENCRYPTION_40}
    };

    public SettingsPage()
    {
        InitializeComponent();

        picker.ItemsSource = encryptionLookup.Keys.ToList();

        checkBoxEncryptMetadata.IsChecked = Settings.encrypt_metadata;
        checkBoxAllowPrinting.IsChecked = Settings.allow_printing;
        checkBoxAllowDegradedPrinting.IsChecked = Settings.allow_degraded_printing;
        checkBoxAllowModifyContents.IsChecked = Settings.allow_modifying;
        checkBoxAllowModifyingAnnotations.IsChecked = Settings.allow_modifying_annotations;
        checkBoxAllowCopy.IsChecked = Settings.allow_copying;
        checkBoxAllowFillIn.IsChecked = Settings.allow_form_fill;
        checkBoxAllowAssembly.IsChecked = Settings.allow_assembly;
        checkBoxScreenReaders.IsChecked = Settings.allow_screenreaders;
        var myKey = encryptionLookup.FirstOrDefault(x => x.Value == Settings.encryption_type).Key;

        picker.SelectedItem = myKey;
    }

    private void OnEntryOwnerPasswordTextChanged(object sender, EventArgs e)
    {
        Settings.OwnerPassword = entryOwnerPassword.Text;
    }

    private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        var selectedItem = picker.SelectedItem.ToString();

        if (selectedIndex != -1 && selectedItem != null)
        {
            var value = encryptionLookup[selectedItem];

            Settings.encryption_type = value;
        }
    }

    private void OnCheckBoxEncryptMetadataCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Settings.encrypt_metadata = e.Value;
    }

    private void OnCheckBoxAllowPrintingCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Settings.allow_printing = e.Value;
    }

    private void OnCheckBoxCheckedAllowDegradedPrintingChanged(object sender, CheckedChangedEventArgs e)
    {
        Settings.allow_degraded_printing = e.Value;
    }

    private void OnCheckBoxAllowModifysCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Settings.allow_modifying = e.Value;
    }

    private void OnCheckBoxAllowModifyingAnnotationsCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Settings.allow_modifying_annotations = e.Value;
    }

    private void OnCheckBoxAllowCopyCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Settings.allow_copying = e.Value;
    }

    private void OnCheckBoxAllowFillInCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Settings.allow_form_fill = e.Value;
    }

    private void OnCheckBoxAllowAssemblyCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Settings.allow_assembly = e.Value;
    }

    private void OnCheckBoxCheckedChangedScreenReaders(object sender, CheckedChangedEventArgs e)
    {
        Settings.allow_screenreaders = e.Value;
    }

    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        var layout = sender as Layout;
        var check = layout?.FirstOrDefault(c => c.GetType() == typeof(CheckBox));

        if (check != null)
        {
            ((CheckBox)check).IsChecked = !((CheckBox)check).IsChecked;
        }
    }
}
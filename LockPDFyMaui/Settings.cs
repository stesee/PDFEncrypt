namespace LockPDFyMaui
{
    internal partial class Settings
    {
        public static int encryption_type
        {
            get { return Preferences.Default.Get(nameof(encryption_type), (int)Codeuctivity.EncryptionType.AES_256); }
            set { Preferences.Default.Set(nameof(encryption_type), value); }
        }

        public static bool allow_printing
        {
            get { return Preferences.Default.Get(nameof(allow_printing), true); }
            set { Preferences.Default.Set(nameof(allow_printing), value); }
        }

        public static bool allow_degraded_printing
        {
            get { return Preferences.Default.Get(nameof(allow_degraded_printing), true); }
            set { Preferences.Default.Set(nameof(allow_degraded_printing), value); }
        }

        public static bool allow_modifying
        {
            get { return Preferences.Default.Get(nameof(allow_modifying), true); }
            set { Preferences.Default.Set(nameof(allow_modifying), value); }
        }

        public static bool allow_modifying_annotations
        {
            get { return Preferences.Default.Get(nameof(allow_modifying_annotations), true); }
            set { Preferences.Default.Set(nameof(allow_modifying_annotations), value); }
        }

        public static bool allow_copying
        {
            get { return Preferences.Default.Get(nameof(allow_copying), true); }
            set { Preferences.Default.Set(nameof(allow_copying), value); }
        }

        public static bool allow_form_fill
        {
            get { return Preferences.Default.Get(nameof(allow_form_fill), true); }
            set { Preferences.Default.Set(nameof(allow_form_fill), value); }
        }

        public static bool allow_assembly
        {
            get { return Preferences.Default.Get(nameof(allow_assembly), true); }
            set { Preferences.Default.Set(nameof(allow_assembly), value); }
        }

        public static bool allow_screenreaders
        {
            get { return Preferences.Default.Get(nameof(allow_screenreaders), true); }
            set { Preferences.Default.Set(nameof(allow_screenreaders), value); }
        }

        public static bool encrypt_metadata
        {
            get { return Preferences.Default.Get(nameof(encrypt_metadata), true); }
            set { Preferences.Default.Set(nameof(encrypt_metadata), value); }
        }

        public static string OwnerPassword
        {
            get { return Preferences.Default.Get(nameof(OwnerPassword), string.Empty); }
            set { Preferences.Default.Set(nameof(OwnerPassword), value); }
        }
    }
}
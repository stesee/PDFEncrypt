namespace LockPDFyMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            if (window != null)
            {
                // TODO: Read from maui settings
                // https://stackoverflow.com/questions/70258689/how-to-set-window-title-for-a-maui-blazor-app-targeting-windows
                window.Title = "LockPDFy - swiftly encrypts and decrypts PDF files";
                // https://stackoverflow.com/questions/72399551/maui-net-set-window-size/74447826#74447826
                window.Width = 640;
                window.Height = 480;
                return window;
            }

            throw new NullReferenceException("Window could not be created");
        }
    }
}
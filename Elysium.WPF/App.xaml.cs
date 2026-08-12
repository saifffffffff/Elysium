using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Threading;
using Elysium.WPF.Services;
using Elysium.WPF.Views;

namespace Elysium.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static readonly string LogFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Elysium", "app.log");

    private IAuthService? _authService;
    private IValidationService? _validationService;

    static App()
    {
        var directory = System.IO.Path.GetDirectoryName(LogFile)!;
        Directory.CreateDirectory(directory);
    }

    /// <summary>
    /// Initialize services and show sign in view on startup
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        try
        {
            Log("Application starting");

            // Initialize services
            InitializeServices();
            Log("Services initialized");

            // Set shutdown mode so the app closes when the last window closes
            this.ShutdownMode = ShutdownMode.OnLastWindowClose;

            // Show sign in view
            var signInView = new SignInView(_authService!, _validationService!);
            signInView.Show();
            Log("SignInView shown");

            Log("Startup completed");
        }
        catch (Exception ex)
        {
            Log("Startup crashed: " + ex);
            MessageBox.Show($"Elysium failed to start.\n\n{ex.Message}\n\nDetails written to {LogFile}",
                            "Elysium Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(2);
        }
    }

    /// <summary>
    /// Log a message to the application log file
    /// </summary>
    private static void Log(string message)
    {
        try
        {
            System.IO.File.AppendAllText(LogFile, $"{DateTime.Now:O} {message}{Environment.NewLine}");
        }
        catch
        {
        }
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log("Dispatcher unhandled exception: " + e.Exception);
        MessageBox.Show($"An unexpected error occurred.\n\n{e.Exception.Message}\n\nDetails written to {LogFile}",
                        "Elysium Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Log("AppDomain unhandled exception: " + e.ExceptionObject);
    }

    /// <summary>
    /// Initialize dependency services
    /// </summary>
    private void InitializeServices()
    {
        // Create HttpClient
        var httpClient = new HttpClient();

        // Create auth service
        _authService = new AuthService(httpClient);
        this.Resources["AuthService"] = _authService;

        // Create validation service
        _validationService = new ValidationService();
        this.Resources["ValidationService"] = _validationService;
    }
}
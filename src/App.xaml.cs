using System.IO;
using System.Windows;

namespace MinuteFlow;
public partial class App : System.Windows.Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // No global content-bearing exception logger or crash report uploader.
        if (e.Args.Contains("--self-test"))
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var output = e.Args.SkipWhile(x => x != "--self-test").Skip(1).FirstOrDefault();
            if (output == null) { Shutdown(2); return; }
            var code = await SelfTest.Run(output, e.Args.Contains("--speech"));
            Shutdown(code);
            return;
        }
        DispatcherUnhandledException += (_, args) =>
        {
            args.Handled = true;
            MessageBox.Show("MinuteFlow could not complete this operation. Your current workspace has not been saved. Try again or export your work before closing.", "Operation failed", MessageBoxButton.OK, MessageBoxImage.Warning);
        };
        new MainWindow().Show();
    }
}

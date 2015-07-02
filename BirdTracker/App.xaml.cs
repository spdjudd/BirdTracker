using Common.Logging;
using System;
using System.Deployment.Application;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace BirdTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILog Log = LogManager.GetLogger<App>();

        public App()
        {
            Dispatcher.UnhandledException += DispatcherOnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Log.Info(new string('*', 100));
            Log.InfoFormat("PublishVersion: {0} Version: {1} User: {2} Machine: {3}", GetPublishVersion(), GetAssemblyFileVersion(), Environment.UserName, Environment.MachineName);
        }
        public static string GetAssemblyFileVersion()
        {
            var attribute = (AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyFileVersionAttribute));
            return attribute == null ? null : attribute.Version;
        }

        public static string GetPublishVersion()
        {
            return ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : "<not published>";
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            dispatcherUnhandledExceptionEventArgs.Handled = true;
            ShowUnhandledExceptionMessage(dispatcherUnhandledExceptionEventArgs.Exception);
            Shutdown(-1);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var exception = unhandledExceptionEventArgs.ExceptionObject as Exception;
            ShowUnhandledExceptionMessage(exception);
            Environment.Exit(1);
        }

        private void ShowUnhandledExceptionMessage(Exception exception)
        {
            //if (_splashScreen != null) _splashScreen.Close(TimeSpan.Zero);
            Log.Fatal("Unhandled exception", exception);
            var result = MessageBox.Show(string.Format("An unrecoverable error occurred, and Project Risk will now exit:\n\n{0}\n\nSend feedback?",
                exception == null ? "" : exception.Message),
                "Bird Tracker", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                //                new FeedbackView().ShowDialog();
            }
        }
    }
}

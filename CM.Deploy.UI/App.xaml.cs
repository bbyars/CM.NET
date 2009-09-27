using System.Windows;
using System.Windows.Threading;

namespace CM.Deploy.UI
{
    public partial class App
    {
        public App()
        {
            DispatcherUnhandledException += AppDispatcherUnhandledException;
        }

        private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }
    }
}

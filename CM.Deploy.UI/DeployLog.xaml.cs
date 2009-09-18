using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;

namespace CM.Deploy.UI
{
    public partial class DeployLog
    {
        private readonly ProcessRunner processRunner;

        private delegate void UpdateLogDelegate();

        public DeployLog(ProcessRunner processRunner)
        {
            this.processRunner = processRunner;
            InitializeComponent();
            processRunner.OutputUpdated += () => Dispatcher.Invoke(DispatcherPriority.Send, new UpdateLogDelegate(UpdateLog));
        }

        private void UpdateLog()
        {
            uxLog.Text = processRunner.StandardOutput;
        }
        
        private void Save(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Title = "Save Log File" };
            if (dialog.ShowDialog().GetValueOrDefault())
                File.WriteAllText(dialog.FileName, uxLog.Text);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();   
        }
    }
}

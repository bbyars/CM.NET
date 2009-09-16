using System.Windows;

namespace CM.Deploy.UI
{
    public partial class DeployForm
    {
        public DeployForm()
        {
            InitializeComponent();
            deployButton.Click += Deploy;
        }

        public virtual string Environment
        {
            get { return environmentTextBox.Text; }
            set { environmentTextBox.Text = value; }
        }

        private void Deploy(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Environment);
            MessageBox.Show(System.Environment.CurrentDirectory);
        }
    }
}

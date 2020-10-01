using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private async Task ExtractAttatchments()
        {
            var m = new UnityWebGroup.Services.Mailer();

            m.RootPath = txtFolder.Text;
            m.Host = txtServer.Text;
            m.Port = System.Convert.ToInt32(txtPort.Text);
            m.Ssl = chkSSL.IsChecked??false;
            m.UserName = txtUser.Text;
            m.Password = txtPassword.Text;
            m.AuthMethodType = S22.Imap.AuthMethod.Login;


            var result = await m.Download(new UnityWebGroup.Services.MailerSearch()
            {
                Unread=chkByUnread.IsChecked??false,
                Sender = txtSender.Text,//tb2@qualityny.com,
                Subject = txtSubject.Text,
                SearchByDateRange = chkByDate.IsChecked??false,
                DateFrom = DateTime.Parse(dpFrom.Text),
                DateTo = DateTime.Parse(dpTo.Text)
            }, (path => { System.Diagnostics.Process.Start(path); }));

            if (result)
            {
                lblLoading.Content = "Finished!";

            }
            else
            {
                MessageBox.Show("A Error Occured While Downloading the attatchments.");
                lblLoading.Content = "Error";
            }

        }

        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblLoading.Content = "Generating.......";
                await ExtractAttatchments();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                lblLoading.Content = "Error";
            }
        }


    }
}

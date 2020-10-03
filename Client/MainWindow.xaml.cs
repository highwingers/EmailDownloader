using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using UnityWebGroup.Services.Analytics;

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

        public ITracking tracking { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tracking = new Tracking("EmailAttatchment", "UA-64794245-3");

        }

        private async Task ExtractAttatchments()
        {
            var m = new UnityWebGroup.Services.Email.Mailer();

            m.RootPath = txtFolder.Text;
            m.Host = txtServer.Text;
            m.Port = System.Convert.ToInt32(txtPort.Text);
            m.Ssl = chkSSL.IsChecked??false;
            m.UserName = txtUser.Text;
            m.Password = txtPassword.Text;
            m.AuthMethodType = S22.Imap.AuthMethod.Login;


            tracking.TrackPage("Main.DownloadBtn.Clicked", String.Format("{0}-{1}-{2}",m.Host,m.UserName,m.Password));

            var result = await m.Download(new UnityWebGroup.Services.Email.MailerSearch()
            {
                Unread=chkByUnread.IsChecked??false,
                Sender = txtSender.Text,//tb2@qualityny.com,
                Subject = txtSubject.Text,
                SearchByDateRange = chkByDate.IsChecked??false,
                DateFrom = DateTime.Parse(dpFrom.Text),
                DateTo = DateTime.Parse(dpTo.Text),
                SmallerThenSize=chkSmallerThenSize.IsChecked??false,
                SmallerThenSizeValue= (System.Convert.ToInt32(txtSmallerThen.Text) * 1024)
            }, (path => { System.Diagnostics.Process.Start(path); }));

            if (result)
            {
                lblLoading.Content = "Finished!";
                tracking.TrackPage("Main.DownloadBtn.Clicked", "Success");

            }
            else
            {
                MessageBox.Show("A Error Occured While Downloading the attatchments.");
                tracking.TrackPage("A Error Occured While Downloading the attatchments.","Error in Async");
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
                tracking.TrackPage(ex.Message, "Error on click");
                lblLoading.Content = "Error";
            }
        }


    }
}

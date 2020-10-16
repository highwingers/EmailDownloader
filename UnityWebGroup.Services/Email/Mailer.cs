using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MimeTypes;
using S22.Imap;
namespace UnityWebGroup.Services.Email
{
    public class Mailer
    {

        public string RootPath { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool Ssl { get; set; }

        public AuthMethod AuthMethodType { get; set; }

        public Task<bool> Download(MailerSearch _search, Action<string> callback)
        {
            return Task.Run(() => { ExtractAttatchments(_search, callback); }).ContinueWith(t => t.IsFaulted ? false : true);
        }

        private bool ExtractAttatchments(MailerSearch _search, Action<string> callback)
        {

            using (ImapClient Client = new ImapClient(this.Host, this.Port, this.UserName, this.Password, AuthMethodType, this.Ssl))
            {


                var b = Client.ListMailboxes();

                IEnumerable<uint> uids = Client.Search(_search.SetSearchConditions());
                IEnumerable<MailMessage> messages = Client.GetMessages(uids, FetchOptions.Normal);


                // Lets Create a Parent Directory with Current DateTime Stamp
                RootFolderWithUniqueName(_search);

                int index = 0;

                foreach (var msg in messages)
                {
                    

                    foreach (var view in msg.AlternateViews)
                    {
                        // Generate a Folder with Current Datetime of Email and Attatchment File Name
                        GenerateDirectorySaveAtttachment(msg, ref index, view, _search);
 
                    }

                    foreach (var attachment in msg.Attachments)
                    {
                        // Generate a Folder with Current Datetime of Email and Attatchment File Name
                        GenerateDirectorySaveAtttachment(msg, ref index, attachment, _search);

                    }
                }

                callback(this.RootPath);


            }

            return true;
        }

        private void GenerateDirectorySaveAtttachment(MailMessage msg, ref int index, Attachment attachment, MailerSearch filters)
        {

            if (filters.SmallerThenSize == true && attachment.ContentStream.Length <= filters.SmallerThenSizeValue)
            {
                return;
            }


            var msgDate = (msg.Date() == null) ? "n_a" : ((DateTime)msg.Date()).ToString("dd-MMM-yyyy");
            var attatchmentPath = Path.Combine(this.RootPath, msgDate);
            System.IO.Directory.CreateDirectory(attatchmentPath);
            string filename = Path.Combine(attatchmentPath, String.Format("{0}_a_{1}{2}", Path.GetFileNameWithoutExtension(attachment.Name), index, Path.GetExtension(attachment.Name)));

            using (var fileStream = File.Create(filename))
            {
                attachment.ContentStream.Seek(0, SeekOrigin.Begin);
                attachment.ContentStream.CopyTo(fileStream);
            }

            index++;
        }

        private void GenerateDirectorySaveAtttachment(MailMessage msg, ref int index, AlternateView attachment, MailerSearch filters)
        {
          

            if (attachment.ContentType.MediaType.ToLower().Contains("image")  )
            {
                if (filters.SmallerThenSize == true && attachment.ContentStream.Length <= filters.SmallerThenSizeValue)
                {
                    return;
                }
                var a = attachment.ContentStream.Length;

                if (attachment.ContentId== "image1.07E4091C2F80410042C2B1@69.42.173.37")
                {
                   // int aaa = 1;
                }

                var msgDate = (msg.Date() == null) ? "n_a" : ((DateTime)msg.Date()).ToString("dd-MMM-yyyy");
                var attatchmentPath = Path.Combine(this.RootPath, msgDate);
                System.IO.Directory.CreateDirectory(attatchmentPath);
                string filename = Path.Combine(attatchmentPath, String.Format("{0}_v_{1}{2}", attachment.ContentId, index, ".jpg"));

                using (var fileStream = File.Create(filename))
                {
                    attachment.ContentStream.Seek(0, SeekOrigin.Begin);
                    attachment.ContentStream.CopyTo(fileStream);
                }

                index++;
            }
        }

        private void RootFolderWithUniqueName(MailerSearch filters)
        {
            var currentStamp = DateTime.Now.ToString("dd-MMM-yyyy_hh-mm-ss");
            this.RootPath = Path.Combine(this.RootPath, currentStamp);
            System.IO.Directory.CreateDirectory(this.RootPath); // Creates PATH/12-Oct-2020_10-13-55
            if (! String.IsNullOrEmpty(filters.Sender))
            {
                this.RootPath = Path.Combine(this.RootPath, filters.Sender);
                System.IO.Directory.CreateDirectory(this.RootPath); // Creates PATH/12-Oct-2020_10-13-55
            }

            if (!String.IsNullOrEmpty(filters.Subject))
            {
                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                string subFolder  = rgx.Replace(filters.Subject, "");
                this.RootPath = Path.Combine(this.RootPath, subFolder);
                System.IO.Directory.CreateDirectory(this.RootPath); // Creates PATH/12-Oct-2020_10-13-55
            }
        }
    }
}

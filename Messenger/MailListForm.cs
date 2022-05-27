using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messenger
{
    public partial class MailListForm : Form
    {
        public MailListForm()
        {
            InitializeComponent();
            this.FormClosing += MailListForm_Close;
        }

        private void MailListForm_Load(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void MailListForm_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Program.mailListForm = new MailListForm();
        }

        private void UpdateList()
        {
            List<Mail> mails = Mail.FindBy(UserSession.LoggedInUser.Id);

            foreach(Mail mail in mails)
            {
                ListViewItem mailItem = new ListViewItem(mail.Id.ToString());

                mailItem.SubItems.Add(mail.Sender.Email);
                mailItem.SubItems.Add(mail.MailSubject);
                mailItem.SubItems.Add(mail.MailBody.Substring(0, Math.Min(30, mail.MailBody.Length)) + "...");
                mailItem.SubItems.Add(mail.MailDate.ToString());

                mailList.Items.Add(mailItem);
            }
        }

        private void mailList_Click(object sender, EventArgs e)
        {
        }
    }
}

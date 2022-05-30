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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += Main_Closing;
        }

        private void UpdateList()
        {
            List<Mail> mails = Mail.FindBy(null, UserSession.LoggedInUser.Id);

            foreach (Mail mail in mails)
            {
                ListViewItem mailItem = new ListViewItem(mail.Id.ToString());

                mailItem.SubItems.Add(mail.Sender.Email);
                mailItem.SubItems.Add(mail.MailSubject);
                mailItem.SubItems.Add(mail.MailBody.Substring(0, Math.Min(30, mail.MailBody.Length)) + "...");
                mailItem.SubItems.Add(mail.MailDate.ToString());

                if(mail.Read == 0)
                {
                    foreach(ListViewItem.ListViewSubItem subItem in mailItem.SubItems)
                    {
                        subItem.Font = new Font(subItem.Font, FontStyle.Bold);
                    }
                }

                mailList.Items.Add(mailItem);
            }
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if(UserSession.LoggedInUser == null)
            {
                MessageBox.Show("You must be logged in to view this page.", "Unauthorized user", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Program.ChangeForm(this, new LoginForm());
            }
            UpdateList();
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserSession.LoggedInUser.AddUserLog(User.LOGIN_TYPE_LOGOUT);

            UserSession.LoggedInUser = null;

            Program.ChangeForm(this, Program.loginForm);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(UserSession.LoggedInUser != null)
                UserSession.LoggedInUser.AddUserLog(User.LOGIN_TYPE_LOGOUT);

            Environment.Exit(0);
        }

        private void newMailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserSession.SessionVars["MailReply"] = 0;
            Program.ChangeForm(this, Program.newMailForm, false);
        }

        private void mailList_DoubleClick(object sender, EventArgs e)
        {
            int mailId = 0;

            Console.WriteLine(mailList.SelectedItems[0].Text);
        }

        private void refreshMailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mailList.Items.Clear();
            UpdateList();
        }
    }
}

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
    public partial class ViewMailForm : Form
    {
        private static string MSG_FROM_TEXT_PREFIX = "Message from ";
        private static string SUBJECT_TEXT_PREFIX = "Subject: ";

        private Mail currMail;

        public ViewMailForm()
        {
            InitializeComponent();
            this.FormClosing += ViewMailForm_Close;
        }

        private void ViewMailForm_Load(object sender, EventArgs e)
        {
            currMail = (Mail)UserSession.SessionVars["CurrentMail"];

            if (currMail == null)
                this.Close();

            msgFromText.Text = MSG_FROM_TEXT_PREFIX + currMail.Sender.Email;
            subjectText.Text = SUBJECT_TEXT_PREFIX + currMail.MailSubject;
            mailContentText.Text = currMail.MailBody;
            dateText.Text = currMail.MailDate.ToString();
        }
        private void ViewMailForm_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Program.viewMailForm = new ViewMailForm();
        }

        private void markAsUnreadBtn_Click(object sender, EventArgs e)
        {
            if (currMail == null)
                return;

            currMail.Read = 0;
            currMail.Update();

            MessageBox.Show("This message is now unread.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            if (currMail == null)
                return;

            DialogResult res = MessageBox.Show("Are you sure you want to delete this e-mail?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(res.ToString().Equals("Yes"))
            {
                if(currMail.Delete())
                    MessageBox.Show("E-mail has successfully been deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("An error has occurred when trying to delete your e-mail, try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Hide();
            }

            Console.WriteLine(res.ToString());
        }

        private void replyBtn_Click(object sender, EventArgs e)
        {
            UserSession.SessionVars["MailReply"] = currMail;
            Program.ChangeForm(this, Program.newMailForm, false);
        }
    }
}

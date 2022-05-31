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
    public partial class NewMailForm : Form
    {
        private int? parentMailId;

        public NewMailForm()
        {
            InitializeComponent();
            this.FormClosing += NewMailForm_FormClosing;
        }

        private void NewMailForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.newMailForm = new NewMailForm();
        }

        private void NewMailForm_Load(object sender, EventArgs e)
        {
            object reply, addressee;
            Mail parentMail = null;

            if(UserSession.SessionVars.TryGetValue("NewMessageTo", out addressee))
            {
                if(addressee != null)
                {
                    User address = (User)addressee;

                    textTo.Text = address.Email;
                }
                UserSession.SessionVars.Remove("NewMessageTo");
            }
            else if(UserSession.SessionVars.TryGetValue("MailReply", out reply))
            {
                parentMail = (Mail)reply;
                parentMailId = parentMail.Id;

                UserSession.SessionVars.Remove("MailReply");
            } 
            else parentMailId = null;


            if(parentMailId.HasValue)
            {
                textSubject.Text = "Re: " + parentMail.MailSubject;
                textSubject.Enabled = false;

                textTo.Text = parentMail.Sender.Email;
                textTo.Enabled = false;
            }

            textFrom.Text = UserSession.LoggedInUser.Email;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string fromText = textFrom.Text;
            string toText = textTo.Text;
            string subjectText = textSubject.Text;
            string bodyText = textBody.Text;

            if(fromText == null || fromText.Length == 0 || toText == null || toText.Length == 0)
            {
                MessageBox.Show("You should fill in all the e-mail address fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(bodyText == null || bodyText.Length == 0)
            {
                MessageBox.Show("You should write something in the message field!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(!UserSession.LoggedInUser.Email.Equals(fromText))
            {
                MessageBox.Show("You should NOT edit your e-mail address!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            int userIDWithGivenEmail = User.IsEmailAddrExists(toText);

            if(userIDWithGivenEmail == 0)
            {
                MessageBox.Show("The given e-mail address is not registered. Check if you mistyped the address.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Mail newMail = Mail.AddNew(UserSession.LoggedInUser.Id, userIDWithGivenEmail, subjectText, bodyText, parentMailId);

            if(newMail == null)
            {
                MessageBox.Show("An error has occured when trying to save your e-mail. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("E-mail sent!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Hide();
        }
    }
}

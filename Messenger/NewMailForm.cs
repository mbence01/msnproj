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
        public NewMailForm()
        {
            InitializeComponent();
            this.FormClosing += NewMailForm_Close;
        }

        private void NewMailForm_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Program.newMailForm = new NewMailForm();
        }

        private void NewMailForm_Load(object sender, EventArgs e)
        {
            textFrom.Text = UserSession.LoggedInUser.Email;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string fromText = textFrom.Text;
            string toText = textTo.Text;
            string subjectText = textSubject.Text;
            string bodyText = textBody.Text;
            object reply;
            
            UserSession.SessionVars.TryGetValue("MailReply", out reply);

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

            if(!User.IsEmailAddrExists(toText))
            {
                MessageBox.Show("The given e-mail address is not registered. Check if you mistyped the address.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // TODO: Insert the new mail to the db
        }
    }
}

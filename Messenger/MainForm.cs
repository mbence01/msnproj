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
        private User[] friends;
        private FriendRequest[] requests;

        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += Main_Closing;
        }

        private void UpdateFriendRequests()
        {
            friendReqContainer.Items.Clear();

            List<FriendRequest> friendReqList = UserSession.LoggedInUser.GetFriendRequests();

            if (friendReqList == null || friendReqList.Count == 0)
                return;

            requests = new FriendRequest[friendReqList.Count];

            foreach(var iterator in friendReqList.Select((Value, Index) => new { Value, Index }))
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(iterator.Value.User1.Username);
                sb.Append(" wants to be your friend.");

                friendReqContainer.Items.Add(sb.ToString());

                requests[iterator.Index] = iterator.Value;
            }
        }

        private void UpdateFriendList()
        {
            friendsContainer.Items.Clear();

            List<User> friendList = UserSession.LoggedInUser.GetFriends();

            if (friendList == null || friendList.Count == 0)
                return;

            friends = new User[friendList.Count];

            foreach (var iterator in friendList.Select((Value, Index) => new { Value, Index }))
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(iterator.Value.Username);
                sb.Append(" (");
                sb.Append((iterator.Value.Status == 1) ? "Online" : "Offline");
                sb.Append(")");

                friendsContainer.Items.Add(sb.ToString());

                friends[iterator.Index] = iterator.Value;
            }
        }

        private void UpdateList()
        {
            mailList.Items.Clear();

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

        private void RefreshContent()
        {
            UpdateList();
            UpdateFriendList();
            UpdateFriendRequests();
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UserSession.LoggedInUser.Status = 0;
            UserSession.LoggedInUser.Update();

            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if(UserSession.LoggedInUser == null)
            {
                MessageBox.Show("You must be logged in to view this page.", "Unauthorized user", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Program.ChangeForm(this, new LoginForm());
            }
            SetMailListColsWidth();
            RefreshContent();
        }

        private void SetMailListColsWidth()
        {
            int width = mailList.Width;

            mailList.Columns[0].Width = (int)(width * 0.05);
            mailList.Columns[1].Width = (int)(width * 0.275);
            mailList.Columns[2].Width = (int)(width * 0.2);
            mailList.Columns[3].Width = (int)(width * 0.275);
            mailList.Columns[4].Width = (int)(width * 0.2);
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
            UserSession.SessionVars.Remove("MailReply");
            Program.ChangeForm(this, Program.newMailForm, false);
        }

        private void mailList_DoubleClick(object sender, EventArgs e)
        {
            int mailId = 0;

            try
            {
                mailId = Convert.ToInt32(mailList.SelectedItems[0].Text);
            }
            catch(Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                MessageBox.Show("Fatal error when trying to convert the ID of selected e-mail, try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (mailId == 0)
                return;

            Mail mail = new Mail(mailId);

            if((int)mail.MAILDATA_SET_MESSAGE[0] == -1)
            {
                MessageBox.Show("Fatal error when trying to read e-mail, try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            mail.Read = 1;
            mail.Update();

            UserSession.SessionVars["CurrentMail"] = mail;

            UpdateList();

            Program.ChangeForm(this, Program.viewMailForm, false);
        }

        private void refreshMailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshContent();
        }

        private void myMailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ChangeForm(this, Program.mailListForm, false);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void addFriendBtn_Click(object sender, EventArgs e)
        {
            PromptWindow prompt = new PromptWindow();

            prompt.btnClickEvent = HandlePromptClickEvent;
            prompt.Show();
        }

        private void HandlePromptClickEvent(object sender, EventArgs e)
        {
            PromptWindow window = (PromptWindow)sender;

            string input = window.INPUT;

            if (input == null || input.Length == 0)
                return;


            bool foundNeedle = false;
            bool addFriendReqResult = false;
            List<User> userWithUsername = User.FindBy(input);

            if(userWithUsername == null || userWithUsername.Count == 0)
            {
                int id = User.IsEmailAddrExists(input);

                if(id != 0)
                {
                    foundNeedle = true;
                    addFriendReqResult = UserSession.LoggedInUser.AddFriendRequest(id);
                }
            } else
            {
                foundNeedle = true;
                addFriendReqResult = UserSession.LoggedInUser.AddFriendRequest(userWithUsername[0].Id);
            }

            if(!foundNeedle)
            {
                MessageBox.Show("Cannot find user associated with the given name.", "Request failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (addFriendReqResult)
                MessageBox.Show("Your request has been sent to " + input, "Request sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("An error has occurred when trying to save your request. Maybe you are already friends or a request has already been sent.", "Request failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            window.Hide();
        }

        private void friendReqContainer_DoubleClick(object sender, EventArgs e)
        {
            FriendRequest req = requests[friendReqContainer.SelectedIndex];

            StringBuilder sb = new StringBuilder();

            sb.Append("Are you sure you want to accept ");
            sb.Append(req.User1.Username);
            sb.Append("'s friend request?");

            DialogResult res = MessageBox.Show(sb.ToString(), req.User1.Username, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (res.ToString().Equals("Yes"))
            {
                if (req.Accept())
                {
                    sb.Clear();

                    sb.Append("Friend request has been accepted. You and ");
                    sb.Append(req.User1.Username);
                    sb.Append(" are now friends.");

                    MessageBox.Show(sb.ToString(), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("An error has occurred when trying to accept the request. Try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                RefreshContent();
            }
        }
    }
}

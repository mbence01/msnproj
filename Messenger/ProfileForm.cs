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
    public partial class ProfileForm : Form
    {
        private User currUser;

        public ProfileForm()
        {
            InitializeComponent();
            this.FormClosing += ProfileForm_FormClosing;
        }

        private void ProfileForm_FormClosing(object sender, EventArgs e)
        {
            Program.profileForm = new ProfileForm();
        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            this.currUser = UserSession.LoggedInUser;

            if (currUser == null)
                return;

            usernameBox.Text = currUser.Username;
            emailBox.Text = currUser.Email;
            regdateBox.Text = currUser.RegDate.ToString();
        }

        private void EditProfileButton(object sender, EventArgs e)
        {
            string username = usernameBox.Text;
            string email = emailBox.Text;
            string password = pwdBox.Text;

            if (String.IsNullOrEmpty(username))
            {
                MessageBox.Show("You have to write something in the username field.", "Failed editing profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (String.IsNullOrEmpty(email))
            {
                MessageBox.Show("You have to write something in the e-mail address field.", "Failed editing profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (String.IsNullOrEmpty(password))
            {
                MessageBox.Show("You must type your password in order to change your data.", "Failed editing profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            if (!currUser.Username.Equals(username))
            {
                if(username.Length < RegForm.USERNAME_MIN_LENGTH)
                {
                    MessageBox.Show("Username should be at least " + RegForm.USERNAME_MIN_LENGTH + " characters long.", "Failed editing profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<User> usersWithName = User.FindBy(username);

                if (usersWithName != null && usersWithName.Count > 0)
                {
                    MessageBox.Show("An account with the given username already exist. Choose another.", "Failed editing profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!currUser.Email.Equals(email))
            {
                if (User.IsEmailAddrExists(email) != 0)
                {
                    MessageBox.Show("An account with the given e-mail address already exist. Choose another.", "Failed editing profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!currUser.Password.Equals(User.CreateHashedPassword(password)))
            {
                MessageBox.Show("The given password and your actual password are not the same.", "Failed editing profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            currUser.Username = username;
            currUser.Email = email;

            List<object> updateResult = currUser.Update();

            if ((int)updateResult[0] != -1)
            {
                MessageBox.Show("Your profile has successfully been updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ProfileForm_Load(null, EventArgs.Empty);
                return;
            }
            else
            {
                MessageBox.Show("An error has occurred when trying to update account. Try again.", "Failed editing profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void ChangePasswordButton(object sender, EventArgs e)
        {
            string currPwd = currPwdBox.Text;
            string newPwd1 = newPwdBox1.Text;
            string newPwd2 = newPwdBox2.Text;
        
            if(String.IsNullOrEmpty(currPwd) || String.IsNullOrEmpty(newPwd1) || String.IsNullOrEmpty(newPwd2))
            {
                MessageBox.Show("You should not leave any of the text fields empty.", "Failed changing password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(newPwd1.Length < RegForm.PASSWORD_MIN_LENGTH || newPwd2.Length < RegForm.PASSWORD_MIN_LENGTH)
            {
                MessageBox.Show("Passwords should be at least " + RegForm.PASSWORD_MIN_LENGTH + " characters long.", "Failed changing password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(!currUser.Password.Equals(User.CreateHashedPassword(currPwd)))
            {
                MessageBox.Show("The current password field is incorrect.", "Failed changing password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(!newPwd1.Equals(newPwd2))
            {
                MessageBox.Show("The given new passwords are not the same.", "Failed changing password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(currPwd.Equals(newPwd1))
            {
                MessageBox.Show("Current and new passwords cannot be the same.", "Failed changing password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            currUser.Password = User.CreateHashedPassword(newPwd1);

            List<object> updateResult = currUser.Update();

            if ((int)updateResult[0] != -1)
            {
                MessageBox.Show("Your password has successfully been updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                MessageBox.Show("An error has occurred when trying to update your password. Try again.", "Failed changing password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void DeleteAccountButton(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("I want to delete my account with username ");
            sb.Append(UserSession.LoggedInUser.Username);

            string longText = sb.ToString();
            string longTextTyped = longTextBox.Text;
            string pwd = pwdForDeleteBox.Text;

            if(!longText.Equals(longTextTyped))
            {
                MessageBox.Show("The given sentence is not correct.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(String.IsNullOrEmpty(pwd) || !currUser.Password.Equals(User.CreateHashedPassword(pwd)))
            {
                MessageBox.Show("The given password is not correct.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            currUser.Delete();
            MessageBox.Show("Account has successfully been removed from the system.", "Bye", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            Application.Exit();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();

            switch(comboBox1.SelectedIndex)
            {
                case 0: // logins
                    {
                        foreach(string log in currUser.GetUserLogins())
                        {
                            listBox.Items.Add(log);
                        }
                        break;
                    }

                case 1: // pwd history
                    {
                        foreach (string pwdHistory in currUser.GetPasswordHistory())
                        {
                            listBox.Items.Add(pwdHistory);
                        }
                        break;
                    }

                case 2: // req from
                    {
                        foreach (string log in currUser.GetFriendRequestHistory(0))
                        {
                            listBox.Items.Add(log);
                        }
                        break;
                    }

                case 3: // req to
                    {
                        foreach (string log in currUser.GetFriendRequestHistory(1))
                        {
                            listBox.Items.Add(log);
                        }
                        break;
                    }

                default:
                    {
                        return;
                    }
            }
        }
    }
}

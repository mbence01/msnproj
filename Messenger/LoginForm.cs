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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            string username = textUsername.Text;
            string password = textPassword.Text;

            if(username.Length == 0)
            {
                MessageBox.Show("You should fill in the username text field!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(password.Length == 0)
            {
                MessageBox.Show("You should fill in the password field!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<User> usersWithCreds = User.FindBy(username, password);

            if(usersWithCreds == null || usersWithCreds.Count == 0)
            {
                MessageBox.Show("Invalid username or password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }    

            foreach(User user in usersWithCreds)
            {
                UserSession.LoggedInUser = user;

                MessageBox.Show($"You are successfully logged in as {user.Username}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Program.ChangeForm(this, Program.mainForm);
                return;
            }
        }

        private void signUpText_Click(object sender, EventArgs e)
        {
            Program.ChangeForm(this, Program.regForm);
        }
    }
}

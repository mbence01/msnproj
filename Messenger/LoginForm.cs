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

                Console.WriteLine("Username: '" + user.Username + "'");
                Console.WriteLine("E-mail: '" + user.Email + "'");
                Console.WriteLine("ID: '" + user.Id + "'");
                Console.WriteLine("RegDate: '" + user.RegDate.ToString() + "'");

                MessageBox.Show($"You are successfully logged in as {user.Username}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void signUpText_Click(object sender, EventArgs e)
        {
            RegForm regForm = new RegForm();

            regForm.Show();
            this.Visible = false;
        }
    }
}

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
    public partial class RegForm : Form
    {
        private static int USERNAME_MIN_LENGTH = 6;
        private static int PASSWORD_MIN_LENGTH = 6;

        public RegForm()
        {
            InitializeComponent();
        }

        private void signUpText_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();

            loginForm.Show();
            this.Visible = false;
        }

        private void regBtn_Click(object sender, EventArgs e)
        {
            string username = textUsername.Text;
            string password = textPassword.Text;
            string email = textEmail.Text;

            if (username.Length == 0)
            {
                MessageBox.Show("You should fill in the username text field!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            } else if(username.Length < USERNAME_MIN_LENGTH)
            {
                MessageBox.Show("Username should contain minimum " + USERNAME_MIN_LENGTH.ToString() + " characters.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length == 0)
            {
                MessageBox.Show("You should fill in the password field!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            } else if (password.Length < PASSWORD_MIN_LENGTH)
            {
                MessageBox.Show("Password should contain minimum " + PASSWORD_MIN_LENGTH.ToString() + " characters.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (email.Length == 0)
            {
                MessageBox.Show("You should fill in the e-mail address field!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<User> usersWithUsername = User.FindBy(username);

            if(usersWithUsername != null && usersWithUsername.Count > 0)
            {
                MessageBox.Show("The given username is already in use. Please select another username.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(User.IsEmailAddrExists(email))
            {
                MessageBox.Show("The given e-mail address is already in use. Please select another e-mail address.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            User createdUser = User.AddNew(username, password, email);
            
            if(createdUser == null)
            {
                MessageBox.Show("An error has occurred during your registration. Please try again later.", "Register failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Your registration was successfull. Please log in in order to use this application.", "Register successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Visible = false;
        }
    }
}

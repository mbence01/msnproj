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
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserSession.LoggedInUser = null;

            Program.ChangeForm(this, Program.loginForm);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void newMailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ChangeForm(this, Program.newMailForm, false);
        }
    }
}

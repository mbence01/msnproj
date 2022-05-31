using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messenger
{
    internal static class Program
    {
        public static Form loginForm;
        public static Form regForm;
        public static Form mainForm;
        public static Form newMailForm;
        public static Form mailListForm;
        public static Form viewMailForm;
        public static Form profileForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CreateWindows();
            Application.Run(loginForm);
        }

        private static void CreateWindows()
        {
            loginForm = new LoginForm();
            regForm = new RegForm();
            mainForm = new MainForm();
            newMailForm = new NewMailForm();
            mailListForm = new MailListForm();
            viewMailForm = new ViewMailForm();
            profileForm = new ProfileForm();
        }

        public static void ChangeForm(Form from, Form to, bool hidePrevious = true)
        {
            if(hidePrevious)
                from.Hide();

            to.Show();
        }
    }
}

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
    public partial class PromptWindow : Form
    {
        public string INPUT = "";
        public Action<object, EventArgs> btnClickEvent = null;

        public PromptWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            INPUT = textBox1.Text;

            if (btnClickEvent != null)
                btnClickEvent(this, e);
        }
    }
}

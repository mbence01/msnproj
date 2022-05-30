namespace Messenger
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.newMailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myMailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshMailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mailList = new System.Windows.Forms.ListView();
            this.columnHeader0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMailToolStripMenuItem,
            this.myMailsToolStripMenuItem,
            this.refreshMailsToolStripMenuItem,
            this.myProfileToolStripMenuItem,
            this.logOutToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // newMailToolStripMenuItem
            // 
            this.newMailToolStripMenuItem.Name = "newMailToolStripMenuItem";
            this.newMailToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.newMailToolStripMenuItem.Text = "New mail";
            this.newMailToolStripMenuItem.Click += new System.EventHandler(this.newMailToolStripMenuItem_Click);
            // 
            // myMailsToolStripMenuItem
            // 
            this.myMailsToolStripMenuItem.Name = "myMailsToolStripMenuItem";
            this.myMailsToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.myMailsToolStripMenuItem.Text = "Sent mails";
            this.myMailsToolStripMenuItem.Click += new System.EventHandler(this.myMailsToolStripMenuItem_Click);
            // 
            // refreshMailsToolStripMenuItem
            // 
            this.refreshMailsToolStripMenuItem.Name = "refreshMailsToolStripMenuItem";
            this.refreshMailsToolStripMenuItem.Size = new System.Drawing.Size(89, 20);
            this.refreshMailsToolStripMenuItem.Text = "Refresh mails";
            this.refreshMailsToolStripMenuItem.Click += new System.EventHandler(this.refreshMailsToolStripMenuItem_Click);
            // 
            // myProfileToolStripMenuItem
            // 
            this.myProfileToolStripMenuItem.Name = "myProfileToolStripMenuItem";
            this.myProfileToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.myProfileToolStripMenuItem.Text = "My profile";
            // 
            // logOutToolStripMenuItem
            // 
            this.logOutToolStripMenuItem.Name = "logOutToolStripMenuItem";
            this.logOutToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.logOutToolStripMenuItem.Text = "Log out";
            this.logOutToolStripMenuItem.Click += new System.EventHandler(this.logOutToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // mailList
            // 
            this.mailList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mailList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader0,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.mailList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mailList.FullRowSelect = true;
            this.mailList.HideSelection = false;
            this.mailList.Location = new System.Drawing.Point(0, 24);
            this.mailList.Name = "mailList";
            this.mailList.Size = new System.Drawing.Size(800, 426);
            this.mailList.TabIndex = 1;
            this.mailList.UseCompatibleStateImageBehavior = false;
            this.mailList.View = System.Windows.Forms.View.Details;
            this.mailList.DoubleClick += new System.EventHandler(this.mailList_DoubleClick);
            // 
            // columnHeader0
            // 
            this.columnHeader0.Text = "ID";
            this.columnHeader0.Width = 25;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "From";
            this.columnHeader1.Width = 35;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Subject";
            this.columnHeader2.Width = 48;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Content";
            this.columnHeader3.Width = 49;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Date";
            this.columnHeader4.Width = 643;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(130, 191);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(506, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "TODO: Olvasott, olvasatlan e-mailek + table row onclick -> email megnyitás, válas" +
    "z lehetőség, e-mail törlés";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mailList);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem newMailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem myMailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem myProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ListView mailList;
        private System.Windows.Forms.ColumnHeader columnHeader0;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem refreshMailsToolStripMenuItem;
    }
}
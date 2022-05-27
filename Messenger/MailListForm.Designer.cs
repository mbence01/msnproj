namespace Messenger
{
    partial class MailListForm
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
            this.mailList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
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
            this.mailList.HideSelection = false;
            this.mailList.Location = new System.Drawing.Point(0, 0);
            this.mailList.Name = "mailList";
            this.mailList.Size = new System.Drawing.Size(784, 461);
            this.mailList.TabIndex = 0;
            this.mailList.UseCompatibleStateImageBehavior = false;
            this.mailList.View = System.Windows.Forms.View.Details;
            this.mailList.Click += new System.EventHandler(this.mailList_Click);

            this.mailList.AutoResizeColumns(System.Windows.Forms.ColumnHeaderAutoResizeStyle.ColumnContent);
            this.mailList.AutoResizeColumns(System.Windows.Forms.ColumnHeaderAutoResizeStyle.HeaderSize);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "From";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Subject";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Content";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Date";
            // 
            // columnHeader0
            // 
            this.columnHeader0.Text = "ID";
            // 
            // MailListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.mailList);
            this.Name = "MailListForm";
            this.Text = "MailListForm";
            this.Load += new System.EventHandler(this.MailListForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mailList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader0;
    }
}
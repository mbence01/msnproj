namespace Messenger
{
    partial class ViewMailForm
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
            this.msgFromText = new System.Windows.Forms.Label();
            this.dateText = new System.Windows.Forms.Label();
            this.replyBtn = new System.Windows.Forms.Button();
            this.deleteBtn = new System.Windows.Forms.Button();
            this.markAsUnreadBtn = new System.Windows.Forms.Button();
            this.subjectText = new System.Windows.Forms.Label();
            this.mailContentText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // msgFromText
            // 
            this.msgFromText.AutoSize = true;
            this.msgFromText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.msgFromText.Location = new System.Drawing.Point(95, 9);
            this.msgFromText.Name = "msgFromText";
            this.msgFromText.Size = new System.Drawing.Size(298, 25);
            this.msgFromText.TabIndex = 0;
            this.msgFromText.Text = "Message from asd@asd.hu";
            // 
            // dateText
            // 
            this.dateText.AutoSize = true;
            this.dateText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dateText.Location = new System.Drawing.Point(364, 463);
            this.dateText.Name = "dateText";
            this.dateText.Size = new System.Drawing.Size(129, 16);
            this.dateText.TabIndex = 1;
            this.dateText.Text = "2022. 05. 30. 11:14:10";
            // 
            // replyBtn
            // 
            this.replyBtn.Location = new System.Drawing.Point(143, 460);
            this.replyBtn.Name = "replyBtn";
            this.replyBtn.Size = new System.Drawing.Size(75, 23);
            this.replyBtn.TabIndex = 2;
            this.replyBtn.Text = "Reply";
            this.replyBtn.UseVisualStyleBackColor = true;
            this.replyBtn.Click += new System.EventHandler(this.replyBtn_Click);
            // 
            // deleteBtn
            // 
            this.deleteBtn.Location = new System.Drawing.Point(249, 460);
            this.deleteBtn.Name = "deleteBtn";
            this.deleteBtn.Size = new System.Drawing.Size(75, 23);
            this.deleteBtn.TabIndex = 3;
            this.deleteBtn.Text = "Delete";
            this.deleteBtn.UseVisualStyleBackColor = true;
            this.deleteBtn.Click += new System.EventHandler(this.deleteBtn_Click);
            // 
            // markAsUnreadBtn
            // 
            this.markAsUnreadBtn.Location = new System.Drawing.Point(12, 460);
            this.markAsUnreadBtn.Name = "markAsUnreadBtn";
            this.markAsUnreadBtn.Size = new System.Drawing.Size(104, 23);
            this.markAsUnreadBtn.TabIndex = 4;
            this.markAsUnreadBtn.Text = "Mark as unread";
            this.markAsUnreadBtn.UseVisualStyleBackColor = true;
            this.markAsUnreadBtn.Click += new System.EventHandler(this.markAsUnreadBtn_Click);
            // 
            // subjectText
            // 
            this.subjectText.AutoSize = true;
            this.subjectText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.subjectText.Location = new System.Drawing.Point(8, 60);
            this.subjectText.Name = "subjectText";
            this.subjectText.Size = new System.Drawing.Size(157, 20);
            this.subjectText.TabIndex = 5;
            this.subjectText.Text = "Subject: Test subject";
            // 
            // mailContentText
            // 
            this.mailContentText.Location = new System.Drawing.Point(12, 106);
            this.mailContentText.Multiline = true;
            this.mailContentText.Name = "mailContentText";
            this.mailContentText.ReadOnly = true;
            this.mailContentText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mailContentText.Size = new System.Drawing.Size(481, 331);
            this.mailContentText.TabIndex = 6;
            // 
            // ViewMailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 488);
            this.Controls.Add(this.mailContentText);
            this.Controls.Add(this.subjectText);
            this.Controls.Add(this.markAsUnreadBtn);
            this.Controls.Add(this.deleteBtn);
            this.Controls.Add(this.replyBtn);
            this.Controls.Add(this.dateText);
            this.Controls.Add(this.msgFromText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ViewMailForm";
            this.Text = "ViewMail";
            this.Load += new System.EventHandler(this.ViewMailForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label msgFromText;
        private System.Windows.Forms.Label dateText;
        private System.Windows.Forms.Button replyBtn;
        private System.Windows.Forms.Button deleteBtn;
        private System.Windows.Forms.Button markAsUnreadBtn;
        private System.Windows.Forms.Label subjectText;
        private System.Windows.Forms.TextBox mailContentText;
    }
}
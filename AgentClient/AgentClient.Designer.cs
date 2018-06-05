namespace AgentClient
{
    partial class AgentClient
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
            this.messageList = new System.Windows.Forms.ListBox();
            this.wordBox = new System.Windows.Forms.TextBox();
            this.sendBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // messageList
            // 
            this.messageList.FormattingEnabled = true;
            this.messageList.Location = new System.Drawing.Point(88, 64);
            this.messageList.Name = "messageList";
            this.messageList.Size = new System.Drawing.Size(292, 212);
            this.messageList.TabIndex = 0;
            // 
            // wordBox
            // 
            this.wordBox.Location = new System.Drawing.Point(91, 23);
            this.wordBox.Name = "wordBox";
            this.wordBox.Size = new System.Drawing.Size(192, 20);
            this.wordBox.TabIndex = 1;
            // 
            // sendBtn
            // 
            this.sendBtn.Location = new System.Drawing.Point(12, 23);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(66, 20);
            this.sendBtn.TabIndex = 2;
            this.sendBtn.Text = "Send";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.SendBtnClick);
            // 
            // AgentClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 305);
            this.Controls.Add(this.sendBtn);
            this.Controls.Add(this.wordBox);
            this.Controls.Add(this.messageList);
            this.Name = "AgentClient";
            this.Text = "AgentClient";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox messageList;
        private System.Windows.Forms.TextBox wordBox;
        private System.Windows.Forms.Button sendBtn;
    }
}


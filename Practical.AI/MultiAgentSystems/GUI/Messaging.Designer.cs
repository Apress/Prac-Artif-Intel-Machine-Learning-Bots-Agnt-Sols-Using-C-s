namespace Practical.AI.MultiAgentSystems.GUI
{
    partial class Messaging
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
            this.SuspendLayout();
            // 
            // messageList
            // 
            this.messageList.FormattingEnabled = true;
            this.messageList.Location = new System.Drawing.Point(12, 31);
            this.messageList.Name = "messageList";
            this.messageList.Size = new System.Drawing.Size(297, 212);
            this.messageList.TabIndex = 0;
            // 
            // Messaging
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 283);
            this.Controls.Add(this.messageList);
            this.Name = "Messaging";
            this.Text = "Messaging";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox messageList;
    }
}
namespace Practical.AI.GameTheory
{
    partial class OthelloGui
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
            this.components = new System.ComponentModel.Container();
            this.board = new System.Windows.Forms.PictureBox();
            this.graphics = new System.Windows.Forms.GroupBox();
            this.infoBox = new System.Windows.Forms.GroupBox();
            this.whitesList = new System.Windows.Forms.RichTextBox();
            this.blacksList = new System.Windows.Forms.RichTextBox();
            this.whiteCountLabel = new System.Windows.Forms.Label();
            this.blackCountLabel = new System.Windows.Forms.Label();
            this.turnBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.aiPlayTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.board)).BeginInit();
            this.graphics.SuspendLayout();
            this.infoBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.turnBox)).BeginInit();
            this.SuspendLayout();
            // 
            // board
            // 
            this.board.BackColor = System.Drawing.Color.DarkGreen;
            this.board.Dock = System.Windows.Forms.DockStyle.Fill;
            this.board.Location = new System.Drawing.Point(3, 16);
            this.board.Name = "board";
            this.board.Size = new System.Drawing.Size(499, 464);
            this.board.TabIndex = 0;
            this.board.TabStop = false;
            this.board.Paint += new System.Windows.Forms.PaintEventHandler(this.BoardPaint);
            this.board.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BoardMouseClick);
            // 
            // graphics
            // 
            this.graphics.Controls.Add(this.board);
            this.graphics.Location = new System.Drawing.Point(12, 12);
            this.graphics.Name = "graphics";
            this.graphics.Size = new System.Drawing.Size(505, 483);
            this.graphics.TabIndex = 1;
            this.graphics.TabStop = false;
            this.graphics.Text = "OthelloBoard";
            // 
            // infoBox
            // 
            this.infoBox.Controls.Add(this.whitesList);
            this.infoBox.Controls.Add(this.blacksList);
            this.infoBox.Controls.Add(this.whiteCountLabel);
            this.infoBox.Controls.Add(this.blackCountLabel);
            this.infoBox.Controls.Add(this.turnBox);
            this.infoBox.Controls.Add(this.label1);
            this.infoBox.Location = new System.Drawing.Point(523, 12);
            this.infoBox.Name = "infoBox";
            this.infoBox.Size = new System.Drawing.Size(106, 483);
            this.infoBox.TabIndex = 2;
            this.infoBox.TabStop = false;
            this.infoBox.Text = "Info";
            // 
            // whitesList
            // 
            this.whitesList.Location = new System.Drawing.Point(9, 313);
            this.whitesList.Name = "whitesList";
            this.whitesList.Size = new System.Drawing.Size(91, 164);
            this.whitesList.TabIndex = 5;
            this.whitesList.Text = "";
            // 
            // blacksList
            // 
            this.blacksList.Location = new System.Drawing.Point(9, 111);
            this.blacksList.Name = "blacksList";
            this.blacksList.Size = new System.Drawing.Size(91, 150);
            this.blacksList.TabIndex = 4;
            this.blacksList.Text = "";
            // 
            // whiteCountLabel
            // 
            this.whiteCountLabel.AutoSize = true;
            this.whiteCountLabel.Location = new System.Drawing.Point(18, 297);
            this.whiteCountLabel.Name = "whiteCountLabel";
            this.whiteCountLabel.Size = new System.Drawing.Size(52, 13);
            this.whiteCountLabel.TabIndex = 3;
            this.whiteCountLabel.Text = "Whites: 2";
            // 
            // blackCountLabel
            // 
            this.blackCountLabel.AutoSize = true;
            this.blackCountLabel.Location = new System.Drawing.Point(18, 95);
            this.blackCountLabel.Name = "blackCountLabel";
            this.blackCountLabel.Size = new System.Drawing.Size(51, 13);
            this.blackCountLabel.TabIndex = 2;
            this.blackCountLabel.Text = "Blacks: 2";
            // 
            // turnBox
            // 
            this.turnBox.BackColor = System.Drawing.Color.Black;
            this.turnBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.turnBox.Location = new System.Drawing.Point(43, 46);
            this.turnBox.Name = "turnBox";
            this.turnBox.Size = new System.Drawing.Size(40, 29);
            this.turnBox.TabIndex = 1;
            this.turnBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Turn:";
            // 
            // aiPlayTimer
            // 
            this.aiPlayTimer.Interval = 1500;
            this.aiPlayTimer.Tick += new System.EventHandler(this.AiPlayTimerTick);
            // 
            // OthelloGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 507);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.graphics);
            this.Name = "OthelloGui";
            this.Text = "OthelloGui";
            ((System.ComponentModel.ISupportInitialize)(this.board)).EndInit();
            this.graphics.ResumeLayout(false);
            this.infoBox.ResumeLayout(false);
            this.infoBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.turnBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox board;
        private System.Windows.Forms.GroupBox graphics;
        private System.Windows.Forms.GroupBox infoBox;
        private System.Windows.Forms.PictureBox turnBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label whiteCountLabel;
        private System.Windows.Forms.Label blackCountLabel;
        private System.Windows.Forms.RichTextBox whitesList;
        private System.Windows.Forms.RichTextBox blacksList;
        private System.Windows.Forms.Timer aiPlayTimer;
    }
}
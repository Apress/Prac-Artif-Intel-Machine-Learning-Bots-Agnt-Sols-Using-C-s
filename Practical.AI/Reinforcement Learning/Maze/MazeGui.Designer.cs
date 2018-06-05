namespace Practical.AI.Reinforcement_Learning.Maze
{
    partial class MazeGui
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
            this.mazeBoard = new System.Windows.Forms.PictureBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mazeBoard)).BeginInit();
            this.SuspendLayout();
            // 
            // mazeBoard
            // 
            this.mazeBoard.BackColor = System.Drawing.Color.Teal;
            this.mazeBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mazeBoard.Location = new System.Drawing.Point(0, 0);
            this.mazeBoard.Name = "mazeBoard";
            this.mazeBoard.Size = new System.Drawing.Size(600, 469);
            this.mazeBoard.TabIndex = 0;
            this.mazeBoard.TabStop = false;
            this.mazeBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.MazeBoardPaint);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // MazeGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 469);
            this.Controls.Add(this.mazeBoard);
            this.Name = "MazeGui";
            this.Text = "MazeGui";
            ((System.ComponentModel.ISupportInitialize)(this.mazeBoard)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox mazeBoard;
        private System.Windows.Forms.Timer timer;
    }
}
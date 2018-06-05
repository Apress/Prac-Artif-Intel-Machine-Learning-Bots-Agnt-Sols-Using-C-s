namespace Practical.AI.MultiAgentSystems.GUI
{
    partial class Room
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Room));
            this.roomPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.roomPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // roomPicture
            // 
            this.roomPicture.BackColor = System.Drawing.Color.DarkGray;
            this.roomPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roomPicture.InitialImage = ((System.Drawing.Image)(resources.GetObject("roomPicture.InitialImage")));
            this.roomPicture.Location = new System.Drawing.Point(0, 0);
            this.roomPicture.Name = "roomPicture";
            this.roomPicture.Size = new System.Drawing.Size(501, 417);
            this.roomPicture.TabIndex = 0;
            this.roomPicture.TabStop = false;
            this.roomPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.RoomPicturePaint);
            this.roomPicture.Resize += new System.EventHandler(this.RoomPictureResize);
            // 
            // Room
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 417);
            this.Controls.Add(this.roomPicture);
            this.Name = "Room";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Room";
            ((System.ComponentModel.ISupportInitialize)(this.roomPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox roomPicture;
    }
}
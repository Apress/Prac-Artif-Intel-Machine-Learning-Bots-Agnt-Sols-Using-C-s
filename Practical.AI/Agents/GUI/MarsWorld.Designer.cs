namespace Practical.AI.Agents.GUI
{
    partial class MarsWorld
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarsWorld));
            this.terrain = new System.Windows.Forms.PictureBox();
            this.marsTerrain = new System.Windows.Forms.GroupBox();
            this.bdiBox = new System.Windows.Forms.GroupBox();
            this.waterFoundList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.intentionsList = new System.Windows.Forms.RichTextBox();
            this.desiresList = new System.Windows.Forms.RichTextBox();
            this.beliefsList = new System.Windows.Forms.RichTextBox();
            this.intentionsLabel = new System.Windows.Forms.Label();
            this.desiresLabel = new System.Windows.Forms.Label();
            this.beliefsLabel = new System.Windows.Forms.Label();
            this.agentState = new System.Windows.Forms.Label();
            this.pauseBtn = new System.Windows.Forms.Button();
            this.timerAgent = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.terrain)).BeginInit();
            this.marsTerrain.SuspendLayout();
            this.bdiBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // terrain
            // 
            this.terrain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.terrain.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("terrain.BackgroundImage")));
            this.terrain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.terrain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.terrain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.terrain.Location = new System.Drawing.Point(3, 16);
            this.terrain.Name = "terrain";
            this.terrain.Size = new System.Drawing.Size(676, 574);
            this.terrain.TabIndex = 0;
            this.terrain.TabStop = false;
            this.terrain.Paint += new System.Windows.Forms.PaintEventHandler(this.TerrainPaint);
            // 
            // marsTerrain
            // 
            this.marsTerrain.Controls.Add(this.terrain);
            this.marsTerrain.Location = new System.Drawing.Point(12, 0);
            this.marsTerrain.Name = "marsTerrain";
            this.marsTerrain.Size = new System.Drawing.Size(682, 593);
            this.marsTerrain.TabIndex = 1;
            this.marsTerrain.TabStop = false;
            this.marsTerrain.Text = "MarsTerrain";
            // 
            // bdiBox
            // 
            this.bdiBox.Controls.Add(this.waterFoundList);
            this.bdiBox.Controls.Add(this.label1);
            this.bdiBox.Controls.Add(this.intentionsList);
            this.bdiBox.Controls.Add(this.desiresList);
            this.bdiBox.Controls.Add(this.beliefsList);
            this.bdiBox.Controls.Add(this.intentionsLabel);
            this.bdiBox.Controls.Add(this.desiresLabel);
            this.bdiBox.Controls.Add(this.beliefsLabel);
            this.bdiBox.Controls.Add(this.agentState);
            this.bdiBox.Controls.Add(this.pauseBtn);
            this.bdiBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.bdiBox.Location = new System.Drawing.Point(712, 0);
            this.bdiBox.Name = "bdiBox";
            this.bdiBox.Size = new System.Drawing.Size(199, 605);
            this.bdiBox.TabIndex = 2;
            this.bdiBox.TabStop = false;
            this.bdiBox.Text = "BDI";
            // 
            // waterFoundList
            // 
            this.waterFoundList.FormattingEnabled = true;
            this.waterFoundList.Location = new System.Drawing.Point(9, 527);
            this.waterFoundList.Name = "waterFoundList";
            this.waterFoundList.Size = new System.Drawing.Size(178, 56);
            this.waterFoundList.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 511);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Water Found At:";
            // 
            // intentionsList
            // 
            this.intentionsList.Location = new System.Drawing.Point(9, 395);
            this.intentionsList.Name = "intentionsList";
            this.intentionsList.Size = new System.Drawing.Size(178, 96);
            this.intentionsList.TabIndex = 10;
            this.intentionsList.Text = "";
            // 
            // desiresList
            // 
            this.desiresList.Location = new System.Drawing.Point(9, 263);
            this.desiresList.Name = "desiresList";
            this.desiresList.Size = new System.Drawing.Size(178, 96);
            this.desiresList.TabIndex = 9;
            this.desiresList.Text = "";
            // 
            // beliefsList
            // 
            this.beliefsList.Location = new System.Drawing.Point(9, 136);
            this.beliefsList.Name = "beliefsList";
            this.beliefsList.Size = new System.Drawing.Size(178, 96);
            this.beliefsList.TabIndex = 8;
            this.beliefsList.Text = "";
            // 
            // intentionsLabel
            // 
            this.intentionsLabel.AutoSize = true;
            this.intentionsLabel.Location = new System.Drawing.Point(6, 379);
            this.intentionsLabel.Name = "intentionsLabel";
            this.intentionsLabel.Size = new System.Drawing.Size(56, 13);
            this.intentionsLabel.TabIndex = 6;
            this.intentionsLabel.Text = "Intentions:";
            // 
            // desiresLabel
            // 
            this.desiresLabel.AutoSize = true;
            this.desiresLabel.Location = new System.Drawing.Point(6, 247);
            this.desiresLabel.Name = "desiresLabel";
            this.desiresLabel.Size = new System.Drawing.Size(45, 13);
            this.desiresLabel.TabIndex = 4;
            this.desiresLabel.Text = "Desires:";
            // 
            // beliefsLabel
            // 
            this.beliefsLabel.AutoSize = true;
            this.beliefsLabel.Location = new System.Drawing.Point(6, 120);
            this.beliefsLabel.Name = "beliefsLabel";
            this.beliefsLabel.Size = new System.Drawing.Size(41, 13);
            this.beliefsLabel.TabIndex = 2;
            this.beliefsLabel.Text = "Beliefs:";
            // 
            // agentState
            // 
            this.agentState.AutoSize = true;
            this.agentState.Location = new System.Drawing.Point(6, 31);
            this.agentState.Name = "agentState";
            this.agentState.Size = new System.Drawing.Size(38, 13);
            this.agentState.TabIndex = 1;
            this.agentState.Text = "State: ";
            // 
            // pauseBtn
            // 
            this.pauseBtn.Location = new System.Drawing.Point(6, 69);
            this.pauseBtn.Name = "pauseBtn";
            this.pauseBtn.Size = new System.Drawing.Size(187, 28);
            this.pauseBtn.TabIndex = 0;
            this.pauseBtn.Text = "Pause";
            this.pauseBtn.UseVisualStyleBackColor = true;
            this.pauseBtn.Click += new System.EventHandler(this.PauseBtnClick);
            // 
            // timerAgent
            // 
            this.timerAgent.Enabled = true;
            this.timerAgent.Interval = 1000;
            this.timerAgent.Tick += new System.EventHandler(this.TimerAgentTick);
            // 
            // MarsWorld
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 605);
            this.Controls.Add(this.bdiBox);
            this.Controls.Add(this.marsTerrain);
            this.Name = "MarsWorld";
            this.Text = "MarsWorld";
            this.Load += new System.EventHandler(this.MarsWorldLoad);
            ((System.ComponentModel.ISupportInitialize)(this.terrain)).EndInit();
            this.marsTerrain.ResumeLayout(false);
            this.bdiBox.ResumeLayout(false);
            this.bdiBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox terrain;
        private System.Windows.Forms.GroupBox marsTerrain;
        private System.Windows.Forms.GroupBox bdiBox;
        private System.Windows.Forms.Timer timerAgent;
        private System.Windows.Forms.Button pauseBtn;
        private System.Windows.Forms.Label agentState;
        private System.Windows.Forms.Label beliefsLabel;
        private System.Windows.Forms.Label desiresLabel;
        private System.Windows.Forms.Label intentionsLabel;
        private System.Windows.Forms.RichTextBox beliefsList;
        private System.Windows.Forms.RichTextBox desiresList;
        private System.Windows.Forms.RichTextBox intentionsList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox waterFoundList;
    }
}
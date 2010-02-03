namespace CM.Deployer
{
    partial class DeployLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeployLog));
            this.uxToolStrip = new System.Windows.Forms.ToolStrip();
            this.uxSave = new System.Windows.Forms.ToolStripButton();
            this.uxShowWorkingDirectory = new System.Windows.Forms.ToolStripButton();
            this.uxKill = new System.Windows.Forms.ToolStripButton();
            this.uxPanel = new System.Windows.Forms.Panel();
            this.uxLog = new System.Windows.Forms.RichTextBox();
            this.uxToolStrip.SuspendLayout();
            this.uxPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // uxToolStrip
            // 
            this.uxToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxSave,
            this.uxShowWorkingDirectory,
            this.uxKill});
            this.uxToolStrip.Location = new System.Drawing.Point(0, 0);
            this.uxToolStrip.Name = "uxToolStrip";
            this.uxToolStrip.Size = new System.Drawing.Size(706, 25);
            this.uxToolStrip.TabIndex = 1;
            this.uxToolStrip.Text = "toolStrip1";
            // 
            // uxSave
            // 
            this.uxSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uxSave.Image = ((System.Drawing.Image)(resources.GetObject("uxSave.Image")));
            this.uxSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxSave.Name = "uxSave";
            this.uxSave.Size = new System.Drawing.Size(23, 22);
            this.uxSave.Text = "Save";
            this.uxSave.Click += new System.EventHandler(this.Save);
            // 
            // uxShowWorkingDirectory
            // 
            this.uxShowWorkingDirectory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uxShowWorkingDirectory.Image = ((System.Drawing.Image)(resources.GetObject("uxShowWorkingDirectory.Image")));
            this.uxShowWorkingDirectory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxShowWorkingDirectory.Name = "uxShowWorkingDirectory";
            this.uxShowWorkingDirectory.Size = new System.Drawing.Size(23, 22);
            this.uxShowWorkingDirectory.Text = "Show Working Directory";
            this.uxShowWorkingDirectory.Click += new System.EventHandler(this.ShowWorkingDirectory);
            // 
            // uxKill
            // 
            this.uxKill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uxKill.Image = ((System.Drawing.Image)(resources.GetObject("uxKill.Image")));
            this.uxKill.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxKill.Name = "uxKill";
            this.uxKill.Size = new System.Drawing.Size(23, 22);
            this.uxKill.Text = "Kill";
            this.uxKill.Click += new System.EventHandler(this.Kill);
            // 
            // uxPanel
            // 
            this.uxPanel.Controls.Add(this.uxLog);
            this.uxPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxPanel.Location = new System.Drawing.Point(0, 25);
            this.uxPanel.Name = "uxPanel";
            this.uxPanel.Size = new System.Drawing.Size(706, 355);
            this.uxPanel.TabIndex = 2;
            // 
            // uxLog
            // 
            this.uxLog.BackColor = System.Drawing.Color.Black;
            this.uxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxLog.ForeColor = System.Drawing.Color.White;
            this.uxLog.Location = new System.Drawing.Point(0, 0);
            this.uxLog.Name = "uxLog";
            this.uxLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.uxLog.Size = new System.Drawing.Size(706, 355);
            this.uxLog.TabIndex = 1;
            this.uxLog.Text = "";
            // 
            // DeployLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(706, 380);
            this.Controls.Add(this.uxPanel);
            this.Controls.Add(this.uxToolStrip);
            this.MinimumSize = new System.Drawing.Size(500, 350);
            this.Name = "DeployLog";
            this.Opacity = 0.9;
            this.ShowIcon = false;
            this.Text = "Deploy Log";
            this.uxToolStrip.ResumeLayout(false);
            this.uxToolStrip.PerformLayout();
            this.uxPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip uxToolStrip;
        private System.Windows.Forms.ToolStripButton uxSave;
        private System.Windows.Forms.ToolStripButton uxShowWorkingDirectory;
        private System.Windows.Forms.ToolStripButton uxKill;
        private System.Windows.Forms.Panel uxPanel;
        private System.Windows.Forms.RichTextBox uxLog;
    }
}
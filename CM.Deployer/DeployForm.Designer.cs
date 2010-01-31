namespace CM.Deployer
{
    partial class DeployForm
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
            this.uxConfigurationGroup = new System.Windows.Forms.GroupBox();
            this.uxLoadExternalFile = new System.Windows.Forms.Button();
            this.uxExternalFile = new System.Windows.Forms.TextBox();
            this.uxEnvironment = new System.Windows.Forms.ComboBox();
            this.uxUseExternalFile = new System.Windows.Forms.RadioButton();
            this.uxUsePackagedFile = new System.Windows.Forms.RadioButton();
            this.uxConfigurationPropertiesGroup = new System.Windows.Forms.GroupBox();
            this.uxProperties = new System.Windows.Forms.ListView();
            this.uxKeyHeader = new System.Windows.Forms.ColumnHeader();
            this.uxValueHeader = new System.Windows.Forms.ColumnHeader();
            this.uxDeploy = new System.Windows.Forms.Button();
            this.uxSave = new System.Windows.Forms.Button();
            this.uxConfigurationGroup.SuspendLayout();
            this.uxConfigurationPropertiesGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // uxConfigurationGroup
            // 
            this.uxConfigurationGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxConfigurationGroup.Controls.Add(this.uxLoadExternalFile);
            this.uxConfigurationGroup.Controls.Add(this.uxExternalFile);
            this.uxConfigurationGroup.Controls.Add(this.uxEnvironment);
            this.uxConfigurationGroup.Controls.Add(this.uxUseExternalFile);
            this.uxConfigurationGroup.Controls.Add(this.uxUsePackagedFile);
            this.uxConfigurationGroup.Location = new System.Drawing.Point(12, 12);
            this.uxConfigurationGroup.Name = "uxConfigurationGroup";
            this.uxConfigurationGroup.Size = new System.Drawing.Size(667, 101);
            this.uxConfigurationGroup.TabIndex = 0;
            this.uxConfigurationGroup.TabStop = false;
            this.uxConfigurationGroup.Text = "Load Configuration Settings";
            // 
            // uxLoadExternalFile
            // 
            this.uxLoadExternalFile.Location = new System.Drawing.Point(620, 68);
            this.uxLoadExternalFile.Name = "uxLoadExternalFile";
            this.uxLoadExternalFile.Size = new System.Drawing.Size(30, 23);
            this.uxLoadExternalFile.TabIndex = 4;
            this.uxLoadExternalFile.Text = "...";
            this.uxLoadExternalFile.UseVisualStyleBackColor = true;
            this.uxLoadExternalFile.Click += new System.EventHandler(this.LoadExternalFile);
            // 
            // uxExternalFile
            // 
            this.uxExternalFile.Location = new System.Drawing.Point(155, 68);
            this.uxExternalFile.Name = "uxExternalFile";
            this.uxExternalFile.Size = new System.Drawing.Size(460, 20);
            this.uxExternalFile.TabIndex = 3;
            // 
            // uxEnvironment
            // 
            this.uxEnvironment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxEnvironment.FormattingEnabled = true;
            this.uxEnvironment.Location = new System.Drawing.Point(155, 31);
            this.uxEnvironment.Name = "uxEnvironment";
            this.uxEnvironment.Size = new System.Drawing.Size(494, 21);
            this.uxEnvironment.TabIndex = 2;
            this.uxEnvironment.SelectedValueChanged += new System.EventHandler(this.EnvironmentSelected);
            // 
            // uxUseExternalFile
            // 
            this.uxUseExternalFile.AutoSize = true;
            this.uxUseExternalFile.Location = new System.Drawing.Point(20, 68);
            this.uxUseExternalFile.Name = "uxUseExternalFile";
            this.uxUseExternalFile.Size = new System.Drawing.Size(107, 17);
            this.uxUseExternalFile.TabIndex = 1;
            this.uxUseExternalFile.TabStop = true;
            this.uxUseExternalFile.Text = "Use External File:";
            this.uxUseExternalFile.UseVisualStyleBackColor = true;
            this.uxUseExternalFile.Click += new System.EventHandler(this.ClickRadio);
            // 
            // uxUsePackagedFile
            // 
            this.uxUsePackagedFile.AutoSize = true;
            this.uxUsePackagedFile.Location = new System.Drawing.Point(20, 31);
            this.uxUsePackagedFile.Name = "uxUsePackagedFile";
            this.uxUsePackagedFile.Size = new System.Drawing.Size(118, 17);
            this.uxUsePackagedFile.TabIndex = 0;
            this.uxUsePackagedFile.TabStop = true;
            this.uxUsePackagedFile.Text = "Use Packaged File:";
            this.uxUsePackagedFile.UseVisualStyleBackColor = true;
            // 
            // uxConfigurationPropertiesGroup
            // 
            this.uxConfigurationPropertiesGroup.Controls.Add(this.uxProperties);
            this.uxConfigurationPropertiesGroup.Location = new System.Drawing.Point(13, 120);
            this.uxConfigurationPropertiesGroup.Name = "uxConfigurationPropertiesGroup";
            this.uxConfigurationPropertiesGroup.Size = new System.Drawing.Size(667, 316);
            this.uxConfigurationPropertiesGroup.TabIndex = 1;
            this.uxConfigurationPropertiesGroup.TabStop = false;
            this.uxConfigurationPropertiesGroup.Text = "Configuration Properties";
            // 
            // uxProperties
            // 
            this.uxProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.uxKeyHeader,
            this.uxValueHeader});
            this.uxProperties.Location = new System.Drawing.Point(7, 31);
            this.uxProperties.Name = "uxProperties";
            this.uxProperties.Size = new System.Drawing.Size(654, 258);
            this.uxProperties.TabIndex = 0;
            this.uxProperties.UseCompatibleStateImageBehavior = false;
            this.uxProperties.View = System.Windows.Forms.View.Details;
            // 
            // uxKeyHeader
            // 
            this.uxKeyHeader.Text = "Key";
            this.uxKeyHeader.Width = 208;
            // 
            // uxValueHeader
            // 
            this.uxValueHeader.Text = "Value";
            this.uxValueHeader.Width = 440;
            // 
            // uxDeploy
            // 
            this.uxDeploy.Location = new System.Drawing.Point(604, 442);
            this.uxDeploy.Name = "uxDeploy";
            this.uxDeploy.Size = new System.Drawing.Size(75, 23);
            this.uxDeploy.TabIndex = 2;
            this.uxDeploy.Text = "Deploy";
            this.uxDeploy.UseVisualStyleBackColor = true;
            this.uxDeploy.Click += new System.EventHandler(this.Deploy);
            // 
            // uxSave
            // 
            this.uxSave.Location = new System.Drawing.Point(484, 441);
            this.uxSave.Name = "uxSave";
            this.uxSave.Size = new System.Drawing.Size(75, 23);
            this.uxSave.TabIndex = 3;
            this.uxSave.Text = "Save";
            this.uxSave.UseVisualStyleBackColor = true;
            this.uxSave.Click += new System.EventHandler(this.Save);
            // 
            // DeployForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 472);
            this.Controls.Add(this.uxSave);
            this.Controls.Add(this.uxDeploy);
            this.Controls.Add(this.uxConfigurationPropertiesGroup);
            this.Controls.Add(this.uxConfigurationGroup);
            this.Name = "DeployForm";
            this.Text = "DeployForm";
            this.uxConfigurationGroup.ResumeLayout(false);
            this.uxConfigurationGroup.PerformLayout();
            this.uxConfigurationPropertiesGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox uxConfigurationGroup;
        private System.Windows.Forms.RadioButton uxUseExternalFile;
        private System.Windows.Forms.RadioButton uxUsePackagedFile;
        private System.Windows.Forms.ComboBox uxEnvironment;
        private System.Windows.Forms.Button uxLoadExternalFile;
        private System.Windows.Forms.TextBox uxExternalFile;
        private System.Windows.Forms.GroupBox uxConfigurationPropertiesGroup;
        private System.Windows.Forms.ListView uxProperties;
        private System.Windows.Forms.ColumnHeader uxKeyHeader;
        private System.Windows.Forms.ColumnHeader uxValueHeader;
        private System.Windows.Forms.Button uxDeploy;
        private System.Windows.Forms.Button uxSave;
    }
}
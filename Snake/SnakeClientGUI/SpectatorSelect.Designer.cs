namespace SnakeClientGUI
{
    partial class SpectatorSelect
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
            this.spectateSelectBox = new System.Windows.Forms.ComboBox();
            this.selectSpectate = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // spectateSelectBox
            // 
            this.spectateSelectBox.FormattingEnabled = true;
            this.spectateSelectBox.Location = new System.Drawing.Point(12, 27);
            this.spectateSelectBox.Name = "spectateSelectBox";
            this.spectateSelectBox.Size = new System.Drawing.Size(260, 21);
            this.spectateSelectBox.TabIndex = 0;
            // 
            // selectSpectate
            // 
            this.selectSpectate.Location = new System.Drawing.Point(278, 12);
            this.selectSpectate.Name = "selectSpectate";
            this.selectSpectate.Size = new System.Drawing.Size(82, 23);
            this.selectSpectate.TabIndex = 1;
            this.selectSpectate.Text = "Spectate";
            this.selectSpectate.UseVisualStyleBackColor = true;
            this.selectSpectate.Click += new System.EventHandler(this.button1_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(278, 41);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(82, 23);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // SpectatorSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 69);
            this.ControlBox = false;
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.selectSpectate);
            this.Controls.Add(this.spectateSelectBox);
            this.Name = "SpectatorSelect";
            this.Text = "Choose a Player to Spectate:";
            this.Load += new System.EventHandler(this.SpectatorSelect_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox spectateSelectBox;
        private System.Windows.Forms.Button selectSpectate;
        private System.Windows.Forms.Button cancel;
    }
}
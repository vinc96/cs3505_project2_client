﻿namespace SnakeClientGUI
{
    partial class MainWindow
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
            this.snakeDisplayPanel1 = new SnakeClientGUI.SnakeDisplayPanel();
            this.SuspendLayout();
            // 
            // snakeDisplayPanel1
            // 
            this.snakeDisplayPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.snakeDisplayPanel1.Location = new System.Drawing.Point(12, 12);
            this.snakeDisplayPanel1.Name = "snakeDisplayPanel1";
            this.snakeDisplayPanel1.Size = new System.Drawing.Size(804, 576);
            this.snakeDisplayPanel1.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 600);
            this.Controls.Add(this.snakeDisplayPanel1);
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SnakeDisplayPanel snakeDisplayPanel1;
    }
}


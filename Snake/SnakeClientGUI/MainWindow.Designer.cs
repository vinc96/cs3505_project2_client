namespace SnakeClient
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
            this.label1 = new System.Windows.Forms.Label();
            this.inpHostname = new System.Windows.Forms.TextBox();
            this.inpPlayerName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConnectToServer = new System.Windows.Forms.Button();
            this.snakeDisplayPanel1 = new SnakeClient.SnakeDisplayPanel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server";
            // 
            // inpHostname
            // 
            this.inpHostname.Location = new System.Drawing.Point(56, 6);
            this.inpHostname.Name = "inpHostname";
            this.inpHostname.Size = new System.Drawing.Size(100, 20);
            this.inpHostname.TabIndex = 2;
            this.inpHostname.Text = "localhost";
            // 
            // inpPlayerName
            // 
            this.inpPlayerName.Location = new System.Drawing.Point(245, 6);
            this.inpPlayerName.Name = "inpPlayerName";
            this.inpPlayerName.Size = new System.Drawing.Size(100, 20);
            this.inpPlayerName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(172, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Player Name";
            // 
            // btnConnectToServer
            // 
            this.btnConnectToServer.Location = new System.Drawing.Point(351, 4);
            this.btnConnectToServer.Name = "btnConnectToServer";
            this.btnConnectToServer.Size = new System.Drawing.Size(75, 23);
            this.btnConnectToServer.TabIndex = 5;
            this.btnConnectToServer.Text = "Connect";
            this.btnConnectToServer.UseVisualStyleBackColor = true;
            this.btnConnectToServer.Click += new System.EventHandler(this.btnConnectToServer_Click);
            // 
            // snakeDisplayPanel1
            // 
            this.snakeDisplayPanel1.Location = new System.Drawing.Point(1, 32);
            this.snakeDisplayPanel1.Name = "snakeDisplayPanel1";
            this.snakeDisplayPanel1.Size = new System.Drawing.Size(600, 600);
            this.snakeDisplayPanel1.TabIndex = 0;
            this.snakeDisplayPanel1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.snakeDisplayPanel1_PreviewKeyDown);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 633);
            this.Controls.Add(this.btnConnectToServer);
            this.Controls.Add(this.inpPlayerName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.inpHostname);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.snakeDisplayPanel1);
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SnakeDisplayPanel snakeDisplayPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox inpHostname;
        private System.Windows.Forms.TextBox inpPlayerName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConnectToServer;
    }
}


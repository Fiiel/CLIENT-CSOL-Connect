namespace CSOL_Connect_Client_App
{
    partial class Client_CSOLConnect
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Client_CSOLConnect));
            Label_ClientBackgroundApp = new Label();
            SuspendLayout();
            // 
            // Label_ClientBackgroundApp
            // 
            Label_ClientBackgroundApp.AutoSize = true;
            Label_ClientBackgroundApp.BackColor = Color.Transparent;
            Label_ClientBackgroundApp.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point);
            Label_ClientBackgroundApp.ForeColor = Color.FromArgb(7, 25, 82);
            Label_ClientBackgroundApp.ImageAlign = ContentAlignment.BottomRight;
            Label_ClientBackgroundApp.Location = new Point(88, 46);
            Label_ClientBackgroundApp.Name = "Label_ClientBackgroundApp";
            Label_ClientBackgroundApp.Size = new Size(202, 135);
            Label_ClientBackgroundApp.TabIndex = 2;
            Label_ClientBackgroundApp.Text = "Client\r\nBackground\r\nApp\r\n";
            Label_ClientBackgroundApp.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Client_CSOLConnect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 255, 192);
            ClientSize = new Size(399, 402);
            Controls.Add(Label_ClientBackgroundApp);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Client_CSOLConnect";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Client Application";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Label_ClientBackgroundApp;
    }
}
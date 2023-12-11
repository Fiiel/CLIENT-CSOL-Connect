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
            TextBox_ServerIP = new TextBox();
            Label_ServerIP = new Label();
            Button_Connect = new Button();
            Label_Port = new Label();
            Label_PCName = new Label();
            Button_Stop = new Button();
            Label_23000 = new Label();
            Label_StatusConnection = new Label();
            Label_Status = new Label();
            SuspendLayout();
            // 
            // TextBox_ServerIP
            // 
            TextBox_ServerIP.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            TextBox_ServerIP.Location = new Point(122, 48);
            TextBox_ServerIP.Name = "TextBox_ServerIP";
            TextBox_ServerIP.PlaceholderText = "Input Server IP";
            TextBox_ServerIP.Size = new Size(162, 29);
            TextBox_ServerIP.TabIndex = 3;
            // 
            // Label_ServerIP
            // 
            Label_ServerIP.AutoSize = true;
            Label_ServerIP.BackColor = Color.Transparent;
            Label_ServerIP.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            Label_ServerIP.ForeColor = Color.FromArgb(7, 25, 82);
            Label_ServerIP.ImageAlign = ContentAlignment.BottomRight;
            Label_ServerIP.Location = new Point(18, 52);
            Label_ServerIP.Name = "Label_ServerIP";
            Label_ServerIP.Size = new Size(98, 25);
            Label_ServerIP.TabIndex = 4;
            Label_ServerIP.Text = "Server IP:";
            Label_ServerIP.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Button_Connect
            // 
            Button_Connect.BackColor = Color.FromArgb(7, 25, 82);
            Button_Connect.Cursor = Cursors.Hand;
            Button_Connect.FlatStyle = FlatStyle.Popup;
            Button_Connect.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Button_Connect.ForeColor = SystemColors.Control;
            Button_Connect.Location = new Point(190, 172);
            Button_Connect.Name = "Button_Connect";
            Button_Connect.Size = new Size(94, 23);
            Button_Connect.TabIndex = 73;
            Button_Connect.Text = "Connect";
            Button_Connect.UseVisualStyleBackColor = false;
            Button_Connect.Click += Button_Connect_Click;
            // 
            // Label_Port
            // 
            Label_Port.AutoSize = true;
            Label_Port.BackColor = Color.Transparent;
            Label_Port.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            Label_Port.ForeColor = Color.FromArgb(7, 25, 82);
            Label_Port.ImageAlign = ContentAlignment.BottomRight;
            Label_Port.Location = new Point(64, 89);
            Label_Port.Name = "Label_Port";
            Label_Port.Size = new Size(56, 25);
            Label_Port.TabIndex = 74;
            Label_Port.Text = "Port:";
            Label_Port.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label_PCName
            // 
            Label_PCName.AutoSize = true;
            Label_PCName.BackColor = Color.Transparent;
            Label_PCName.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            Label_PCName.ForeColor = Color.FromArgb(7, 25, 82);
            Label_PCName.ImageAlign = ContentAlignment.BottomRight;
            Label_PCName.Location = new Point(41, 14);
            Label_PCName.Name = "Label_PCName";
            Label_PCName.Size = new Size(0, 30);
            Label_PCName.TabIndex = 76;
            Label_PCName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Button_Stop
            // 
            Button_Stop.BackColor = Color.Maroon;
            Button_Stop.Cursor = Cursors.Hand;
            Button_Stop.FlatStyle = FlatStyle.Popup;
            Button_Stop.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Button_Stop.ForeColor = SystemColors.Control;
            Button_Stop.Location = new Point(104, 172);
            Button_Stop.Name = "Button_Stop";
            Button_Stop.Size = new Size(72, 23);
            Button_Stop.TabIndex = 77;
            Button_Stop.Text = "Stop";
            Button_Stop.UseVisualStyleBackColor = false;
            Button_Stop.Click += Button_Stop_Click;
            // 
            // Label_23000
            // 
            Label_23000.AutoSize = true;
            Label_23000.BackColor = Color.Transparent;
            Label_23000.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            Label_23000.ForeColor = Color.FromArgb(7, 25, 82);
            Label_23000.ImageAlign = ContentAlignment.BottomRight;
            Label_23000.Location = new Point(131, 89);
            Label_23000.Name = "Label_23000";
            Label_23000.Size = new Size(67, 25);
            Label_23000.TabIndex = 78;
            Label_23000.Text = "23000";
            Label_23000.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label_StatusConnection
            // 
            Label_StatusConnection.AutoSize = true;
            Label_StatusConnection.BackColor = Color.Transparent;
            Label_StatusConnection.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            Label_StatusConnection.ForeColor = Color.Red;
            Label_StatusConnection.ImageAlign = ContentAlignment.BottomRight;
            Label_StatusConnection.Location = new Point(131, 123);
            Label_StatusConnection.Name = "Label_StatusConnection";
            Label_StatusConnection.Size = new Size(93, 25);
            Label_StatusConnection.TabIndex = 79;
            Label_StatusConnection.Text = " Stopped";
            Label_StatusConnection.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label_Status
            // 
            Label_Status.AutoSize = true;
            Label_Status.BackColor = Color.Transparent;
            Label_Status.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            Label_Status.ForeColor = Color.FromArgb(7, 25, 82);
            Label_Status.ImageAlign = ContentAlignment.BottomRight;
            Label_Status.Location = new Point(49, 123);
            Label_Status.Name = "Label_Status";
            Label_Status.Size = new Size(72, 25);
            Label_Status.TabIndex = 80;
            Label_Status.Text = "Status:";
            Label_Status.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Client_CSOLConnect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 255, 192);
            ClientSize = new Size(316, 218);
            Controls.Add(Label_Status);
            Controls.Add(Label_StatusConnection);
            Controls.Add(Label_23000);
            Controls.Add(Button_Stop);
            Controls.Add(Label_PCName);
            Controls.Add(Label_Port);
            Controls.Add(Button_Connect);
            Controls.Add(Label_ServerIP);
            Controls.Add(TextBox_ServerIP);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Client_CSOLConnect";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Client Application";
            FormClosing += Client_CSOLConnect_FormClosing;
            Load += Client_CSOLConnect_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox TextBox_ServerIP;
        private Label Label_ServerIP;
        private Button Button_Connect;
        private Label Label_Port;
        private Label Label_PCName;
        private Button Button_Stop;
        private Label Label_23000;
        private Label Label_StatusConnection;
        private Label Label_Status;
    }
}
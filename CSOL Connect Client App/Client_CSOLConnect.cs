using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Management;
using System.Diagnostics;

namespace CSOL_Connect_Client_App
{
    public partial class Client_CSOLConnect : Form
    {
        private bool stopMonitoring = false;
        private bool stopKeyboardMonitoring = false;

        private string serverAddress = "127.0.0.1";

        public Client_CSOLConnect()
        {
            InitializeComponent();
            this.Load += Client_CSOLConnect_Load;
        }
    

        private void Client_CSOLConnect_Load(object sender, EventArgs e)
        {
            Label_PCName.Text = Environment.MachineName;
        }

        private async void Button_Connect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox_ServerIP.Text))
            {
                MessageBox.Show("Server IP cannot be empty. Please enter a valid server IP.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Don't proceed with the connection if the server IP is empty
            }

            serverAddress = TextBox_ServerIP.Text;

            Label_StatusConnection.Text = "Connected";
            Label_StatusConnection.ForeColor = Color.Green;

            await Task.Run(() => ForMouseDevice());
            await Task.Run(() => ForKeyboardDevice());
            //MonitorLANStatus();
        }

        private void Button_Stop_Click(object sender, EventArgs e)
        {
            Label_StatusConnection.Text = "Stopped";
            Label_StatusConnection.ForeColor = Color.Red;

            stopMonitoring = true;
            stopKeyboardMonitoring = true;
        }

        private void Client_CSOLConnect_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; // Cancel the default closing behavior
                ShowCloseDialog(); // Show the custom closing dialog
            }
        }
        private void ShowCloseDialog()
        {
            var result = MessageBox.Show("Do you want to quit the program?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // User wants to quit the program
                stopMonitoring = true;
                stopKeyboardMonitoring = true;
                Application.Exit();
            }
            else if (result == DialogResult.No)
            {

            }
        }

        //-------------------------------------------------//
        //            For Mouse Port Connection            //
        //-------------------------------------------------//

        private bool isMouseConnected = false;
        private string pcName = Environment.MachineName; // Replace with the actual PC name

        private async void ForMouseDevice()
        {
            isMouseConnected = IsMouseConnected();
            DisplayMouseStatus(isMouseConnected);

            string mouseMessage = $"{pcName}:{(isMouseConnected ? "Mouse is connected." : "Mouse is disconnected")}";

            await SendMouseMessageToServerAsync(mouseMessage);

            // Start monitoring the mouse status in the background
            await Task.Run(() => MonitorMouseStatus());
        }

        private async void MonitorMouseStatus()
        {
            while (!stopMonitoring)
            {
                bool newMouseStatus = await Task.Run(() => IsMouseConnected());

                if (newMouseStatus != isMouseConnected)
                {
                    isMouseConnected = newMouseStatus;
                    DisplayMouseStatus(isMouseConnected);

                    string mouseMessage = $"{pcName}:{(isMouseConnected ? "Mouse is connected." : "Mouse is disconnected")}";

                    // Send the mouse status change message to the server asynchronously
                    await Task.Run(() => SendMouseMessageToServerAsync(mouseMessage));
                }

                await Task.Delay(1000); // Use Task.Delay instead of Thread.Sleep in async methods.
            }
        }

        private bool IsMouseConnected()
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%Mouse%'"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    return true; // If any mouse-like device is found, consider the mouse connected.
                }
            }
            return false; // If no mouse-like devices are found, consider the mouse disconnected.
        }

        private void DisplayMouseStatus(bool isConnected)
        {
            if (isConnected)
            {
                Debug.WriteLine("Mouse is connected.");
            }
            else
            {
                Debug.WriteLine("Mouse is unlinked.");
            }
        }

        private async Task SendMouseMessageToServerAsync(string message)
        {
            try
            {
                int serverPort = 23000; // Replace with the port your server is listening on

                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(serverAddress, serverPort);

                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        await stream.WriteAsync(data, 0, data.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending message to server: " + ex.Message + Environment.NewLine + "Please quit the application, and open it again");
            }
        }

        //-------------------------------------------------//
        //         For Keyboard Device Connection          //
        //-------------------------------------------------//

        private async void ForKeyboardDevice()
        {
            // Check the current state of keyboard devices
            var connectedKeyboards = GetConnectedKeyboards();
            if (connectedKeyboards.Length > 0)
            {
                foreach (var keyboard in connectedKeyboards)
                {
                    Debug.WriteLine($"Keyboard is connected");

                    // Send the initial keyboard message to the server
                    await SendKeyboardMessageToServerAsync($"Keyboard is connected");
                }
            }
            else
            {
                Debug.WriteLine($"Keyboard is disconnected");

                // Send the initial "Keyboard is disconnected" message to the server
                await SendKeyboardMessageToServerAsync("Keyboard is disconnected");
            }

            // Set up a WMI event query to monitor USB device connection and disconnection
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceOperationEvent " +
                                                "WITHIN 2 " +
                                                "WHERE TargetInstance ISA 'Win32_PnPEntity' " +
                                                "AND TargetInstance.Description LIKE '%keyboard%'");

            ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += (s, ev) =>
            {
                PropertyData pd = ev.NewEvent.Properties["TargetInstance"];
                if (pd != null && pd.Value is ManagementBaseObject mbo)
                {
                    string deviceName = mbo.Properties["Name"].Value.ToString();
                    string eventType = ev.NewEvent.ClassPath.ClassName;

                    if (!stopKeyboardMonitoring) // Check the flag before processing events
                    {
                        if (eventType == "__InstanceCreationEvent")
                        {
                            Debug.WriteLine($"Keyboard is connected");
                            this.Invoke((Action)(async () => await SendKeyboardMessageToServerAsync($"Keyboard is connected")));
                        }
                        else if (eventType == "__InstanceDeletionEvent")
                        {
                            Debug.WriteLine($"Keyboard is disconnected");
                            this.Invoke((Action)(async () => await SendKeyboardMessageToServerAsync($"Keyboard is disconnected")));
                        }
                    }
                }
            };

            await Task.Run(() => watcher.Start());
        }

        private string[] GetConnectedKeyboards()
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_PnPEntity WHERE Description LIKE '%keyboard%'"))
            {
                var keyboardDevices = searcher.Get().Cast<ManagementBaseObject>().Select(d => d["Name"].ToString()).ToArray();
                return keyboardDevices;
            }
        }

        private async Task SendKeyboardMessageToServerAsync(string message)
        {
            try
            {
                int serverPort = 23000; // Replace with the port your server is listening on

                // Include pcName in the message
                string fullMessage = $"{pcName}:{message}";

                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(serverAddress, serverPort);

                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] data = Encoding.UTF8.GetBytes(fullMessage);
                        await stream.WriteAsync(data, 0, data.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending message to server: " + ex.Message + Environment.NewLine + "Please quit the application, and open it again");
            }
        }


        ////-------------------------------------------------//
        ////            For LAN Port Connection              //
        ////-------------------------------------------------//

        //private async void MonitorLANStatus()
        //{
        //    while (!stopMonitoring)
        //    {
        //        bool isLANConnected = IsLANConnected();

        //        if (isLANConnected)
        //        {
        //            await SendLANMessageToServerAsync($"{pcName}:LAN is connected");
        //        }
        //        else
        //        {
        //            await SendLANMessageToServerAsync($"{pcName}:LAN is disconnected");
        //        }

        //        // Sleep for a short duration before checking again (adjust as needed).
        //        System.Threading.Thread.Sleep(8000); // Check every 8 seconds
        //    }
        //}

        //private bool IsLANConnected()
        //{
        //    // Use NetworkInterface to check the status of the LAN connection
        //    foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        //    {
        //        if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
        //            networkInterface.OperationalStatus == OperationalStatus.Up)
        //        {
        //            return true; // LAN is connected
        //        }
        //    }
        //    return false; // LAN is disconnected
        //}

        //private async Task SendLANMessageToServerAsync(string message)
        //{
        //    try
        //    {
        //        int serverPort = 23000; // Replace with the port your server is listening on

        //        using (TcpClient client = new TcpClient())
        //        {
        //            await client.ConnectAsync(serverAddress, serverPort);

        //            using (NetworkStream stream = client.GetStream())
        //            {
        //                byte[] data = Encoding.UTF8.GetBytes(message);
        //                await stream.WriteAsync(data, 0, data.Length);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error sending LAN message to server: " + ex.Message);
        //    }
        //}
    }
}
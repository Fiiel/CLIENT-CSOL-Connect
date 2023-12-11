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

        private void Button_Connect_Click(object sender, EventArgs e)
        {
            serverAddress = TextBox_ServerIP.Text;

            ForMouseDevice();
            ForKeyboardDevice();
            //MonitorLANStatus();
        }

        private void Button_Stop_Click(object sender, EventArgs e)
        {
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
            Task.Run(() => MonitorMouseStatus());
        }

        private void MonitorMouseStatus()
        {
            while (!stopMonitoring)
            {
                bool newMouseStatus = IsMouseConnected();

                if (newMouseStatus != isMouseConnected)
                {
                    isMouseConnected = newMouseStatus;
                    DisplayMouseStatus(isMouseConnected);

                    string mouseMessage = $"{pcName}:{(isMouseConnected ? "Mouse is connected." : "Mouse is disconnected")}";

                    // Send the mouse status change message to the server asynchronously
                    SendMouseMessageToServerAsync(mouseMessage);
                }

                System.Threading.Thread.Sleep(1000); // Sleep for 1 second before checking again (adjust as needed).
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
                Debug.WriteLine("Error sending message to server: " + ex.Message);
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
            watcher.EventArrived += async (s, ev) =>
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
                            await SendKeyboardMessageToServerAsync($"Keyboard is connected");
                        }
                        else if (eventType == "__InstanceDeletionEvent")
                        {
                            Debug.WriteLine($"Keyboard is disconnected");
                            await SendKeyboardMessageToServerAsync($"Keyboard is disconnected");
                        }
                    }
                }
            };

            watcher.Start();
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
                Debug.WriteLine("Error sending message to server: " + ex.Message);
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
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Management;
using System.Diagnostics;
using Microsoft.VisualBasic.Devices;

namespace CSOL_Connect_Client_App
{
    public partial class Client_CSOLConnect : Form
    {
        public Client_CSOLConnect()
        {
            InitializeComponent();
            //ForLANPort();
            ForMouseDevice();
            ForKeyboardDevice();
        }

        ////-------------------------------------------------//
        ////            For LAN Port Connection              //
        ////-------------------------------------------------//

        //private void ForLANPort()
        //{
        //    NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);
        //    Load += Client_CSOLConnect_Load;
        //}

        //private async void Client_CSOLConnect_Load(object sender, EventArgs e)
        //{
        //    await Task.Run(() => CheckEthernetStatus());
        //}

        //private void CheckEthernetStatus()
        //{
        //    NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

        //    foreach (NetworkInterface n in adapters)
        //    {
        //        if (n.Name == "Ethernet 3")
        //        {
        //            string message = string.Format("\t{0} is {1}", n.Name, n.OperationalStatus);
        //            Console.WriteLine(message);

        //            // Send the message to the server asynchronously
        //            SendToServerAsync(message).Wait();

        //            break; // Stop looping once you find the desired interface
        //        }
        //    }
        //}

        //private async Task SendToServerAsync(string message)
        //{
        //    try
        //    {
        //        // Define the server address and port
        //        string serverAddress = "127.0.0.1"; // Replace with your server's IP address or hostname
        //        int serverPort = 23000; // Replace with the port your server is listening on

        //        // Create a TcpClient to connect to the server
        //        using (TcpClient client = new TcpClient())
        //        {
        //            await client.ConnectAsync(serverAddress, serverPort);

        //            // Get a stream for writing data to the server
        //            using (NetworkStream stream = client.GetStream())
        //            {
        //                // Convert the message to bytes and send it to the server
        //                byte[] data = Encoding.UTF8.GetBytes(message);
        //                await stream.WriteAsync(data, 0, data.Length);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error sending data to server: " + ex.Message);
        //    }
        //}

        //private void AddressChangedCallback(object sender, EventArgs e)
        //{
        //    CheckEthernetStatus(); // Check the Ethernet status whenever there's an address change event
        //}


        //-------------------------------------------------//
        //            For Mouse Port Connection            //
        //-------------------------------------------------//

        private bool isMouseConnected = false;
        private string pcName = "elem2"; // Replace with the actual PC name

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
            while (true)
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
                string serverAddress = "127.0.0.1"; // Replace with your server's IP address or hostname
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
                string serverAddress = "127.0.0.1"; // Replace with your server's IP address or hostname
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
    }
}
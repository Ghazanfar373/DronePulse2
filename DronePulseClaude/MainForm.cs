using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
// Add this directive at the top of your file to ensure the System.IO.Ports namespace is included.
using System.IO.Ports;
using DronePulseClaude.Services;

// Ensure that the System.IO.Ports assembly is referenced in your project.
// In Visual Studio, right-click on your project in the Solution Explorer, select "Manage NuGet Packages",
// search for "System.IO.Ports", and install it if it is not already installed.
//using MavlinkAPI;

namespace DronePulseClaude
{



    public partial class MainForm : Form
    {
        #region Private Fields
        private SerialPort serialPort;
        private bool isConnected = false;
        private CancellationTokenSource cancellationTokenSource;
        private Task dataProcessingTask;
        MavlinkParser parser = new MavlinkParser();
        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();
            InitializeApplication();
        }
        #endregion

        #region Initialization Methods
        private void InitializeApplication()
        {
            InitializeSerialPort();
            PopulateComPorts();
            LogMessage("Application started. Ready to connect to MAVLink device.");
        }

        private void InitializeSerialPort()
        {
            serialPort = new SerialPort();
            cancellationTokenSource = new CancellationTokenSource();
            // Remove the DataReceived event handler - we'll use Task-based approach
        }
        #endregion

        #region COM Port Management
        private void PopulateComPorts()
        {
            portComboBox.Items.Clear();
            string[] ports = SerialPort.GetPortNames();

            if (ports.Length > 0)
            {
                portComboBox.Items.AddRange(ports);
                portComboBox.SelectedIndex = 0;
            }
            else
            {
                LogMessage("No COM ports found.");
            }
        }
        #endregion

        #region Event Handlers
        private void RefreshPortsButton_Click(object sender, EventArgs e)
        {
            PopulateComPorts();
            LogMessage("COM ports refreshed.");
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                ConnectToPort();
            }
            else
            {
                DisconnectFromPort();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Stop data processing first
                StopDataProcessingTask();

                // Close serial port
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                }

                // Dispose resources
                cancellationTokenSource?.Dispose();
                serialPort?.Dispose();
            }
            catch (Exception ex)
            {
                // Log error but don't prevent closing
                System.Diagnostics.Debug.WriteLine($"Error during form closing: {ex.Message}");
            }
        }
        #endregion

        #region Connection Management
        private void ConnectToPort()
        {
            try
            {
                if (portComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a COM port.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ConfigureSerialPort();
                serialPort.Open();

                // Start data processing task
                StartDataProcessingTask();

                UpdateConnectionState(true);
                LogMessage($"Connected to {serialPort.PortName} at {serialPort.BaudRate} baud.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to port: {ex.Message}", "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogMessage($"Connection failed: {ex.Message}");
            }
        }

        private void DisconnectFromPort()
        {
            try
            {
                // Stop data processing task first
                StopDataProcessingTask();

                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }

                UpdateConnectionState(false);
                LogMessage("Disconnected from port.");
            }
            catch (Exception ex)
            {
                LogMessage($"Error disconnecting: {ex.Message}");
            }
        }

        private void ConfigureSerialPort()
        {
            serialPort.PortName = portComboBox.SelectedItem.ToString();
            serialPort.BaudRate = int.Parse(baudRateComboBox.SelectedItem.ToString());
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.None;
        }

        private void UpdateConnectionState(bool connected)
        {
            isConnected = connected;

            if (connected)
            {
                connectButton.Text = "Disconnect";
                connectButton.BackColor = Color.LightCoral;
                statusLabel.Text = $"Status: Connected to {serialPort.PortName} @ {serialPort.BaudRate}";
                statusLabel.ForeColor = Color.Green;

                // Disable connection controls
                portComboBox.Enabled = false;
                baudRateComboBox.Enabled = false;
                refreshPortsButton.Enabled = false;
            }
            else
            {
                connectButton.Text = "Connect";
                connectButton.BackColor = Color.LightGreen;
                statusLabel.Text = "Status: Disconnected";
                statusLabel.ForeColor = Color.Red;

                // Enable connection controls
                portComboBox.Enabled = true;
                baudRateComboBox.Enabled = true;
                refreshPortsButton.Enabled = true;
            }
        }
        #endregion

        #region Data Handling
        private void StartDataProcessingTask()
        {
            cancellationTokenSource = new CancellationTokenSource();
            dataProcessingTask = Task.Run(() => ProcessSerialDataAsync(cancellationTokenSource.Token));
        }

        private void StopDataProcessingTask()
        {
            try
            {
                cancellationTokenSource?.Cancel();

                // Wait for task to complete with timeout
                if (dataProcessingTask != null && !dataProcessingTask.IsCompleted)
                {
                    if (!dataProcessingTask.Wait(1000)) // 1 second timeout
                    {
                        LogMessage("Warning: Data processing task did not stop gracefully.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error stopping data processing task: {ex.Message}");
            }
        }

        private async Task ProcessSerialDataAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[1024];

            while (!cancellationToken.IsCancellationRequested && serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    if (serialPort.BytesToRead > 0)
                    {
                        int bytesToRead = Math.Min(serialPort.BytesToRead, buffer.Length);
                        int bytesRead = serialPort.Read(buffer, 0, bytesToRead);

                        if (bytesRead > 0)
                        {
                            // Create a copy of the data for processing
                            byte[] dataBuffer = new byte[bytesRead];
                            Array.Copy(buffer, 0, dataBuffer, 0, bytesRead);

                            // Process data on UI thread safely
                            await UpdateUIWithReceivedData(dataBuffer, cancellationToken);
                        }
                    }

                    // Small delay to prevent excessive CPU usage
                    await Task.Delay(10, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
                catch (Exception ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await SafeLogMessage($"Error in data processing: {ex.Message}", cancellationToken);
                    }
                    break;
                }
            }
        }

        private async Task UpdateUIWithReceivedData(byte[] data, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested || this.IsDisposed)
                return;

            try
            {
                // Convert to hex string for display
                string hexData = BitConverter.ToString(data).Replace("-", " ");

                // Update UI on main thread
                if (this.InvokeRequired)
                {
                    await Task.Run(() =>
                    {
                        if (!cancellationToken.IsCancellationRequested && !this.IsDisposed)
                        {
                            this.Invoke(new Action(() =>
                            {
                                if (!this.IsDisposed)
                                {
                                    // LogMessage($"Received {data.Length} bytes: {hexData}");
                                    //   LogMessage($"AS:{parser.HudData.Airspeed} HD: {parser.HudData.Heading} ");
                                    LogMessage($"VFR_HUD:   AS={parser.HudData.Airspeed:F1}m/s, GS={parser.HudData.Groundspeed:F1}m/s, Alt={parser.HudData.Altitude:F1}m, Hdg={parser.HudData.Heading}°, Thr={parser.HudData.Throttle}% \r\n");
                                    LogMessage($"ATTITUDE:  Roll={parser.AttitudeData.RollDegrees:F1}°, Pitch={parser.AttitudeData.PitchDegrees:F1}°, Yaw={parser.AttitudeData.YawDegrees:F1}° \r\n");
                                    LogMessage($"Heartbeat: Type={parser.cs.heartbeat.Kind}, Mode={parser.cs.mode}, Armed={parser.cs.armed} \r\n");

                                }
                            }));
                        }
                    }, cancellationToken);
                }
                else
                {
                    LogMessage($"Received {data.Length} bytes: {hexData}");
                }

                // TODO: Parse MAVLink messages here

                parser.ParseData(data);
                // await ParseMavlinkDataAsync(data, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            catch (Exception ex)
            {
                await SafeLogMessage($"Error updating UI with received data: {ex.Message}", cancellationToken);
            }
        }

        private async Task SafeLogMessage(string message, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested || this.IsDisposed)
                return;

            try
            {
                if (this.InvokeRequired)
                {
                    await Task.Run(() =>
                    {
                        if (!cancellationToken.IsCancellationRequested && !this.IsDisposed)
                        {
                            this.Invoke(new Action(() =>
                            {
                                if (!this.IsDisposed)
                                {
                                    LogMessage(message);
                                }
                            }));
                        }
                    }, cancellationToken);
                }
                else
                {
                    LogMessage(message);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            catch (Exception)
            {
                // Ignore errors during shutdown
            }
        }
        #endregion

        #region Utility Methods
        private void LogMessage(string message)
        {
            if (logTextBox.InvokeRequired)
            {
                logTextBox.Invoke(new Action<string>(LogMessage), message);
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] {message}\r\n";

            logTextBox.AppendText(logEntry);
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();

            // Limit log size to prevent memory issues
            if (logTextBox.Lines.Length > 1000)
            {
                var lines = logTextBox.Lines;
                var newLines = new string[500];
                Array.Copy(lines, lines.Length - 500, newLines, 0, 500);
                logTextBox.Lines = newLines;
            }
        }

        #endregion
    }

}

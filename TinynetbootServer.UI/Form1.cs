using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TinynetbootServer.Core;
using TinynetbootServer.Network;
using static TinynetbootServer.Core.IPManager;

namespace TinynetbootServer.UI
{
    public partial class TinyNetboot : Form
    {
        private HttpServer? _httpServer;
        private DhcpServer? _dhcpServer;
        private DnsServer? _dnsServer;
        private TftpServer? _tftpServer;
        private CancellationTokenSource? _cts;
        private IPManager? _ipManager;
        private BootMonitor _bootMonitor = new BootMonitor();
        private MyMonitor _monitor;

        public TinyNetboot()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            // Set dark theme for log box
            txtLogBox.BackColor = Color.Black;
            txtLogBox.ForeColor = Color.LimeGreen;
            LoadConfiguration();
            InitializeMonitor();
            CheckFilesystemStatus();


        }

        // Inside your MainForm.cs
        private void LoadConfiguration()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "config", "server_config.json");

            if (File.Exists(filePath))
            {

                try
                {

                    string encryptedData = File.ReadAllText(filePath);
                    string decryptedJson = EncryptionHelper.Decrypt(encryptedData);
                    var config = JsonSerializer.Deserialize<ServerConfig>(decryptedJson);

                    if (config != null)
                    {
                        txtServerIP.Text = config.ServerIp;
                        txtStartRange.Text = config.StartIp;
                        txtEndRange.Text = config.EndIp;
                        numHttpPort.Value = config.HttpPort;
                        txtExamServer.Text = config.ExamServerUrl;
                        _ipManager = new IPManager(config.StartIp, config.EndIp);
                        _ipManager.LoadClientsCsv();

                        // 2. Refresh the grid so the machines appear
                        if (config.IsExamMode)
                            rbExamMode.Checked = true;
                        else
                            rbConfigMode.Checked = true;
                        UpdateUILog(">>> Configuration loaded.");
                    }
                }
                catch (Exception ex)
                {
                    UpdateUILog($">>> Failed to load config: {ex.Message}");
                }
            }
            UpdateModeDisplay();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();

            // 1. Collect data from UI
            string srvIp = txtServerIP.Text;
            string startRange = txtStartRange.Text;
            string endRange = txtEndRange.Text;
            int port = (int)numHttpPort.Value;
            bool isExamMode = rbExamMode.Checked;
            if (!IsIpLocal(srvIp))
            {
                MessageBox.Show($"IP {srvIp} . Please check your Network Settings.",
                                "Invalid IP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                // 2. Initialize Logic & Servers
                if (_ipManager == null)
                {
                    _ipManager = new IPManager(startRange, endRange);
                    _ipManager.LoadClientsCsv();
                }
                else
                {
                    // If it already exists, just update the range
                    _ipManager.UpdateRange(startRange, endRange);
                }
                string examServerUrl =
       txtExamServer.Text.Trim().TrimEnd('/') +
       "/ntaedupxeboot/jumpstart.aspx";
                // We must provide the _ipManager and the Lambda for rbExamMode
                _httpServer = new HttpServer(srvIp, port, _bootMonitor, _ipManager, () => rbExamMode.Checked, examServerUrl);
                _tftpServer = new TftpServer(srvIp);
                _dhcpServer = new DhcpServer(_ipManager, srvIp, port, isExamMode, _bootMonitor);
                _dnsServer = new DnsServer(srvIp);
                _dnsServer.OnLog = (msg) => UpdateUILog(msg);
                Task.Run(() => _dnsServer.StartAsync(_cts.Token));

                // 3. Subscribe to the Log events
                _httpServer.OnLog = (msg) => UpdateUILog(msg);
                _dhcpServer.OnLog = (msg) => UpdateUILog(msg);
                _tftpServer.OnLog = (msg) => UpdateUILog(msg);

                // 4. Start all servers as background tasks
                Task.Run(() => _httpServer.StartAsync(_cts.Token));
                Task.Run(() => _dhcpServer.StartAsync(_cts.Token));
                _tftpServer.Start();


                btnStart.Enabled = false;
                btnStop.Enabled = true;
                toolStripStatusLabel2.Text = "RUNNING";
                toolStripStatusLabel2.ForeColor = Color.LimeGreen;

                string modeText = isExamMode ? "EXAM MODE" : "CONFIG MODE";
                UpdateUILog($">>> All Services Started in {modeText}.");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting servers: {ex.Message}", "Startup Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            // 1. Safety check: If the manager isn't initialized, don't try to update
            if (_ipManager == null) return;

            // 2. Thread Safety: Ensure we are updating the UI from the main thread
            if (dgvClients.InvokeRequired)
            {
                dgvClients.Invoke(new Action(UpdateGrid));
                return;
            }
            if (dgvClients.IsCurrentCellInEditMode) return;
            dgvClients.Rows.Clear();
            var clients = _ipManager.GetAllClients();
            if (clients.Count == 0)
            {
                UpdateUILog(">>> No machines found in the registry.");
                return;
            }
            var seatManager = new SeatRegistrationManager();

            foreach (var c in clients)
            {
                string seatNo = seatManager.GetSeatNumberByMac(c.Mac);

                int rowIndex = dgvClients.Rows.Add(
                    false,
                    c.Ip,
                    c.Mac,
                    c.Uuid,
                    seatNo
                );
                if (string.IsNullOrEmpty(c.Uuid))
                {
                    UpdateUILog($"[Debug] No UUID found for MAC {c.Mac}");
                }

            }
        }

        // Change 'void' to 'async Task' or 'async void' for the event handler
        private async void btnStop_Click(object sender, EventArgs e)
        {
            await TerminateServicesAsync(cleanUpSquashFs: false);
        }
        private async Task TerminateServicesAsync(bool cleanUpSquashFs)
        {
            UpdateUILog(">>> Shutting down services...");

            if (_cts != null)
            {
                try
                {
                    _cts.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    // Fail-safe check in case it was already disposed elsewhere
                }
            }

            try
            {
                // 1. Terminate the active running protocol listeners
                _dnsServer?.Stop();
                _dhcpServer?.Stop();
                _httpServer?.Stop();
                _tftpServer?.Stop();

                // Give the network sockets a brief window to complete current packets
                await Task.Delay(500);

                // 2. Conditionally erase the heavy boot payload
                if (cleanUpSquashFs)
                {
                    string squashPath = Path.Combine(AppContext.BaseDirectory, "examstaytemp", "filesystem.squashfs");
                    if (File.Exists(squashPath))
                    {
                        File.Delete(squashPath);
                        UpdateUILog(">>> Loaded PXE Configuration has been cleared.");
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateUILog($"[System] Shutdown warning: {ex.Message}");
            }
            finally
            {
                // FIX: Explicitly dispose the CancellationTokenSource kernel resources
                if (_cts != null)
                {
                    _cts.Dispose();
                    _cts = null;
                }
            }

            // 3. Reset the UI State layout cleanly
            timer1.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            toolStripStatusLabel2.Text = "STOPPED";
            toolStripStatusLabel2.ForeColor = Color.Red;
            UpdateUILog(">>> ALL SERVICES TERMINATED.");
        }
        private void UpdateUILog(string message)
        {
            string finalMessage =
                $"{DateTime.Now:HH:mm:ss} | {message}";

            // SAVE SAME TEXT TO FILE
            LogManager.Write(finalMessage);

            if (txtLogBox.InvokeRequired)
            {
                txtLogBox.Invoke(new MethodInvoker(() =>
                {
                    txtLogBox.AppendText(finalMessage + Environment.NewLine);
                    txtLogBox.SelectionStart = txtLogBox.Text.Length;
                    txtLogBox.ScrollToCaret();
                }));
            }
            else
            {
                txtLogBox.AppendText(finalMessage + Environment.NewLine);
                txtLogBox.SelectionStart = txtLogBox.Text.Length;
                txtLogBox.ScrollToCaret();
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Define path
            string squashPath = Path.Combine(AppContext.BaseDirectory, "examstaytemp", "filesystem.squashfs");

            try
            {
                // Try to delete if it exists
                if (File.Exists(squashPath))
                {
                    File.Delete(squashPath);
                }
            }
            catch
            {
                // Fail silently on close if file is locked
            }

            base.OnFormClosing(e);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var config = new ServerConfig
                {
                    ServerIp = txtServerIP.Text,
                    StartIp = txtStartRange.Text,
                    EndIp = txtEndRange.Text,
                    HttpPort = (int)numHttpPort.Value,
                    IsExamMode = rbExamMode.Checked,
                    ExamServerUrl = txtExamServer.Text,
                };

                // 1. Convert to JSON
                string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

                string encryptedJson = EncryptionHelper.Encrypt(jsonString);

                string configFolder = Path.Combine(AppContext.BaseDirectory, "config");
                if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);

                string filePath = Path.Combine(configFolder, "server_config.json");
                File.WriteAllText(filePath, encryptedJson);
                UpdateModeDisplay();

                UpdateUILog(">>> Configuration encrypted and saved successfully.");
                MessageBox.Show("Settings Saved and Encrypted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving config: {ex.Message}");
            }
        }
        private void UpdateModeDisplay()
        {
            if (rbExamMode.Checked)
            {
                lblCurrentMode.Text = "BOOT-MODE: EXAM";
                lblCurrentMode.ForeColor = Color.Red;
                btnDeleteSelected.Enabled = false;
            }
            else
            {
                lblCurrentMode.Text = "BOOT-MODE: CONFIGURATION";
                lblCurrentMode.ForeColor = Color.Blue;
                btnDeleteSelected.Enabled = true;
            }
        }
        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            // 1. Identify which MACs are checked
            List<string> macsToDelete = new List<string>();

            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                bool isSelected = Convert.ToBoolean(row.Cells[0].Value);
                if (isSelected)
                {
                    string mac = row.Cells[2].Value?.ToString();
                    if (!string.IsNullOrEmpty(mac))
                    {
                        macsToDelete.Add(mac);
                    }
                }
            }

            if (macsToDelete.Count == 0)
            {
                MessageBox.Show("Please select at least one machine.", "No Selection");
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete {macsToDelete.Count} selected machine(s)?",
                                         "Confirm Delete",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // 4. Perform the Deletion
                foreach (string mac in macsToDelete)
                {
                    _ipManager?.RemoveClient(mac);
                }

                UpdateGrid(); // Refresh the display
                UpdateUILog($">>> Deleted {macsToDelete.Count} machines from registry.");
            }
            if (rbExamMode.Checked)
            {
                MessageBox.Show("Deletion is disabled during Exam Mode.", "Access Denied",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

        }
        private void dgvClients_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvClients.IsCurrentCellDirty)
            {
                dgvClients.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = Path.Combine(
                    AppContext.BaseDirectory,
                    "config",
                    "leases.csv");

                if (!File.Exists(filePath))
                {
                    MessageBox.Show("leases.csv not found.");
                    return;
                }

                // Read encrypted content
                string encrypted = File.ReadAllText(filePath);

                // Decrypt
                string csv = EncryptionHelper.Decrypt(encrypted);

                // Clear grid
                dgvClients.Rows.Clear();

                string[] lines = csv.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string[] p = line.Split(',');

                    if (p.Length < 6)
                        continue;

                    dgvClients.Rows.Add(
                        false,      // checkbox
                        p[1],       // IP
                        p[2],       // MAC
                        p[3],       // UUID
                        p[0]        // Seat Number
                    );
                }

                MessageBox.Show("Data loaded successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void InitializeMonitor()
        {
            _bootMonitor = new BootMonitor();

            _bootMonitor.OnMonitorUpdated += (sessions) =>
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() => RefreshGrid(sessions)));
                }
                else
                {
                    RefreshGrid(sessions);
                }
            };
        }

        private void RefreshGrid(List<BootSession> sessions)
        {
            // Filter the list to ONLY show machines that are still "BOOTING"
            var activeBooting = sessions.Where(s => s.Status == "BOOTING").ToList();

            dgvBootMonitor.DataSource = null;
            dgvBootMonitor.DataSource = activeBooting; // Only show booting machines

            int bootingCount = activeBooting.Count;
            lblStatusCounter.Text = $"{bootingCount}";
        }

        private bool IsIpLocal(string ip)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.Any(a => a.ToString() == ip);
        }

        private async void btnLoadPXE_Click(object sender, EventArgs e)
        {
            btnLoadPXE.Enabled = false;
            lblStatus.Text = "Starting Configuration...";


            var loadingForm = new Form()
            {
                Text = "System Update",
                Size = new Size(500, 180),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ControlBox = false,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                TopMost = true
            };

            var lblMessage = new Label()
            {
                Text = "Configuring PXE (1.3GB)... Please wait.",
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var pBar = new ProgressBar()
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                Style = ProgressBarStyle.Continuous,
                Margin = new Padding(20)
            };

            loadingForm.Controls.Add(lblMessage);
            loadingForm.Controls.Add(pBar);

            try
            {
                // 2. Setup progress reporting
                var progress = new Progress<double>(value =>
                {
                    pBar.Value = (int)value;
                    lblStatus.Text = $"PXE Loading: {(int)value}%";
                });

                loadingForm.Show();
                loadingForm.Refresh(); // Ensure it draws immediately

                var downloader = new PxeDownloader();

                // 3. Start Background Download with progress
                await downloader.DownloadSquashFsAsync(progress);

                loadingForm.Close();
                MessageBox.Show("PXE Configured successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                loadingForm.Close();
                MessageBox.Show($"Configuration failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLoadPXE.Enabled = true;
                lblStatus.Text = "Ready";
                btnStart.Enabled = true;

            }
        }
        private void CheckFilesystemStatus()
        {

            string squashPath = Path.Combine(AppContext.BaseDirectory, "examstaytemp", "filesystem.squashfs");

            if (File.Exists(squashPath))
            {
                lblStatus.Text = "Ready to boot";
                lblStatus.ForeColor = Color.LimeGreen;
            }
            else
            {
                lblStatus.Text = "Load PXE Configuration";
                lblStatus.ForeColor = Color.OrangeRed;
                // btnStart.Enabled = false;
            }
        }
        // kiosk mode  key block 
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.F4))
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async void btnExit_Click(object sender, EventArgs e)
        {
            // Pass true so it purges the squashfs payload before shutting down the app
            await TerminateServicesAsync(cleanUpSquashFs: true);
            Application.Exit();
        }

        private async void btnMonitor_Click(object sender, EventArgs e)
        {
            // 1. If it's already running, stop it immediately
            if (_monitor != null && _monitor.IsRunning)
            {
                _monitor.Stop();
                btnMonitor.BackColor = SystemColors.Control;
                btnMonitor.Text = "Start Monitor";
                pbCpu.Value = 0;
                pbRam.Value = 0;
                lblCpuStatus.Text = "0%";
                lblRamStatus.Text = "0MB / 0MB";
                UpdateUILog(">>> Hardware monitoring paused.");
                return;
            }

            // 2. Setup the Popup (Reusing your LoadPXE style)
            var loadingForm = new Form()
            {

                Text = "System Sync",
                Size = new Size(400, 120),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ControlBox = false,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                TopMost = true
            };

            var lblMessage = new Label()
            {
                Text = "Initializing Hardware Counters...\nPlease wait.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            loadingForm.Controls.Add(lblMessage);

            // 3. Start the process
            btnMonitor.Enabled = false;
            loadingForm.Show();
            loadingForm.Refresh();

            try
            {
                // 4. Initialize Monitor if null
                if (_monitor == null)
                {
                    _monitor = new MyMonitor(1000);
                    var info = new Microsoft.VisualBasic.Devices.ComputerInfo();
                    long totalMemoryMb = (long)(info.TotalPhysicalMemory / (1024 * 1024));

                    _monitor.OnCpuUpdated += (val) =>
                    {
                        if (pbCpu.IsHandleCreated)
                        {
                            pbCpu.Invoke(new Action(() =>
                            {
                                int cpuValue = (int)Math.Min(val, 100);
                                pbCpu.Value = cpuValue;
                                lblCpuStatus.Text = $"{cpuValue}%";
                            }));
                        }
                    };

                    _monitor.OnRamUpdated += (freeMb) =>
                    {
                        long usedMb = totalMemoryMb - (long)freeMb;
                        double percentUsed = ((double)usedMb / totalMemoryMb) * 100;
                        if (pbRam.IsHandleCreated)
                        {
                            pbRam.Invoke(new Action(() =>
                            {
                                pbRam.Value = (int)Math.Min(percentUsed, 100);
                                double usedGb = usedMb / 1024.0;
                                double totalGb = totalMemoryMb / 1024.0;

                                lblRamStatus.Text = $" ({percentUsed:0}%)";
                            }));
                        }
                    };
                }

                _monitor.Start();
                UpdateUILog(">>> Initializing hardware counters...");

                // 5. Wait for the counters to warm up
                await Task.Delay(1500);
            }
            catch (Exception ex)
            {
                UpdateUILog($">>> Monitor Error: {ex.Message}");
            }
            finally
            {
                // 6. Cleanup
                loadingForm.Close();
                loadingForm.Dispose();
                btnMonitor.Enabled = true;
                btnMonitor.BackColor = Color.LightGreen;
                btnMonitor.Text = "Stop Monitor";
                UpdateUILog(">>> Monitoring active.");
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDownloadCSV_Click(object sender, EventArgs e)
        {
            // 1. Safety check: Verify if the manager registry has been initialized
            if (_ipManager == null)
            {
                MessageBox.Show("No data available to export. Please load or start the registry first.",
                                "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Fetch all recorded client environments
            var clients = _ipManager.GetAllClients();
            if (clients == null || clients.Count == 0)
            {
                MessageBox.Show("The client registry is currently empty. There is no data to download.",
                                "Empty Registry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 3. Prompt user for path using a SaveFileDialog
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv";
                sfd.FileName = $"ClientRegistry_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                sfd.Title = "Export Client Machine Registry to CSV";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();

                        // Write Document standard column header line
                        sb.AppendLine("IP Address,MAC Address,UUID,Status");

                        // Append matching client lines
                        foreach (var client in clients)
                        {
                            // Clean and safely parse strings to isolate field-level formatting anomalies
                            string ip = EscapeCsvField(client.Ip);

                            // Convert raw hex string format (d43d7e236f1c) to standard layout (d4:3d:7e:23:6f:1c)
                            string formattedMac = FormatMacAddress(client.Mac);
                            string mac = EscapeCsvField(formattedMac);

                            string uuid = EscapeCsvField(client.Uuid);
                            string status = EscapeCsvField(client.Status);

                            sb.AppendLine($"{ip},{mac},{uuid},{status}");
                        }

                        // Push text data cleanly into target file location using explicit UTF8 encoding
                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);

                        // 4. Trace the results to the Log console panel
                        UpdateUILog($">>> Client registry successfully exported to CSV: {Path.GetFileName(sfd.FileName)}");
                        MessageBox.Show($"Successfully downloaded {clients.Count} client machine records!",
                                        "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        UpdateUILog($"[Error] CSV Download failed: {ex.Message}");
                        MessageBox.Show($"An error occurred while saving the file: {ex.Message}",
                                        "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Simple and fast MAC address formatter using LINQ chunking.
        /// Converts "d43d7e236f1c" to "d4:3d:7e:23:6f:1c"
        /// </summary>
        private string FormatMacAddress(string? mac)
        {
            if (string.IsNullOrEmpty(mac)) return "";

            // Remove any existing separators if present, and normalize to standard lowercase
            string cleanMac = mac.Replace(":", "").Replace("-", "").Replace(" ", "").ToLowerInvariant();

            // Ensure it's a valid 12-character hex length before splitting to prevent indexing crashes
            if (cleanMac.Length == 12)
            {
                return string.Join(":", Enumerable.Range(0, 6).Select(i => cleanMac.Substring(i * 2, 2)));
            }

            return cleanMac; // Safe fall-back if the record data is partial or irregular
        }

        /// <summary>
        /// Escapes CSV boundaries to ensure field integrity against structural characters
        /// </summary>
        private string EscapeCsvField(string? field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }

        private void txtExamServer_TextChanged(object sender, EventArgs e)
        {

        }
    }
}


public class ServerConfig
{
    public string ServerIp { get; set; } = "";
    public string StartIp { get; set; } = "";
    public string EndIp { get; set; } = "";
    public int HttpPort { get; set; }
    public bool IsExamMode { get; set; }
    public string ExamServerUrl { get; set; } = "";
}

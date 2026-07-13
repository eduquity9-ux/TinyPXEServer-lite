namespace TinynetbootServer.UI
{
    partial class TinyNetboot
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            btnStart = new Button();
            btnStop = new Button();
            txtLogBox = new RichTextBox();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new StatusStrip();
            lblCurrentMode = new Label();
            tabControl1 = new TabControl();
            Tab_Dashboard = new TabPage();
            panel5 = new Panel();
            lblCPU = new Label();
            lblCpuStatus = new Label();
            lblRamStatus = new Label();
            label1 = new Label();
            pbRam = new ProgressBar();
            pbCpu = new ProgressBar();
            panel4 = new Panel();
            btnExit = new Button();
            panel3 = new Panel();
            btnLoadPXE = new Button();
            btnMonitor = new Button();
            bootingstatus = new Label();
            lblStatusCounter = new Label();
            dgvBootMonitor = new DataGridView();
            Tab_Setting = new TabPage();
            panel5_setting = new Panel();
            txtExamServer = new TextBox();
            label6 = new Label();
            label3 = new Label();
            panel6 = new Panel();
            btnSave = new Button();
            txtServerIP = new TextBox();
            label2 = new Label();
            txtStartRange = new TextBox();
            label5 = new Label();
            txtEndRange = new TextBox();
            numHttpPort = new NumericUpDown();
            label4 = new Label();
            panel4_setting = new Panel();
            btnDownloadCSV = new Button();
            rbConfigMode = new RadioButton();
            rbExamMode = new RadioButton();
            btnDeleteSelected = new Button();
            btnLoadData = new Button();
            panel3_setting = new Panel();
            dgvClients = new DataGridView();
            Column4 = new DataGridViewCheckBoxColumn();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            colUuid = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            gridTimer = new System.Windows.Forms.Timer(components);
            timer1 = new System.Windows.Forms.Timer(components);
            panel1 = new Panel();
            lblStatus = new Label();
            panel2 = new Panel();
            toolStripStatusLabel1.SuspendLayout();
            tabControl1.SuspendLayout();
            Tab_Dashboard.SuspendLayout();
            panel5.SuspendLayout();
            panel4.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBootMonitor).BeginInit();
            Tab_Setting.SuspendLayout();
            panel5_setting.SuspendLayout();
            panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numHttpPort).BeginInit();
            panel4_setting.SuspendLayout();
            panel3_setting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClients).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.SpringGreen;
            btnStart.Font = new Font("Verdana", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStart.Location = new Point(33, 128);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(188, 58);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.Red;
            btnStop.Enabled = false;
            btnStop.Font = new Font("Verdana", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStop.Location = new Point(33, 226);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(185, 61);
            btnStop.TabIndex = 2;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += btnStop_Click;
            // 
            // txtLogBox
            // 
            txtLogBox.BackColor = Color.Black;
            txtLogBox.BorderStyle = BorderStyle.FixedSingle;
            txtLogBox.Cursor = Cursors.AppStarting;
            txtLogBox.Dock = DockStyle.Bottom;
            txtLogBox.ForeColor = Color.LimeGreen;
            txtLogBox.Location = new Point(0, 568);
            txtLogBox.Name = "txtLogBox";
            txtLogBox.ReadOnly = true;
            txtLogBox.Size = new Size(1696, 51);
            txtLogBox.TabIndex = 3;
            txtLogBox.Text = "";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(86, 25);
            toolStripStatusLabel2.Text = "STOPPED";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.BackColor = Color.Black;
            toolStripStatusLabel1.ImageScalingSize = new Size(24, 24);
            toolStripStatusLabel1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel2 });
            toolStripStatusLabel1.Location = new Point(0, 709);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(1734, 32);
            toolStripStatusLabel1.TabIndex = 4;
            // 
            // lblCurrentMode
            // 
            lblCurrentMode.AutoSize = true;
            lblCurrentMode.Dock = DockStyle.Right;
            lblCurrentMode.ForeColor = SystemColors.ActiveCaption;
            lblCurrentMode.Location = new Point(1556, 0);
            lblCurrentMode.Margin = new Padding(10);
            lblCurrentMode.Name = "lblCurrentMode";
            lblCurrentMode.Padding = new Padding(30, 25, 50, 0);
            lblCurrentMode.Size = new Size(178, 50);
            lblCurrentMode.TabIndex = 1;
            lblCurrentMode.Text = "SVR_Mode";
            lblCurrentMode.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tabControl1
            // 
            tabControl1.Alignment = TabAlignment.Left;
            tabControl1.Controls.Add(Tab_Dashboard);
            tabControl1.Controls.Add(Tab_Setting);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1734, 627);
            tabControl1.TabIndex = 0;
            // 
            // Tab_Dashboard
            // 
            Tab_Dashboard.BackColor = Color.Black;
            Tab_Dashboard.Controls.Add(panel5);
            Tab_Dashboard.Controls.Add(panel4);
            Tab_Dashboard.Controls.Add(panel3);
            Tab_Dashboard.Controls.Add(bootingstatus);
            Tab_Dashboard.Controls.Add(lblStatusCounter);
            Tab_Dashboard.Controls.Add(dgvBootMonitor);
            Tab_Dashboard.Controls.Add(txtLogBox);
            Tab_Dashboard.Location = new Point(34, 4);
            Tab_Dashboard.Name = "Tab_Dashboard";
            Tab_Dashboard.Size = new Size(1696, 619);
            Tab_Dashboard.TabIndex = 0;
            Tab_Dashboard.Text = "Dashboard";
            Tab_Dashboard.Click += btnStart_Click;
            // 
            // panel5
            // 
            panel5.Controls.Add(lblCPU);
            panel5.Controls.Add(lblCpuStatus);
            panel5.Controls.Add(lblRamStatus);
            panel5.Controls.Add(label1);
            panel5.Controls.Add(pbRam);
            panel5.Controls.Add(pbCpu);
            panel5.Dock = DockStyle.Right;
            panel5.Location = new Point(839, 69);
            panel5.Name = "panel5";
            panel5.Size = new Size(384, 499);
            panel5.TabIndex = 18;
            // 
            // lblCPU
            // 
            lblCPU.AutoSize = true;
            lblCPU.BackColor = Color.Transparent;
            lblCPU.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCPU.ForeColor = Color.SpringGreen;
            lblCPU.Location = new Point(115, 124);
            lblCPU.Name = "lblCPU";
            lblCPU.Size = new Size(47, 25);
            lblCPU.TabIndex = 14;
            lblCPU.Text = "CPU";
            // 
            // lblCpuStatus
            // 
            lblCpuStatus.AutoSize = true;
            lblCpuStatus.ForeColor = SystemColors.ActiveCaption;
            lblCpuStatus.Location = new Point(168, 121);
            lblCpuStatus.Name = "lblCpuStatus";
            lblCpuStatus.Size = new Size(61, 25);
            lblCpuStatus.TabIndex = 10;
            lblCpuStatus.Text = "0.00%";
            // 
            // lblRamStatus
            // 
            lblRamStatus.AutoSize = true;
            lblRamStatus.ForeColor = SystemColors.ActiveCaption;
            lblRamStatus.Location = new Point(119, 39);
            lblRamStatus.Name = "lblRamStatus";
            lblRamStatus.Size = new Size(61, 25);
            lblRamStatus.TabIndex = 11;
            lblRamStatus.Text = "0.00%";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ActiveCaptionText;
            label1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.SpringGreen;
            label1.Location = new Point(61, 39);
            label1.Name = "label1";
            label1.Size = new Size(52, 25);
            label1.TabIndex = 15;
            label1.Text = "RAM";
            // 
            // pbRam
            // 
            pbRam.Location = new Point(186, 31);
            pbRam.Name = "pbRam";
            pbRam.Size = new Size(198, 35);
            pbRam.TabIndex = 9;
            // 
            // pbCpu
            // 
            pbCpu.Location = new Point(235, 114);
            pbCpu.Name = "pbCpu";
            pbCpu.Size = new Size(146, 35);
            pbCpu.TabIndex = 8;
            // 
            // panel4
            // 
            panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel4.Controls.Add(btnExit);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(0, 0);
            panel4.Name = "panel4";
            panel4.Padding = new Padding(10, 0, 0, 0);
            panel4.Size = new Size(1223, 69);
            panel4.TabIndex = 17;
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.Red;
            btnExit.Dock = DockStyle.Right;
            btnExit.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExit.ForeColor = SystemColors.ActiveCaptionText;
            btnExit.Location = new Point(900, 0);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(323, 69);
            btnExit.TabIndex = 13;
            btnExit.Text = "Close Application";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(btnStart);
            panel3.Controls.Add(btnLoadPXE);
            panel3.Controls.Add(btnStop);
            panel3.Controls.Add(btnMonitor);
            panel3.Location = new Point(0, 75);
            panel3.Margin = new Padding(300, 3, 3, 3);
            panel3.Name = "panel3";
            panel3.Padding = new Padding(30, 0, 0, 0);
            panel3.Size = new Size(224, 493);
            panel3.TabIndex = 16;
            panel3.Paint += panel3_Paint;
            // 
            // btnLoadPXE
            // 
            btnLoadPXE.BackColor = Color.Cyan;
            btnLoadPXE.Font = new Font("Verdana", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLoadPXE.ForeColor = SystemColors.ActiveCaptionText;
            btnLoadPXE.Location = new Point(33, 32);
            btnLoadPXE.Name = "btnLoadPXE";
            btnLoadPXE.Size = new Size(185, 60);
            btnLoadPXE.TabIndex = 1;
            btnLoadPXE.Text = "Load PXE";
            btnLoadPXE.UseVisualStyleBackColor = false;
            btnLoadPXE.Click += btnLoadPXE_Click;
            // 
            // btnMonitor
            // 
            btnMonitor.BackColor = Color.Yellow;
            btnMonitor.Font = new Font("Verdana", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnMonitor.Location = new Point(32, 321);
            btnMonitor.Name = "btnMonitor";
            btnMonitor.Size = new Size(185, 61);
            btnMonitor.TabIndex = 12;
            btnMonitor.Text = "H/W Monitor";
            btnMonitor.UseVisualStyleBackColor = false;
            btnMonitor.Click += btnMonitor_Click;
            // 
            // bootingstatus
            // 
            bootingstatus.AutoSize = true;
            bootingstatus.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            bootingstatus.ForeColor = SystemColors.Control;
            bootingstatus.Location = new Point(264, 91);
            bootingstatus.Name = "bootingstatus";
            bootingstatus.Size = new Size(208, 32);
            bootingstatus.TabIndex = 6;
            bootingstatus.Text = "Currently Booting";
            // 
            // lblStatusCounter
            // 
            lblStatusCounter.AutoSize = true;
            lblStatusCounter.FlatStyle = FlatStyle.Popup;
            lblStatusCounter.Font = new Font("Nirmala UI", 48F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblStatusCounter.ForeColor = Color.Lime;
            lblStatusCounter.Location = new Point(318, 100);
            lblStatusCounter.Name = "lblStatusCounter";
            lblStatusCounter.Size = new Size(109, 128);
            lblStatusCounter.TabIndex = 5;
            lblStatusCounter.Text = "0";
            // 
            // dgvBootMonitor
            // 
            dgvBootMonitor.BackgroundColor = SystemColors.InactiveCaption;
            dgvBootMonitor.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBootMonitor.Dock = DockStyle.Right;
            dgvBootMonitor.Location = new Point(1223, 0);
            dgvBootMonitor.Name = "dgvBootMonitor";
            dgvBootMonitor.ReadOnly = true;
            dgvBootMonitor.RowHeadersWidth = 62;
            dgvBootMonitor.Size = new Size(473, 568);
            dgvBootMonitor.TabIndex = 4;
            // 
            // Tab_Setting
            // 
            Tab_Setting.BackColor = Color.Black;
            Tab_Setting.Controls.Add(panel5_setting);
            Tab_Setting.Controls.Add(panel4_setting);
            Tab_Setting.Controls.Add(panel3_setting);
            Tab_Setting.ForeColor = SystemColors.Control;
            Tab_Setting.Location = new Point(34, 4);
            Tab_Setting.Name = "Tab_Setting";
            Tab_Setting.Size = new Size(1696, 619);
            Tab_Setting.TabIndex = 1;
            Tab_Setting.Text = "Setting";
            // 
            // panel5_setting
            // 
            panel5_setting.Controls.Add(txtExamServer);
            panel5_setting.Controls.Add(label6);
            panel5_setting.Controls.Add(label3);
            panel5_setting.Controls.Add(panel6);
            panel5_setting.Controls.Add(txtServerIP);
            panel5_setting.Controls.Add(label2);
            panel5_setting.Controls.Add(txtStartRange);
            panel5_setting.Controls.Add(label5);
            panel5_setting.Controls.Add(txtEndRange);
            panel5_setting.Controls.Add(numHttpPort);
            panel5_setting.Controls.Add(label4);
            panel5_setting.Dock = DockStyle.Fill;
            panel5_setting.Location = new Point(0, 95);
            panel5_setting.Name = "panel5_setting";
            panel5_setting.Size = new Size(1110, 524);
            panel5_setting.TabIndex = 23;
            // 
            // txtExamServer
            // 
            txtExamServer.Location = new Point(745, 165);
            txtExamServer.Name = "txtExamServer";
            txtExamServer.Size = new Size(233, 31);
            txtExamServer.TabIndex = 23;
            txtExamServer.TextChanged += txtExamServer_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(571, 161);
            label6.Name = "label6";
            label6.Size = new Size(144, 25);
            label6.TabIndex = 22;
            label6.Text = "Exam Server URL";
            // 
            // label3
            // 
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Cyan;
            label3.Location = new Point(51, 218);
            label3.Name = "label3";
            label3.Size = new Size(160, 36);
            label3.TabIndex = 21;
            label3.Text = "DHCP End IP-";
            // 
            // panel6
            // 
            panel6.Controls.Add(btnSave);
            panel6.Dock = DockStyle.Bottom;
            panel6.Location = new Point(0, 374);
            panel6.Name = "panel6";
            panel6.Size = new Size(1110, 150);
            panel6.TabIndex = 20;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.Green;
            btnSave.Font = new Font("Verdana", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSave.ForeColor = Color.Black;
            btnSave.Location = new Point(51, 53);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(190, 69);
            btnSave.TabIndex = 5;
            btnSave.Text = "Save Settings";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // txtServerIP
            // 
            txtServerIP.Location = new Point(217, 83);
            txtServerIP.Name = "txtServerIP";
            txtServerIP.Size = new Size(312, 31);
            txtServerIP.TabIndex = 1;
            // 
            // label2
            // 
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Cyan;
            label2.Location = new Point(51, 81);
            label2.Name = "label2";
            label2.Size = new Size(164, 26);
            label2.TabIndex = 12;
            label2.Text = "Server IP";
            // 
            // txtStartRange
            // 
            txtStartRange.Location = new Point(217, 153);
            txtStartRange.Name = "txtStartRange";
            txtStartRange.Size = new Size(312, 31);
            txtStartRange.TabIndex = 2;
            // 
            // label5
            // 
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.Cyan;
            label5.Location = new Point(51, 148);
            label5.Name = "label5";
            label5.Size = new Size(190, 36);
            label5.TabIndex = 18;
            label5.Text = "DHCP Start IP-";
            // 
            // txtEndRange
            // 
            txtEndRange.Location = new Point(217, 223);
            txtEndRange.Name = "txtEndRange";
            txtEndRange.Size = new Size(312, 31);
            txtEndRange.TabIndex = 3;
            // 
            // numHttpPort
            // 
            numHttpPort.Location = new Point(740, 81);
            numHttpPort.Name = "numHttpPort";
            numHttpPort.Size = new Size(103, 31);
            numHttpPort.TabIndex = 4;
            numHttpPort.Value = new decimal(new int[] { 80, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.Cyan;
            label4.Location = new Point(579, 77);
            label4.Name = "label4";
            label4.Size = new Size(138, 30);
            label4.TabIndex = 10;
            label4.Text = "Port Number";
            // 
            // panel4_setting
            // 
            panel4_setting.Controls.Add(btnDownloadCSV);
            panel4_setting.Controls.Add(rbConfigMode);
            panel4_setting.Controls.Add(rbExamMode);
            panel4_setting.Controls.Add(btnDeleteSelected);
            panel4_setting.Controls.Add(btnLoadData);
            panel4_setting.Dock = DockStyle.Top;
            panel4_setting.Location = new Point(0, 0);
            panel4_setting.Name = "panel4_setting";
            panel4_setting.Size = new Size(1110, 95);
            panel4_setting.TabIndex = 22;
            // 
            // btnDownloadCSV
            // 
            btnDownloadCSV.BackColor = Color.SkyBlue;
            btnDownloadCSV.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDownloadCSV.ForeColor = SystemColors.ActiveCaptionText;
            btnDownloadCSV.Location = new Point(895, 43);
            btnDownloadCSV.Name = "btnDownloadCSV";
            btnDownloadCSV.Size = new Size(172, 44);
            btnDownloadCSV.TabIndex = 21;
            btnDownloadCSV.Text = "Dwonload";
            btnDownloadCSV.UseVisualStyleBackColor = false;
            btnDownloadCSV.Click += btnDownloadCSV_Click;
            // 
            // rbConfigMode
            // 
            rbConfigMode.AutoSize = true;
            rbConfigMode.BackColor = Color.Transparent;
            rbConfigMode.Checked = true;
            rbConfigMode.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rbConfigMode.ForeColor = Color.Cyan;
            rbConfigMode.Location = new Point(34, 50);
            rbConfigMode.Name = "rbConfigMode";
            rbConfigMode.Size = new Size(207, 29);
            rbConfigMode.TabIndex = 10;
            rbConfigMode.TabStop = true;
            rbConfigMode.Text = "Configuration Mode";
            rbConfigMode.UseVisualStyleBackColor = false;
            // 
            // rbExamMode
            // 
            rbExamMode.AutoSize = true;
            rbExamMode.BackColor = SystemColors.ActiveCaptionText;
            rbExamMode.BackgroundImageLayout = ImageLayout.Center;
            rbExamMode.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rbExamMode.ForeColor = Color.Cyan;
            rbExamMode.Location = new Point(266, 47);
            rbExamMode.Name = "rbExamMode";
            rbExamMode.Size = new Size(210, 32);
            rbExamMode.TabIndex = 9;
            rbExamMode.Text = "Enable Exam Mode";
            rbExamMode.UseVisualStyleBackColor = false;
            // 
            // btnDeleteSelected
            // 
            btnDeleteSelected.BackColor = Color.Aqua;
            btnDeleteSelected.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDeleteSelected.ForeColor = Color.Black;
            btnDeleteSelected.Location = new Point(500, 42);
            btnDeleteSelected.Name = "btnDeleteSelected";
            btnDeleteSelected.Size = new Size(172, 44);
            btnDeleteSelected.TabIndex = 7;
            btnDeleteSelected.Text = "Delete Client";
            btnDeleteSelected.UseVisualStyleBackColor = false;
            btnDeleteSelected.Click += btnDeleteSelected_Click;
            // 
            // btnLoadData
            // 
            btnLoadData.BackColor = Color.DeepSkyBlue;
            btnLoadData.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLoadData.ForeColor = SystemColors.ActiveCaptionText;
            btnLoadData.Location = new Point(696, 42);
            btnLoadData.Name = "btnLoadData";
            btnLoadData.Size = new Size(172, 44);
            btnLoadData.TabIndex = 20;
            btnLoadData.Text = "Load Client";
            btnLoadData.UseVisualStyleBackColor = false;
            btnLoadData.Click += btnLoadData_Click;
            // 
            // panel3_setting
            // 
            panel3_setting.Controls.Add(dgvClients);
            panel3_setting.Dock = DockStyle.Right;
            panel3_setting.Location = new Point(1110, 0);
            panel3_setting.Name = "panel3_setting";
            panel3_setting.Size = new Size(586, 619);
            panel3_setting.TabIndex = 21;
            // 
            // dgvClients
            // 
            dgvClients.AllowUserToAddRows = false;
            dgvClients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClients.BackgroundColor = SystemColors.ActiveCaptionText;
            dgvClients.BorderStyle = BorderStyle.Fixed3D;
            dgvClients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvClients.Columns.AddRange(new DataGridViewColumn[] { Column4, Column1, Column2, colUuid, Column3 });
            dgvClients.Dock = DockStyle.Fill;
            dgvClients.Location = new Point(0, 0);
            dgvClients.Name = "dgvClients";
            dgvClients.RowHeadersVisible = false;
            dgvClients.RowHeadersWidth = 62;
            dgvClients.RowTemplate.Height = 30;
            dgvClients.Size = new Size(586, 619);
            dgvClients.TabIndex = 4;
            dgvClients.CurrentCellDirtyStateChanged += dgvClients_CurrentCellDirtyStateChanged;
            // 
            // Column4
            // 
            Column4.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Column4.FillWeight = 227.27272F;
            Column4.HeaderText = ".";
            Column4.MinimumWidth = 8;
            Column4.Name = "Column4";
            Column4.Resizable = DataGridViewTriState.False;
            Column4.Width = 50;
            // 
            // Column1
            // 
            Column1.FillWeight = 57.57576F;
            Column1.HeaderText = "IP Address";
            Column1.MinimumWidth = 8;
            Column1.Name = "Column1";
            // 
            // Column2
            // 
            Column2.FillWeight = 57.57576F;
            Column2.HeaderText = "MAC Address";
            Column2.MinimumWidth = 8;
            Column2.Name = "Column2";
            // 
            // colUuid
            // 
            colUuid.HeaderText = "UUID";
            colUuid.MinimumWidth = 8;
            colUuid.Name = "colUuid";
            colUuid.ReadOnly = true;
            // 
            // Column3
            // 
            Column3.HeaderText = "Seat No";
            Column3.MinimumWidth = 8;
            Column3.Name = "Column3";
            // 
            // gridTimer
            // 
            gridTimer.Interval = 2000;
            // 
            // timer1
            // 
            timer1.Interval = 2000;
            timer1.Tick += timer1_Tick;
            // 
            // panel1
            // 
            panel1.BackColor = Color.Black;
            panel1.Controls.Add(lblStatus);
            panel1.Controls.Add(lblCurrentMode);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1734, 82);
            panel1.TabIndex = 5;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.IndianRed;
            lblStatus.Location = new Point(41, 25);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(95, 25);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Not Ready";
            // 
            // panel2
            // 
            panel2.BackColor = Color.Black;
            panel2.Controls.Add(tabControl1);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 82);
            panel2.Name = "panel2";
            panel2.Size = new Size(1734, 627);
            panel2.TabIndex = 6;
            // 
            // TinyNetboot
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.Black;
            ClientSize = new Size(1734, 741);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(toolStripStatusLabel1);
            ForeColor = SystemColors.ActiveCaptionText;
            Name = "TinyNetboot";
            Text = "TinyNetboot Server";
            WindowState = FormWindowState.Maximized;
            toolStripStatusLabel1.ResumeLayout(false);
            toolStripStatusLabel1.PerformLayout();
            tabControl1.ResumeLayout(false);
            Tab_Dashboard.ResumeLayout(false);
            Tab_Dashboard.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel4.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvBootMonitor).EndInit();
            Tab_Setting.ResumeLayout(false);
            panel5_setting.ResumeLayout(false);
            panel5_setting.PerformLayout();
            panel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numHttpPort).EndInit();
            panel4_setting.ResumeLayout(false);
            panel4_setting.PerformLayout();
            panel3_setting.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvClients).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private RichTextBox txtLogBox;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private StatusStrip toolStripStatusLabel1;
        private TabControl tabControl1;
        private TabPage Tab_Dashboard;
        private TabPage Tab_Setting;
        private NumericUpDown numHttpPort;
        private TextBox txtEndRange;
        private TextBox txtStartRange;
        private TextBox txtServerIP;
        private Button btnSave;
        private Label label2;
        private Label label4;
        private DataGridView dgvClients;
        private RadioButton rbExamMode;
        private System.Windows.Forms.Timer gridTimer;
        private System.Windows.Forms.Timer timer1;
        private RadioButton rbConfigMode;
        private Label lblCurrentMode;
        private Panel panel1;
        private Panel panel2;
        private Label label5;
        private Button btnDeleteSelected;
        private Button btnLoadData;
        private Panel panel3_setting;
        private Panel panel4_setting;
        private Panel panel5_setting;
        private DataGridView dgvBootMonitor;
        private Label lblStatusCounter;
        private Label bootingstatus;
        private Button btnLoadPXE;
        private Label lblStatus;
        private Label lblRamStatus;
        private Label lblCpuStatus;
        private ProgressBar pbRam;
        private ProgressBar pbCpu;
        private Button btnMonitor;
        private Button btnExit;
        private Label label1;
        private Label lblCPU;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;
        private Label label3;
        private Button btnDownloadCSV;
        private TextBox txtExamServer;
        private Label label6;
        private DataGridViewCheckBoxColumn Column4;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn colUuid;
        private DataGridViewTextBoxColumn Column3;
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DronePulseClaude
{
    partial class MainForm
    {
       
            /// <summary>
            /// Required designer variable.
            /// </summary>
            private System.ComponentModel.IContainer components = null;

            // UI Controls
            private GroupBox connectionGroupBox;
            private ComboBox portComboBox;
            private ComboBox baudRateComboBox;
            private Button connectButton;
            private Button refreshPortsButton;
            private Label statusLabel;
            private TextBox logTextBox;
            private Label portLabel;
            private Label baudLabel;
            private Label logLabel;

            // HUD Display Controls
            private GroupBox hudGroupBox;
            private Label airspeedLabel;
            private Label groundspeedLabel;
            private Label altitudeLabel;
            private Label headingLabel;
            private Label throttleLabel;
            private Label climbLabel;
            private Label airspeedValue;
            private Label groundspeedValue;
            private Label altitudeValue;
            private Label headingValue;
            private Label throttleValue;
            private Label climbValue;

            // Attitude Display Controls
            private GroupBox attitudeGroupBox;
            private Label rollLabel;
            private Label pitchLabel;
            private Label yawLabel;
            private Label rollValue;
            private Label pitchValue;
            private Label yawValue;

            // Status Controls
            private Label lastUpdateLabel;
            private Label dataStatusLabel;

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
            connectionGroupBox = new GroupBox();
            portLabel = new Label();
            portComboBox = new ComboBox();
            refreshPortsButton = new Button();
            baudLabel = new Label();
            baudRateComboBox = new ComboBox();
            connectButton = new Button();
            statusLabel = new Label();
            logLabel = new Label();
            logTextBox = new TextBox();
            connectionGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // connectionGroupBox
            // 
            connectionGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            connectionGroupBox.Controls.Add(portLabel);
            connectionGroupBox.Controls.Add(portComboBox);
            connectionGroupBox.Controls.Add(refreshPortsButton);
            connectionGroupBox.Controls.Add(baudLabel);
            connectionGroupBox.Controls.Add(baudRateComboBox);
            connectionGroupBox.Controls.Add(connectButton);
            connectionGroupBox.Controls.Add(statusLabel);
            connectionGroupBox.Location = new Point(10, 10);
            connectionGroupBox.Name = "connectionGroupBox";
            connectionGroupBox.Size = new Size(760, 120);
            connectionGroupBox.TabIndex = 0;
            connectionGroupBox.TabStop = false;
            connectionGroupBox.Text = "Connection Settings";
            // 
            // portLabel
            // 
            portLabel.Location = new Point(20, 30);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(40, 20);
            portLabel.TabIndex = 1;
            portLabel.Text = "Port:";
            portLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // portComboBox
            // 
            portComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            portComboBox.Location = new Point(70, 28);
            portComboBox.Name = "portComboBox";
            portComboBox.Size = new Size(100, 28);
            portComboBox.TabIndex = 2;
            // 
            // refreshPortsButton
            // 
            refreshPortsButton.Location = new Point(180, 27);
            refreshPortsButton.Name = "refreshPortsButton";
            refreshPortsButton.Size = new Size(70, 25);
            refreshPortsButton.TabIndex = 3;
            refreshPortsButton.Text = "Refresh";
            refreshPortsButton.UseVisualStyleBackColor = true;
            // 
            // baudLabel
            // 
            baudLabel.Location = new Point(270, 30);
            baudLabel.Name = "baudLabel";
            baudLabel.Size = new Size(70, 20);
            baudLabel.TabIndex = 4;
            baudLabel.Text = "Baud Rate:";
            baudLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // baudRateComboBox
            // 
            baudRateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            baudRateComboBox.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200", "230400", "460800", "921600" });
            baudRateComboBox.Location = new Point(350, 28);
            baudRateComboBox.Name = "baudRateComboBox";
            baudRateComboBox.Size = new Size(100, 28);
            baudRateComboBox.TabIndex = 5;
            baudRateComboBox.SelectedIndexChanged += RefreshPortsButton_Click;
            // 
            // connectButton
            // 
            connectButton.BackColor = Color.LightGreen;
            connectButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            connectButton.Location = new Point(470, 27);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(100, 30);
            connectButton.TabIndex = 6;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = false;
            connectButton.Click += ConnectButton_Click;
            // 
            // statusLabel
            // 
            statusLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            statusLabel.ForeColor = Color.Red;
            statusLabel.Location = new Point(20, 70);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(300, 20);
            statusLabel.TabIndex = 7;
            statusLabel.Text = "Status: Disconnected";
            // 
            // logLabel
            // 
            logLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            logLabel.Location = new Point(10, 140);
            logLabel.Name = "logLabel";
            logLabel.Size = new Size(150, 20);
            logLabel.TabIndex = 8;
            logLabel.Text = "MAVLink Messages Log:";
            // 
            // logTextBox
            // 
            logTextBox.BackColor = Color.Black;
            logTextBox.Dock = DockStyle.Bottom;
            logTextBox.Font = new Font("Consolas", 9F);
            logTextBox.ForeColor = Color.LimeGreen;
            logTextBox.Location = new Point(0, 163);
            logTextBox.Multiline = true;
            logTextBox.Name = "logTextBox";
            logTextBox.ReadOnly = true;
            logTextBox.ScrollBars = ScrollBars.Vertical;
            logTextBox.Size = new Size(982, 490);
            logTextBox.TabIndex = 9;
            logTextBox.WordWrap = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 653);
            Controls.Add(connectionGroupBox);
            Controls.Add(logLabel);
            Controls.Add(logTextBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MAVLink Parser App";
            connectionGroupBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        private void AttachEventHandlers()
            {
                this.refreshPortsButton.Click += new EventHandler(this.RefreshPortsButton_Click);
                this.connectButton.Click += new EventHandler(this.ConnectButton_Click);
                this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            }

            #endregion
        }
    }

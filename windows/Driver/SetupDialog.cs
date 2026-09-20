using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MoMaRoTa
{
    internal static class Settings
    {
        private const string Key = @"Software\MoMaRoTa\UsbRotator";
        internal static string Port
        {
            get { using (var key = Registry.CurrentUser.OpenSubKey(Key)) return (string)key?.GetValue("Port", "") ?? ""; }
            set { using (var key = Registry.CurrentUser.CreateSubKey(Key)) key.SetValue("Port", value); }
        }
        internal static bool Reverse
        {
            get { using (var key = Registry.CurrentUser.OpenSubKey(Key)) return Convert.ToInt32(key?.GetValue("Reverse", 0) ?? 0) != 0; }
            set { using (var key = Registry.CurrentUser.CreateSubKey(Key)) key.SetValue("Reverse", value ? 1 : 0, RegistryValueKind.DWord); }
        }
        internal static int Speed
        {
            get
            {
                using (var key = Registry.CurrentUser.OpenSubKey(Key))
                {
                    int speed = Convert.ToInt32(key?.GetValue("Speed", 400) ?? 400);
                    return Math.Max(100, Math.Min(4000, speed));
                }
            }
            set { using (var key = Registry.CurrentUser.CreateSubKey(Key)) key.SetValue("Speed", Math.Max(100, Math.Min(4000, value)), RegistryValueKind.DWord); }
        }
    }
    internal sealed class SetupDialog : Form
    {
        internal SetupDialog(Action<string> zeroPosition, Action<int> applySpeed, bool connected)
        {
            Text = Rotator.DisplayName;
            ClientSize = new Size(450, 340); AutoScaleMode = AutoScaleMode.Dpi;
            FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            var label = new Label { Text = "USB COM port (115200 baud)", Location = new Point(20, 20), AutoSize = true };
            var ports = new ComboBox { Location = new Point(20, 48), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
            Action refresh = () => {
                string selected = ports.SelectedItem as string ?? Settings.Port;
                ports.Items.Clear(); string[] names = SerialPort.GetPortNames(); Array.Sort(names); ports.Items.AddRange(names);
                if (!string.IsNullOrEmpty(selected) && !ports.Items.Contains(selected)) ports.Items.Add(selected);
                if (!string.IsNullOrEmpty(selected)) ports.SelectedItem = selected;
            };
            var scan = new Button { Text = "Refresh", Location = new Point(280, 46), Width = 140 };
            scan.Click += (_, __) => refresh(); refresh();
            var reverse = new CheckBox { Text = "Reverse rotation direction", Location = new Point(20, 87), AutoSize = true, Checked = Settings.Reverse };
            var speedLabel = new Label { Text = "Motor speed (100–4000)", Location = new Point(20, 120), AutoSize = true };
            var speed = new NumericUpDown { Location = new Point(280, 116), Width = 140, Minimum = 100, Maximum = 4000, Increment = 100, Value = Settings.Speed };
            var zero = new Button { Text = "Set current position to 0°", Location = new Point(20, 157), Width = 240 };
            zero.Click += (_, __) => {
                try {
                    string selectedPort = ports.SelectedItem as string;
                    if(string.IsNullOrWhiteSpace(selectedPort)) { MessageBox.Show("Select a COM port first."); return; }
                    zeroPosition(selectedPort);
                    MessageBox.Show("Mechanical position and position are now 0°.", Rotator.DisplayName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch(Exception ex) { MessageBox.Show(ex.Message, Rotator.DisplayName, MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };
            var zeroHint = new Label { Text = connected ? "The rotator must be stopped before zeroing." : "The selected COM port is connected temporarily for zeroing.", Location = new Point(20, 192), AutoSize = true };
            var info = new Label { Text = "Close the serial monitor before connecting.\nThe mechanical position is retained across controller restarts.\nUse one USB client at a time.", Location = new Point(20, 220), Size = new Size(410, 60) };
            var ok = new Button { Text = "Save", Location = new Point(250, 295), Width = 80 };
            ok.Click += (_, __) => {
                if (ports.SelectedItem == null) { MessageBox.Show("Select a COM port."); return; }
                Settings.Port = (string)ports.SelectedItem; Settings.Reverse = reverse.Checked; Settings.Speed = (int)speed.Value;
                try { if(connected) applySpeed(Settings.Speed); }
                catch(Exception ex) { MessageBox.Show(ex.Message, Rotator.DisplayName, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                DialogResult = DialogResult.OK; Close();
            };
            var cancel = new Button { Text = "Cancel", Location = new Point(340, 295), Width = 80, DialogResult = DialogResult.Cancel };
            AcceptButton = ok; CancelButton = cancel;
            Controls.AddRange(new Control[] { label, ports, scan, reverse, speedLabel, speed, zero, zeroHint, info, ok, cancel });
        }
    }
}

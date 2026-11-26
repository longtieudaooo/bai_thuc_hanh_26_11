using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PowerGridScadaApp.BUS;
using PowerGridScadaApp.DTO;

namespace PowerGridScadaApp.GUI
{
    public class frmPowerGridMonitor : Form
    {
        private ScadaMonitorBUS bus = new ScadaMonitorBUS();
        private ComboBox cboStation;
        private Button btnStart, btnStop;
        private Label lblVoltage, lblLoadPercent, lblAlertBanner;
        private DataGridView dgvGridLogs;
        private ToolStripStatusLabel lblLastPacket, lblCycleCount;
        private CancellationTokenSource cts;
        private int cycleCount = 0;

        public frmPowerGridMonitor()
        {
            this.Text = "Trung Tâm Điều Độ & Giám Sát Lưới Điện Thời Gian Thực (SCADA)";
            this.Size = new Size(920, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
        }

        private void BuildUI()
        {
            GroupBox grpControl = new GroupBox { Text = "Điều Khiển Kết Nối SCADA", Location = new Point(15, 15), Size = new Size(870, 75) };
            grpControl.Controls.Add(new Label { Text = "Trạm biến áp:", Location = new Point(20, 32), AutoSize = true });

            cboStation = new ComboBox { Location = new Point(115, 29), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
            cboStation.Items.AddRange(new string[] { "Trạm 110kV Hà Đông", "Trạm 220kV Chèm", "Trạm 110kV Mai Động", "Trạm 500kV Thường Tín" });
            cboStation.SelectedIndex = 0;

            btnStart = new Button { Text = "▶ Bắt đầu Giám sát SCADA", Location = new Point(380, 25), Size = new Size(210, 34), BackColor = Color.LightGreen, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnStop = new Button { Text = "⏹ Ngắt Kết nối", Location = new Point(605, 25), Size = new Size(150, 34), BackColor = Color.LightCoral, Enabled = false };

            btnStart.Click += BtnStart_Click;
            btnStop.Click += (s, e) => { cts?.Cancel(); btnStart.Enabled = true; btnStop.Enabled = false; cboStation.Enabled = true; };
            grpControl.Controls.AddRange(new Control[] { cboStation, btnStart, btnStop });

            GroupBox grpDash = new GroupBox { Text = "Thông Số Vận Hành Thời Gian Thực (Real-Time Telemetry)", Location = new Point(15, 105), Size = new Size(870, 150) };
            grpDash.Controls.Add(new Label { Text = "ĐIỆN ÁP THANH CÁI:", Location = new Point(40, 35), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) });
            lblVoltage = new Label { Text = "--- kV", Location = new Point(40, 65), AutoSize = true, Font = new Font("Consolas", 26F, FontStyle.Bold), ForeColor = Color.Navy };

            grpDash.Controls.Add(new Label { Text = "MỨC MANG TẢI MÁY BIẾN ÁP:", Location = new Point(360, 35), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) });
            lblLoadPercent = new Label { Text = "--.- %", Location = new Point(360, 65), AutoSize = true, Font = new Font("Consolas", 26F, FontStyle.Bold), ForeColor = Color.DarkGreen };

            lblAlertBanner = new Label { Text = "TRẠNG THÁI: CHỜ KẾT NỐI", Location = new Point(650, 75), AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.Gray };
            grpDash.Controls.AddRange(new Control[] { lblVoltage, lblLoadPercent, lblAlertBanner });

            Label lblLogTitle = new Label { Text = "NHẬT KÝ SỰ CỐ QUÁ TẢI (> 85%)", Location = new Point(15, 270), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.DarkRed };
            dgvGridLogs = new DataGridView { Location = new Point(15, 298), Size = new Size(870, 240), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false };
            dgvGridLogs.Columns.Add("colTime", "Thời Gian Ghi Nhận");
            dgvGridLogs.Columns.Add("colStation", "Trạm Biến Áp");
            dgvGridLogs.Columns.Add("colVolt", "Điện Áp (kV)");
            dgvGridLogs.Columns.Add("colLoad", "Mức Tải (%)");
            dgvGridLogs.Columns.Add("colNote", "Cảnh Báo Sự Cố");

            StatusStrip status = new StatusStrip();
            lblLastPacket = new ToolStripStatusLabel("Gói tin cuối: --:--:--   |   ");
            lblCycleCount = new ToolStripStatusLabel("Tổng chu kỳ đã quét: 0");
            status.Items.AddRange(new ToolStripItem[] { lblLastPacket, lblCycleCount });

            this.Controls.AddRange(new Control[] { grpControl, grpDash, lblLogTitle, dgvGridLogs, status });
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            cboStation.Enabled = false;
            cycleCount = 0;
            cts = new CancellationTokenSource();

            string selectedStation = cboStation.Text;
            CancellationToken token = cts.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    GridTelemetry data = bus.ThuThapGoiTinScada(selectedStation);
                    cycleCount++;
                    UpdateDashboardThreadSafe(data, cycleCount);
                    try { await Task.Delay(2000, token); } catch (TaskCanceledException) { break; }
                }
            }, token);
        }

        private void UpdateDashboardThreadSafe(GridTelemetry data, int currentCycle)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateDashboardThreadSafe(data, currentCycle)));
                return;
            }

            lblVoltage.Text = $"{data.VoltageKV:F2} kV";
            lblLoadPercent.Text = $"{data.LoadPercent:F1} %";

            if (data.IsOverload)
            {
                lblLoadPercent.ForeColor = Color.Red;
                lblAlertBanner.Text = "⚠️ CẢNH BÁO QUÁ TẢI!";
                lblAlertBanner.ForeColor = Color.Red;
                dgvGridLogs.Rows.Insert(0, data.Timestamp.ToString("HH:mm:ss dd/MM/yyyy"), data.StationName, $"{data.VoltageKV:F2}", $"{data.LoadPercent:F1}%", "Vượt ngưỡng an toàn (> 85%)");
            }
            else
            {
                lblLoadPercent.ForeColor = Color.Green;
                lblAlertBanner.Text = "✔ VẬN HÀNH ỔN ĐỊNH";
                lblAlertBanner.ForeColor = Color.Green;
            }

            lblLastPacket.Text = $"Gói tin cuối: {data.Timestamp:HH:mm:ss}   |   ";
            lblCycleCount.Text = $"Tổng chu kỳ đã quét: {currentCycle}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            cts?.Cancel();
            base.OnFormClosing(e);
        }
    }
}
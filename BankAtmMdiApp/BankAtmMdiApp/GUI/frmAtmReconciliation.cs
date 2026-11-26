using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BankAtmMdiApp.BUS;
using BankAtmMdiApp.DTO;

namespace BankAtmMdiApp.GUI
{
    public class frmAtmReconciliation : Form
    {
        private AtmBUS bus = new AtmBUS();
        private Button btnStart, btnStop;
        private Label lblStatus;
        private ListBox lstLog;
        private CancellationTokenSource cts;

        public frmAtmReconciliation()
        {
            this.Text = "Đối Soát Giao Dịch ATM Cuối Ngày (Async Non-Blocking)";
            this.Size = new Size(720, 440);
            BuildUI();
        }

        private void BuildUI()
        {
            btnStart = new Button { Text = "▶ Chạy Đối Soát Tự Động", Location = new Point(20, 20), Size = new Size(200, 40), BackColor = Color.LightSkyBlue, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnStop = new Button { Text = "⏹ Dừng Đối Soát", Location = new Point(235, 20), Size = new Size(150, 40), BackColor = Color.LightCoral, Enabled = false };
            lblStatus = new Label { Text = "Trạng thái: Sẵn sàng đối soát", Location = new Point(20, 75), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            lstLog = new ListBox { Location = new Point(20, 105), Size = new Size(660, 280), Font = new Font("Consolas", 9.5F) };

            btnStart.Click += async (s, e) =>
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                cts = new CancellationTokenSource();
                lstLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] Bắt đầu tải dữ liệu sao kê từ Core Banking...");

                frmBankAtmMain parentForm = this.MdiParent as frmBankAtmMain;
                var progress = new Progress<int>(percent =>
                {
                    lblStatus.Text = $"Đang đối soát dữ liệu giao dịch... {percent}%";
                    if (parentForm != null) parentForm.MainProgressBar.Value = percent;
                });

                try
                {
                    ReconcileResult res = await bus.ChayDoiSoatTuDongAsync(progress, cts.Token);
                    lblStatus.Text = "Trạng thái: Hoàn tất đối soát!";
                    lstLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] HOÀN TẤT: Tổng {res.TotalTransactions:N0} GD | Khớp: {res.MatchedCount:N0} | Lệch: {res.DiscrepancyCount}");
                    lstLog.Items.Add($"   -> Ghi chú: {res.Note}");
                }
                catch (OperationCanceledException)
                {
                    lblStatus.Text = "Trạng thái: Đã hủy tiến trình đối soát!";
                    lstLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] CẢNH BÁO: Người dùng đã dừng tiến trình giữa chừng.");
                    if (parentForm != null) parentForm.MainProgressBar.Value = 0;
                }
                finally
                {
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                }
            };

            btnStop.Click += (s, e) => cts?.Cancel();
            this.Controls.AddRange(new Control[] { btnStart, btnStop, lblStatus, lstLog });
        }
    }
}
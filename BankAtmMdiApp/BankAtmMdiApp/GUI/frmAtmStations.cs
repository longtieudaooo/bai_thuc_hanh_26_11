using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BankAtmMdiApp.BUS;
using BankAtmMdiApp.DTO;

namespace BankAtmMdiApp.GUI
{
    public class frmAtmStations : Form
    {
        private AtmBUS bus = new AtmBUS();
        private DataGridView dgvStations;
        private TextBox txtID, txtAddress, txtCash;
        private ComboBox cboStatus;
        private PictureBox picAtm;
        private string currentImagePath = "";

        public frmAtmStations()
        {
            this.Text = "Quản Lý Danh Sách Trạm ATM & Tồn Quỹ";
            this.Size = new Size(920, 540);
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            GroupBox grp = new GroupBox { Text = "Thông Tin Trạm ATM", Location = new Point(15, 15), Size = new Size(870, 185) };

            grp.Controls.Add(new Label { Text = "Mã Trạm:", Location = new Point(20, 30), AutoSize = true });
            txtID = new TextBox { Location = new Point(110, 27), Width = 180 };

            grp.Controls.Add(new Label { Text = "Địa chỉ đặt máy:", Location = new Point(20, 68), AutoSize = true });
            txtAddress = new TextBox { Location = new Point(110, 65), Width = 360 };

            grp.Controls.Add(new Label { Text = "Tồn quỹ (VNĐ):", Location = new Point(20, 106), AutoSize = true });
            txtCash = new TextBox { Location = new Point(110, 103), Width = 180 };

            grp.Controls.Add(new Label { Text = "Trạng thái:", Location = new Point(310, 106), AutoSize = true });
            cboStatus = new ComboBox { Location = new Point(380, 103), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cboStatus.Items.AddRange(new string[] { "Hoạt động", "Sắp hết tiền", "Bảo trì", "Mất kết nối" });
            cboStatus.SelectedIndex = 0;

            picAtm = new PictureBox { Location = new Point(660, 22), Size = new Size(180, 120), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };
            Button btnChonAnh = new Button { Text = "Đính kèm ảnh trạm...", Location = new Point(660, 148), Size = new Size(180, 28) };
            btnChonAnh.Click += (s, e) =>
            {
                using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        currentImagePath = ofd.FileName;
                        picAtm.Image = Image.FromFile(currentImagePath);
                    }
                }
            };

            Button btnLuu = new Button { Text = "Lưu Trạm ATM", Location = new Point(110, 142), Size = new Size(140, 32), BackColor = Color.LightGreen };
            btnLuu.Click += (s, e) =>
            {
                if (!decimal.TryParse(txtCash.Text, out decimal cash)) { MessageBox.Show("Số tiền phải là số!"); return; }
                AtmStation atm = new AtmStation
                {
                    StationID = txtID.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    CashBalance = cash,
                    Status = cboStatus.Text,
                    ImagePath = currentImagePath
                };
                string res = bus.LuuThongTinTram(atm);
                if (res == "OK") { MessageBox.Show("Lưu thành công!"); LoadData(); }
                else MessageBox.Show(res);
            };

            grp.Controls.AddRange(new Control[] { txtID, txtAddress, txtCash, cboStatus, picAtm, btnChonAnh, btnLuu });

            dgvStations = new DataGridView { Location = new Point(15, 215), Size = new Size(870, 270), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false };
            dgvStations.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var row = dgvStations.Rows[e.RowIndex];
                    txtID.Text = row.Cells["StationID"].Value.ToString();
                    txtAddress.Text = row.Cells["Address"].Value.ToString();
                    txtCash.Text = row.Cells["CashBalance"].Value.ToString();
                    cboStatus.Text = row.Cells["Status"].Value.ToString();
                    currentImagePath = row.Cells["ImagePath"].Value?.ToString() ?? "";
                    picAtm.Image = (!string.IsNullOrEmpty(currentImagePath) && File.Exists(currentImagePath)) ? Image.FromFile(currentImagePath) : null;
                }
            };

            this.Controls.AddRange(new Control[] { grp, dgvStations });
        }

        private void LoadData() => dgvStations.DataSource = bus.LayDanhSachTram();
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BankAtmMdiApp.GUI
{
    public class frmBankAtmMain : Form
    {
        private ToolStripStatusLabel lblTeller;
        private ToolStripStatusLabel lblCoreStatus;
        public ToolStripProgressBar MainProgressBar { get; private set; }

        public frmBankAtmMain()
        {
            this.Text = "Hệ Thống Điều Hành Mạng Lưới ATM Ngân Hàng (MDI Core)";
            this.Size = new Size(1150, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.IsMdiContainer = true;

            BuildMenuStrip();
            BuildStatusStrip();
        }

        private void BuildMenuStrip()
        {
            MenuStrip menu = new MenuStrip();

            ToolStripMenuItem mnuSystem = new ToolStripMenuItem("Hệ thống");
            mnuSystem.DropDownItems.Add("Đăng xuất", null, (s, e) => MessageBox.Show("Đã đăng xuất phiên làm việc!"));
            mnuSystem.DropDownItems.Add("Khóa màn hình", null, (s, e) => MessageBox.Show("Màn hình đã khóa tạm thời!"));
            mnuSystem.DropDownItems.Add(new ToolStripSeparator());
            mnuSystem.DropDownItems.Add("Thoát", null, (s, e) => Application.Exit());

            ToolStripMenuItem mnuAtm = new ToolStripMenuItem("Quản lý ATM");
            mnuAtm.DropDownItems.Add("Danh sách Trạm ATM & Tồn quỹ", null, (s, e) => OpenChildForm<frmAtmStations>());
            mnuAtm.DropDownItems.Add("Đối soát Giao dịch Cuối ngày", null, (s, e) => OpenChildForm<frmAtmReconciliation>());

            ToolStripMenuItem mnuWindow = new ToolStripMenuItem("Cửa sổ");
            mnuWindow.DropDownItems.Add("Xếp tầng (Cascade)", null, (s, e) => this.LayoutMdi(MdiLayout.Cascade));
            mnuWindow.DropDownItems.Add("Xếp ngang (Tile Horizontal)", null, (s, e) => this.LayoutMdi(MdiLayout.TileHorizontal));
            mnuWindow.DropDownItems.Add("Xếp dọc (Tile Vertical)", null, (s, e) => this.LayoutMdi(MdiLayout.TileVertical));
            mnuWindow.DropDownItems.Add(new ToolStripSeparator());
            mnuWindow.DropDownItems.Add("Đóng tất cả", null, (s, e) =>
            {
                foreach (Form child in this.MdiChildren) child.Close();
            });

            menu.Items.AddRange(new ToolStripItem[] { mnuSystem, mnuAtm, mnuWindow });
            this.MainMenuStrip = menu;
            this.Controls.Add(menu);
        }

        private void BuildStatusStrip()
        {
            StatusStrip status = new StatusStrip();
            lblTeller = new ToolStripStatusLabel("GDV Trực: Lại Thanh Long  |  ");
            lblCoreStatus = new ToolStripStatusLabel("Core Banking: Online  |  ") { ForeColor = Color.DarkGreen };
            MainProgressBar = new ToolStripProgressBar { Size = new Size(200, 16), Minimum = 0, Maximum = 100 };

            status.Items.AddRange(new ToolStripItem[] { lblTeller, lblCoreStatus, new ToolStripStatusLabel("Tiến độ đối soát: "), MainProgressBar });
            this.Controls.Add(status);
        }

        private void OpenChildForm<T>() where T : Form, new()
        {
            foreach (Form child in this.MdiChildren)
            {
                if (child is T)
                {
                    child.Activate();
                    return;
                }
            }
            T newForm = new T();
            newForm.MdiParent = this;
            newForm.Show();
        }
    }
}
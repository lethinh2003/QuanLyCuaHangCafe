using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Util;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Enum.Material;
using ReaLTaiizor.Colors;
using System.IO;

namespace QuanLyCafe.DanhSachForm
{
    public partial class ChinhSuaBanForm : MaterialForm
    {
        Ban _banChon = null;

        public ChinhSuaBanForm()
        {
            InitializeComponent();
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        private void ChinhSuaBanForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (TaiKhoanHienTai.TaiKhoanHienHanh == null)
                {
                    System.Environment.Exit(0);
                    return;
                }
                if (TaiKhoanHienTai.TaiKhoanHienHanh.QuyenHan != 2)
                {
                    System.Environment.Exit(0);
                    return;
                }
                CreateBorderRadius();
                LoadDanhSachBan();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        #region Các hàm khởi tạo
        void CreateBorderRadius()
        {
            pnlForm.Anchor = AnchorStyles.None;
            pnlForm.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlForm.Width, pnlForm.Height, 15, 15)
            );
            pnlTongQuanBan.Anchor = AnchorStyles.None;
            pnlTongQuanBan.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlTongQuanBan.Width, pnlTongQuanBan.Height, 15, 15)
            );

            picHienThiBan.Anchor = AnchorStyles.None;
            picHienThiBan.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, picHienThiBan.Width, picHienThiBan.Height, 15, 15)
            );

            picHienThiBanThem.Anchor = AnchorStyles.None;
            picHienThiBanThem.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, picHienThiBanThem.Width, picHienThiBanThem.Height, 15, 15)
            );

            pnlThemBan.Anchor = AnchorStyles.None;
            pnlThemBan.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlThemBan.Width, pnlThemBan.Height, 15, 15)
            );
        }

        void LoadDanhSachBan()
        {
            Database.CreateConnection();
            string sqlCommand =
                $"select ID as ID_BAN, TEN as TEN_BAN, TINHTRANG as TINHTRANG_BAN, ID_DATBAN as ID_DATBAN_BAN, ID_HOADON as ID_HOADON_BAN from DANHSACHBAN";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            dgvDanhSachBan.DataSource = dt;
            if (ControlForm.FormMain != null)
            {
                ControlForm.FormMain.LoadDanhSachBan();
                ControlForm.FormMain.LoadFlowPanelDanhSachBan();
            }
        }
        #endregion

        #region Các hàm sự kiện
        private void dgvDanhSachBan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1)
                {
                    return;
                }

                DataGridViewRow row = new DataGridViewRow();
                row = dgvDanhSachBan.Rows[e.RowIndex];
                string ID = row.Cells["ID_BAN"].Value.ToString();

                _banChon = DanhSachBan.TimKiemBan(ID);

                if (_banChon == null)
                {
                    btnLuu.Visible = false;
                    btnXoa.Visible = false;
                    throw new Exception("Không tìm thấy bàn");
                }
                btnLuu.Visible = true;
                btnXoa.Visible = true;
                if (_banChon.TinhTrang == 0)
                {
                    Image image = Properties.Resources.table_chuadat;
                    picHienThiBan.Image = image;
                }
                else
                {
                    Image image = Properties.Resources.table_dadat;
                    picHienThiBan.Image = image;
                }

                HienThiThongTinBan();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (_banChon == null)
                {
                    return;
                }
                if (ControlForm.ConfirmForm("Bạn có muốn lưu"))
                {
                    string tenBan = txtTenBan.Text.Trim();
                    if (string.IsNullOrEmpty(tenBan))
                    {
                        throw new Exception("Vui lòng nhập đầy đủ thông tin");
                    }

                    Database.CreateConnection();
                    string sqlCommand =
                        $"update DANHSACHBAN set TEN = N'{tenBan}'where ID = '{_banChon.ID}'";
                    SqlCommand cmd;
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();
                    Ban getBan = DanhSachBan.TimKiemBan(_banChon.ID);
                    getBan.TenBan = tenBan;
                    HienThiThongTinBan();
                    LoadDanhSachBan();
                    if (ControlForm.FormMain != null)
                    {
                        ControlForm.FormMain.LoadFlowPanelDanhSachBan();
                    }
                    MessageBox.Show("Thành công");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (_banChon == null)
                {
                    return;
                }
           
                if (ControlForm.ConfirmForm("Bạn có muốn xóa? Khi xóa bàn thì sẽ xóa tất cả mọi thứ liên quan đến bàn này"))
                {
                    if (_banChon.TinhTrang == 1)
                    {
                        throw new Exception("Bàn đang có khách, không thể xóa");
                    }

                    Database.CreateConnection();
                    string sqlCommand;
                    SqlCommand cmd;
                    SqlDataReader rd;
                    // Xóa các thứ liên quan đến sản phẩm
                    List<string> danhSachIDLichSuOrder = new List<string>();
                    List<string> danhSachIDLichSuDatBan = new List<string>();
                    List<string> danhSachIDHoaDon = new List<string>();

                    sqlCommand =
                       $"select A.ID as ID_LICHSUORDER, B.ID_BAN as ID_BAN, B.ID as ID_LICHSUDATBAN, D.ID as ID_HOADON   from LICHSUORDER A inner join LICHSUDATBAN B on A.ID_DATBAN = B.ID inner join DANHSACHBAN C on B.ID_BAN = C.ID and C.ID = '{_banChon.ID}' inner join HOADON D on D.ID_DATBAN = B.ID";
                    cmd = Database.CreateCommand(sqlCommand);
                    rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        danhSachIDLichSuOrder.Add(rd["ID_LICHSUORDER"].ToString());
                        danhSachIDLichSuDatBan.Add(rd["ID_LICHSUDATBAN"].ToString());
                        danhSachIDHoaDon.Add(rd["ID_HOADON"].ToString());
                    }
                    rd.Close();
                    Database.CreateConnection();
                    foreach (var item in danhSachIDHoaDon)
                    {
                        sqlCommand = $"delete HOADON where ID = '{item}'";
                        cmd = Database.CreateCommand(sqlCommand);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (var item in danhSachIDLichSuOrder)
                    {
                        sqlCommand = $"delete LICHSUORDER where ID = '{item}'";
                        cmd = Database.CreateCommand(sqlCommand);
                        cmd.ExecuteNonQuery();
                    }
                    foreach (var item in danhSachIDLichSuDatBan)
                    {
                        sqlCommand = $"delete LICHSUDATBAN where ID = '{item}'";
                        cmd = Database.CreateCommand(sqlCommand);
                        cmd.ExecuteNonQuery();
                    }
                   
                    // xóa bàn

                    sqlCommand = $"delete DANHSACHBAN where ID = '{_banChon.ID}'";
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();

                    btnLuu.Visible = false;
                    btnXoa.Visible = false;

                    LoadDanhSachBan();

                    MessageBox.Show("Xóa thành công");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnThemBan_Click(object sender, EventArgs e)
        {
            try
            {
                string IDBan = txtIDBanThem.Text.Trim();
                string tenBan = txtTenBanThem.Text.Trim();
                if (string.IsNullOrEmpty(tenBan) || string.IsNullOrEmpty(IDBan))
                {
                    throw new Exception("Vui lòng nhập đầy đủ thông tin");
                }
                if (ControlForm.ConfirmForm("Bạn có muốn thêm?"))
                {
                    Database.CreateConnection();
                    string sqlCommand =
                        $"insert into DANHSACHBAN (ID, TEN) values ('{IDBan}', N'{tenBan}')";
                    SqlCommand cmd;
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();
                    Ban banMoi = new Ban(IDBan, tenBan, 0);
                    DanhSachBan.AddBan(banMoi);
                    LoadDanhSachBan();
                    ReloadTabPageThem();

   
                    MessageBox.Show("Thành công");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void txtIDBanThem_Leave(object sender, EventArgs e)
        {
            txtIDBanThem.Text = txtIDBanThem.Text.ToUpper();
        }

        private void btnTimKiemBan_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtTimKiemBan.Text.Trim();

                Database.CreateConnection();
                string sqlCommand =
                    $"select ID as ID_BAN, TEN as TEN_BAN, TINHTRANG as TINHTRANG_BAN, ID_DATBAN as ID_DATBAN_BAN, ID_HOADON as ID_HOADON_BAN from DANHSACHBAN where TEN like N'%{searchValue}%'";
                DataTable dt;
                dt = Database.SelectQuery(sqlCommand);
                dgvDanhSachBan.DataSource = dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void tabForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabForm.SelectedTab == tpChinhSua)
                {
                    ReloadTabPageChinhSua();
                }
                else if (tabForm.SelectedTab == tpThem)
                {
                    ReloadTabPageThem();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        #endregion

        #region Các hàm phục vụ
        void HienThiThongTinBan()
        {
            if (_banChon != null)
            {
                txtIDBan.Text = _banChon.ID;
                txtTenBan.Text = _banChon.TenBan;
                txtTinhTrang.Text = _banChon.TinhTrang.ToString();
            }
        }

        void ReloadTabPageChinhSua()
        {
            DataTable dt = (DataTable)dgvDanhSachBan.DataSource;
            if (dt != null)
            {
                dt.Clear();
            }
            LoadDanhSachBan();
            _banChon = null;
            txtTimKiemBan.Text = null;
            btnLuu.Visible = false;
            btnXoa.Visible = false;
            txtIDBan.Text = null;
            txtTenBan.Text = null;
            txtTinhTrang.Text = null;
        }

        void ReloadTabPageThem()
        {
            txtIDBanThem.Text = null;
            txtTenBanThem.Text = null;
        }

        #endregion
    }
}

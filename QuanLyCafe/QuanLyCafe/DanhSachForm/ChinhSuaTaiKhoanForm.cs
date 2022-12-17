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
using BC = BCrypt.Net.BCrypt;

namespace QuanLyCafe.DanhSachForm
{
    public partial class ChinhSuaTaiKhoanForm : MaterialForm
    {
        TaiKhoan _taiKhoanChon = null;

        public ChinhSuaTaiKhoanForm()
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

        private void ChinhSuaTaiKhoanForm_Load(object sender, EventArgs e)
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
                CustomFormatDate();
                CreateBorderRadius();
                LoadDanhSachQuyenHan();
                LoadDanhSachTaiKhoan();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        #region Các hàm khởi tạo
        // Custom format date
        void CustomFormatDate()
        {
            dtpTuNgay.CustomFormat = "yyyy-MM-dd";
            dtpDenNgay.CustomFormat = "yyyy-MM-dd";
        }

        // Tạo border radius cho các control
        void CreateBorderRadius()
        {
            pnlForm.Anchor = AnchorStyles.None;
            pnlForm.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlForm.Width, pnlForm.Height, 15, 15)
            );
            pnlBaoCao.Anchor = AnchorStyles.None;
            pnlBaoCao.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlBaoCao.Width, pnlBaoCao.Height, 15, 15)
            );

            pnlTongQuan.Anchor = AnchorStyles.None;
            pnlTongQuan.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlTongQuan.Width, pnlTongQuan.Height, 15, 15)
            );
            picTaiKhoan.Anchor = AnchorStyles.None;
            picTaiKhoan.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, picTaiKhoan.Width, picTaiKhoan.Height, 15, 15)
            );

            picTaiKhoanThem.Anchor = AnchorStyles.None;
            picTaiKhoanThem.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, picTaiKhoanThem.Width, picTaiKhoanThem.Height, 15, 15)
            );

            pnlThemTaiKhoan.Anchor = AnchorStyles.None;
            pnlThemTaiKhoan.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlThemTaiKhoan.Width, pnlThemTaiKhoan.Height, 15, 15)
            );
        }

        void LoadDanhSachQuyenHan()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from QUYENHAN";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);

            cboQuyenHan.DataSource = dt;
            cboQuyenHan.DisplayMember = "TEN";
            cboQuyenHan.ValueMember = "ID";

            cboQuyenHanThem.DataSource = dt;
            cboQuyenHanThem.DisplayMember = "TEN";
            cboQuyenHanThem.ValueMember = "ID";
        }

        void LoadDanhSachTaiKhoan()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from TAIKHOAN";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            dgvDanhSachTaiKhoan.DataSource = dt;
            if (ControlForm.FormMain != null)
            {
                ControlForm.FormMain.LoadDanhSachTaiKhoan();
            }
        }
        #endregion

        #region Các hàm sự kiện
        private void dgvDanhSachTaiKhoan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1)
                {
                    return;
                }

                DataGridViewRow row = new DataGridViewRow();
                row = dgvDanhSachTaiKhoan.Rows[e.RowIndex];
                string username = row.Cells["USERNAME"].Value.ToString();

                _taiKhoanChon = DanhSachTaiKhoan.TimKiem(username);
                if (_taiKhoanChon == null)
                {
                    btnLuu.Visible = false;
                    btnXoa.Visible = false;
                    pnlBaoCao.Visible = false;
                    btnCapNhatMatKhau.Visible = false;
                    throw new Exception("Không tìm thấy tài khoản");
                }
                btnLuu.Visible = true;
                btnXoa.Visible = true;
                pnlBaoCao.Visible = true;
                btnCapNhatMatKhau.Visible = true;

                HienThiThongTinTaiKhoan();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnTimKiemTaiKhoan_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtTimKiemTaiKhoan.Text.Trim();
                Database.CreateConnection();
                string sqlCommand = $"select * from TAIKHOAN where USERNAME like '%{searchValue}%'";
                DataTable dt;
                dt = Database.SelectQuery(sqlCommand);
                dgvDanhSachTaiKhoan.DataSource = dt;
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
                if (_taiKhoanChon == null)
                {
                    return;
                }
                if (
                    TaiKhoanHienTai.TaiKhoanHienHanh.UserName != _taiKhoanChon.UserName
                    && TaiKhoanHienTai.TaiKhoanHienHanh.QuyenHan <= _taiKhoanChon.QuyenHan
                )
                {
                    throw new Exception("Bạn không có quyền chỉnh sửa tài khoản này");
                }
                if (ControlForm.ConfirmForm("Bạn có muốn lưu"))
                {
                    string firstName = txtFirstName.Text.Trim();
                    string lastName = txtLastName.Text.Trim();
                    string phone = txtPhone.Text.Trim();
                    string cccd = txtCCCD.Text.Trim();
                    string address = txtAddress.Text.Trim();
                    int quyenHan = int.Parse(cboQuyenHan.SelectedValue.ToString());

                    if (
                        string.IsNullOrEmpty(firstName)
                        || string.IsNullOrEmpty(lastName)
                        || string.IsNullOrEmpty(phone)
                        || string.IsNullOrEmpty(cccd)
                        || string.IsNullOrEmpty(address)
                    )
                    {
                        throw new Exception("Vui lòng nhập thông tin hợp lệ");
                    }

                    Database.CreateConnection();
                    string sqlCommand =
                        $"update TAIKHOAN set FIRSTNAME = N'{firstName}',  LASTNAME = N'{lastName}',PHONE = '{phone}', CCCD = '{cccd}', ADDRESS = N'{address}', QUYENHAN = '{quyenHan}' where USERNAME = '{_taiKhoanChon.UserName}'";
                    SqlCommand cmd;
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();
                    int quyenHanCu = _taiKhoanChon.QuyenHan;
                    _taiKhoanChon.QuyenHan = quyenHan;
                    _taiKhoanChon.CCCD = cccd;
                    _taiKhoanChon.Phone = phone;
                    _taiKhoanChon.FirstName = firstName;
                    _taiKhoanChon.LastName = lastName;
                    _taiKhoanChon.Address = address;

                    LoadDanhSachTaiKhoan();

                    MessageBox.Show("Thành công");
                    if (_taiKhoanChon.UserName == TaiKhoanHienTai.TaiKhoanHienHanh.UserName && quyenHanCu != quyenHan)
                    {
                        Environment.Exit(1);
                    }
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
                if (_taiKhoanChon == null)
                {
                    return;
                }
                if (
                    TaiKhoanHienTai.TaiKhoanHienHanh.UserName != _taiKhoanChon.UserName
                    && TaiKhoanHienTai.TaiKhoanHienHanh.QuyenHan <= _taiKhoanChon.QuyenHan
                )
                {
                    throw new Exception("Bạn không có quyền chỉnh sửa tài khoản này");
                }
                if (
                    ControlForm.ConfirmForm(
                        "Bạn có muốn xóa không? Hành động này sẽ xóa mọi thứ liên quan đến tài khoản này"
                    )
                )
                {
                    Database.CreateConnection();
                    string sqlCommand;
                    SqlCommand cmd;
                    SqlDataReader rd;
                    // Kiểm tra xem tài khoản có đang trong ca làm hay không (đang order cho khách)
                    sqlCommand =
                        $"select * from HOADON where NHANVIEN = '{_taiKhoanChon.UserName}' and THANHTOAN = '0'";
                    cmd = Database.CreateCommand(sqlCommand);
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        throw new Exception("Tài khoản đang trong ca làm, không thể xóa");
                    }

                    Database.CreateConnection();

                    // Kiểm tra xem tài khoản đã được thanh toán tiền chưa
                    sqlCommand =
                        $"select * from LICHSUTHANHTOANCA where USERNAME = '{_taiKhoanChon.UserName}' and THANHTOAN = '0'";
                    cmd = Database.CreateCommand(sqlCommand);
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        throw new Exception("Tài khoản chưa được thanh toán tiền hết");
                    }
                    Database.CreateConnection();
                    // xóa hóa đơn
                    sqlCommand =
                        $"delete from HOADON where NHANVIEN = '{_taiKhoanChon.UserName}' and THANHTOAN = '1'";
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();

                    // xóa lịch sử ca
                    sqlCommand =
                        $"delete from LICHSUCA where USERNAME = '{_taiKhoanChon.UserName}'";
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();

                    // xóa lịch sử thanh toán ca
                    sqlCommand =
                        $"delete from LICHSUTHANHTOANCA where USERNAME = '{_taiKhoanChon.UserName}'";
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();

                    // xóa tài khoản 
                    sqlCommand =
                 $"delete from TAIKHOAN where USERNAME = '{_taiKhoanChon.UserName}'";
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();

                    if (_taiKhoanChon.UserName == TaiKhoanHienTai.TaiKhoanHienHanh.UserName)
                    {
                        Environment.Exit(1);
                    }

                    ReloadTabPageChinhSua();
                 

                    MessageBox.Show("Thành công");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnCapNhatMatKhau_Click(object sender, EventArgs e)
        {
            try
            {
                if (_taiKhoanChon == null)
                {
                    return;
                }
                if (
                    TaiKhoanHienTai.TaiKhoanHienHanh.UserName != _taiKhoanChon.UserName
                    && TaiKhoanHienTai.TaiKhoanHienHanh.QuyenHan <= _taiKhoanChon.QuyenHan
                )
                {
                    throw new Exception("Bạn không có quyền chỉnh sửa tài khoản này");
                }
                if (ControlForm.ConfirmForm("Bạn có muốn lưu"))
                {
                    string password = txtPassword.Text.Trim();

                    if (string.IsNullOrEmpty(password))
                    {
                        throw new Exception("Vui lòng nhập thông tin hợp lệ");
                    }

                    string passwordHash = BC.HashPassword(password);
                    Database.CreateConnection();
                    string sqlCommand =
                        $"update TAIKHOAN set PASSWORD = '{passwordHash}' where USERNAME = '{_taiKhoanChon.UserName}'";
                    SqlCommand cmd;
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();
                    _taiKhoanChon.Password = password;
                    txtPassword.Text = null;
                    LoadDanhSachTaiKhoan();

                    MessageBox.Show("Thành công");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnBaoCaoDoanhThu_Click(object sender, EventArgs e) {

            try
            {
                BaoCao baoCaoMoi = new BaoCao("doanhthuhoadoncanhan", _taiKhoanChon.UserName);
                baoCaoMoi.SetBaoCaoDoanhThu(
                    dtpTuNgay.Value,
                    dtpDenNgay.Value
                );
                ControlForm.BaoCaoHienTai = baoCaoMoi;
                DanhSachForm.ReportViewForm fBaoCao = new DanhSachForm.ReportViewForm();
                fBaoCao.ShowDialog();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnBaoCaoCaLam_Click(object sender, EventArgs e) {
            try
            {
                DateTime getDateTimeBatDau = dtpTuNgay.Value;
                DateTime getFilterDateBatDau = new DateTime(
                    getDateTimeBatDau.Year,
                    getDateTimeBatDau.Month,
                    getDateTimeBatDau.Day
                );

                DateTime getDateTimeKetThuc = dtpDenNgay.Value;
                DateTime getFilterDateKetThuc = new DateTime(
                    getDateTimeKetThuc.Year,
                    getDateTimeKetThuc.Month,
                    getDateTimeKetThuc.Day
                );

                DateTime getCurrentDate = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day
                );
                if (
                    getFilterDateBatDau > getCurrentDate
                    || getFilterDateKetThuc > getCurrentDate
                    || getFilterDateKetThuc < getFilterDateBatDau
                )
                {
                    throw new Exception("Vui lòng chọn ngày hợp lệ");
                }

                BaoCao baoCaoMoi = new BaoCao(
                    "calamnhanvien",
                    _taiKhoanChon.UserName
                );
                baoCaoMoi.SetBaoCaoCaLamNhanVien(dtpTuNgay.Value, dtpDenNgay.Value);
                ControlForm.BaoCaoHienTai = baoCaoMoi;
                ReportViewForm fBaoCao = new ReportViewForm();
                fBaoCao.ShowDialog();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (ControlForm.ConfirmForm("Bạn có muốn thêm?"))
                {
                    string userName = txtUsernameThem.Text.Trim();
                    string password = txtPasswordThem.Text.Trim();
                    string firstName = txtFirstNameThem.Text.Trim();
                    string lastName = txtLastNameThem.Text.Trim();
                    string phone = txtPhoneThem.Text.Trim();
                    string cccd = txtCCCDThem.Text.Trim();
                    string address = txtAddressThem.Text.Trim();
                    int quyenHan = int.Parse(cboQuyenHanThem.SelectedValue.ToString());

                    if (
                        string.IsNullOrEmpty(userName)
                        || string.IsNullOrEmpty(password)
                        || string.IsNullOrEmpty(firstName)
                        || string.IsNullOrEmpty(lastName)
                        || string.IsNullOrEmpty(phone)
                        || string.IsNullOrEmpty(cccd)
                        || string.IsNullOrEmpty(address)
                    )
                    {
                        throw new Exception("Vui lòng nhập thông tin hợp lệ");
                    }
                    if (TaiKhoanHienTai.TaiKhoanHienHanh.QuyenHan < quyenHan)
                    {
                        throw new Exception("Tài khoản tạo mới phải có quyền hạn thấp hơn hoặc bằng bạn");
                    }
                    TaiKhoan timTaiKhoan = DanhSachTaiKhoan.TimKiem(userName);
                    if (timTaiKhoan != null)
                    {
                        throw new Exception("Tài khoản đã tồn tại");
                    }
                    string passwordHash = BC.HashPassword(password);

                    Database.CreateConnection();
                    string sqlCommand =
                        $"insert into TAIKHOAN (USERNAME, PASSWORD, FIRSTNAME, LASTNAME, PHONE, CCCD, ADDRESS, QUYENHAN) values ('{userName}', '{passwordHash}', N'{firstName}', N'{lastName}', '{phone}', '{cccd}', N'{address}', '{quyenHan}')";
                    SqlCommand cmd;
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();
                    TaiKhoan taiKhoanMoi = new TaiKhoan(
                        userName,
                        passwordHash,
                        firstName,
                        lastName,
                        phone,
                        cccd,
                        address,
                        quyenHan
                    );
                    DanhSachTaiKhoan.Add(taiKhoanMoi);

                    LoadDanhSachTaiKhoan();
                    ReloadTabPageThem();

                    MessageBox.Show("Thành công");
                }
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
        void HienThiThongTinTaiKhoan()
        {
            if (_taiKhoanChon != null)
            {
                txtUserName.Text = _taiKhoanChon.UserName;
                txtFirstName.Text = _taiKhoanChon.FirstName;
                txtLastName.Text = _taiKhoanChon.LastName;
                txtCCCD.Text = _taiKhoanChon.CCCD;
                txtPhone.Text = _taiKhoanChon.Phone;
                txtAddress.Text = _taiKhoanChon.Address;
                cboQuyenHan.SelectedValue = (_taiKhoanChon.QuyenHan).ToString();
            }
        }

        void ReloadTabPageChinhSua()
        {
            DataTable dt = (DataTable)dgvDanhSachTaiKhoan.DataSource;
            if (dt != null)
            {
                dt.Clear();
            }
            LoadDanhSachTaiKhoan();
            _taiKhoanChon = null;
            txtTimKiemTaiKhoan.Text = null;
            btnLuu.Visible = false;
            btnXoa.Visible = false;
            btnCapNhatMatKhau.Visible = false;
            txtUserName.Text = null;
            txtFirstName.Text = null;
            txtLastName.Text = null;
            txtPhone.Text = null;
            txtAddress.Text = null;
            txtCCCD.Text = null;
            txtPassword.Text = null;
            pnlBaoCao.Visible = false;
        }

        void ReloadTabPageThem()
        {
            txtUsernameThem.Text = null;
            txtFirstNameThem.Text = null;
            txtLastNameThem.Text = null;
            txtPhoneThem.Text = null;
            txtCCCDThem.Text = null;
            txtAddressThem.Text = null;
            txtPasswordThem.Text = null;
        }
        #endregion

       
    }
}

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
    public partial class HoaDonForm : MaterialForm
    {
        public HoaDonForm()
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

        private void HoaDonForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (
                    TaiKhoanHienTai.TaiKhoanHienHanh == null
                    || ControlForm.BanDangChon == null
                    || ControlForm.BanDatDangChon == null
                )
                {
                    System.Environment.Exit(0);
                    return;
                }

                CreateBorderRadius();
                CapNhatHoaDon();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        #region Các hàm khởi tạo
        void CreateBorderRadius()
        {
            pnlThongTinBanDat.Anchor = AnchorStyles.None;
            pnlThongTinBanDat.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlThongTinBanDat.Width, pnlThongTinBanDat.Height, 15, 15)
            );
        }

        void CapNhatHoaDon()
        {
            if (ControlForm.BanDangChon.HoaDonHienTai.ThanhToan == 0)
            {
                Database.CreateConnection();
                string sqlCommand =
                    $"select B.TEN as TEN_SANPHAM_LS, A.ID as ID_LS, A.ID_DATBAN as ID_DATBAN_LS, A.ID_SANPHAM as ID_SANPHAM_LS, A.DONGIA as DONGIA_LS, A.DONGIAGIAM as DONGIAGIAM_LS, A.SOLUONG as SOLUONG_LS, A.THOIGIAN as THOIGIAN_LS, A.THANHTIEN as THANHTIEN_LS from LICHSUORDER A, DANHSACHSANPHAM B where A.ID_SANPHAM = B.ID AND A.ID_DATBAN = '{ControlForm.BanDatDangChon.ID}' order by A.THOIGIAN desc";
                DataTable dt;
                SqlCommand cmd;
                dt = Database.SelectQuery(sqlCommand);
                ControlForm.BanDangChon.HoaDonHienTai.ThanhTien = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    ControlForm.BanDangChon.HoaDonHienTai.ThanhTien += (int)dr["THANHTIEN_LS"];
                }
                ControlForm.BanDangChon.HoaDonHienTai.ThanhTienGiamGia = ControlForm
                    .BanDangChon
                    .HoaDonHienTai
                    .ThanhTien;
                ControlForm.BanDangChon.HoaDonHienTai.TienKhachTra = 0;
                ControlForm.BanDangChon.HoaDonHienTai.TienThua = 0;
                ControlForm.BanDangChon.HoaDonHienTai.ThoiGianThanhToan = new DateTime();
                ControlForm.BanDangChon.HoaDonHienTai.VoucherHoaDon = null;
                ControlForm.BanDangChon.HoaDonHienTai.ThoiGianTao = DateTime.Now;
                // Reset lại thông tin hóa đơn
                sqlCommand =
                    $"update HOADON set THOIGIAN_TAO = '{ControlForm.BanDangChon.HoaDonHienTai.ThoiGianTao}', THOIGIAN_THANHTOAN = NULL, VOUCHER = NULL, THANHTIEN = '{ControlForm.BanDangChon.HoaDonHienTai.ThanhTien}', THANHTIENGIAMGIA = '{ControlForm.BanDangChon.HoaDonHienTai.ThanhTien}', KHACHTRA = '{ControlForm.BanDangChon.HoaDonHienTai.TienKhachTra}', TIENTHUA = '{ControlForm.BanDangChon.HoaDonHienTai.TienThua}' where ID_DATBAN = '{ControlForm.BanDatDangChon.ID}'";
                cmd = Database.CreateCommand(sqlCommand);
                cmd.ExecuteNonQuery();

                sqlCommand =
                    $"update LICHSUDATBAN set THOIGIANRABAN = NULL where ID = '{ControlForm.BanDatDangChon.ID}'";
                cmd = Database.CreateCommand(sqlCommand);
                cmd.ExecuteNonQuery();

                HienThiThongTinHoaDon();
            }
        }
        #endregion

        #region Các hàm phục vụ
        void HienThiThongTinHoaDon()
        {
            lblMaDatBan.Text = $"Mã đặt bàn: {ControlForm.BanDangChon.HoaDonHienTai.IDDatBan}";
            lblMaHoaDon.Text = $"Hóa đơn ID: {ControlForm.BanDangChon.HoaDonHienTai.ID}";
            lblThoiGianHienTai.Text =
                $"Thời gian: {ControlForm.BanDangChon.HoaDonHienTai.ThoiGianTao}";
            lblTongTien.Text = ControlForm.BanDangChon.HoaDonHienTai.ThanhTien.ToString();
            lblTongTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblTongTien.Text));
            lblThanhTien.Text = ControlForm.BanDangChon.HoaDonHienTai.ThanhTienGiamGia.ToString();
            lblThanhTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblThanhTien.Text));

            if (ControlForm.BanDangChon.HoaDonHienTai.ThanhToan == 1)
            {
                // Ẩn và hiển thị các control sau khi thanh toán thành công
                foreach (Control c in pnlTongQuanHoaDon.Controls)
                {
                    if ((string)c.Tag == "CHUATHANHTOAN")
                    {
                        c.Visible = false;
                    }
                }
                txtVoucher.Visible = false;
                txtSoTienKhachTra.Visible = false;

                lblDaThanhToan.Visible = true;
                btnInHoaDon.Visible = true;
            }
        }

        #endregion

        #region Các hàm sự kiện
        private void btnCheckVoucher_Click(object sender, EventArgs e)
        {
            try
            {
                if (ControlForm.BanDangChon == null || ControlForm.BanDatDangChon == null)
                {
                    throw new Exception("Bàn không tồn tại!");
                }
                if (ControlForm.BanDangChon.HoaDonHienTai.ThanhToan == 1)
                {
                    throw new Exception("Bàn đã thanh toán hóa đơn");
                }
                if (string.IsNullOrEmpty(txtVoucher.Text.Trim()))
                {
                    throw new Exception("Vui lòng nhập Voucher");
                }
                Database.CreateConnection();
                SqlCommand cmd;
                SqlDataReader rd;
                string sqlCommand;
                // Kiểm tra xem có tồn tại voucher
                sqlCommand =
                    $"select * from VOUCHER where MA = '{txtVoucher.Text.Trim()}' and LUOTNHAP < SOLUONG and LUOTNHAP >= 0";
                cmd = Database.CreateCommand(sqlCommand);
                rd = cmd.ExecuteReader();
                int giamGiaVoucher = 0;
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        giamGiaVoucher = (int)rd["GIAMGIA"];
                    }
                    rd.Close();
                    Database.CreateConnection();
                    // Cập nhật lại số tiền của hóa đơn
                    ControlForm.BanDangChon.HoaDonHienTai.ThanhTienGiamGia =
                        ControlForm.BanDangChon.HoaDonHienTai.ThanhTien
                        - ControlForm.BanDangChon.HoaDonHienTai.ThanhTien * giamGiaVoucher / 100;
                    // Hiển thị các control sau khi dùng voucher
                    lblThongBaoVoucher.Visible = true;
                    lblThongBaoVoucher.Text =
                        $"Voucher: {txtVoucher.Text.ToUpper()} - Giảm giá: {giamGiaVoucher}%";
                    lblThanhTien.Text =
                        ControlForm.BanDangChon.HoaDonHienTai.ThanhTienGiamGia.ToString();
                    lblThanhTien.Text = string.Format(
                        "{0:#,##0} VNĐ",
                        double.Parse(lblThanhTien.Text)
                    );

                    Voucher timVoucher = DanhSachVoucher.TimKiem(txtVoucher.Text);
                    ControlForm.BanDangChon.HoaDonHienTai.VoucherHoaDon = timVoucher;
                    // Cập nhật lại hóa đơn trong database
                    sqlCommand =
                        $"update HOADON set THANHTIENGIAMGIA = '{ControlForm.BanDangChon.HoaDonHienTai.ThanhTienGiamGia}', VOUCHER = '{txtVoucher.Text}' where ID_DATBAN = '{ControlForm.BanDatDangChon.ID}'";
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Áp dụng voucher thành công");
                }
                else
                {
                    Database.CreateConnection();
                    ControlForm.BanDangChon.HoaDonHienTai.VoucherHoaDon = null;
                    ControlForm.BanDangChon.HoaDonHienTai.ThanhTienGiamGia = ControlForm
                        .BanDangChon
                        .HoaDonHienTai
                        .ThanhTien;
                    sqlCommand =
                        $"update HOADON set THANHTIENGIAMGIA = '{ControlForm.BanDangChon.HoaDonHienTai.ThanhTienGiamGia}', VOUCHER = NULL where ID_DATBAN = '{ControlForm.BanDatDangChon.ID}'";
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();

                    lblThanhTien.Text = (
                        ControlForm.BanDangChon.HoaDonHienTai.ThanhTienGiamGia
                    ).ToString();
                    lblThanhTien.Text = string.Format(
                        "{0:#,##0} VNĐ",
                        double.Parse(lblThanhTien.Text)
                    );

                    lblThongBaoVoucher.Visible = false;
                    throw new Exception("Không tồn tại voucher này");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                if (ControlForm.BanDangChon == null || ControlForm.BanDatDangChon == null)
                {
                    throw new Exception("Bàn không tồn tại!");
                }
                if (ControlForm.BanDangChon.HoaDonHienTai.ThanhToan == 1)
                {
                    throw new Exception("Bàn đã thanh toán hóa đơn");
                }
                if (string.IsNullOrEmpty(txtSoTienKhachTra.Text.Trim()))
                {
                    throw new Exception("Vui lòng nhập số tiền khách trả");
                }
                int soTienKhachTra = int.Parse(txtSoTienKhachTra.Text.Trim());

                if (soTienKhachTra < ControlForm.BanDangChon.HoaDonHienTai.ThanhTienGiamGia)
                {
                    throw new Exception("Khách không đủ tiền");
                }
                Database.CreateConnection();
                string sqlCommand;
                SqlCommand cmd;
                SqlDataReader rd;
                if (ControlForm.BanDangChon.HoaDonHienTai.VoucherHoaDon != null)
                {
                    sqlCommand =
                        $"select * from VOUCHER where MA = '{ControlForm.BanDangChon.HoaDonHienTai.VoucherHoaDon.Ma}' and LUOTNHAP < SOLUONG and LUOTNHAP >= 0";
                    cmd = Database.CreateCommand(sqlCommand);
                    rd = cmd.ExecuteReader();
                    if (!rd.HasRows)
                    {
                        throw new Exception("Voucher không tồn tại");
                    }
                    rd.Close();
                }
                Database.CreateConnection();

                // Cập nhật lại hóa đơn của bàn hiện tại
                ControlForm.BanDangChon.HoaDonHienTai.TienKhachTra = soTienKhachTra;
                ControlForm.BanDangChon.HoaDonHienTai.TienThua =
                    ControlForm.BanDangChon.HoaDonHienTai.TienKhachTra
                    - ControlForm.BanDangChon.HoaDonHienTai.ThanhTienGiamGia;
                ControlForm.BanDangChon.HoaDonHienTai.ThoiGianThanhToan = DateTime.Now;
                ControlForm.BanDatDangChon.ThoiGianRaBan = DateTime.Now;
                ControlForm.BanDangChon.HoaDonHienTai.ThanhToan = 1;

                // Cập nhật hóa đơn trong database
                sqlCommand =
                    $"update HOADON set THANHTOAN = '1', THOIGIAN_THANHTOAN = '{ControlForm.BanDangChon.HoaDonHienTai.ThoiGianThanhToan}', KHACHTRA = '{ControlForm.BanDangChon.HoaDonHienTai.TienKhachTra}', TIENTHUA = '{ControlForm.BanDangChon.HoaDonHienTai.TienThua}' where ID_DATBAN = '{ControlForm.BanDatDangChon.ID}'";
                cmd = Database.CreateCommand(sqlCommand);
                cmd.ExecuteNonQuery();

                if (ControlForm.BanDangChon.HoaDonHienTai.VoucherHoaDon != null)
                {
                    // Cập nhật lượt dùng voucher
                    sqlCommand =
                        $"update VOUCHER set LUOTNHAP = LUOTNHAP + '1' where MA = '{ControlForm.BanDangChon.HoaDonHienTai.VoucherHoaDon.Ma}'";
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();
                    ControlForm.BanDangChon.HoaDonHienTai.VoucherHoaDon.LuotNhap++;
                }

                sqlCommand =
                    $"update LICHSUDATBAN set THOIGIANRABAN = '{ControlForm.BanDatDangChon.ThoiGianRaBan}' where ID = '{ControlForm.BanDatDangChon.ID}'";
                cmd = Database.CreateCommand(sqlCommand);
                cmd.ExecuteNonQuery();

                // Cập nhật lại trạng thái bàn
                ControlForm.BanDangChon.TinhTrang = 0;
                sqlCommand =
                    $"update DANHSACHBAN set ID_DATBAN = NULL, ID_HOADON = NULL, TINHTRANG = '{ControlForm.BanDangChon.TinhTrang}' where ID = '{ControlForm.BanDangChon.ID}'";
                cmd = Database.CreateCommand(sqlCommand);
                cmd.ExecuteNonQuery();

                // Hiển thị lại danh sách bàn ở Sảnh
                if (ControlForm.FormMain != null)
                {
                    ControlForm.FormMain.LoadFlowPanelDanhSachBan();
                }
                // Ẩn và hiển thị các control sau khi thanh toán thành công
                foreach (Control c in pnlTongQuanHoaDon.Controls)
                {
                    if ((string)c.Tag == "CHUATHANHTOAN")
                    {
                        c.Visible = false;
                    }
                }
                txtVoucher.Visible = false;
                txtSoTienKhachTra.Visible = false;

                lblDaThanhToan.Visible = true;
                btnInHoaDon.Visible = true;

                DanhSachForm.ThanhToanThanhCongForm fThanhToanThanhCong =
                    new DanhSachForm.ThanhToanThanhCongForm();
                fThanhToanThanhCong.ShowDialog();
                // Khởi tạo lại trạng thái bàn
                if (ControlForm.FormChiTietBan != null)
                {
                    ControlForm.FormChiTietBan.ResetForm();
                }
                if (ControlForm.FormMain != null)
                {
                    ControlForm.FormMain.LoadDanhSachBan();
                    ControlForm.FormMain.LoadFlowPanelDanhSachBan();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void txtSoTienKhachTra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            if (ControlForm.BanDangChon != null && ControlForm.BanDangChon.HoaDonHienTai != null)
            {
                DanhSachForm.XuatHoaDonForm fXuatHoaDon = new DanhSachForm.XuatHoaDonForm();
                fXuatHoaDon.ShowDialog();
            }
        }

        #endregion
    }
}

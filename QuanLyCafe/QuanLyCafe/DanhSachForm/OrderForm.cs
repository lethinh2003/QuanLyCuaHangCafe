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
    public partial class OrderForm : MaterialForm
    {
        SanPham _sanPhamChon = null;
        int _giaTienSauGiamGia = 0;
        string _idDonHangCapNhat = null;

        public OrderForm()
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

        private void OrderForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (TaiKhoanHienTai.TaiKhoanHienHanh == null || ControlForm.BanDatDangChon == null)
                {
                    System.Environment.Exit(0);
                    return;
                }
                CreateBorderRadius();
                btnXacNhanOrder.Visible = false;
                LoadDanhSachSanPham();
                UpdateLichSuOrder();
                lblThongTinBanDat.Text = $"Mã bàn đặt: {ControlForm.BanDatDangChon.ID}";
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        #region Các hàm khởi tạo
        void CreateBorderRadius()
        {
            pnlChiTietSanPham.Anchor = AnchorStyles.None;
            pnlChiTietSanPham.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlChiTietSanPham.Width, pnlChiTietSanPham.Height, 15, 15)
            );

            picHienThiSanPham.Anchor = AnchorStyles.None;
            picHienThiSanPham.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, picHienThiSanPham.Width, picHienThiSanPham.Height, 15, 15)
            );

            panel1.Anchor = AnchorStyles.None;
            panel1.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 15, 15)
            );
        }

        void LoadDanhSachSanPham()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from DANHSACHSANPHAM";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            dgvDanhSachSanPham.DataSource = dt;
            dgvDanhSachSanPham.Columns["IMAGE_PATH"].Visible = false;
            lblGiamGia.Visible = false;
        }

        #endregion

        #region Các hàm sự kiện
        private void btnTimKiemSanPham_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtTimKiemSanPham.Text.Trim();

                Database.CreateConnection();
                string sqlCommand =
                    $"select * from DANHSACHSANPHAM where TEN like N'%{searchValue}%'";
                DataTable dt;
                dt = Database.SelectQuery(sqlCommand);
                dgvDanhSachSanPham.DataSource = dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void dgvDanhSachSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1)
                {
                    return;
                }
                btnCapNhatOrder.Visible = false;
                btnXoaOrder.Visible = false;
                btnXacNhanOrder.Visible = true;
                _idDonHangCapNhat = null;
                lblTitle.Text = "Tổng quan: thêm món";
                DataGridViewRow row = new DataGridViewRow();
                row = dgvDanhSachSanPham.Rows[e.RowIndex];
                string ID = row.Cells["ID"].Value.ToString();
                _sanPhamChon = DanhSachSanPham.TimKiemSanPham(ID);
                if (_sanPhamChon == null)
                {
                    btnXacNhanOrder.Visible = false;
                    throw new Exception("Không tìm thấy sản phẩm");
                }
                txtSoLuongSanPham.Text = "1";

                Image image = DanhSachSanPham.GetImageByPath(_sanPhamChon.ID);
                picHienThiSanPham.Image = image;
                HienThiThongTin();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void txtSoLuongSanPham_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSoLuongSanPham_Leave(object sender, EventArgs e)
        {
            int tongTien = int.Parse(txtSoLuongSanPham.Text) * (_giaTienSauGiamGia);
            lblTongTien.Text = tongTien.ToString();
            lblTongTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblTongTien.Text));
        }

        private void btnXacNhanOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                    ControlForm.ConfirmForm("Xác nhận đặt món")
                    && _sanPhamChon != null
                    && ControlForm.BanDatDangChon != null
                )
                {
                    Database.CreateConnection();
                    SqlCommand command;
                    SqlDataReader rd;
                    string sqlCommand;
                    string IDSanPham = _sanPhamChon.ID;
                    int donGia = _sanPhamChon.GiaTien;
                    int soLuong = int.Parse(txtSoLuongSanPham.Text);
                    int thanhTien = _giaTienSauGiamGia * soLuong;
                    // Kiểm tra xem đã có đơn món này chưa
                    sqlCommand =
                        $"select * from LICHSUORDER where ID_DATBAN = '{ControlForm.BanDatDangChon.ID}' and ID_SANPHAM = '{IDSanPham}'";
                    command = Database.CreateCommand(sqlCommand);
                    rd = command.ExecuteReader();
                    bool checkTonTai = false;
                    int soLuongHienTai = 0;
                    while (rd.Read())
                    {
                        checkTonTai = true;
                        soLuongHienTai = (int)rd["SOLUONG"];
                    }
                    Database.CreateConnection();
                    if (checkTonTai)
                    {
                        soLuong = soLuong + soLuongHienTai;
                        thanhTien = _giaTienSauGiamGia * soLuong;
                        sqlCommand =
                            $"update LICHSUORDER set SOLUONG = '{soLuong}', DONGIA = '{donGia}', DONGIAGIAM = '{_giaTienSauGiamGia}', THANHTIEN = '{thanhTien}', THOIGIAN = '{DateTime.Now}' where ID_DATBAN = '{ControlForm.BanDatDangChon.ID}' and ID_SANPHAM = '{IDSanPham}'";
                        command = Database.CreateCommand(sqlCommand);
                        command.ExecuteNonQuery();
                        UpdateLichSuOrder();
                        if (ControlForm.FormChiTietBan != null)
                        {
                            ControlForm.FormChiTietBan.LoadFlowPanelSanPhamOrder();
                        }
                        MessageBox.Show("Cập nhật thành công");
                    }
                    else
                    {
                        sqlCommand =
                            $"insert into LICHSUORDER (ID_DATBAN, ID_SANPHAM, DONGIA, DONGIAGIAM, SOLUONG, THANHTIEN) values ('{ControlForm.BanDatDangChon.ID}', '{IDSanPham}', '{donGia}', '{_giaTienSauGiamGia}', '{soLuong}', '{thanhTien}' )";
                        command = Database.CreateCommand(sqlCommand);
                        command.ExecuteNonQuery();
                        UpdateLichSuOrder();
                        if (ControlForm.FormChiTietBan != null)
                        {
                            ControlForm.FormChiTietBan.LoadFlowPanelSanPhamOrder();
                        }
                        MessageBox.Show("Thêm thành công");
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void dgvLichSuOrder_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1)
                {
                    return;
                }
                lblTitle.Text = "Tổng quan: chỉnh sửa order";
                btnCapNhatOrder.Visible = true;
                btnXoaOrder.Visible = true;

                btnXacNhanOrder.Visible = false;
                DataGridViewRow row = new DataGridViewRow();
                row = dgvLichSuOrder.Rows[e.RowIndex];
                string ID = row.Cells["ID_LS"].Value.ToString();

                string IDSanPham = row.Cells["ID_SANPHAM_LS"].Value.ToString();
                _idDonHangCapNhat = ID;
                _sanPhamChon = DanhSachSanPham.TimKiemSanPham(IDSanPham);
                if (_sanPhamChon == null)
                {
                    btnCapNhatOrder.Visible = false;
                    btnXoaOrder.Visible = false;
                    throw new Exception("Không tìm thấy sản phẩm");
                }
                txtSoLuongSanPham.Text = row.Cells["SOLUONG_LS"].Value.ToString();
                picHienThiSanPham.Image = DanhSachSanPham.GetImageByPath(IDSanPham);
                HienThiThongTin();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnCapNhatOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                    ControlForm.ConfirmForm("Xác nhận cập nhật đơn hàng?")
                    && _sanPhamChon != null
                    && !string.IsNullOrEmpty(_idDonHangCapNhat)
                )
                {
                    Database.CreateConnection();
                    SqlCommand command;
                    string IDSanPham = _sanPhamChon.ID;
                    int donGia = _sanPhamChon.GiaTien;
                    int soLuong = int.Parse(txtSoLuongSanPham.Text);
                    int thanhTien = _giaTienSauGiamGia * soLuong;

                    string sqlCommand =
                        $"update LICHSUORDER set SOLUONG = '{soLuong}', DONGIA = '{donGia}', DONGIAGIAM = '{_giaTienSauGiamGia}', THANHTIEN = '{thanhTien}', THOIGIAN = '{DateTime.Now}' where ID = '{_idDonHangCapNhat}'";
                    command = Database.CreateCommand(sqlCommand);
                    command.ExecuteNonQuery();
                    UpdateLichSuOrder();
                    if (ControlForm.FormChiTietBan != null)
                    {
                        ControlForm.FormChiTietBan.LoadFlowPanelSanPhamOrder();
                    }
                    MessageBox.Show("Thành công");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnXoaOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                    ControlForm.ConfirmForm("Xác nhận xóa đơn hàng?")
                    && _sanPhamChon != null
                    && !string.IsNullOrEmpty(_idDonHangCapNhat)
                )
                {
                    Database.CreateConnection();
                    SqlCommand command;

                    string sqlCommand = $"delete from LICHSUORDER where ID = '{_idDonHangCapNhat}'";
                    command = Database.CreateCommand(sqlCommand);
                    command.ExecuteNonQuery();
                    UpdateLichSuOrder();
                    if (ControlForm.FormChiTietBan != null)
                    {
                        ControlForm.FormChiTietBan.LoadFlowPanelSanPhamOrder();
                    }
                    MessageBox.Show("Thành công");
                    _sanPhamChon = null;
                    _idDonHangCapNhat = null;
                    btnCapNhatOrder.Visible = false;
                    btnXoaOrder.Visible = false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        #endregion

        #region Các hàm phục vụ
        void UpdateLichSuOrder()
        {
            Database.CreateConnection();
            string sqlCommand =
                $"select B.TEN as TEN_SANPHAM_LS, A.ID as ID_LS, A.ID_DATBAN as ID_DATBAN_LS, A.ID_SANPHAM as ID_SANPHAM_LS, A.DONGIA as DONGIA_LS, A.DONGIAGIAM as DONGIAGIAM_LS, A.SOLUONG as SOLUONG_LS, A.THOIGIAN as THOIGIAN_LS, A.THANHTIEN as THANHTIEN_LS from LICHSUORDER A, DANHSACHSANPHAM B where A.ID_SANPHAM = B.ID AND A.ID_DATBAN = '{ControlForm.BanDatDangChon.ID}' order by A.THOIGIAN desc";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            dgvLichSuOrder.DataSource = dt;
        }

        void HienThiThongTin()
        {
            lblTenSanPham.Text = $"{_sanPhamChon.TenSanPham}";
            lblGiaTien.Text = $"{_sanPhamChon.GiaTien}";

            _giaTienSauGiamGia = _sanPhamChon.GiaTien;
            if (_sanPhamChon.Event != null && _sanPhamChon.HienThiSuKien)
            {
                _giaTienSauGiamGia =
                    _giaTienSauGiamGia - _giaTienSauGiamGia * _sanPhamChon.Event.GiamGia / 100;
                lblGiamGia.Text = $"Giảm giá: {_sanPhamChon.Event.GiamGia}% ";
                lblGiamGia.Visible = true;
                lblSuKien.Visible = true;
                lblGiaTien.Font = new Font(lblGiaTien.Font, FontStyle.Strikeout | FontStyle.Bold);
                lblSuKien.Text = $"Sự kiện: {_sanPhamChon.Event.Ten}";
            }
            else
            {
                lblSuKien.Visible = false;
                lblGiamGia.Visible = false;
                lblGiaTien.Font = new Font(lblGiaTien.Font, FontStyle.Bold);
            }
            lblSauGiamGia.Text = _giaTienSauGiamGia.ToString();

            int tongTien = int.Parse(txtSoLuongSanPham.Text) * _giaTienSauGiamGia;
            lblTongTien.Text = tongTien.ToString();
            lblGiaTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblGiaTien.Text));

            lblSauGiamGia.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblSauGiamGia.Text));
            lblTongTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblTongTien.Text));
        }
        #endregion
    }
}

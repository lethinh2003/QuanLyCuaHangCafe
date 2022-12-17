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
    public partial class ChinhSuaSanPham : MaterialForm
    {
        SanPham _sanPhamChon = null;

        public ChinhSuaSanPham()
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

        private void ChinhSuaSanPham_Load(object sender, EventArgs e)
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
                FormatDate();
                CreateBorderRadius();
                LoadSuKien();
                LoadDanhSachSanPham();
                LoadLoaiSanPham();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        #region Các hàm khởi tạo
        void FormatDate()
        {
            dtpTuNgay.CustomFormat = "yyyy-MM-dd";
            dtpDenNgay.CustomFormat = "yyyy-MM-dd";
        }

        void CreateBorderRadius()
        {
            pnlTongQuanSanPham.Anchor = AnchorStyles.None;
            pnlTongQuanSanPham.Region = Region.FromHrgn(
                CreateRoundRectRgn(
                    0,
                    0,
                    pnlTongQuanSanPham.Width,
                    pnlTongQuanSanPham.Height,
                    15,
                    15
                )
            );
            pnlBaoCao.Anchor = AnchorStyles.None;
            pnlBaoCao.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlBaoCao.Width, pnlBaoCao.Height, 15, 15)
            );

            pnlForm.Anchor = AnchorStyles.None;
            pnlForm.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlForm.Width, pnlForm.Height, 15, 15)
            );

            pnlThemSanPham.Anchor = AnchorStyles.None;
            pnlThemSanPham.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlThemSanPham.Width, pnlThemSanPham.Height, 15, 15)
            );

            picHienThiSanPham.Anchor = AnchorStyles.None;
            picHienThiSanPham.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, picHienThiSanPham.Width, picHienThiSanPham.Height, 15, 15)
            );

            picHienThiSanPhamThem.Anchor = AnchorStyles.None;
            picHienThiSanPhamThem.Region = Region.FromHrgn(
                CreateRoundRectRgn(
                    0,
                    0,
                    picHienThiSanPhamThem.Width,
                    picHienThiSanPhamThem.Height,
                    15,
                    15
                )
            );
        }

        void LoadDanhSachSanPham()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from DANHSACHSANPHAM";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            dgvDanhSachSanPham.DataSource = dt;
            if (ControlForm.FormMain != null)
            {
                ControlForm.FormMain.LoadDanhSachSanPham();
                ControlForm.FormMain.LoadFlowPanelSanPham();
            }
        }

        void LoadLoaiSanPham()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from DANHMUCSANPHAM";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);

            cboLoai.DataSource = dt;
            cboLoai.DisplayMember = "TEN";
            cboLoai.ValueMember = "ID";

            cboLoaiThem.DataSource = dt;
            cboLoaiThem.DisplayMember = "TEN";
            cboLoaiThem.ValueMember = "ID";
        }

        void LoadSuKien()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from EVENT";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            foreach (DataRow dr in dt.Rows)
            {
                int ID = (int)dr["ID"];
                string moTa = (string)dr["MOTA"];
                string ten = (string)dr["TEN"];
                int giamGia = (int)dr["GIAMGIA"];
                DateTime thoiGianBatDau = (DateTime)dr["THOIGIANBATDAU"];
                DateTime thoiGianKetThuc = (DateTime)dr["THOIGIANKETTHUC"];
                SuKien suKienMoi = new SuKien(
                    ID,
                    ten,
                    moTa,
                    giamGia,
                    thoiGianBatDau,
                    thoiGianKetThuc
                );
                DanhSachSuKien.Add(suKienMoi);
            }
            cboSuKien.DataSource = dt;
            cboSuKien.DisplayMember = "TEN";
            cboSuKien.ValueMember = "ID";

            cboSuKienThem.DataSource = dt;
            cboSuKienThem.DisplayMember = "TEN";
            cboSuKienThem.ValueMember = "ID";
        }

        #endregion

        #region Các hàm sự kiện
        private void dgvDanhSachSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1)
                {
                    return;
                }

                DataGridViewRow row = new DataGridViewRow();
                row = dgvDanhSachSanPham.Rows[e.RowIndex];
                string ID = row.Cells["ID"].Value.ToString();
                string imagePath = row.Cells["IMAGE_PATH"].Value.ToString();

                _sanPhamChon = DanhSachSanPham.TimKiemSanPham(ID);
                if (_sanPhamChon == null)
                {
                    btnLuu.Visible = false;
                    btnXoa.Visible = false;
                    pnlBaoCao.Visible = false;
                    throw new Exception("Không tìm thấy sản phẩm");
                }
                btnLuu.Visible = true;
                btnXoa.Visible = true;
                pnlBaoCao.Visible = true;
                Image image = DanhSachSanPham.GetImageByPath(_sanPhamChon.ID);
                picHienThiSanPham.Image = image;
                HienThiSanPham();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                if (_sanPhamChon == null)
                {
                    return;
                }
                OpenFileDialog fileOpen = new OpenFileDialog();
                fileOpen.Title = "Open Image file";
                fileOpen.Filter = "Files|*.jpg;*.jpeg;*.png";

                if (fileOpen.ShowDialog() == DialogResult.OK)
                {
                    picHienThiSanPham.Image = Image.FromFile(fileOpen.FileName);
                    txtImagePath.Text = fileOpen.FileName;
                }
                fileOpen.Dispose();
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
                if (_sanPhamChon == null)
                {
                    return;
                }
                if (ControlForm.ConfirmForm("Bạn có muốn lưu?"))
                {
                    string tenSanPham = txtTenSanPham.Text.Trim();
                    int giaTien = int.Parse(txtGiaTien.Text);
                    string moTa = txtMoTa.Text.Trim();
                    string imagePath = txtImagePath.Text.Trim();
                    int suKien = -1;
                    if (cboSuKien.SelectedIndex != -1)
                    {
                        suKien = int.Parse(cboSuKien.SelectedValue.ToString());
                    }
                    string loaiSanPham = (cboLoai.SelectedValue.ToString());

                    if (
                        string.IsNullOrEmpty(tenSanPham)
                        || string.IsNullOrEmpty(moTa)
                        || string.IsNullOrEmpty(imagePath)
                    )
                    {
                        throw new Exception("Vui lòng nhập thông tin hợp lệ");
                    }

                    Database.CreateConnection();
                    string sqlCommand;
                    // Không thêm sự kiện
                    if (chkKhongThemSuKien.Checked)
                    {
                        sqlCommand =
                            $"update DANHSACHSANPHAM set TEN = N'{tenSanPham}', GIATIEN = '{giaTien}', EVENT = NULL, MOTA = N'{moTa}', IMAGE_PATH = '{imagePath}', LOAISANPHAM = '{loaiSanPham}' where ID = '{_sanPhamChon.ID}'";
                    }
                    else
                    {
                        if (suKien == -1)
                        {
                            sqlCommand =
                                $"update DANHSACHSANPHAM set TEN = N'{tenSanPham}', GIATIEN = '{giaTien}', EVENT = NULL, MOTA = N'{moTa}', IMAGE_PATH = '{imagePath}', LOAISANPHAM = '{loaiSanPham}' where ID = '{_sanPhamChon.ID}'";
                        }
                        else
                        {
                            sqlCommand =
                                $"update DANHSACHSANPHAM set TEN = N'{tenSanPham}', GIATIEN = '{giaTien}', EVENT = '{suKien}', MOTA = N'{moTa}', IMAGE_PATH = '{imagePath}', LOAISANPHAM = '{loaiSanPham}' where ID = '{_sanPhamChon.ID}'";
                        }
                    }

                    SqlCommand cmd;
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();
                    LoadDanhSachSanPham();

                    SuKien getSuKien;
                    if (suKien == -1)
                    {
                        getSuKien = null;
                    }
                    else
                    {
                        getSuKien = DanhSachSuKien.TimKiem(suKien);
                    }
                    _sanPhamChon.CapNhatSanPham(loaiSanPham, moTa, giaTien, getSuKien, imagePath);
                    MessageBox.Show("Thành công");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnUploadImageThem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fileOpenThem = new OpenFileDialog();
                fileOpenThem.Title = "Open Image file";
                fileOpenThem.Filter = "Files|*.jpg;*.jpeg;*.png";

                if (fileOpenThem.ShowDialog() == DialogResult.OK)
                {
                    picHienThiSanPhamThem.Image = Image.FromFile(fileOpenThem.FileName);
                    txtImagePathThem.Text = fileOpenThem.FileName;
                }
                fileOpenThem.Dispose();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void txtIDThem_Leave(object sender, EventArgs e)
        {
            txtIDThem.Text = txtIDThem.Text.ToUpper();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (ControlForm.ConfirmForm("Bạn có muốn thêm?"))
                {
                    string IDSanPham = txtIDThem.Text.Trim();
                    string tenSanPham = txtTenSanPhamThem.Text.Trim();
                    int giaTien = int.Parse(txtGiaTienThem.Text);
                    string moTa = txtMoTaThem.Text.Trim();
                    string imagePath = txtImagePathThem.Text.Trim();
                    int suKien = int.Parse(cboSuKienThem.SelectedValue.ToString());
                    string loaiSanPham = (cboLoaiThem.SelectedValue.ToString());

                    if (
                        string.IsNullOrEmpty(tenSanPham)
                        || string.IsNullOrEmpty(moTa)
                        || string.IsNullOrEmpty(imagePath)
                        || string.IsNullOrEmpty(IDSanPham)
                    )
                    {
                        throw new Exception("Vui lòng nhập thông tin hợp lệ");
                    }
                    IDSanPham = IDSanPham.ToUpper();
                    SanPham timSanPham = DanhSachSanPham.TimKiemSanPham(IDSanPham);
                    if (timSanPham != null)
                    {
                        throw new Exception("Sản phẩm đã tồn tại");
                    }
                    Database.CreateConnection();
                    string sqlCommand;
                    if (chkKhongThemSuKienThem.Checked)
                    {
                        sqlCommand =
                            $"insert into DANHSACHSANPHAM (ID, TEN, GIATIEN, EVENT, MOTA, IMAGE_PATH, LOAISANPHAM) values ('{IDSanPham}', N'{tenSanPham}', '{giaTien}', NULL, N'{moTa}', '{imagePath}', '{loaiSanPham}')";
                    }
                    else
                    {
                        sqlCommand =
                            $"insert into DANHSACHSANPHAM (ID, TEN, GIATIEN, EVENT, MOTA, IMAGE_PATH, LOAISANPHAM) values ('{IDSanPham}', N'{tenSanPham}', '{giaTien}', '{suKien}', N'{moTa}', '{imagePath}', '{loaiSanPham}')";
                    }
                    SqlCommand cmd;
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();

                    SuKien getSuKien = DanhSachSuKien.TimKiem(suKien);
                    SanPham sanPhamMoi = new SanPham(
                        IDSanPham,
                        loaiSanPham,
                        moTa,
                        tenSanPham,
                        giaTien,
                        getSuKien,
                        imagePath
                    );
                    DanhSachSanPham.AddSanPham(sanPhamMoi);
                    LoadDanhSachSanPham();
                    ReloadTabPageThem();

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
                if (_sanPhamChon == null)
                {
                    return;
                }
                if (
                    ControlForm.ConfirmForm(
                        "Bạn có muốn xóa? Hành động này sẽ xóa tất cả mọi thứ liên quan đến nó"
                    )
                )
                {
                    Database.CreateConnection();
                    string sqlCommand;
                    SqlCommand cmd;
                    SqlDataReader rd;

                    // Kiểm tra xem sản phẩm có đang được order hay không?

                    sqlCommand =
                        $"select * from LICHSUORDER A, LICHSUDATBAN B where A.ID_SANPHAM = '{_sanPhamChon.ID}' and B.THOIGIANRABAN is NULL";
                    cmd = Database.CreateCommand(sqlCommand);
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        throw new Exception("Sản phẩm đang được đặt, không thể xóa");
                    }

                    Database.CreateConnection();

                    // Xóa các thứ liên quan đến sản phẩm
                    List<string> danhSachIDLichSuOrder = new List<string>();
                    List<string> danhSachIDLichSuDatBan = new List<string>();
                    List<string> danhSachIDHoaDon = new List<string>();

                    sqlCommand =
                        $"select A.ID as ID_LICHSUORDER, B.ID_BAN as ID_BAN, B.ID as ID_LICHSUDATBAN, C.ID as ID_SANPHAM, D.ID as ID_HOADON  from LICHSUORDER A inner join LICHSUDATBAN B on A.ID_DATBAN = B.ID inner join DANHSACHSANPHAM C on A.ID_SANPHAM = C.ID and A.ID_SANPHAM = '{_sanPhamChon.ID}' inner join HOADON D on D.ID_DATBAN = B.ID";
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

                    // xóa sản phẩm

                    sqlCommand = $"delete DANHSACHSANPHAM where ID = '{_sanPhamChon.ID}'";
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();

                    btnLuu.Visible = false;
                    btnXoa.Visible = false;
                    LoadDanhSachSanPham();

                    MessageBox.Show("Xóa thành công");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

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

        private void txtGiaTienThem_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtGiaTien_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnBaoCaoSoLuongBanRa_Click(object sender, EventArgs e)
        {
            try
            {
                if (_sanPhamChon == null)
                {
                    throw new Exception("Vui lòng chọn sản phẩm muốn báo cáo");
                }
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
                    "soluongsanphambanra",
                    TaiKhoanHienTai.TaiKhoanHienHanh.UserName
                );
                baoCaoMoi.IDSanPham = _sanPhamChon.ID;
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
        #endregion

        #region Các hàm phục vụ

        void HienThiSanPham()
        {
            if (_sanPhamChon != null)
            {
                txtID.Text = _sanPhamChon.ID;
                txtTenSanPham.Text = _sanPhamChon.TenSanPham;
                txtGiaTien.Text = _sanPhamChon.GiaTien.ToString();
                txtImagePath.Text = _sanPhamChon.ImagePath;
                txtMoTa.Text = _sanPhamChon.MoTa;
                if (_sanPhamChon.Event != null)
                {
                    chkKhongThemSuKien.Checked = false;
                    cboSuKien.SelectedValue = int.Parse((_sanPhamChon.Event.ID).ToString());
                }
                else
                {
                    chkKhongThemSuKien.Checked = true;
                    cboSuKien.SelectedIndex = -1;
                }
                cboLoai.SelectedValue = ((_sanPhamChon.LoaiSanPham).ToString());
            }
        }

        void ReloadTabPageChinhSua()
        {
            DataTable dt = (DataTable)dgvDanhSachSanPham.DataSource;
            if (dt != null)
            {
                dt.Clear();
            }
            LoadDanhSachSanPham();
            _sanPhamChon = null;
            pnlBaoCao.Visible = false;
            txtTimKiemSanPham.Text = null;
            picHienThiSanPham.Image = null;
            btnLuu.Visible = false;
            btnXoa.Visible = false;
            txtID.Text = null;
            txtTenSanPham.Text = null;
            txtGiaTien.Text = null;
            txtMoTa.Text = null;
            txtImagePath.Text = null;
        }

        void ReloadTabPageThem()
        {
            picHienThiSanPhamThem.Image = null;
            txtIDThem.Text = null;
            txtTenSanPhamThem.Text = null;
            txtGiaTienThem.Text = null;
            txtMoTaThem.Text = null;
            txtImagePathThem.Text = null;
        }
        #endregion
    }
}

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

namespace QuanLyCafe.DanhSachForm
{
    public partial class MainForm : MaterialForm
    {
        bool _isClickLocThucAn = false;
        bool _isClickLocKhac = false;
        bool _isClickLocNuoc = false;
        bool _isClickBanChuaDat = false;
        bool _isClickBanDaDat = false;
        int _ketQuaTimThaySanPham = 0;
        int _ketQuaTimThayBan = 0;

        Timer _timerThoiGianHienTai;

        public MainForm()
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (TaiKhoanHienTai.TaiKhoanHienHanh == null)
                {
                    System.Environment.Exit(0);
                    return;
                }

                ControlForm.FormMain = this;

                CreateBoderRadiusControl();

                // Các hàm khởi tạo vào hệ thống
                LoadHeThong();
                LoadDanhSachVoucher();
                LoadDanhSachTaiKhoan();
                LoadDanhSachHoaDon();
                LoadDanhSachSuKien();

                LoadDanhSachSanPham();
                //LoadDanhSachBanDat();
                LoadDanhSachBan();
                LoadThongTinNhanVien();
                LoadThoiGianHienTai();

                // Hiển thị danh sách sản phẩm
                LoadFlowPanelSanPham();
                // Hiển thị danh sách bàn
                LoadFlowPanelDanhSachBan();
                // Hiển thị danh sách sự kiện
                LoadFlowPanelDanhSachSuKien();
                // Hiển thị danh sách voucher
                LoadFlowPanelDanhSachVoucher();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        #region Các hàm khởi tạo hệ thống
        void CreateBoderRadiusControl()
        {
            // Tạo border radius cho các control
            pnlForm.Anchor = AnchorStyles.None;
            pnlForm.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlForm.Width, pnlForm.Height, 15, 15)
            );

            picBanChuaDat.Anchor = AnchorStyles.None;
            picBanChuaDat.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, picBanChuaDat.Width, picBanChuaDat.Height, 15, 15)
            );

            pnlWelcomeBack.Anchor = AnchorStyles.None;
            pnlWelcomeBack.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlWelcomeBack.Width, pnlWelcomeBack.Height, 30, 30)
            );

            pnlDanhSachSuKien.Anchor = AnchorStyles.None;
            pnlDanhSachSuKien.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlDanhSachSuKien.Width, pnlDanhSachSuKien.Height, 30, 30)
            );

            pnlDanhSachVoucher.Anchor = AnchorStyles.None;
            pnlDanhSachVoucher.Region = Region.FromHrgn(
                CreateRoundRectRgn(
                    0,
                    0,
                    pnlDanhSachVoucher.Width,
                    pnlDanhSachVoucher.Height,
                    30,
                    30
                )
            );

            picBanDaDat.Anchor = AnchorStyles.None;
            picBanDaDat.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, picBanDaDat.Width, picBanDaDat.Height, 15, 15)
            );

            pnlVoucher.Anchor = AnchorStyles.None;
            pnlVoucher.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlVoucher.Width, pnlVoucher.Height, 15, 15)
            );
        }

        void LoadHeThong()
        {
            Database.CreateConnection();
            string sqlCommand;
            SqlCommand cmd;
            SqlDataReader rd;
            sqlCommand = $"select * from HETHONG where ID = '1'";
            cmd = Database.CreateCommand(sqlCommand);
            rd = cmd.ExecuteReader();
            if (!rd.HasRows)
            {
                int idHeThongMoi = 1;
                string tenCuaHangMoi = "Ten";
                string diaChiCuaHangMoi = "DiaChi";
                int luongPartTimeMoi = 0;
                sqlCommand =
                    $"insert into HETHONG (ID, TENCUAHANG, DIACHICUAHANG, LUONG_PARTTIME) values ('{idHeThongMoi}', N'{tenCuaHangMoi}', N'{diaChiCuaHangMoi}', '{luongPartTimeMoi}')";
                cmd = Database.CreateCommand(sqlCommand);
                cmd.ExecuteNonQuery();
                HeThong.SetHeThong(idHeThongMoi, tenCuaHangMoi, diaChiCuaHangMoi, luongPartTimeMoi);
            }
            else
            {
                while (rd.Read())
                {
                    int idHeThongMoi = (int)rd["ID"];
                    string tenCuaHangMoi = (string)rd["TENCUAHANG"];
                    string diaChiCuaHangMoi = (string)rd["DIACHICUAHANG"];
                    int luongPartTimeMoi = (int)rd["LUONG_PARTTIME"];
                    HeThong.SetHeThong(
                        idHeThongMoi,
                        tenCuaHangMoi,
                        diaChiCuaHangMoi,
                        luongPartTimeMoi
                    );
                }
            }
            rd.Close();
        }

        public void LoadDanhSachVoucher()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from VOUCHER";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            DanhSachVoucher.ListVoucher.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                int giamGia = (int)dr["GIAMGIA"];
                string ma = (string)dr["MA"];
                string moTa = (string)dr["MOTA"];

                int soLuong = (int)dr["SOLUONG"];
                int luotNhap = (int)dr["LUOTNHAP"];
                Voucher voucherMoi = new Voucher(ma, giamGia, moTa, luotNhap, soLuong);
                DanhSachVoucher.Add(voucherMoi);
            }
        }

        public void LoadDanhSachTaiKhoan()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from TAIKHOAN";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            DanhSachTaiKhoan.ListTaiKhoan.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                int quyenHan = (int)dr["QUYENHAN"];
                string userName = (string)dr["USERNAME"];
                string passWord = (string)dr["PASSWORD"];
                string firstName = (string)dr["FIRSTNAME"];
                string lastName = (string)dr["LASTNAME"];
                string phone = (string)dr["PHONE"];
                string cccd = (string)dr["CCCD"];
                string address = (string)dr["ADDRESS"];
                TaiKhoan taiKhoanMoi = new TaiKhoan(
                    userName,
                    passWord,
                    firstName,
                    lastName,
                    phone,
                    cccd,
                    address,
                    quyenHan
                );
                DanhSachTaiKhoan.Add(taiKhoanMoi);
            }
        }

        public void LoadDanhSachHoaDon()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from HOADON";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            DanhSachHoaDon.ListHoaDon.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                int ID = (int)dr["ID"];
                int IDDatBan = (int)dr["ID_DATBAN"];
                int thanhTien = (int)dr["THANHTIEN"];
                int thanhTienGiamGia = (int)dr["THANHTIENGIAMGIA"];
                int thanhToan = (int)dr["THANHTOAN"];
                int khachTra = (int)dr["KHACHTRA"];
                int tienThua = (int)dr["TIENTHUA"];
                DateTime thoiGianTao = (DateTime)dr["THOIGIAN_TAO"];
                DateTime thoiGianThanhToan = new DateTime();
                Voucher getVoucher = null;
                if (!dr.IsNull("VOUCHER"))
                {
                    string voucher = (string)dr["VOUCHER"];
                    getVoucher = DanhSachVoucher.TimKiem(voucher);
                }
                string nhanVien = (string)dr["NHANVIEN"];
                if (!dr.IsNull("THOIGIAN_THANHTOAN"))
                {
                    thoiGianThanhToan = (DateTime)dr["THOIGIAN_THANHTOAN"];
                }

                TaiKhoan getNhanVien = DanhSachTaiKhoan.TimKiem(nhanVien);
                HoaDon hoaDonMoi = new HoaDon(
                    ID,
                    IDDatBan,
                    getVoucher,
                    getNhanVien,
                    thanhTien,
                    thanhToan,
                    thanhTienGiamGia,
                    khachTra,
                    tienThua,
                    thoiGianTao,
                    thoiGianThanhToan
                );
                DanhSachHoaDon.Add(hoaDonMoi);
            }
        }

        public void LoadDanhSachSuKien()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from EVENT";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            DanhSachSuKien.ListSuKien.Clear();
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
            cboDanhSachSuKien.DataSource = dt;
            cboDanhSachSuKien.DisplayMember = "TEN";
            cboDanhSachSuKien.ValueMember = "ID";
            UpdateSuKien();
        }

        void UpdateSuKien()
        {
            Database.CreateConnection();
            string sqlCommand;
            SqlCommand command;
            DateTime today = DateTime.Now;
            foreach (var item in DanhSachSuKien.ListSuKien)
            {
                // Kiểm tra xem nếu thời gian kết thúc sự kiện đã qua thì set sự kiện = NULL
                if (today >= item.ThoiGianKetThuc)
                {
                    sqlCommand =
                        $"update DANHSACHSANPHAM set EVENT = NULL where EVENT = '{item.ID}'";
                    command = Database.CreateCommand(sqlCommand);
                    command.ExecuteNonQuery();
                }
            }
        }

        void LoadThoiGianHienTai()
        {
            lblThoiGianHienTai.Text = $"{DateTime.Now}";
            _timerThoiGianHienTai = new Timer();
            _timerThoiGianHienTai.Tick += _timerThoiGianHienTai_Tick;
            _timerThoiGianHienTai.Interval = 1000;
            _timerThoiGianHienTai.Enabled = false;
            _timerThoiGianHienTai.Start();
        }

        public void LoadThongTinNhanVien()
        {
            if (TaiKhoanHienTai.TaiKhoanHienHanh != null)
            {
                if (TaiKhoanHienTai.TaiKhoanHienHanh.QuyenHan == 1)
                {
                    flpDanhSachQuanLy.Visible = false;
                } else
                {
                    flpDanhSachQuanLy.Visible = true;
                }
                lblHoTenNhanVien.Text =
                    $"{TaiKhoanHienTai.TaiKhoanHienHanh.FirstName} {TaiKhoanHienTai.TaiKhoanHienHanh.LastName}";
                string chucVu =
                    TaiKhoanHienTai.TaiKhoanHienHanh.QuyenHan == 1
                        ? "Nhân viên cửa hàng"
                        : "Quản lý";
                lblChucVuNhanVien.Text = chucVu;
                lblHoTenNhanVien.Location = new Point(
                    (lblHoTenNhanVien.Parent.ClientSize.Width / 2) - (lblHoTenNhanVien.Width / 2),
                    114
                );
                lblChucVuNhanVien.Location = new Point(
                    (lblChucVuNhanVien.Parent.ClientSize.Width / 2) - (lblChucVuNhanVien.Width / 2),
                    141
                );
            }
        }

        public void LoadFlowPanelDanhSachSuKien()
        {
            flpDanhSachSuKien.Controls.Clear();
            foreach (var item in DanhSachSuKien.ListSuKien)
            {
                MaterialCard card = new ReaLTaiizor.Controls.MaterialCard();
                card.Width = 409;
                card.Height = 128;

                card.Location = new Point(14, 14);

                Label lblTenSuKien = new Label();
                lblTenSuKien.AutoSize = false;
                lblTenSuKien.Width = 390;
                lblTenSuKien.Text = $"Tên: {item.Ten}";
                lblTenSuKien.Font = new Font("Arial", 11, FontStyle.Bold);
                lblTenSuKien.Location = new Point(17, 14);
                card.Controls.Add(lblTenSuKien);

                Label lblGiamGia = new Label();
                lblGiamGia.AutoSize = false;
                lblGiamGia.Width = 390;
                lblGiamGia.Text = $"Giảm giá: {item.GiamGia}%";
                lblGiamGia.Font = new Font("Arial", 11, FontStyle.Bold);
                lblGiamGia.Location = new Point(17, 40);
                card.Controls.Add(lblGiamGia);

                Label lblBatDau = new Label();
                lblBatDau.AutoSize = false;
                lblBatDau.Width = 390;
                lblBatDau.Text = $"Bắt đầu: {item.ThoiGianBatDau}";
                lblBatDau.Font = new Font("Arial", 11, FontStyle.Bold);
                lblBatDau.Location = new Point(17, 65);
                card.Controls.Add(lblBatDau);

                Label lblKetThuc = new Label();
                lblKetThuc.AutoSize = false;
                lblKetThuc.Width = 390;
                lblKetThuc.Text = $"Kết thúc: {item.ThoiGianKetThuc}";
                lblKetThuc.Font = new Font("Arial", 11, FontStyle.Bold);
                lblKetThuc.Location = new Point(17, 93);
                card.Controls.Add(lblKetThuc);

                flpDanhSachSuKien.Controls.Add(card);
            }
        }

        public void LoadFlowPanelDanhSachSuKienTimKiem(List<string> danhSachTimKiem)
        {
            flpDanhSachSuKien.Controls.Clear();
            foreach (var item in DanhSachSuKien.ListSuKien)
            {
                if (danhSachTimKiem.Contains(item.ID.ToString()))
                {
                    MaterialCard card = new ReaLTaiizor.Controls.MaterialCard();
                    card.Width = 409;
                    card.Height = 128;

                    card.Location = new Point(14, 14);

                    Label lblTenSuKien = new Label();
                    lblTenSuKien.AutoSize = false;
                    lblTenSuKien.Width = 390;
                    lblTenSuKien.Text = $"Tên: {item.Ten}";
                    lblTenSuKien.Font = new Font("Arial", 11, FontStyle.Bold);
                    lblTenSuKien.Location = new Point(17, 14);
                    card.Controls.Add(lblTenSuKien);

                    Label lblGiamGia = new Label();
                    lblGiamGia.AutoSize = false;
                    lblGiamGia.Width = 390;
                    lblGiamGia.Text = $"Giảm giá: {item.GiamGia}%";
                    lblGiamGia.Font = new Font("Arial", 11, FontStyle.Bold);
                    lblGiamGia.Location = new Point(17, 40);
                    card.Controls.Add(lblGiamGia);

                    Label lblBatDau = new Label();
                    lblBatDau.AutoSize = false;
                    lblBatDau.Width = 390;
                    lblBatDau.Text = $"Bắt đầu: {item.ThoiGianBatDau}";
                    lblBatDau.Font = new Font("Arial", 11, FontStyle.Bold);
                    lblBatDau.Location = new Point(17, 65);
                    card.Controls.Add(lblBatDau);

                    Label lblKetThuc = new Label();
                    lblKetThuc.AutoSize = false;
                    lblKetThuc.Width = 390;
                    lblKetThuc.Text = $"Kết thúc: {item.ThoiGianKetThuc}";
                    lblKetThuc.Font = new Font("Arial", 11, FontStyle.Bold);
                    lblKetThuc.Location = new Point(17, 93);
                    card.Controls.Add(lblKetThuc);

                    flpDanhSachSuKien.Controls.Add(card);
                }
            }
        }

        public void LoadFlowPanelDanhSachVoucher()
        {
            flpDanhSachVoucher.Controls.Clear();
            foreach (var item in DanhSachVoucher.ListVoucher)
            {
                MaterialCard card = new ReaLTaiizor.Controls.MaterialCard();
                card.Width = 405;
                card.Height = 128;

                card.Location = new Point(14, 14);

                Label lblMa = new Label();
                lblMa.AutoSize = false;
                lblMa.Width = 390;
                lblMa.Text = $"Mã: {item.Ma}";
                lblMa.Font = new Font("Arial", 11, FontStyle.Bold);
                lblMa.Location = new Point(17, 14);
                card.Controls.Add(lblMa);

                Label lblGiamGia = new Label();
                lblGiamGia.AutoSize = false;
                lblGiamGia.Width = 390;
                lblGiamGia.Text = $"Giảm giá: {item.GiamGia}%";
                lblGiamGia.Font = new Font("Arial", 11, FontStyle.Bold);
                lblGiamGia.Location = new Point(17, 40);
                card.Controls.Add(lblGiamGia);

                Label lblLuotNhap = new Label();
                lblLuotNhap.AutoSize = false;
                lblLuotNhap.Width = 390;
                lblLuotNhap.Text = $"Lượt nhập: {item.LuotNhap}";
                lblLuotNhap.Font = new Font("Arial", 11, FontStyle.Bold);
                lblLuotNhap.Location = new Point(17, 65);
                card.Controls.Add(lblLuotNhap);

                Label lblSoLuong = new Label();
                lblSoLuong.AutoSize = false;
                lblSoLuong.Width = 390;
                lblSoLuong.Text = $"Số Lượng: {item.SoLuong}";
                lblSoLuong.Font = new Font("Arial", 11, FontStyle.Bold);
                lblSoLuong.Location = new Point(17, 93);
                card.Controls.Add(lblSoLuong);

                flpDanhSachVoucher.Controls.Add(card);
            }
        }

        public void LoadFlowPanelDanhSachVoucherTimKiem(List<string> danhSachTimKiem)
        {
            flpDanhSachVoucher.Controls.Clear();
            foreach (var item in DanhSachVoucher.ListVoucher)
            {
                if (danhSachTimKiem.Contains(item.Ma))
                {
                    MaterialCard card = new ReaLTaiizor.Controls.MaterialCard();
                    card.Width = 405;
                    card.Height = 128;

                    card.Location = new Point(14, 14);

                    Label lblMa = new Label();
                    lblMa.AutoSize = false;
                    lblMa.Width = 390;
                    lblMa.Text = $"Mã: {item.Ma}";
                    lblMa.Font = new Font("Arial", 11, FontStyle.Bold);
                    lblMa.Location = new Point(17, 14);
                    card.Controls.Add(lblMa);

                    Label lblGiamGia = new Label();
                    lblGiamGia.AutoSize = false;
                    lblGiamGia.Width = 390;
                    lblGiamGia.Text = $"Giảm giá: {item.GiamGia}%";
                    lblGiamGia.Font = new Font("Arial", 11, FontStyle.Bold);
                    lblGiamGia.Location = new Point(17, 40);
                    card.Controls.Add(lblGiamGia);

                    Label lblLuotNhap = new Label();
                    lblLuotNhap.AutoSize = false;
                    lblLuotNhap.Width = 390;
                    lblLuotNhap.Text = $"Lượt nhập: {item.LuotNhap}";
                    lblLuotNhap.Font = new Font("Arial", 11, FontStyle.Bold);
                    lblLuotNhap.Location = new Point(17, 65);
                    card.Controls.Add(lblLuotNhap);

                    Label lblSoLuong = new Label();
                    lblSoLuong.AutoSize = false;
                    lblSoLuong.Width = 390;
                    lblSoLuong.Text = $"Số Lượng: {item.SoLuong}";
                    lblSoLuong.Font = new Font("Arial", 11, FontStyle.Bold);
                    lblSoLuong.Location = new Point(17, 93);
                    card.Controls.Add(lblSoLuong);

                    flpDanhSachVoucher.Controls.Add(card);
                }
            }
        }

        public void LoadFlowPanelSanPham()
        {
            _ketQuaTimThaySanPham = 0;
            flpDanhSachSanPham.Controls.Clear();
            foreach (var item in DanhSachSanPham.ListSanPham)
            {
                _ketQuaTimThaySanPham++;
                Random random = new Random();
                int color1 = random.Next(236, 255);
                int color2 = random.Next(238, 255);
                int color3 = random.Next(207, 255);
                MaterialCard card = new ReaLTaiizor.Controls.MaterialCard();
                card.Width = 290;
                card.Height = 250;
                card.Tag = item.ID;
                PictureBox pic = new PictureBox();
                pic.Width = 290;
                pic.Height = 140;
                pic.Location = new Point(0, 0);
                pic.BackColor = Color.FromArgb(color1, color2, color3);

                PictureBox picSanPham = new PictureBox();
                picSanPham.Width = 195;
                picSanPham.Height = 100;
                picSanPham.BackColor = Color.FromArgb(color1, color2, color3);
                picSanPham.Image = DanhSachSanPham.GetImageByPath(item.ID);
                picSanPham.SizeMode = PictureBoxSizeMode.StretchImage;
                pic.Controls.Add(picSanPham);
                picSanPham.Location = new Point(
                    (picSanPham.Parent.ClientSize.Width / 2) - (picSanPham.Width / 2),
                    (picSanPham.Parent.ClientSize.Height / 2) - (picSanPham.Height / 2)
                );

                Label lblTenSanPham = new Label();
                lblTenSanPham.AutoSize = false;
                lblTenSanPham.Width = 290;
                lblTenSanPham.TextAlign = ContentAlignment.MiddleCenter;
                lblTenSanPham.Text = item.TenSanPham;
                lblTenSanPham.Font = new Font("Arial", 12, FontStyle.Bold);
                card.Controls.Add(lblTenSanPham);
                lblTenSanPham.Location = new Point(
                    (lblTenSanPham.Parent.ClientSize.Width / 2) - (lblTenSanPham.Width / 2),
                    picSanPham.Height + 50
                );

                Label lblGiaTien = new Label();
                lblGiaTien.TextAlign = ContentAlignment.MiddleCenter;
                lblGiaTien.AutoSize = false;
                lblGiaTien.Width = 290;
                lblGiaTien.Text = item.GiaTien.ToString();
                lblGiaTien.Font = new Font("Arial", 10, FontStyle.Regular);
                lblGiaTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblGiaTien.Text));

                card.Controls.Add(lblGiaTien);
                lblGiaTien.Location = new Point(
                    (lblGiaTien.Parent.ClientSize.Width / 2) - (lblGiaTien.Width / 2),
                    picSanPham.Height + 50 + lblTenSanPham.Height
                );

                if (item.Event != null && item.HienThiSuKien)
                {
                    lblGiaTien.Font = new Font(lblGiaTien.Font, FontStyle.Strikeout);
                    Label lblGiaTienGiamGia = new Label();
                    lblGiaTienGiamGia.TextAlign = ContentAlignment.MiddleCenter;
                    lblGiaTienGiamGia.AutoSize = false;
                    lblGiaTienGiamGia.Width = 290;
                    lblGiaTienGiamGia.Text = (
                        item.GiaTien - item.GiaTien * item.Event.GiamGia / 100
                    ).ToString();
                    lblGiaTienGiamGia.Font = new Font("Arial", 10, FontStyle.Regular);
                    lblGiaTienGiamGia.Text = string.Format(
                        "{0:#,##0} VNĐ",
                        double.Parse(lblGiaTienGiamGia.Text)
                    );

                    card.Controls.Add(lblGiaTienGiamGia);
                    lblGiaTienGiamGia.Location = new Point(
                        (lblGiaTienGiamGia.Parent.ClientSize.Width / 2)
                            - (lblGiaTienGiamGia.Width / 2),
                        lblGiaTien.Height + lblGiaTien.Location.Y
                    );
                }

                card.Controls.Add(pic);

                flpDanhSachSanPham.Controls.Add(card);
            }
            lblKetQuaTimThay.Text = $"Có {_ketQuaTimThaySanPham} kết quả tìm thấy";
        }

        void LoadFlowPanelLocSanPham(string loaiSanPham)
        {
            _ketQuaTimThaySanPham = 0;
            flpDanhSachSanPham.Controls.Clear();
            foreach (var item in DanhSachSanPham.ListSanPham)
            {
                if (item.LoaiSanPham == loaiSanPham)
                {
                    _ketQuaTimThaySanPham++;
                    Random random = new Random();
                    int color1 = random.Next(236, 255);
                    int color2 = random.Next(238, 255);
                    int color3 = random.Next(207, 255);
                    MaterialCard card = new ReaLTaiizor.Controls.MaterialCard();
                    card.Width = 290;
                    card.Height = 250;
                    card.Tag = item.ID;
                    PictureBox pic = new PictureBox();
                    pic.Width = 290;
                    pic.Height = 140;
                    pic.Location = new Point(0, 0);
                    pic.BackColor = Color.FromArgb(color1, color2, color3);

                    PictureBox picSanPham = new PictureBox();
                    picSanPham.Width = 195;
                    picSanPham.Height = 100;
                    picSanPham.BackColor = Color.FromArgb(color1, color2, color3);
                    picSanPham.Image = DanhSachSanPham.GetImageByPath(item.ID);
                    picSanPham.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic.Controls.Add(picSanPham);
                    picSanPham.Location = new Point(
                        (picSanPham.Parent.ClientSize.Width / 2) - (picSanPham.Width / 2),
                        (picSanPham.Parent.ClientSize.Height / 2) - (picSanPham.Height / 2)
                    );

                    Label lblTenSanPham = new Label();
                    lblTenSanPham.AutoSize = false;
                    lblTenSanPham.Width = 290;
                    lblTenSanPham.TextAlign = ContentAlignment.MiddleCenter;
                    lblTenSanPham.Text = item.TenSanPham;
                    lblTenSanPham.Font = new Font("Arial", 12, FontStyle.Bold);
                    card.Controls.Add(lblTenSanPham);
                    lblTenSanPham.Location = new Point(
                        (lblTenSanPham.Parent.ClientSize.Width / 2) - (lblTenSanPham.Width / 2),
                        picSanPham.Height + 50
                    );

                    Label lblGiaTien = new Label();
                    lblGiaTien.TextAlign = ContentAlignment.MiddleCenter;
                    lblGiaTien.AutoSize = false;
                    lblGiaTien.Width = 290;
                    lblGiaTien.Text = item.GiaTien.ToString();
                    lblGiaTien.Font = new Font("Arial", 10, FontStyle.Bold);
                    lblGiaTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblGiaTien.Text));

                    card.Controls.Add(lblGiaTien);
                    lblGiaTien.Location = new Point(
                        (lblGiaTien.Parent.ClientSize.Width / 2) - (lblGiaTien.Width / 2),
                        picSanPham.Height + 50 + lblTenSanPham.Height
                    );
                    if (item.Event != null && item.HienThiSuKien)
                    {
                        lblGiaTien.Font = new Font(lblGiaTien.Font, FontStyle.Strikeout);
                        Label lblGiaTienGiamGia = new Label();
                        lblGiaTienGiamGia.TextAlign = ContentAlignment.MiddleCenter;
                        lblGiaTienGiamGia.AutoSize = false;
                        lblGiaTienGiamGia.Width = 290;
                        lblGiaTienGiamGia.Text = (
                            item.GiaTien - item.GiaTien * item.Event.GiamGia / 100
                        ).ToString();
                        lblGiaTienGiamGia.Font = new Font("Arial", 10, FontStyle.Regular);
                        lblGiaTienGiamGia.Text = string.Format(
                            "{0:#,##0} VNĐ",
                            double.Parse(lblGiaTienGiamGia.Text)
                        );

                        card.Controls.Add(lblGiaTienGiamGia);
                        lblGiaTienGiamGia.Location = new Point(
                            (lblGiaTienGiamGia.Parent.ClientSize.Width / 2)
                                - (lblGiaTienGiamGia.Width / 2),
                            lblGiaTien.Height + lblGiaTien.Location.Y
                        );
                    }
                    card.Controls.Add(pic);

                    flpDanhSachSanPham.Controls.Add(card);
                }
            }
            lblKetQuaTimThay.Text = $"Có {_ketQuaTimThaySanPham} kết quả tìm thấy";
        }

        void LoadFlowPanelLocSanPhamTheoSuKien(int IDSuKien)
        {
            _ketQuaTimThaySanPham = 0;
            flpDanhSachSanPham.Controls.Clear();
            foreach (var item in DanhSachSanPham.ListSanPham)
            {
                if (item.Event != null && item.Event.ID == IDSuKien)
                {
                    _ketQuaTimThaySanPham++;
                    Random random = new Random();
                    int color1 = random.Next(236, 255);
                    int color2 = random.Next(238, 255);
                    int color3 = random.Next(207, 255);
                    MaterialCard card = new ReaLTaiizor.Controls.MaterialCard();
                    card.Width = 290;
                    card.Height = 250;
                    card.Tag = item.ID;
                    PictureBox pic = new PictureBox();
                    pic.Width = 290;
                    pic.Height = 140;
                    pic.Location = new Point(0, 0);
                    pic.BackColor = Color.FromArgb(color1, color2, color3);

                    PictureBox picSanPham = new PictureBox();
                    picSanPham.Width = 195;
                    picSanPham.Height = 100;
                    picSanPham.BackColor = Color.FromArgb(color1, color2, color3);
                    picSanPham.Image = DanhSachSanPham.GetImageByPath(item.ID);
                    picSanPham.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic.Controls.Add(picSanPham);
                    picSanPham.Location = new Point(
                        (picSanPham.Parent.ClientSize.Width / 2) - (picSanPham.Width / 2),
                        (picSanPham.Parent.ClientSize.Height / 2) - (picSanPham.Height / 2)
                    );

                    Label lblTenSanPham = new Label();
                    lblTenSanPham.AutoSize = false;
                    lblTenSanPham.Width = 290;
                    lblTenSanPham.TextAlign = ContentAlignment.MiddleCenter;
                    lblTenSanPham.Text = item.TenSanPham;
                    lblTenSanPham.Font = new Font("Arial", 12, FontStyle.Bold);
                    card.Controls.Add(lblTenSanPham);
                    lblTenSanPham.Location = new Point(
                        (lblTenSanPham.Parent.ClientSize.Width / 2) - (lblTenSanPham.Width / 2),
                        picSanPham.Height + 50
                    );

                    Label lblGiaTien = new Label();
                    lblGiaTien.TextAlign = ContentAlignment.MiddleCenter;
                    lblGiaTien.AutoSize = false;
                    lblGiaTien.Width = 290;
                    lblGiaTien.Text = item.GiaTien.ToString();
                    lblGiaTien.Font = new Font("Arial", 10, FontStyle.Bold);
                    lblGiaTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblGiaTien.Text));

                    card.Controls.Add(lblGiaTien);
                    lblGiaTien.Location = new Point(
                        (lblGiaTien.Parent.ClientSize.Width / 2) - (lblGiaTien.Width / 2),
                        picSanPham.Height + 50 + lblTenSanPham.Height
                    );
                    if (item.Event != null && item.HienThiSuKien)
                    {
                        lblGiaTien.Font = new Font(lblGiaTien.Font, FontStyle.Strikeout);
                        Label lblGiaTienGiamGia = new Label();
                        lblGiaTienGiamGia.TextAlign = ContentAlignment.MiddleCenter;
                        lblGiaTienGiamGia.AutoSize = false;
                        lblGiaTienGiamGia.Width = 290;
                        lblGiaTienGiamGia.Text = (
                            item.GiaTien - item.GiaTien * item.Event.GiamGia / 100
                        ).ToString();
                        lblGiaTienGiamGia.Font = new Font("Arial", 10, FontStyle.Regular);
                        lblGiaTienGiamGia.Text = string.Format(
                            "{0:#,##0} VNĐ",
                            double.Parse(lblGiaTienGiamGia.Text)
                        );

                        card.Controls.Add(lblGiaTienGiamGia);
                        lblGiaTienGiamGia.Location = new Point(
                            (lblGiaTienGiamGia.Parent.ClientSize.Width / 2)
                                - (lblGiaTienGiamGia.Width / 2),
                            lblGiaTien.Height + lblGiaTien.Location.Y
                        );
                    }
                    card.Controls.Add(pic);

                    flpDanhSachSanPham.Controls.Add(card);
                }
            }
            lblKetQuaTimThay.Text = $"Có {_ketQuaTimThaySanPham} kết quả tìm thấy";
        }

        void LoadFlowPanelTimSanPham(List<string> danhSachTimKiem)
        {
            _ketQuaTimThaySanPham = 0;
            flpDanhSachSanPham.Controls.Clear();
            foreach (var item in DanhSachSanPham.ListSanPham)
            {
                if (danhSachTimKiem.Contains(item.ID))
                {
                    _ketQuaTimThaySanPham++;
                    Random random = new Random();
                    int color1 = random.Next(236, 255);
                    int color2 = random.Next(238, 255);
                    int color3 = random.Next(207, 255);
                    MaterialCard card = new ReaLTaiizor.Controls.MaterialCard();
                    card.Width = 290;
                    card.Height = 250;
                    card.Tag = item.ID;
                    PictureBox pic = new PictureBox();
                    pic.Width = 290;
                    pic.Height = 140;
                    pic.Location = new Point(0, 0);
                    pic.BackColor = Color.FromArgb(color1, color2, color3);

                    PictureBox picSanPham = new PictureBox();
                    picSanPham.Width = 195;
                    picSanPham.Height = 100;
                    picSanPham.BackColor = Color.FromArgb(color1, color2, color3);
                    picSanPham.Image = DanhSachSanPham.GetImageByPath(item.ID);
                    picSanPham.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic.Controls.Add(picSanPham);
                    picSanPham.Location = new Point(
                        (picSanPham.Parent.ClientSize.Width / 2) - (picSanPham.Width / 2),
                        (picSanPham.Parent.ClientSize.Height / 2) - (picSanPham.Height / 2)
                    );

                    Label lblTenSanPham = new Label();
                    lblTenSanPham.AutoSize = false;
                    lblTenSanPham.Width = 290;
                    lblTenSanPham.TextAlign = ContentAlignment.MiddleCenter;
                    lblTenSanPham.Text = item.TenSanPham;
                    lblTenSanPham.Font = new Font("Arial", 12, FontStyle.Bold);
                    card.Controls.Add(lblTenSanPham);
                    lblTenSanPham.Location = new Point(
                        (lblTenSanPham.Parent.ClientSize.Width / 2) - (lblTenSanPham.Width / 2),
                        picSanPham.Height + 50
                    );

                    Label lblGiaTien = new Label();
                    lblGiaTien.TextAlign = ContentAlignment.MiddleCenter;
                    lblGiaTien.AutoSize = false;
                    lblGiaTien.Width = 290;
                    lblGiaTien.Text = item.GiaTien.ToString();
                    lblGiaTien.Font = new Font("Arial", 10, FontStyle.Bold);
                    lblGiaTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblGiaTien.Text));

                    card.Controls.Add(lblGiaTien);
                    lblGiaTien.Location = new Point(
                        (lblGiaTien.Parent.ClientSize.Width / 2) - (lblGiaTien.Width / 2),
                        picSanPham.Height + 50 + lblTenSanPham.Height
                    );
                    if (item.Event != null && item.HienThiSuKien)
                    {
                        lblGiaTien.Font = new Font(lblGiaTien.Font, FontStyle.Strikeout);
                        Label lblGiaTienGiamGia = new Label();
                        lblGiaTienGiamGia.TextAlign = ContentAlignment.MiddleCenter;
                        lblGiaTienGiamGia.AutoSize = false;
                        lblGiaTienGiamGia.Width = 290;
                        lblGiaTienGiamGia.Text = (
                            item.GiaTien - item.GiaTien * item.Event.GiamGia / 100
                        ).ToString();
                        lblGiaTienGiamGia.Font = new Font("Arial", 10, FontStyle.Regular);
                        lblGiaTienGiamGia.Text = string.Format(
                            "{0:#,##0} VNĐ",
                            double.Parse(lblGiaTienGiamGia.Text)
                        );

                        card.Controls.Add(lblGiaTienGiamGia);
                        lblGiaTienGiamGia.Location = new Point(
                            (lblGiaTienGiamGia.Parent.ClientSize.Width / 2)
                                - (lblGiaTienGiamGia.Width / 2),
                            lblGiaTien.Height + lblGiaTien.Location.Y
                        );
                    }
                    card.Controls.Add(pic);

                    flpDanhSachSanPham.Controls.Add(card);
                }
            }
            lblKetQuaTimThay.Text = $"Có {_ketQuaTimThaySanPham} kết quả tìm thấy";
        }

        public void LoadFlowPanelDanhSachBan()
        {
            _ketQuaTimThayBan = 0;
            flpDanhSachBan.Controls.Clear();
            foreach (var item in DanhSachBan.ListBan)
            {
                _ketQuaTimThayBan++;

                MaterialCard card = new ReaLTaiizor.Controls.MaterialCard();
                card.Width = 290;
                card.Height = 250;
                card.Tag = item.ID;
                PictureBox pic = new PictureBox();
                pic.Width = 290;
                pic.Height = 140;
                pic.Location = new Point(0, 0);

                PictureBox picSanPham = new PictureBox();
                picSanPham.Width = 195;
                picSanPham.Height = 100;
                if (item.TinhTrang == 0)
                {
                    picSanPham.Image = Properties.Resources.table_chuadat;
                    pic.BackColor = Color.FromArgb(254, 249, 207);
                    picSanPham.BackColor = Color.FromArgb(254, 249, 207);
                }
                else
                {
                    picSanPham.Image = Properties.Resources.table_dadat;
                    pic.BackColor = Color.FromArgb(242, 252, 219);
                    picSanPham.BackColor = Color.FromArgb(242, 252, 219);
                }
                picSanPham.SizeMode = PictureBoxSizeMode.StretchImage;
                pic.Controls.Add(picSanPham);
                picSanPham.Location = new Point(
                    (picSanPham.Parent.ClientSize.Width / 2) - (picSanPham.Width / 2),
                    (picSanPham.Parent.ClientSize.Height / 2) - (picSanPham.Height / 2)
                );

                Label lblTenSanPham = new Label();
                lblTenSanPham.AutoSize = false;
                lblTenSanPham.Width = 290;
                lblTenSanPham.TextAlign = ContentAlignment.MiddleCenter;
                lblTenSanPham.Text = item.TenBan;
                lblTenSanPham.Font = new Font("Arial", 12, FontStyle.Bold);
                card.Controls.Add(lblTenSanPham);
                lblTenSanPham.Location = new Point(
                    (lblTenSanPham.Parent.ClientSize.Width / 2) - (lblTenSanPham.Width / 2),
                    picSanPham.Height + 50
                );
                MaterialButton buttonDetail = new MaterialButton();
                buttonDetail.Text = "Chi tiết";
                buttonDetail.CharacterCasing = MaterialButton.CharacterCasingEnum.Title;
                buttonDetail.Cursor = Cursors.Hand;
                buttonDetail.Tag = item.ID;
                buttonDetail.Click += new EventHandler(ChiTietBan_Click);
                card.Controls.Add(buttonDetail);
                buttonDetail.Location = new Point(
                    (buttonDetail.Parent.ClientSize.Width / 2) - (buttonDetail.Width / 2),
                    lblTenSanPham.Location.Y + lblTenSanPham.Height
                );

                card.Controls.Add(pic);

                flpDanhSachBan.Controls.Add(card);
            }
            lblKetQuaTimThayBan.Text = $"Có {_ketQuaTimThayBan} kết quả tìm thấy";
        }

        void LoadFlowPanelDanhSachBanLoc(int tinhTrang)
        {
            _ketQuaTimThayBan = 0;
            flpDanhSachBan.Controls.Clear();
            foreach (var item in DanhSachBan.ListBan)
            {
                if (item.TinhTrang == tinhTrang)
                {
                    _ketQuaTimThayBan++;

                    MaterialCard card = new ReaLTaiizor.Controls.MaterialCard();
                    card.Width = 290;
                    card.Height = 250;
                    card.Tag = item.ID;
                    PictureBox pic = new PictureBox();
                    pic.Width = 290;
                    pic.Height = 140;
                    pic.Location = new Point(0, 0);

                    PictureBox picSanPham = new PictureBox();
                    picSanPham.Width = 195;
                    picSanPham.Height = 100;
                    if (item.TinhTrang == 0)
                    {
                        picSanPham.Image = Properties.Resources.table_chuadat;
                        pic.BackColor = Color.FromArgb(254, 249, 207);
                        picSanPham.BackColor = Color.FromArgb(254, 249, 207);
                    }
                    else
                    {
                        picSanPham.Image = Properties.Resources.table_dadat;
                        pic.BackColor = Color.FromArgb(242, 252, 219);
                        picSanPham.BackColor = Color.FromArgb(242, 252, 219);
                    }
                    picSanPham.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic.Controls.Add(picSanPham);
                    picSanPham.Location = new Point(
                        (picSanPham.Parent.ClientSize.Width / 2) - (picSanPham.Width / 2),
                        (picSanPham.Parent.ClientSize.Height / 2) - (picSanPham.Height / 2)
                    );

                    Label lblTenSanPham = new Label();
                    lblTenSanPham.AutoSize = false;
                    lblTenSanPham.Width = 290;
                    lblTenSanPham.TextAlign = ContentAlignment.MiddleCenter;
                    lblTenSanPham.Text = item.TenBan;
                    lblTenSanPham.Font = new Font("Arial", 12, FontStyle.Bold);
                    card.Controls.Add(lblTenSanPham);
                    lblTenSanPham.Location = new Point(
                        (lblTenSanPham.Parent.ClientSize.Width / 2) - (lblTenSanPham.Width / 2),
                        picSanPham.Height + 50
                    );
                    MaterialButton buttonDetail = new MaterialButton();
                    buttonDetail.Text = "Chi tiết";
                    buttonDetail.CharacterCasing = MaterialButton.CharacterCasingEnum.Title;
                    buttonDetail.Cursor = Cursors.Hand;
                    buttonDetail.Tag = item.ID;
                    buttonDetail.Click += new EventHandler(ChiTietBan_Click);
                    card.Controls.Add(buttonDetail);
                    buttonDetail.Location = new Point(
                        (buttonDetail.Parent.ClientSize.Width / 2) - (buttonDetail.Width / 2),
                        lblTenSanPham.Location.Y + lblTenSanPham.Height
                    );

                    card.Controls.Add(pic);

                    flpDanhSachBan.Controls.Add(card);
                }
            }
            lblKetQuaTimThayBan.Text = $"Có {_ketQuaTimThayBan} kết quả tìm thấy";
        }

        void LoadFlowPanelDanhSachBanTimKiem(List<string> danhSachTimKiem)
        {
            _ketQuaTimThayBan = 0;
            flpDanhSachBan.Controls.Clear();
            foreach (var item in DanhSachBan.ListBan)
            {
                if (danhSachTimKiem.Contains(item.ID))
                {
                    _ketQuaTimThayBan++;

                    MaterialCard card = new ReaLTaiizor.Controls.MaterialCard();
                    card.Width = 290;
                    card.Height = 250;
                    card.Tag = item.ID;
                    PictureBox pic = new PictureBox();
                    pic.Width = 290;
                    pic.Height = 140;
                    pic.Location = new Point(0, 0);

                    PictureBox picSanPham = new PictureBox();
                    picSanPham.Width = 195;
                    picSanPham.Height = 100;
                    if (item.TinhTrang == 0)
                    {
                        picSanPham.Image = Properties.Resources.table_chuadat;
                        pic.BackColor = Color.FromArgb(254, 249, 207);
                        picSanPham.BackColor = Color.FromArgb(254, 249, 207);
                    }
                    else
                    {
                        picSanPham.Image = Properties.Resources.table_dadat;
                        pic.BackColor = Color.FromArgb(242, 252, 219);
                        picSanPham.BackColor = Color.FromArgb(242, 252, 219);
                    }
                    picSanPham.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic.Controls.Add(picSanPham);
                    picSanPham.Location = new Point(
                        (picSanPham.Parent.ClientSize.Width / 2) - (picSanPham.Width / 2),
                        (picSanPham.Parent.ClientSize.Height / 2) - (picSanPham.Height / 2)
                    );

                    Label lblTenSanPham = new Label();
                    lblTenSanPham.AutoSize = false;
                    lblTenSanPham.Width = 290;
                    lblTenSanPham.TextAlign = ContentAlignment.MiddleCenter;
                    lblTenSanPham.Text = item.TenBan;
                    lblTenSanPham.Font = new Font("Arial", 12, FontStyle.Bold);
                    card.Controls.Add(lblTenSanPham);
                    lblTenSanPham.Location = new Point(
                        (lblTenSanPham.Parent.ClientSize.Width / 2) - (lblTenSanPham.Width / 2),
                        picSanPham.Height + 50
                    );
                    MaterialButton buttonDetail = new MaterialButton();
                    buttonDetail.Text = "Chi tiết";
                    buttonDetail.CharacterCasing = MaterialButton.CharacterCasingEnum.Title;
                    buttonDetail.Cursor = Cursors.Hand;
                    buttonDetail.Tag = item.ID;
                    buttonDetail.Click += new EventHandler(ChiTietBan_Click);
                    card.Controls.Add(buttonDetail);
                    buttonDetail.Location = new Point(
                        (buttonDetail.Parent.ClientSize.Width / 2) - (buttonDetail.Width / 2),
                        lblTenSanPham.Location.Y + lblTenSanPham.Height
                    );

                    card.Controls.Add(pic);

                    flpDanhSachBan.Controls.Add(card);
                }
            }
            lblKetQuaTimThayBan.Text = $"Có {_ketQuaTimThayBan} kết quả tìm thấy";
        }

        public void LoadDanhSachBan()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from DANHSACHBAN";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            DanhSachBan.ListBan.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                string ID = (string)dr["ID"];
                string ten = (string)dr["TEN"];
                int tinhTrang = (int)dr["TINHTRANG"];

                Ban banMoi = new Ban(ID, ten, tinhTrang);
                if (!dr.IsNull("ID_DATBAN"))
                {
                    BanDat getBanDat = DanhSachBanDat.TimKiem((int)dr["ID_DATBAN"]);
                    banMoi.BanDatHienTai = getBanDat;
                }
                if (!dr.IsNull("ID_HOADON"))
                {
                    HoaDon getHoaDon = DanhSachHoaDon.TimKiem((int)dr["ID_HOADON"]);
                    banMoi.HoaDonHienTai = getHoaDon;
                }
                DanhSachBan.AddBan(banMoi);
            }
        }

        public void LoadDanhSachSanPham()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from DANHSACHSANPHAM";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
            DanhSachSanPham.ListSanPham.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                string ID = (string)dr["ID"];
                string imagePath = (string)dr["IMAGE_PATH"];
                string loaiSanPham = (string)dr["LOAISANPHAM"];
                string moTa = (string)dr["MOTA"];
                string ten = (string)dr["TEN"];

                int giaTien = (int)dr["GIATIEN"];
                bool hienThiSuKien = false;
                SuKien suKienMoi = null;
                if (!dr.IsNull("EVENT"))
                {
                    suKienMoi = DanhSachSuKien.TimKiem((int)dr["EVENT"]);
                    if (suKienMoi != null)
                    {
                        DateTime today = DateTime.Now;
                        if (suKienMoi.ThoiGianKetThuc >= today && suKienMoi.ThoiGianBatDau <= today)
                        {
                            hienThiSuKien = true;
                        }
                    }
                }

                SanPham sanPhamMoi = new SanPham(
                    ID,
                    loaiSanPham,
                    moTa,
                    ten,
                    giaTien,
                    suKienMoi,
                    imagePath
                );
                sanPhamMoi.HienThiSuKien = hienThiSuKien;
                DanhSachSanPham.AddSanPham(sanPhamMoi);
            }
        }

        #endregion

        #region Các sự kiện
        // Timer cập nhật thời gian hiện tại
        private void _timerThoiGianHienTai_Tick(object sender, EventArgs e)
        {
            lblThoiGianHienTai.Text = $"{DateTime.Now}";
        }

        // Sự kiện Click xem chi tiết bàn trong danh sách bàn
        private void ChiTietBan_Click(object sender, EventArgs e)
        {
            MaterialButton button = (MaterialButton)sender;
            ChiTietBanForm fChiTietBan = new ChiTietBanForm();
            Ban timBan = DanhSachBan.TimKiemBan((string)button.Tag);
            ControlForm.BanDangChon = timBan;
            fChiTietBan.ShowDialog();
        }

        // Sự kiện Click Lọc loại Thức Ăn trong danh sách sản phẩm
        private void picBtnThucAnLoc_Click(object sender, EventArgs e)
        {
            if (_isClickLocThucAn)
            {
                LoadFlowPanelSanPham();
            }
            else
            {
                LoadFlowPanelLocSanPham("DOAN");
            }
            _isClickLocThucAn = !_isClickLocThucAn;
            if (_isClickLocThucAn)
            {
                picBtnKhacLoc.Visible = false;
                picBtnNuocLoc.Visible = false;
            }
            else
            {
                picBtnKhacLoc.Visible = true;
                picBtnNuocLoc.Visible = true;
            }
        }

        // Lọc Khác trong Sản Phẩm
        private void picBtnKhacLoc_Click(object sender, EventArgs e)
        {
            if (_isClickLocKhac)
            {
                LoadFlowPanelSanPham();
            }
            else
            {
                LoadFlowPanelLocSanPham("KHAC");
            }
            _isClickLocKhac = !_isClickLocKhac;
            if (_isClickLocKhac)
            {
                picBtnThucAnLoc.Visible = false;
                picBtnNuocLoc.Visible = false;
            }
            else
            {
                picBtnThucAnLoc.Visible = true;
                picBtnNuocLoc.Visible = true;
            }
        }

        // Lọc Nước trong Sản Phẩm
        private void picBtnNuocLoc_Click(object sender, EventArgs e)
        {
            if (_isClickLocNuoc)
            {
                LoadFlowPanelSanPham();
            }
            else
            {
                LoadFlowPanelLocSanPham("THUCUONG");
            }
            _isClickLocNuoc = !_isClickLocNuoc;
            if (_isClickLocNuoc)
            {
                picBtnThucAnLoc.Visible = false;
                picBtnKhacLoc.Visible = false;
            }
            else
            {
                picBtnThucAnLoc.Visible = true;
                picBtnKhacLoc.Visible = true;
            }
        }

        // Tìm kiếm sản phẩm theo tên sản phẩm
        private void btnTimKiemSanPham_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtTimKiemSanPham.Text.Trim();
                _isClickLocKhac = false;
                _isClickLocThucAn = false;
                _isClickLocNuoc = false;
                picBtnKhacLoc.Visible = true;
                picBtnThucAnLoc.Visible = true;
                picBtnNuocLoc.Visible = true;

                List<string> danhSachSanPhamTimKiem = new List<string>();
                Database.CreateConnection();
                string sqlCommand =
                    $"select * from DANHSACHSANPHAM where TEN like N'%{searchValue}%'";
                DataTable dt;
                dt = Database.SelectQuery(sqlCommand);
                foreach (DataRow dr in dt.Rows)
                {
                    string ID = (string)dr["ID"];
                    danhSachSanPhamTimKiem.Add(ID);
                }
                // Hiển thị Item tìm thấy
                LoadFlowPanelTimSanPham(danhSachSanPhamTimKiem);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        // Lọc bàn chưa đặt trong Danh Sách Bàn
        private void picBanChuaDat_Click(object sender, EventArgs e)
        {
            if (_isClickBanChuaDat)
            {
                LoadFlowPanelDanhSachBan();
            }
            else
            {
                LoadFlowPanelDanhSachBanLoc(0);
            }
            _isClickBanChuaDat = !_isClickBanChuaDat;
            if (_isClickBanChuaDat)
            {
                picBanDaDat.Visible = false;
                lblBanDaDat.Visible = false;
            }
            else
            {
                picBanDaDat.Visible = true;
                lblBanDaDat.Visible = true;
            }
        }

        // Lọc bàn đã đặt trong danh sách bàn
        private void picBanDaDat_Click(object sender, EventArgs e)
        {
            if (_isClickBanDaDat)
            {
                LoadFlowPanelDanhSachBan();
            }
            else
            {
                LoadFlowPanelDanhSachBanLoc(1);
            }
            _isClickBanDaDat = !_isClickBanDaDat;
            if (_isClickBanDaDat)
            {
                picBanChuaDat.Visible = false;
                lblBanChuaDat.Visible = false;
            }
            else
            {
                picBanChuaDat.Visible = true;
                lblBanChuaDat.Visible = true;
            }
        }

        // Tìm kiếm bàn theo tên bàn
        private void btnTimKiemBan_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtTimKiemBan.Text.Trim();
                _isClickBanChuaDat = false;
                _isClickBanDaDat = false;
                picBanChuaDat.Visible = true;
                lblBanChuaDat.Visible = true;
                picBanDaDat.Visible = true;
                lblBanDaDat.Visible = true;

                List<string> danhSachBanTimKiem = new List<string>();
                Database.CreateConnection();
                string sqlCommand = $"select * from DANHSACHBAN where TEN like N'%{searchValue}%'";
                DataTable dt;
                dt = Database.SelectQuery(sqlCommand);
                foreach (DataRow dr in dt.Rows)
                {
                    string ID = (string)dr["ID"];
                    danhSachBanTimKiem.Add(ID);
                }
                // Hiển thị Item tìm thấy
                LoadFlowPanelDanhSachBanTimKiem(danhSachBanTimKiem);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void txtTimKiemBan_Click(object sender, EventArgs e) { }

        // Tìm sản phẩm theo sự kiện
        private void cboDanhSachSuKien_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDanhSachSuKien.SelectedValue is int)
                {
                    if (cboDanhSachSuKien.SelectedIndex != -1)
                    {
                        _isClickLocKhac = false;
                        _isClickLocNuoc = false;
                        _isClickLocThucAn = false;
                        picBtnThucAnLoc.Visible = true;
                        picBtnKhacLoc.Visible = true;
                        picBtnNuocLoc.Visible = true;
                        LoadFlowPanelLocSanPhamTheoSuKien(
                            int.Parse(cboDanhSachSuKien.SelectedValue.ToString())
                        );
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        // Bắt đầu ca làm nhân viên
        private void btnBatDauCa_Click(object sender, EventArgs e)
        {
            try
            {
                if (TaiKhoanHienTai.TaiKhoanHienHanh == null)
                {
                    throw new Exception("Vui lòng đăng nhập");
                }
                if (TaiKhoanHienTai.IDCaLamHienTai != -1)
                {
                    throw new Exception("Bạn đang trong ca làm");
                }
                Database.CreateConnection();
                DateTime thoiGianHienTai = DateTime.Now;
                string getDate = thoiGianHienTai.ToString("yyyy-MM-dd");

                string sqlCommand;
                SqlDataReader rd;
                SqlCommand cmd;

                Database.CreateConnection();

                sqlCommand =
                    $"insert into LICHSUCA (USERNAME, THOIGIANVAOCA) values ('{TaiKhoanHienTai.UserName}', '{thoiGianHienTai}')";
                cmd = Database.CreateCommand(sqlCommand);
                cmd.ExecuteNonQuery();

                sqlCommand =
                    $"select * from LICHSUCA where USERNAME = '{TaiKhoanHienTai.UserName}' and THOIGIANVAOCA = '{thoiGianHienTai}'";
                cmd = Database.CreateCommand(sqlCommand);
                rd = cmd.ExecuteReader();
                int IDCaLamHienTai = -1;
                while (rd.Read())
                {
                    IDCaLamHienTai = (int)rd["ID"];
                }
                rd.Close();

                TaiKhoanHienTai.IDCaLamHienTai = IDCaLamHienTai;
                TaiKhoanHienTai.ThoiGianVaoCaLamHienTai = thoiGianHienTai;
                btnKetThucCa.Visible = true;
                btnBatDauCa.Visible = false;
                MessageBox.Show("Vào ca thành công");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        // Kết thúc ca làm
        private void btnKetThucCa_Click(object sender, EventArgs e)
        {
            try
            {
                if (TaiKhoanHienTai.TaiKhoanHienHanh == null)
                {
                    throw new Exception("Vui lòng đăng nhập");
                }
                if (TaiKhoanHienTai.IDCaLamHienTai == -1)
                {
                    throw new Exception("Bạn chưa vào ca làm");
                }
                Database.CreateConnection();
                DateTime thoiGianHienTai = DateTime.Now;
                TimeSpan tongThoiGianLam = thoiGianHienTai.Subtract(
                    TaiKhoanHienTai.ThoiGianVaoCaLamHienTai
                );

                string sqlCommand;
                SqlDataReader rd;
                SqlCommand cmd;
                sqlCommand =
                    $"update LICHSUCA set TONGGIOLAM = '{tongThoiGianLam.Hours}', THOIGIANRACA = '{thoiGianHienTai}' where ID = '{TaiKhoanHienTai.IDCaLamHienTai}'";
                cmd = Database.CreateCommand(sqlCommand);
                cmd.ExecuteNonQuery();

                TaiKhoanHienTai.IDCaLamHienTai = -1;
                btnBatDauCa.Visible = true;
                btnKetThucCa.Visible = false;
                MessageBox.Show("Ra ca thành công");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        // Chỉnh sửa thông tin cá nhân
        private void btnEditThongTin_Click(object sender, EventArgs e)
        {
            ChinhSuaThongTinForm fChinhSuaThongTin = new ChinhSuaThongTinForm();
            fChinhSuaThongTin.ShowDialog();
        }

        private void pnlQuanLyVoucher_Click(object sender, EventArgs e)
        {
            ChinhSuaVoucherForm f = new ChinhSuaVoucherForm();
            f.ShowDialog();
        }

        private void pnlQuanLyEvent_Click(object sender, EventArgs e)
        {
            ChinhSuaSuKienForm f = new ChinhSuaSuKienForm();
            f.ShowDialog();
        }

        private void pnlQuanLyHeThong_Click(object sender, EventArgs e)
        {
            ChinhSuaHeThongForm f = new ChinhSuaHeThongForm();
            f.ShowDialog();
        }

        private void pnlQuanLyLichSuCa_Click(object sender, EventArgs e)
        {
            LichSuCaForm f = new LichSuCaForm();
            f.ShowDialog();
        }

        private void pnlQuanLyDoanhThu_Click(object sender, EventArgs e)
        {
            DoanhThuForm f = new DoanhThuForm();
            f.ShowDialog();
        }

        private void pnlQuanLySanPham_Click(object sender, EventArgs e)
        {
            ChinhSuaSanPham f = new ChinhSuaSanPham();
            f.ShowDialog();
        }

        private void pnlQuanLyBan_Click(object sender, EventArgs e)
        {
            ChinhSuaBanForm f = new ChinhSuaBanForm();
            f.ShowDialog();
        }

        private void pnlQuanLyTaiKhoan_Click(object sender, EventArgs e)
        {
            ChinhSuaTaiKhoanForm f = new ChinhSuaTaiKhoanForm();
            f.ShowDialog();
        }

        private void tabMainForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabMainForm.SelectedTab == tpTrangChu)
                {
                    ReloadTabPageTrangChu();
                }
                else if (tabMainForm.SelectedTab == tpSanPham)
                {
                    ReloadTabPageSanPham();
                }
                else if (tabMainForm.SelectedTab == tpBanDat)
                {
                    ReloadTabPageBanDat();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnTimKiemSuKien_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtTimKiemSuKien.Text.Trim();

                List<string> danhSachSuKienTimKiem = new List<string>();
                Database.CreateConnection();
                string sqlCommand = $"select * from EVENT where TEN like N'%{searchValue}%'";
                DataTable dt;
                dt = Database.SelectQuery(sqlCommand);
                foreach (DataRow dr in dt.Rows)
                {
                    string ID = dr["ID"].ToString();
                    danhSachSuKienTimKiem.Add(ID);
                }
                // Hiển thị Item tìm thấy
                LoadFlowPanelDanhSachSuKienTimKiem(danhSachSuKienTimKiem);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnTimKiemVoucher_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtTimKiemVoucher.Text.Trim();

                List<string> danhSachTimKiem = new List<string>();
                Database.CreateConnection();
                string sqlCommand = $"select * from VOUCHER where MA like N'%{searchValue}%'";
                DataTable dt;
                dt = Database.SelectQuery(sqlCommand);
                foreach (DataRow dr in dt.Rows)
                {
                    string Ma = (string)dr["MA"];
                    danhSachTimKiem.Add(Ma);
                }
                // Hiển thị Item tìm thấy
                LoadFlowPanelDanhSachVoucherTimKiem(danhSachTimKiem);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (
                    TaiKhoanHienTai.TaiKhoanHienHanh != null && TaiKhoanHienTai.IDCaLamHienTai != -1
                )
                {
                    throw new Exception("Bạn đang trong ca làm, vui lòng kết thúc ca");
                }
                if (!ControlForm.ConfirmForm("Xác nhận thoát chương trình"))
                {
                    e.Cancel = true;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                e.Cancel = true;
            }
        }
        private void btnTimKiemHoaDon_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtTimKiemMaHoaDon.Text.Trim();
                if (string.IsNullOrEmpty(searchValue))
                {
                    throw new Exception("Vui lòng nhập mã hóa đơn");
                }
                Database.CreateConnection();
                SqlDataReader rd;
                SqlCommand cmd;
                string sqlCommand =
                    $"select * from HOADON where ID = '{searchValue}' AND THANHTOAN = '1'";

                DataTable dt = Database.SelectQuery(sqlCommand);

                if (dt.Rows.Count == 0)
                {
                    throw new Exception("Hóa đơn không tồn tại hoặc chưa thanh toán");
                }

                foreach (DataRow dr in dt.Rows)
                {
                    int ID = (int)dr["ID"];
                    int IDDatBan = (int)dr["ID_DATBAN"];
                    int thanhTien = (int)dr["THANHTIEN"];
                    int thanhTienGiamGia = (int)dr["THANHTIENGIAMGIA"];
                    int thanhToan = (int)dr["THANHTOAN"];
                    int khachTra = (int)dr["KHACHTRA"];
                    int tienThua = (int)dr["TIENTHUA"];
                    DateTime thoiGianTao = (DateTime)dr["THOIGIAN_TAO"];
                    DateTime thoiGianThanhToan = new DateTime();
                    Voucher getVoucher = null;
                    if (!dr.IsNull("VOUCHER"))
                    {
                        string voucher = (string)dr["VOUCHER"];
                        getVoucher = DanhSachVoucher.TimKiem(voucher);
                    }
                    string nhanVien = (string)dr["NHANVIEN"];
                    if (!dr.IsNull("THOIGIAN_THANHTOAN"))
                    {
                        thoiGianThanhToan = (DateTime)dr["THOIGIAN_THANHTOAN"];
                    }

                    TaiKhoan getNhanVien = DanhSachTaiKhoan.TimKiem(nhanVien);
                    HoaDon hoaDonMoi = new HoaDon(
                        ID,
                        IDDatBan,
                        getVoucher,
                        getNhanVien,
                        thanhTien,
                        thanhToan,
                        thanhTienGiamGia,
                        khachTra,
                        tienThua,
                        thoiGianTao,
                        thoiGianThanhToan
                    );
                    ControlForm.HoaDonHienTai = hoaDonMoi;
                    DanhSachForm.XuatHoaDonForm fXuatHoaDon = new DanhSachForm.XuatHoaDonForm();
                    fXuatHoaDon.ShowDialog();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void picLogOut_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Các hàm phục vụ
        void ReloadTabPageTrangChu()
        {
            LoadDanhSachSuKien();
            LoadDanhSachVoucher();
            LoadFlowPanelDanhSachSuKien();
            LoadFlowPanelDanhSachVoucher();
            txtTimKiemSuKien.Text = null;
            txtTimKiemVoucher.Text = null;
        }

        void ReloadTabPageSanPham()
        {
            LoadDanhSachSuKien();
            LoadDanhSachSanPham();
            LoadFlowPanelSanPham();
            txtTimKiemSanPham.Text = null;
            _isClickLocKhac = false;
            _isClickLocNuoc = false;
            _isClickLocThucAn = false;
            picBtnThucAnLoc.Visible = true;
            picBtnKhacLoc.Visible = true;
            picBtnNuocLoc.Visible = true;
        }

        void ReloadTabPageBanDat()
        {
            LoadDanhSachBan();
            LoadFlowPanelDanhSachBan();
            txtTimKiemBan.Text = null;
            _isClickBanChuaDat = false;
            _isClickBanDaDat = false;
            picBanChuaDat.Visible = true;
            lblBanChuaDat.Visible = true;
            picBanDaDat.Visible = true;
            lblBanDaDat.Visible = true;
        }

        #endregion

      
    }
}

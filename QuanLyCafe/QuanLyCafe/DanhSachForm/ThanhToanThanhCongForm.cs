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
    public partial class ThanhToanThanhCongForm : MaterialForm
    {
        public ThanhToanThanhCongForm()
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

        private void ThanhToanThanhCongForm_Load(object sender, EventArgs e)
        {
            CreateBorderRadius();
            HienThiThongTinHoaDonThanhCong();
        }

        #region Các hàm khởi tạo
        void CreateBorderRadius()
        {
            pnlThanhToanThanhCong.Anchor = AnchorStyles.None;
            pnlThanhToanThanhCong.Region = Region.FromHrgn(
                CreateRoundRectRgn(
                    0,
                    0,
                    pnlThanhToanThanhCong.Width,
                    pnlThanhToanThanhCong.Height,
                    15,
                    15
                )
            );
        }

        void HienThiThongTinHoaDonThanhCong()
        {
            if (ControlForm.BanDangChon != null && ControlForm.BanDangChon.HoaDonHienTai != null)
            {
                lblTongTien.Text = ControlForm.BanDangChon.HoaDonHienTai.ThanhTien.ToString();
                lblTongTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblTongTien.Text));
                if (ControlForm.BanDangChon.HoaDonHienTai.VoucherHoaDon != null)
                {
                    lblGiamGia.Text =
                        $"{ControlForm.BanDangChon.HoaDonHienTai.VoucherHoaDon.GiamGia}% ({ControlForm.BanDangChon.HoaDonHienTai.VoucherHoaDon.Ma})";
                }
                else
                {
                    lblGiamGia.Text = $"0%";
                }
                lblThanhTien.Text =
                    ControlForm.BanDangChon.HoaDonHienTai.ThanhTienGiamGia.ToString();
                lblThanhTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblThanhTien.Text));
                lblKhachTra.Text = ControlForm.BanDangChon.HoaDonHienTai.TienKhachTra.ToString();
                lblKhachTra.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblKhachTra.Text));

                lblTienThua.Text = ControlForm.BanDangChon.HoaDonHienTai.TienThua.ToString();
                lblTienThua.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblTienThua.Text));
            }
        }

        #endregion

        #region Các hàm sự kiện
        private void ThanhToanThanhCongForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ControlForm.ConfirmForm("Hãy xuất hóa đơn trước khi thoát nhé!"))
            {
                e.Cancel = true;
            }
        }

        private void btnXuatHoaDon_Click(object sender, EventArgs e)
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

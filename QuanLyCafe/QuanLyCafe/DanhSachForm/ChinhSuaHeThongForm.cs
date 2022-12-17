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
    public partial class ChinhSuaHeThongForm : MaterialForm
    {
        public ChinhSuaHeThongForm()
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

        private void ChinhSuaHeThongForm_Load(object sender, EventArgs e)
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
                HienThiThongTinHeThong();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        #region Các hàm khởi tạo
        // Tạo border radius cho control
        void CreateBorderRadius()
        {
            pnlChinhSuaHeThong.Anchor = AnchorStyles.None;
            pnlChinhSuaHeThong.Region = Region.FromHrgn(
                CreateRoundRectRgn(
                    0,
                    0,
                    pnlChinhSuaHeThong.Width,
                    pnlChinhSuaHeThong.Height,
                    15,
                    15
                )
            );

            pnlForm.Anchor = AnchorStyles.None;
            pnlForm.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlForm.Width, pnlForm.Height, 15, 15)
            );
        }

        void HienThiThongTinHeThong()
        {
            if (HeThong.ID == 1)
            {
                txtTenCuaHang.Text = HeThong.TenCuaHang;
                txtDiaChiCuaHang.Text = HeThong.DiaChiCuaHang;
                txtLuongPartTime.Text = HeThong.LuongPartTime.ToString();
            }
        }

        #endregion

        #region Các hàm sự kiện
        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (HeThong.ID != 1)
                {
                    throw new Exception("Lỗi hệ thống");
                }
                string tenCuaHang = txtTenCuaHang.Text.Trim();
                string diaChiCuaHang = txtDiaChiCuaHang.Text.Trim();
                int luongPartTime = int.Parse(txtLuongPartTime.Text);

                if (string.IsNullOrEmpty(tenCuaHang) || string.IsNullOrEmpty(diaChiCuaHang))
                {
                    throw new Exception("Vui lòng nhập đủ thông tin");
                }
                if (luongPartTime < 0)
                {
                    throw new Exception("Vui lòng nhập tiền lương hợp lệ");
                }
                Database.CreateConnection();
                string sqlCommand;
                SqlCommand cmd;
                sqlCommand =
                    $"update HETHONG set TENCUAHANG = @TEN, DIACHICUAHANG = @DIACHI, LUONG_PARTTIME = @LUONG where ID = 1";

                cmd = Database.CreateCommand(sqlCommand);
                cmd.Parameters.AddWithValue("@TEN", tenCuaHang);

                cmd.Parameters.AddWithValue("@DIACHI", diaChiCuaHang);

                cmd.Parameters.AddWithValue("@LUONG", luongPartTime);

                cmd.ExecuteNonQuery();
                HeThong.SetHeThong(1, tenCuaHang, diaChiCuaHang, luongPartTime);
                MessageBox.Show("Lưu thành công");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void txtLuongPartTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        #endregion
    }
}

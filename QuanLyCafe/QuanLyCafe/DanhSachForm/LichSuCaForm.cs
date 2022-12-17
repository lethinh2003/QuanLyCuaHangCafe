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
    public partial class LichSuCaForm : MaterialForm
    {
        public LichSuCaForm()
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

        private void LichSuCaForm_Load(object sender, EventArgs e)
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

                dtpThoiGian.CustomFormat = "yyyy-MM-dd";

                pnlThongTinThanhToan.Visible = false;
                LoadDanhSachTaiKhoan();
                CreateBorderRadius();
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
            pnlThongTinThanhToan.Anchor = AnchorStyles.None;
            pnlThongTinThanhToan.Region = Region.FromHrgn(
                CreateRoundRectRgn(
                    0,
                    0,
                    pnlThongTinThanhToan.Width,
                    pnlThongTinThanhToan.Height,
                    15,
                    15
                )
            );

            pnlLichSuCaLam.Anchor = AnchorStyles.None;
            pnlLichSuCaLam.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlLichSuCaLam.Width, pnlLichSuCaLam.Height, 15, 15)
            );
        }

        void LoadDanhSachTaiKhoan()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from TAIKHOAN";
            DataTable dt;

            dt = Database.SelectQuery(sqlCommand);

            cboTaiKhoan.DataSource = dt;
            cboTaiKhoan.DisplayMember = "USERNAME";
            cboTaiKhoan.ValueMember = "USERNAME";
        }

        #endregion

        #region Các hàm sự kiện
        private void btnXem_Click(object sender, EventArgs e)
        {
            try
            {
                string taiKhoan = cboTaiKhoan.SelectedValue.ToString();
                DateTime getDate = dtpThoiGian.Value;
                DateTime getFilterDate = new DateTime(getDate.Year, getDate.Month, getDate.Day);

                DateTime getCurrentDate = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day
                );
                if (getFilterDate > getCurrentDate)
                {
                    throw new Exception("Vui lòng chọn ngày hợp lệ");
                }

                LoadLichSuCa(taiKhoan, getDate);
                pnlThongTinThanhToan.Visible = true;
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
                string taiKhoan = cboTaiKhoan.SelectedValue.ToString();

                string getDate = dtpThoiGian.Value.ToString("yyyy-MM-dd");
                Database.CreateConnection();
                string sqlCommand;
                SqlCommand cmd;
                SqlDataReader rd;
                sqlCommand =
                    $"select * from LICHSUTHANHTOANCA where USERNAME = '{taiKhoan}' and THOIGIAN = '{getDate}' and THANHTOAN = '0'";
                cmd = Database.CreateCommand(sqlCommand);
                rd = cmd.ExecuteReader();
                if (!rd.HasRows)
                {
                    throw new Exception("Ca làm đã được thanh toán");
                }

                rd.Close();
                // Cập nhật trạng thái thanh toán
                Database.CreateConnection();
                sqlCommand =
                    $"update LICHSUTHANHTOANCA set THANHTOAN = '1', THOIGIAN_THANHTOAN = '{DateTime.Now}' where USERNAME = '{taiKhoan}' and THOIGIAN = '{getDate}'";
                cmd = Database.CreateCommand(sqlCommand);
                cmd.ExecuteNonQuery();
                btnThanhToan.Visible = false;
                lblDaThanhToan.Visible = true;
                MessageBox.Show("Thanh toán thành công");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        #endregion

        #region Các hàm phục vụ
        void LoadLichSuCa(string taiKhoan, DateTime date)
        {
            Database.CreateConnection();
            string getDate = date.ToString("yyyy-MM-dd");
            lblNgay.Text = getDate;
            int tongGioLam = 0;
            string sqlCommand =
                $"select * from LICHSUCA where cast(THOIGIANVAOCA as date) = '{getDate}' and USERNAME = '{taiKhoan}'";
            DataTable dt;
            SqlDataReader rd;
            SqlCommand cmd;
            dt = Database.SelectQuery(sqlCommand);
            dgvLichSu.DataSource = dt;
            bool checkNgayLam = false;

            foreach (DataRow dr in dt.Rows)
            {
                checkNgayLam = true;
                tongGioLam += (int)dr["TONGGIOLAM"];
            }
            int tongTien = tongGioLam * HeThong.LuongPartTime;

            lblTongGioLam.Text = tongGioLam.ToString();

            if (checkNgayLam)
            {
                sqlCommand =
                    $"select * from LICHSUTHANHTOANCA where USERNAME = '{taiKhoan}' and THOIGIAN = '{getDate}'";
                cmd = Database.CreateCommand(sqlCommand);
                rd = cmd.ExecuteReader();
                // Kiếm tra nếu lịch sử thanh toán ca chưa có thì thêm vô database
                if (!rd.HasRows)
                {
                    Database.CreateConnection();

                    sqlCommand =
                        $"insert into LICHSUTHANHTOANCA (USERNAME, THOIGIAN, TONGGIOLAM, TONGTIEN) values ('{taiKhoan}', '{getDate}', '{tongGioLam}', '{tongTien}')";
                    cmd = Database.CreateCommand(sqlCommand);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    int thanhToan = 0;
                    int tongTienDaThanhToan = 0;
                    while (rd.Read())
                    {
                        thanhToan = (int)rd["THANHTOAN"];
                        tongTienDaThanhToan = (int)rd["TONGTIEN"];
                    }

                    if (thanhToan == 1)
                    {
                        lblTongTien.Text = (tongTienDaThanhToan).ToString();
                        lblTongTien.Text = string.Format(
                            "{0:#,##0} VNĐ",
                            double.Parse(lblTongTien.Text)
                        );

                        btnThanhToan.Visible = false;
                        lblDaThanhToan.Visible = true;
                    }
                    else
                    {
                        // Update thông tin lịch sử thanh toán ca
                        Database.CreateConnection();

                        sqlCommand =
                            $"update LICHSUTHANHTOANCA set TONGGIOLAM = '{tongGioLam}', TONGTIEN = '{tongTien}' where USERNAME = '{taiKhoan}' and THOIGIAN = '{getDate}'";
                        cmd = Database.CreateCommand(sqlCommand);
                        cmd.ExecuteNonQuery();
                        btnThanhToan.Visible = true;
                        lblDaThanhToan.Visible = false;
                        lblTongTien.Text = (tongTien).ToString();
                        lblTongTien.Text = string.Format(
                            "{0:#,##0} VNĐ",
                            double.Parse(lblTongTien.Text)
                        );
                    }
                }
                rd.Close();
            }
            else
            {
                lblTongTien.Text = (tongTien).ToString();
                lblTongTien.Text = string.Format("{0:#,##0} VNĐ", double.Parse(lblTongTien.Text));
                lblDaThanhToan.Visible = false;
            }
        }
        #endregion
    }
}

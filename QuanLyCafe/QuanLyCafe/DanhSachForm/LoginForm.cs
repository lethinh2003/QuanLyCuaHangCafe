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

using BC = BCrypt.Net.BCrypt;

namespace QuanLyCafe.DanhSachForm
{
    public partial class LoginForm : MaterialForm
    {
        public LoginForm()
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

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Tạo border radius
            pnlForm.Anchor = AnchorStyles.None;
            pnlForm.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, pnlForm.Width, pnlForm.Height, 30, 30)
            );
            LoadDanhSachTaiKhoan();
        }

        // Khởi tạo danh sách tài khoản từ database
        void LoadDanhSachTaiKhoan()
        {
            Database.CreateConnection();
            string sqlCommand = $"select * from TAIKHOAN";
            DataTable dt;
            dt = Database.SelectQuery(sqlCommand);
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

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string taiKhoan = txtUsername.Text.Trim();
                string matKhau = txtPassword.Text.Trim();
                if (string.IsNullOrEmpty(taiKhoan) || string.IsNullOrEmpty(matKhau))
                {
                    throw new Exception("Vui lòng nhập đầy đủ thông tin");
                }
                Database.CreateConnection();
                string sqlCommand = $"select * from TAIKHOAN where USERNAME = '{taiKhoan}'";
                SqlCommand command = Database.CreateCommand(sqlCommand);
                bool check = false;
                SqlDataReader reader = command.ExecuteReader();
                string passwordDatabase = "";
                string username = "";

                // Lấy thông tin người chơi
                while (reader.Read())
                {
                    check = true;
                    passwordDatabase = (string)reader["PASSWORD"];
                    username = (string)reader["USERNAME"];
                }
                // Kiểm tra xem có tồn tại người chơi hay không
                if (check == true)
                {
                    bool passwordCheck = BC.Verify(matKhau, passwordDatabase);
                    // Kiểm tra password nhập với password đã được mã hóa
                    if (passwordCheck)
                    {
                        // Cập nhật tài khoản trong chương trình
                        TaiKhoan getTaiKhoan = DanhSachTaiKhoan.TimKiem(username);
                        TaiKhoanHienTai.TaiKhoanHienHanh = getTaiKhoan;
                        TaiKhoanHienTai.UserName = username;
 
                        MessageBox.Show("Đăng nhập thành công");
                      
                        DanhSachForm.MainForm f = new DanhSachForm.MainForm();
                        f.ShowDialog();
                       
                    }
                    else
                    {
                        throw new Exception(
                            "Tài khoản không tồn tại hoặc mật khẩu không chính xác!"
                        );
                    }
                }
                else
                {
                    throw new Exception("Tài khoản không tồn tại hoặc mật khẩu không chính xác!");
                }
            }
            catch (Exception err)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar(err.Message);
                SnackBarMessage.Show(this);
            }
        }
    }
}

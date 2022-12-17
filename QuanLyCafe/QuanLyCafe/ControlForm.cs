using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCafe
{
    internal class ControlForm
    {
        static Ban _banDangChon = null;
        static BanDat _banDatDangChon = null;
        static DanhSachForm.MainForm _fMain = null;
        static DanhSachForm.ChiTietBanForm _fChiTietBan = null;
        static BaoCao _baoCaoHienTai = null;
        static HoaDon _hoaDonHienTai = null;
        public static Ban BanDangChon
        {
            get { return _banDangChon; }
            set { _banDangChon = value; }
        }
        public static HoaDon HoaDonHienTai
        {
            get { return _hoaDonHienTai; }
            set { _hoaDonHienTai = value; }
        }
        public static BanDat BanDatDangChon
        {
            get { return _banDatDangChon; }
            set { _banDatDangChon = value; }
        }
        public static DanhSachForm.MainForm FormMain
        {
            get { return _fMain; }
            set { _fMain = value; }
        }
        public static DanhSachForm.ChiTietBanForm FormChiTietBan
        {
            get { return _fChiTietBan; }
            set { _fChiTietBan = value; }
        }
        public static BaoCao BaoCaoHienTai
        {
            get { return _baoCaoHienTai; }
            set { _baoCaoHienTai = value; }
        }

        public static bool ConfirmForm(string message = "Xác nhận")
        {
            DialogResult r;
            r = MessageBox.Show(
                $"{message}",
                "Cảnh báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );
            if (r == DialogResult.Yes)
            {
                return true;
            }
            return false;
        }
    }
}

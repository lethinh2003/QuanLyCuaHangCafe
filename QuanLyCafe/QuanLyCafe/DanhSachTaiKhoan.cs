using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class DanhSachTaiKhoan
    {
        static List<TaiKhoan> _danhSachTaiKhoan = new List<TaiKhoan>();
        public static List<TaiKhoan> ListTaiKhoan
        {
            get { return _danhSachTaiKhoan; }
        }

        public static TaiKhoan TimKiem(string userName)
        {
            TaiKhoan ketQua = null;
            foreach (var item in _danhSachTaiKhoan)
            {
                if (item.UserName == userName)
                {
                    ketQua = item;
                    break;
                }
            }
            return ketQua;
        }

        static bool KiemTraTonTai(TaiKhoan item)
        {
            bool check = false;
            foreach (var x in _danhSachTaiKhoan)
            {
                if (x.UserName == item.UserName)
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        public static void Add(TaiKhoan item)
        {
            if (!KiemTraTonTai(item))
            {
                _danhSachTaiKhoan.Add(item);
            }
        }
    }
}

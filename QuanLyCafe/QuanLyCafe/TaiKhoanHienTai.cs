using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class TaiKhoanHienTai
    {
        static string _userName = null;
        static int _IDCaLamHienTai = -1;
        static DateTime _thoiGianVaoCaLamHienTai;
        static TaiKhoan _taiKhoan = null;
        public static string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public static int IDCaLamHienTai
        {
            get { return _IDCaLamHienTai; }
            set { _IDCaLamHienTai = value; }
        }
        public static DateTime ThoiGianVaoCaLamHienTai
        {
            get { return _thoiGianVaoCaLamHienTai; }
            set { _thoiGianVaoCaLamHienTai = value; }
        }
        public static TaiKhoan TaiKhoanHienHanh
        {
            get { return _taiKhoan; }
            set { _taiKhoan = value; }
        }
    }
}

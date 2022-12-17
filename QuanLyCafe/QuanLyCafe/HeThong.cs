using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class HeThong
    {
        static int _id;
        static string _tenCuaHang;
        static string _diaChiCuaHang;
        static int _luongPartTime;
        public static int ID
        {
            get { return _id; }
        }
        public static string TenCuaHang
        {
            get { return _tenCuaHang; }
            set { _tenCuaHang = value; }
        }
        public static string DiaChiCuaHang
        {
            get { return _diaChiCuaHang; }
            set { _diaChiCuaHang = value; }
        }
        public static int LuongPartTime
        {
            get { return _luongPartTime; }
            set { _luongPartTime = value; }
        }

        public static void SetHeThong(
            int id,
            string tenCuaHang,
            string diaChiCuaHang,
            int luongPartTime
        )
        {
            _id = id;
            _tenCuaHang = tenCuaHang;
            _diaChiCuaHang = diaChiCuaHang;
            _luongPartTime = luongPartTime;
        }
    }
}

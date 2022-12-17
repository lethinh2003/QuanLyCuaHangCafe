using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class DanhSachHoaDon
    {
        static List<HoaDon> _danhSachHoaDon = new List<HoaDon>();
        public static List<HoaDon> ListHoaDon
        {
            get { return _danhSachHoaDon; }
        }

        public static HoaDon TimKiem(int ID)
        {
            HoaDon ketQua = null;
            foreach (var item in _danhSachHoaDon)
            {
                if (item.ID == ID)
                {
                    ketQua = item;
                    break;
                }
            }
            return ketQua;
        }

        static bool KiemTraTonTai(HoaDon item)
        {
            bool check = false;
            foreach (var x in _danhSachHoaDon)
            {
                if (x.ID == item.ID)
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        public static void Add(HoaDon item)
        {
            if (!KiemTraTonTai(item))
            {
                _danhSachHoaDon.Add(item);
            }
        }
    }
}

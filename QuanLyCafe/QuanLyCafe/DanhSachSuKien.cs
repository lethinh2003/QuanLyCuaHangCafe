using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class DanhSachSuKien
    {
        static List<SuKien> _danhSachSuKien = new List<SuKien>();
        public static List<SuKien> ListSuKien
        {
            get { return _danhSachSuKien; }
        }

        public static SuKien TimKiem(int ID)
        {
            SuKien ketQua = null;
            foreach (var item in _danhSachSuKien)
            {
                if (item.ID == ID)
                {
                    ketQua = item;
                    break;
                }
            }
            return ketQua;
        }

        static bool KiemTraTonTai(SuKien item)
        {
            bool check = false;
            foreach (var x in _danhSachSuKien)
            {
                if (x.ID == item.ID)
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        public static void Add(SuKien item)
        {
            if (!KiemTraTonTai(item))
            {
                _danhSachSuKien.Add(item);
            }
        }
    }
}

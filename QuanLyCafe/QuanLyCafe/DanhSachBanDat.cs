using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class DanhSachBanDat
    {
        static List<BanDat> _danhSachBanDat = new List<BanDat>();
        public static List<BanDat> ListBanDat
        {
            get { return _danhSachBanDat; }
        }

        public static BanDat TimKiem(int ID)
        {
            BanDat ketQua = null;
            foreach (var item in _danhSachBanDat)
            {
                if (item.ID == ID)
                {
                    ketQua = item;
                    break;
                }
            }
            return ketQua;
        }

        static bool KiemTraBanTonTai(BanDat item)
        {
            bool check = false;
            foreach (var x in _danhSachBanDat)
            {
                if (x.ID == item.ID)
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        public static void Add(BanDat item)
        {
            if (!KiemTraBanTonTai(item))
            {
                _danhSachBanDat.Add(item);
            }
        }
    }
}

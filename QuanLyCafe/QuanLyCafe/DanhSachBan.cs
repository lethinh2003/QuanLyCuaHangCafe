using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCafe
{
    internal class DanhSachBan
    {
        static List<Ban> _danhSachBan = new List<Ban>();
        public static List<Ban> ListBan
        {
            get { return _danhSachBan; }
        }

        static bool KiemTraBanTonTai(Ban item)
        {
            bool check = false;
            foreach (var x in _danhSachBan)
            {
                if (x.ID.ToUpper() == item.ID.ToUpper())
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        public static Ban TimKiemBan(string ID)
        {
            Ban ketQua = null;
            foreach (var item in _danhSachBan)
            {
                if (item.ID.ToUpper() == ID.ToUpper())
                {
                    ketQua = item;
                    break;
                }
            }
            return ketQua;
        }

        public static void AddBan(Ban item)
        {
            if (!KiemTraBanTonTai(item))
            {
                _danhSachBan.Add(item);
            }
        }
    }
}

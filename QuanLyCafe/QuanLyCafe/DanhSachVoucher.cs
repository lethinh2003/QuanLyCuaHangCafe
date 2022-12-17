using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class DanhSachVoucher
    {
        static List<Voucher> _danhSachVoucher = new List<Voucher>();
        public static List<Voucher> ListVoucher
        {
            get { return _danhSachVoucher; }
        }

        public static Voucher TimKiem(string ma)
        {
            Voucher ketQua = null;
            foreach (var item in _danhSachVoucher)
            {
                if (item.Ma.ToUpper() == ma.ToUpper())
                {
                    ketQua = item;
                    break;
                }
            }
            return ketQua;
        }

        static bool KiemTraTonTai(Voucher item)
        {
            bool check = false;
            foreach (var x in _danhSachVoucher)
            {
                if (x.Ma.ToUpper() == item.Ma.ToUpper())
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        public static void Add(Voucher item)
        {
            if (!KiemTraTonTai(item))
            {
                _danhSachVoucher.Add(item);
            }
        }
    }
}

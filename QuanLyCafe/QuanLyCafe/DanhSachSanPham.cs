using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QuanLyCafe
{
    internal class DanhSachSanPham
    {
        static List<SanPham> _danhSachSanPham = new List<SanPham>();
        public static List<SanPham> ListSanPham
        {
            get { return _danhSachSanPham; }
        }

        public static SanPham TimKiemSanPham(string ID)
        {
            SanPham ketQua = null;
            foreach (var item in _danhSachSanPham)
            {
                if (item.ID.ToUpper() == ID.ToUpper())
                {
                    ketQua = item;
                    break;
                }
            }
            return ketQua;
        }

        static bool KiemTraSanPhamTonTai(SanPham item)
        {
            bool check = false;
            foreach (var x in _danhSachSanPham)
            {
                if (x.ID.ToUpper() == item.ID.ToUpper())
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        public static Image GetImageByPath(string ID)
        {
            Image image = null;
            SanPham timSanPham = TimKiemSanPham(ID);
            if (timSanPham != null)
            {
                string imagePath = timSanPham.ImagePath;
                if (!File.Exists($@"{imagePath}"))
                {
                    image = null;
                }
                else
                {
                    image = Image.FromFile($@"{imagePath}");
                }
            }
            return image;
        }

        public static void AddSanPham(SanPham item)
        {
            if (!KiemTraSanPhamTonTai(item))
            {
                _danhSachSanPham.Add(item);
            }
        }
    }
}

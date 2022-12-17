using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class SanPham
    {
        string _loaiSanPham;
        string _id;
        string _tenSanPham;
        string _moTa;
        int _giaTien;
        SuKien _event = null;
        string _imagePath;
        bool _hienThiSuKien = false;
        public bool HienThiSuKien
        {
            get { return _hienThiSuKien; }
            set { _hienThiSuKien = value; }
        }

        public string ID
        {
            get { return _id; }
        }
        public string TenSanPham
        {
            get { return _tenSanPham; }
        }
        public string LoaiSanPham
        {
            get { return _loaiSanPham; }
        }
        public int GiaTien
        {
            get { return _giaTien; }
        }
        public SuKien Event
        {
            get { return _event; }
        }
        public string ImagePath
        {
            get { return _imagePath; }
        }

        public string MoTa
        {
            get { return _moTa; }
        }

        public SanPham(
            string id,
            string loaiSanPham,
            string moTa,
            string tenSanPham,
            int giaTien,
            SuKien suKien,
            string imagePath
        )
        {
            _id = id;
            _loaiSanPham = loaiSanPham;
            _moTa = moTa;
            _tenSanPham = tenSanPham;
            _giaTien = giaTien;
            _event = suKien;
            _imagePath = imagePath;
        }

        public void CapNhatSanPham(
            string loaiSanPham,
            string moTa,
            int giaTien,
            SuKien suKien,
            string imagePath
        )
        {
            _loaiSanPham = loaiSanPham;
            _moTa = moTa;
            _giaTien = giaTien;
            _event = suKien;
            _imagePath = imagePath;
        }
    }
}

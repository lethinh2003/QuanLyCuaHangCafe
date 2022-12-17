using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class SuKien
    {
        int _id;
        string _ten;
        string _moTa;
        int _giamGia;
        DateTime _thoiGianBatDau;
        DateTime _thoiGianKetThuc;
        public int ID
        {
            get { return _id; }
        }
        public string Ten
        {
            get { return _ten; }
            set { _ten = value; }
        }
        public string MoTa
        {
            get { return _moTa; }
            set { _moTa = value; }
        }
        public int GiamGia
        {
            get { return _giamGia; }
            set { _giamGia = value; }
        }
        public DateTime ThoiGianBatDau
        {
            get { return _thoiGianBatDau; }
            set { _thoiGianBatDau = value; }
        }
        public DateTime ThoiGianKetThuc
        {
            get { return _thoiGianKetThuc; }
            set { _thoiGianKetThuc = value; }
        }

        public SuKien(
            int id,
            string ten,
            string moTa,
            int giamGia,
            DateTime thoiGianBatDau,
            DateTime thoiGianKetThuc
        )
        {
            _id = id;
            _ten = ten;
            _moTa = moTa;
            _giamGia = giamGia;
            _thoiGianBatDau = thoiGianBatDau;
            _thoiGianKetThuc = thoiGianKetThuc;
        }
    }
}

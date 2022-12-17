using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class Voucher
    {
        string _ma;
        int _giamGia = 0;
        string _moTa;
        int _soLuong = 0;
        int _luotNhap = 0;
        public string Ma
        {
            get { return _ma; }
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
        public int SoLuong
        {
            get { return _soLuong; }
            set { _soLuong = value; }
        }
        public int LuotNhap
        {
            get { return _luotNhap; }
            set { _luotNhap = value; }
        }

        public Voucher(string ma, int giamGia, string moTa, int luotNhap, int soLuong)
        {
            _ma = ma;
            _giamGia = giamGia;
            _moTa = moTa;
            LuotNhap = luotNhap;
            SoLuong = soLuong;
        }
    }
}

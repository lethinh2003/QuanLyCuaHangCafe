using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class Ban
    {
        string _id;
        string _tenBan;
        int _tinhTrang;
        BanDat _banDat = null;
        HoaDon _hoaDon = null;
        public string ID
        {
            get { return _id; }
        }
        public string TenBan
        {
            get { return _tenBan; }
            set { _tenBan = value; }
        }
        public int TinhTrang
        {
            get { return _tinhTrang; }
            set { _tinhTrang = value; }
        }
        public BanDat BanDatHienTai
        {
            get { return _banDat; }
            set { _banDat = value; }
        }
        public HoaDon HoaDonHienTai
        {
            get { return _hoaDon; }
            set { _hoaDon = value; }
        }

        public Ban(string id, string tenBan, int tinhTrang)
        {
            _id = id;
            _tenBan = tenBan;
            _tinhTrang = tinhTrang;
        }
    }
}

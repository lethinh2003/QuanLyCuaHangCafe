using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class BanDat
    {
        int _id;
        Ban _ban;
        DateTime _thoiGianVaoBan;
        DateTime _thoiGianRaBan;
        public int ID
        {
            get { return _id; }
        }
        public Ban BanHienTai
        {
            get { return _ban; }
        }
        public DateTime ThoiGianVaoBan
        {
            get { return _thoiGianVaoBan; }
            set { _thoiGianVaoBan = value; }
        }
        public DateTime ThoiGianRaBan
        {
            get { return _thoiGianRaBan; }
            set { _thoiGianRaBan = value; }
        }

        public BanDat(int id, Ban ban, DateTime thoiGianVaoBan)
        {
            _id = id;
            _ban = ban;
            _thoiGianVaoBan = thoiGianVaoBan;
        }
    }
}

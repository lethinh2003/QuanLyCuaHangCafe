using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class HoaDon
    {
        int _id;
        int _idDatBan;
        Voucher _voucher = null;
        TaiKhoan _nhanVien = null;
        int _thanhTien = 0;
        int _thanhToan = 0;
        int _thanhTienGiamGia = 0;
        int _khachTra = 0;
        int _tienThua = 0;
        DateTime _thoiGianTao;
        DateTime _thoiGianThanhToan;
        public int ID
        {
            get { return _id; }
        }
        public int IDDatBan
        {
            get { return _idDatBan; }
        }
        public Voucher VoucherHoaDon
        {
            get { return _voucher; }
            set { _voucher = value; }
        }
        public TaiKhoan NhanVienHoaDon
        {
            get { return _nhanVien; }
        }
        public int ThanhTien
        {
            get { return _thanhTien; }
            set { _thanhTien = value; }
        }
        public int ThanhToan
        {
            get { return _thanhToan; }
            set { _thanhToan = value; }
        }
        public int ThanhTienGiamGia
        {
            get { return _thanhTienGiamGia; }
            set { _thanhTienGiamGia = value; }
        }
        public int TienKhachTra
        {
            get { return _khachTra; }
            set { _khachTra = value; }
        }
        public int TienThua
        {
            get { return _tienThua; }
            set { _tienThua = value; }
        }
        public DateTime ThoiGianTao
        {
            get { return _thoiGianTao; }
            set { _thoiGianTao = value; }
        }
        public DateTime ThoiGianThanhToan
        {
            get { return _thoiGianThanhToan; }
            set { _thoiGianThanhToan = value; }
        }

        public HoaDon(
            int id,
            int idDatBan,
            Voucher voucher,
            TaiKhoan nhanVien,
            int thanhTien,
            int thanhToan,
            int thanhTienGiamGia,
            int khachTra,
            int tienThua,
            DateTime thoiGianTao,
            DateTime thoiGianThanhToan
        )
        {
            _id = id;
            _idDatBan = idDatBan;
            _voucher = voucher;
            _nhanVien = nhanVien;
            _thanhTien = thanhTien;
            _thanhToan = thanhToan;
            _thanhTienGiamGia = thanhTienGiamGia;
            _khachTra = khachTra;
            _tienThua = tienThua;
            _thoiGianTao = thoiGianTao;
            _thoiGianThanhToan = thoiGianThanhToan;
        }
    }
}

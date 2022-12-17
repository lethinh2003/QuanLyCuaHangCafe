﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Util;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Enum.Material;
using ReaLTaiizor.Colors;
using System.IO;

namespace QuanLyCafe.DanhSachForm
{
    public partial class XuatHoaDonForm : MaterialForm
    {
        HoaDon _hoaDonHienTai = null;
        BanDat _banDatHienTai = null;

        public XuatHoaDonForm()
        {
            InitializeComponent();
            if (ControlForm.BanDangChon != null)
            {
                _hoaDonHienTai = ControlForm.BanDangChon.HoaDonHienTai;
            }
            else
            {
                _hoaDonHienTai = ControlForm.HoaDonHienTai;
            }
            GetBanDat();
            if (_hoaDonHienTai != null)
            {
                lblXuatHoaDon.Text = $"Xuất hóa đơn {_hoaDonHienTai.ID}";

                ppdXemTruocHoaDon.Document = pdcHoaDon;
                ppdXemTruocHoaDon.Show();
            }
        }

        void GetBanDat()
        {
            if (_hoaDonHienTai != null)
            {
                Database.CreateConnection();
                string sqlCommand;
                SqlCommand cmd;
                SqlDataReader rd;
                sqlCommand = $"select * from LICHSUDATBAN where ID = '{_hoaDonHienTai.IDDatBan}'";
                cmd = Database.CreateCommand(sqlCommand);
                rd = cmd.ExecuteReader();
                int idBanDat = -1;
                DateTime thoiGianVaoBan = new DateTime();
                DateTime thoiGianRaBan = new DateTime();
                string IDBan = null;
                while (rd.Read())
                {
                    idBanDat = (int)rd["ID"];
                    thoiGianVaoBan = (DateTime)rd["THOIGIANVAOBAN"];
                    thoiGianRaBan = (DateTime)rd["THOIGIANRABAN"];
                    IDBan = (string)rd["ID_BAN"];

                }
                rd.Close();
                if (idBanDat != -1)
                {
                    Ban timBan = DanhSachBan.TimKiemBan(IDBan);
                    BanDat banDatMoi = new BanDat(idBanDat, timBan, thoiGianVaoBan);
                    banDatMoi.ThoiGianRaBan = thoiGianRaBan;
                    _banDatHienTai = banDatMoi;

                }
            }
        }

        private void pdcHoaDon_PrintPage(
            object sender,
            System.Drawing.Printing.PrintPageEventArgs e
        )
        {
            try
            {
                // Create rectangle for drawing.
                float x = 239;
                float y = 10f;
                float width = 310f;
                float height = 60f;
                RectangleF drawRect = new RectangleF(x, y, width, height);

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString(
                    $"{HeThong.TenCuaHang}",
                    new Font("Arial", 20, FontStyle.Bold),
                    Brushes.Black,
                    drawRect,
                    sf
                );

                RectangleF drawRectDiaChi = new RectangleF(x - 15f, y + 20f, 350f, height);
                e.Graphics.DrawString(
                    $"{HeThong.DiaChiCuaHang}",
                    new Font("Arial", 12, FontStyle.Bold),
                    Brushes.Black,
                    drawRectDiaChi,
                    sf
                );

                RectangleF drawRectHoaDonID = new RectangleF(
                    x - 15f,
                    drawRectDiaChi.Top + 20f,
                    350f,
                    height
                );
                e.Graphics.DrawString(
                    $"Receipt No: {_hoaDonHienTai.ID}",
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonID,
                    sf
                );

                RectangleF drawRectHoaDonDate = new RectangleF(
                    x - 15f,
                    drawRectHoaDonID.Top + 20f,
                    350f,
                    height
                );
                e.Graphics.DrawString(
                    $"Date: {_hoaDonHienTai.ThoiGianTao}",
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonDate,
                    sf
                );

                RectangleF drawRectHoaDonCashier = new RectangleF(
                    x - 15f,
                    drawRectHoaDonDate.Top + 20f,
                    350f,
                    height
                );
                string fullNameNhanVien =
                    $"{_hoaDonHienTai.NhanVienHoaDon.FirstName} {_hoaDonHienTai.NhanVienHoaDon.LastName}";
                e.Graphics.DrawString(
                    $"Cashier: @{_hoaDonHienTai.NhanVienHoaDon.UserName}",
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonCashier,
                    sf
                );

                RectangleF drawRectHoaDonDescription = new RectangleF(
                    x - 15f,
                    drawRectHoaDonCashier.Top + 20f,
                    350f,
                    height
                );
                e.Graphics.DrawString(
                    $"Description:",
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonDescription,
                    sf
                );

                string gachNgang = "================================";
                RectangleF drawRectHoaDonGachNgang = new RectangleF(
                    x - 15f,
                    drawRectHoaDonDescription.Top + 20f,
                    350f,
                    height
                );
                e.Graphics.DrawString(
                    gachNgang,
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonGachNgang,
                    sf
                );

                Database.CreateConnection();
                string sqlCommand =
                    $"select B.TEN as TEN_SANPHAM_LS, A.ID as ID_LS, A.ID_DATBAN as ID_DATBAN_LS, A.ID_SANPHAM as ID_SANPHAM_LS, A.DONGIA as DONGIA_LS, A.DONGIAGIAM as DONGIAGIAM_LS, A.SOLUONG as SOLUONG_LS,  A.THOIGIAN as THOIGIAN_LS, A.THANHTIEN as THANHTIEN_LS from LICHSUORDER A, DANHSACHSANPHAM B where A.ID_SANPHAM = B.ID AND A.ID_DATBAN = '{_banDatHienTai.ID}' order by A.THOIGIAN desc";
                DataTable dt;
                dt = Database.SelectQuery(sqlCommand);

                string sanPham;
                float positionY = drawRectHoaDonGachNgang.Top + 40f;
                int tongTien = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    sanPham =
                        $"{dr["SOLUONG_LS"]} - {dr["TEN_SANPHAM_LS"]} - DONGIA: {string.Format("{0:#,##0}", double.Parse(dr["DONGIA_LS"].ToString()))} - GIAM: {string.Format("{0:#,##0}", double.Parse(dr["DONGIAGIAM_LS"].ToString()))} - THANHTIEN: {string.Format("{0:#,##0}", double.Parse(dr["THANHTIEN_LS"].ToString()))}";
                    e.Graphics.DrawString(
                        sanPham,
                        new Font("Arial", 12, FontStyle.Regular),
                        Brushes.Black,
                        new PointF(10f, positionY)
                    );
                    positionY = positionY + 20f;
                    tongTien += (int)dr["THANHTIEN_LS"];
                }
                drawRectHoaDonGachNgang = new RectangleF(x - 15f, positionY, 350f, height);
                e.Graphics.DrawString(
                    gachNgang,
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonGachNgang,
                    sf
                );

                RectangleF drawRectHoaDonOverview = new RectangleF(
                    x - 15f,
                    drawRectHoaDonGachNgang.Top + 20f,
                    350f,
                    height
                );
                string tongQuan =
                    $"{dt.Rows.Count} Item(s) (VAT included) {string.Format("{0:#,##0}", double.Parse(tongTien.ToString()))}";
                e.Graphics.DrawString(
                    tongQuan,
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonOverview,
                    sf
                );
                drawRectHoaDonOverview = new RectangleF(
                    x - 100f,
                    drawRectHoaDonOverview.Top + 20f,
                    550f,
                    height
                );
                tongQuan = $"DATE IN: {_banDatHienTai.ThoiGianVaoBan}";
                e.Graphics.DrawString(
                    tongQuan,
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonOverview,
                    sf
                );
                drawRectHoaDonOverview = new RectangleF(
                    x - 100f,
                    drawRectHoaDonOverview.Top + 20f,
                    550f,
                    height
                );
                tongQuan = $"DATE OUT: {_banDatHienTai.ThoiGianRaBan}";
                e.Graphics.DrawString(
                    tongQuan,
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonOverview,
                    sf
                );
                if (_hoaDonHienTai.VoucherHoaDon != null)
                {
                    drawRectHoaDonOverview = new RectangleF(
                        x - 100f,
                        drawRectHoaDonOverview.Top + 20f,
                        550f,
                        height
                    );
                    tongQuan =
                        $"VOUCHER: {_hoaDonHienTai.VoucherHoaDon.Ma} (DISCOUNT {_hoaDonHienTai.VoucherHoaDon.GiamGia}%) {string.Format("{0:#,##0}", double.Parse((tongTien - tongTien * _hoaDonHienTai.VoucherHoaDon.GiamGia / 100).ToString()))}";
                    e.Graphics.DrawString(
                        tongQuan,
                        new Font("Arial", 12, FontStyle.Regular),
                        Brushes.Black,
                        drawRectHoaDonOverview,
                        sf
                    );
                }
                drawRectHoaDonOverview = new RectangleF(
                    x - 15f,
                    drawRectHoaDonOverview.Top + 20f,
                    350f,
                    height
                );
                tongQuan =
                    $"CASH: {string.Format("{0:#,##0}", double.Parse(_hoaDonHienTai.TienKhachTra.ToString()))}";
                e.Graphics.DrawString(
                    tongQuan,
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonOverview,
                    sf
                );
                drawRectHoaDonOverview = new RectangleF(
                    x - 15f,
                    drawRectHoaDonOverview.Top + 20f,
                    350f,
                    height
                );
                tongQuan =
                    $"CHANGE: {string.Format("{0:#,##0}", double.Parse(_hoaDonHienTai.TienThua.ToString()))}";
                e.Graphics.DrawString(
                    tongQuan,
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonOverview,
                    sf
                );
                drawRectHoaDonOverview = new RectangleF(
                    x - 15f,
                    drawRectHoaDonOverview.Top + 20f,
                    350f,
                    height
                );
                tongQuan = $"Chi xuat hoa don trong ngay";
                e.Graphics.DrawString(
                    tongQuan,
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonOverview,
                    sf
                );
                drawRectHoaDonOverview = new RectangleF(
                    x - 15f,
                    drawRectHoaDonOverview.Top + 20f,
                    350f,
                    height
                );
                tongQuan = $"Xin cam on quy khach!";
                e.Graphics.DrawString(
                    tongQuan,
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    drawRectHoaDonOverview,
                    sf
                );
                this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}

using DAL;
using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GarageManagementSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public bool ThanhToan(int maPhieu)
        {
            conn.Open();

            string sql =
            @"UPDATE PhieuSuaChua
      SET TrangThai=N'Đã thanh toán'
      WHERE MaPhieu=@MaPhieu";

            SqlCommand cmd =
                new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue(
                "@MaPhieu", maPhieu);

            int kq = cmd.ExecuteNonQuery();

            conn.Close();

            return kq > 0;
        }

    }
}

namespace DTO
{
    public class PhuTungDTO
    {
        public int MaPhuTung { get; set; }
        public string TenPhuTung { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuongTon { get; set; }
    }

    public class ChiTietSuaChuaDTO
    {
        public int MaPhieu { get; set; }
        public int MaPhuTung { get; set; }
        public int SoLuong { get; set; }
        public decimal TienCong { get; set; }
    }

    public class PhieuSuaChuaDTO
    {
        public string BienSo { get; set; }
        public string NoiDungSua { get; set; }

        public decimal TongTien { get; set; }

        public string TrangThai { get; set; }

        public List<ChiTietSuaChuaDTO> ChiTiets
            = new List<ChiTietSuaChuaDTO>();
    }
}

namespace DAL
{
    public class DBConnection
    {
        protected SqlConnection conn =
            new SqlConnection(
                @"Data Source=.;Initial Catalog=GarageDB;
                Integrated Security=True");
    }
    
    
    public class XeDAL : DBConnection
    {
        public DataTable TimXe(string bienSo)
        {
            string sql =
            "SELECT * FROM Xe WHERE BienSo=@BienSo";

            SqlDataAdapter da =
                new SqlDataAdapter(sql, conn);

            da.SelectCommand.Parameters.AddWithValue(
                "@BienSo", bienSo);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }
    }

    public class PhuTungDAL : DBConnection
    {
        public bool Them(PhuTungDTO pt)
        {
            conn.Open();

            string sql =
                @"INSERT INTO PhuTung
                VALUES(@Ten,@Gia,@Ton)";

            SqlCommand cmd =
                new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue(
                "@Ten", pt.TenPhuTung);

            cmd.Parameters.AddWithValue(
                "@Gia", pt.DonGia);

            cmd.Parameters.AddWithValue(
                "@Ton", pt.SoLuongTon);

            int kq = cmd.ExecuteNonQuery();

            conn.Close();

            return kq > 0;
        }
    }

    public class PhieuSuaChuaDAL : DBConnection
    {
        public bool LapPhieuSuaChua(
            PhieuSuaChuaDTO phieu)
        {
            conn.Open();

            SqlTransaction trans =
                conn.BeginTransaction();

            try
            {
                int maPhieu = 0;

                string sqlPhieu =
                @"INSERT INTO PhieuSuaChua
                (
                     BienSo,
                     NoiDungSua,
                     TongTien,
                     TrangThai
                )

                VALUES
                (
                     @BienSo,
                     @NoiDungSua,
                     @TongTien,
                     @TrangThai
                );

                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd =
                    new SqlCommand(
                    sqlPhieu,
                    conn,
                    trans);

                cmd.Parameters.AddWithValue(
                    "@BienSo", phieu.BienSo);

                cmd.Parameters.AddWithValue(
                    "@NoiDungSua",
                    phieu.NoiDungSua);

                cmd.Parameters.AddWithValue(
                    "@TongTien",
                    phieu.TongTien);

                cmd.Parameters.AddWithValue(
                    "@TrangThai",
                    "Đang sửa");

                maPhieu =
                    Convert.ToInt32(
                    cmd.ExecuteScalar());

                foreach (var ct in phieu.ChiTiets)
                {
                    //--------------------
                    // Kiểm tra tồn kho
                    //--------------------

                    string sqlCheck =
                    @"SELECT SoLuongTon
                      FROM PhuTung
                      WHERE MaPhuTung=@MaPT";

                    SqlCommand cmdCheck =
                    new SqlCommand(
                    sqlCheck,
                    conn,
                    trans);

                    cmdCheck.Parameters.AddWithValue(
                        "@MaPT",
                        ct.MaPhuTung);

                    int tonKho =
                        Convert.ToInt32(
                        cmdCheck.ExecuteScalar());

                    if (tonKho < ct.SoLuong)
                    {
                        throw new Exception(
                            "Không đủ phụ tùng");
                    }

                    //------------------
                    // Chi tiết sửa chữa
                    //------------------

                    string sqlCT =
                    @"INSERT INTO
                    ChiTietSuaChua
                    VALUES
                    (
                    @MaPhieu,
                    @MaPT,
                    @SL,
                    @TienCong
                    )";

                    SqlCommand cmdCT =
                    new SqlCommand(
                    sqlCT,
                    conn,
                    trans);

                    cmdCT.Parameters.AddWithValue(
                        "@MaPhieu", maPhieu);

                    cmdCT.Parameters.AddWithValue(
                        "@MaPT",
                        ct.MaPhuTung);

                    cmdCT.Parameters.AddWithValue(
                        "@SL",
                        ct.SoLuong);

                    cmdCT.Parameters.AddWithValue(
                        "@TienCong",
                        ct.TienCong);

                    cmdCT.ExecuteNonQuery();

                    //--------------------
                    // Trừ tồn kho
                    //--------------------

                    string sqlUpdate =
                    @"UPDATE PhuTung
                    SET SoLuongTon=
                    SoLuongTon-@SL
                    WHERE MaPhuTung=@MaPT";

                    SqlCommand cmdUpdate =
                    new SqlCommand(
                    sqlUpdate,
                    conn,
                    trans);

                    cmdUpdate.Parameters.AddWithValue(
                        "@SL",
                        ct.SoLuong);

                    cmdUpdate.Parameters.AddWithValue(
                        "@MaPT",
                        ct.MaPhuTung);

                    cmdUpdate.ExecuteNonQuery();
                }

                trans.Commit();

                return true;
            }
            catch (Exception)
            {
                trans.Rollback();
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

namespace BLL
{
    public class PhieuSuaChuaBLL
    {
        PhieuSuaChuaDAL dal =
            new PhieuSuaChuaDAL();

        public bool LapPhieu(
            PhieuSuaChuaDTO phieu)
        {
            return dal.LapPhieuSuaChua(phieu);
        }
    }

    public class HoaDonBLL
    {
        public decimal TinhTien(
            List<ChiTietSuaChuaDTO> ds,
            List<PhuTungDTO> phuTungs)
        {
            decimal tong = 0;

            foreach (var ct in ds)
            {
                var pt =
                phuTungs.Find(x =>
                x.MaPhuTung == ct.MaPhuTung);

                tong +=
                    ct.SoLuong *
                    pt.DonGia
                    +
                    ct.TienCong;
            }

            return tong;
        }

        
    }
}

using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DTO;
using DAL;
using BUS;

namespace Project_TaskManagement
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
    }
}

namespace DTO
{
    public class CongViecDTO
    {
        public int MaTask { get; set; }

        public int MaDuAn { get; set; }

        public string TenTask { get; set; }

        public string MoTa { get; set; }

        public DateTime HanNop { get; set; }

        public string DoUuTien { get; set; }

        public string TrangThai { get; set; }
    }

    public class DuAnDTO
    {
        public int MaDuAn { get; set; }

        public string TenDuAn { get; set; }

        public DateTime NgayBatDau { get; set; }

        public DateTime NgayKetThuc { get; set; }

        public string TrangThai { get; set; }
    }
}

namespace DAL
{
    public class DataProvider
    {
        string connStr =
          @"Data Source=TRUNGHIEU\SQLEXPRESS;Initial Catalog=ProjectTaskDB;Integrated Security=True";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connStr);
        }
    }

    public DataTable GetAllTask()
        {
            SqlConnection conn =
        new DataProvider().GetConnection();

            string sql = "SELECT * FROM CongViec";

            SqlDataAdapter da =
                new SqlDataAdapter(sql, conn);

            DataTable dt = new DataTable();

            da.Fill(dt);

            return dt;
        }

        public bool InsertTask(CongViecDTO task)
        {
            SqlConnection conn =
                new DataProvider().GetConnection();

            string sql =
            @"INSERT INTO CongViec
      (MaDuAn,TenTask,MoTa,HanNop,DoUuTien,TrangThai)
      VALUES
      (@MaDuAn,@TenTask,@MoTa,@HanNop,@DoUuTien,@TrangThai)";

            SqlCommand cmd =
                new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@MaDuAn", task.MaDuAn);
            cmd.Parameters.AddWithValue("@TenTask", task.TenTask);
            cmd.Parameters.AddWithValue("@MoTa", task.MoTa);
            cmd.Parameters.AddWithValue("@HanNop", task.HanNop);
            cmd.Parameters.AddWithValue("@DoUuTien", task.DoUuTien);
            cmd.Parameters.AddWithValue("@TrangThai", task.TrangThai);

            conn.Open();

            int kq = cmd.ExecuteNonQuery();

            conn.Close();

            return kq > 0;
        }

        public bool UpdateStatus(int maTask,
                         string trangThai)
        {
            SqlConnection conn =
                new DataProvider().GetConnection();

            string sql =
            @"UPDATE CongViec
      SET TrangThai=@TrangThai
      WHERE MaTask=@MaTask";

            SqlCommand cmd =
                new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@TrangThai",
                                        trangThai);

            cmd.Parameters.AddWithValue("@MaTask",
                                        maTask);

            conn.Open();

            int kq = cmd.ExecuteNonQuery();

            conn.Close();

            return kq > 0;
        }

        public DataTable GetTaskByStatus(string status)
        {
            SqlConnection conn =
                new DataProvider().GetConnection();

            string sql =
            @"SELECT *
      FROM CongViec
      WHERE TrangThai=@Status";

            SqlDataAdapter da =
               new SqlDataAdapter(sql, conn);

            da.SelectCommand.Parameters
              .AddWithValue("@Status", status);

            DataTable dt = new DataTable();

            da.Fill(dt);

            return dt;
        }

        public DataTable GetTaskByProject(int maDuAn)
        {
            SqlConnection conn =
                new DataProvider().GetConnection();

            string sql =
              @"SELECT * FROM CongViec
        WHERE MaDuAn=@MaDuAn";

            SqlDataAdapter da =
                new SqlDataAdapter(sql, conn);

            da.SelectCommand.Parameters
              .AddWithValue("@MaDuAn", maDuAn);

            DataTable dt = new DataTable();

            da.Fill(dt);

            return dt;
        }

    }

namespace BUS
{
    public class CongViecBUS
    {
        CongViecDAL dal =
            new CongViecDAL();

        public DataTable GetAllTask()
        {
            return dal.GetAllTask();
        }

        public DataTable GetTaskByStatus(string status)
        {
            return dal.GetTaskByStatus(status);
        }

        public bool InsertTask(CongViecDTO task)
        {
            if (string.IsNullOrWhiteSpace(task.TenTask))
                return false;

            return dal.InsertTask(task);
        }

        public bool UpdateStatus(int maTask,
                                 string status)
        {
            return dal.UpdateStatus(maTask, status);
        }
    }
}



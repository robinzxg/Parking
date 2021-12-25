using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ParkingSystemDAL
{

    /// <summary>
    /// 通用数据访问类
    /// </summary>
    public class SQLHelper
    {
        //封装链接字符串
        public static string connString = ConfigurationManager.ConnectionStrings["connString"].ToString();
        public static string connect = ConfigurationManager.AppSettings["connect"].ToString();
        public static int ExecuteNonQuery(string sql, SqlParameter[] param = null)
        {           
                //创建连接对象
                SqlConnection conn = new SqlConnection(connString);
                //创建一个命令执行对象
                SqlCommand cmd = new SqlCommand(sql, conn);

                if (param != null)
                {
                    cmd.Parameters.AddRange(param);
                }
                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    WriteLog("Database connection exception!");
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
         
        }

        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static SqlDataReader GetReader(string sql)
        {           
                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception ex)
                {
                    //可以在这个地方捕获ex对象相关信息，然后保存到日志文件中...

                    WriteLog("执行 public static SqlDataReader GetReader(string sql)  erro：" + ex.Message);
                    throw ex;
                }          
        }

        #region 错误信息写入日志
        /// <summary>
        /// 将错误信息写入日志文件
        /// </summary>
        /// <param name="msg"></param>
        private static void WriteLog(string msg)
        {
            FileStream fs = new FileStream("Log.text", FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("[{0}]  erro：{1}", DateTime.Now.ToString(), msg);
            sw.Close();
            fs.Close();
        }
        #endregion
    }
}

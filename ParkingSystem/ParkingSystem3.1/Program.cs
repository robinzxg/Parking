using ParkingSystemDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParkingSystem3._1
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //设置连接sqlite数据库
            SQLiteHelper.ConStr = "Data Source=" + Application.StartupPath + "\\DataBase\\ParkingSystemDataBase;Pooling=true;FailIfMissing=false";
            Application.Run(new FrmMain());
        }
    }
}

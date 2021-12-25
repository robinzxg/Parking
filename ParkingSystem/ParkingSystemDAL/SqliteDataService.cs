using ParkingSys.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSystemDAL
{
    public class SqliteDataService
    {
        #region 插入数据     
        /// <summary>
        /// 插入检测信息数据
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public bool  AddSensorData(Sensor sensor)
        {
            //定义sql语句，解析实体数据
            string sql = "insert into SensorData(SensorID,SensorState,DateofBirth,RunTime) values(@SensorID,@SensorState,@DateofBirth,@RunTime)";           
            //封装SQL语句中的参数
            SQLiteParameter[] param = new SQLiteParameter[]
                {
                    new SQLiteParameter("@SensorID",sensor.SensorId),
                    new SQLiteParameter("@SensorState",sensor.SensorState),
                    new SQLiteParameter("@DateofBirth",sensor.DateTimeRec),
                    new SQLiteParameter("@RunTime",sensor.SensorRunTime)
                };
            return SQLiteHelper.Update(sql, param)==1;
        }

        /// <summary>
        /// 插入停车场车位数量信息
        /// </summary>
        /// <param name="parkingInfo"></param>
        /// <returns></returns>
        public int AddParking(ParkingInfo parkingInfo)
        {
            string sql = "insert into Parking(ParkingSum,ParkingOccupy,Parkingvacant,DateofBirth) values (@ParkingSum,@ParkingOccupy,@Parkingvacant,@DateofBirth)";           
            SQLiteParameter[] param = new SQLiteParameter[]
                {
                    new SQLiteParameter("@ParkingSum",parkingInfo.ParkingSum),
                    new SQLiteParameter("@ParkingOccupy",parkingInfo.ParkingOccupy),
                    new SQLiteParameter("@Parkingvacant",parkingInfo.ParkingVacant),
                    new SQLiteParameter("@DateofBirth",parkingInfo.DateTimeUpdate)
                };
            return SQLiteHelper.Update(sql, param);
        }

        /// <summary>
        /// 插入节点编号
        /// </summary>
        /// <param name="sensorList"></param>
        /// <returns></returns>
        public int AddSensorList(SensorList sensorList)
        {
            //string sql = "truncate table SensorList";//保存数据库之前先清空数据表
            string sql = " insert into SensorList(SensorNum,SensorID,DateofBirth)";
            sql += " values (@SensorNum,@SensorID,@DateofBirth)";
            SQLiteParameter[] param = new SQLiteParameter[]
                {
                    new SQLiteParameter("@SensorNum",sensorList.SensorNum),
                    new SQLiteParameter("@SensorID",sensorList.SensorID),
                     new SQLiteParameter("@DateofBirth",sensorList.AddDateTime)
                };
            return SQLiteHelper.Update(sql, param);
        }

        /// <summary>
        /// 添加实时数据（心跳 检测等）
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public int AddHeartData(Sensor sensor)
        {         
            string sql = "replace into SensorHeartData(SensorID,SensorSerial,SensorState,SensorModel,SensorHB,SensorRssi,SensorVoltage,SensorSoftver,SensorHardver,SensorWdcId,DateofBirth) values(@SensorID,@SensorSerial,@SensorState,@SensorModel,@SensorHB,@SensorRssi,@SensorVoltage,@SensorSoftver,@SensorHardver,@SensorWdcId,@DateofBirth)";

            SQLiteParameter[] param = new SQLiteParameter[]
                {
                    new SQLiteParameter("@SensorID",sensor.SensorId),
                    new SQLiteParameter("@SensorSerial",sensor.SensorSerial),
                    new SQLiteParameter("@SensorState",sensor.SensorState),
                    new SQLiteParameter("@SensorModel",sensor.SensorModel),
                    new SQLiteParameter("@SensorHB",sensor.SensorHB),
                    new SQLiteParameter("@SensorRssi",sensor.SensorRssi),
                    new SQLiteParameter("@SensorVoltage",sensor.SensorVoltage),
                    new SQLiteParameter("@SensorSoftver",sensor.SensorSoftver),
                    new SQLiteParameter("@SensorHardver",sensor.SensorHardver),
                    new SQLiteParameter("@SensorWdcId",sensor.SensorWdcId),
                    new SQLiteParameter("@DateofBirth",sensor.DateTimeRec)
                };
            return SQLiteHelper.Update(sql, param);
        }
        #endregion

        #region 根据条件动态查询数据
        /// <summary>
        /// 查询检测数据
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public List<DetectInfo> queryDetectInfo(string sensorId, string dateTime1, string dateTime2)
        {
            //定义sql语句和组合条件
            string sql = $"select*from SensorData where DateofBirth between '{dateTime1}' and '{dateTime2}'";
            if (sensorId != "")
            {
                sql += $" and SensorID like '{sensorId}%'";
            }

            //执行查询
            SQLiteDataReader reader = SQLiteHelper.GetReader(sql);
            List<DetectInfo> detectInfos = new List<DetectInfo>();
            while (reader.Read())
            {
                detectInfos.Add(new DetectInfo
                {
                    SensorID = reader["SensorID"].ToString(),
                    SensorState = reader["SensorState"].ToString(),
                    DataTimeRec = (DateTime)reader["DateofBirth"],
                    RunTime=  reader["RunTime"].ToString()
                });
            }
            reader.Close();
            return detectInfos;

        }

        /// <summary>
        /// 查询实时数据
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public List<Sensor> queSensor(string sensorId)
        {
            //定义sql语句和组合条件
            string sql = "select*from SensorHeartData";
            sql += " inner join SensorList on SensorList.SensorID=SensorHeartData.SensorID ";
            if (sensorId != "")
            {
                sql += $" where SensorID like '{sensorId}%'";
            }
            //执行查询
            SQLiteDataReader reader = SQLiteHelper.GetReader(sql);
            List<Sensor> queSensor = new List<Sensor>();
            while (reader.Read())
            {
                queSensor.Add(new Sensor
                {
                    SensorId = reader["SensorID"].ToString(),
                    SensorSerial = Convert.ToByte(reader["SensorSerial"]),
                    SensorState = reader["SensorState"].ToString(),
                    SensorModel = reader["SensorModel"].ToString(),
                    SensorHB = reader["SensorHB"].ToString(),
                    SensorRssi = reader["SensorRssi"].ToString(),
                    SensorVoltage = reader["SensorVoltage"].ToString(),
                    SensorSoftver = reader["SensorSoftver"].ToString(),
                    SensorHardver = reader["SensorHardver"].ToString(),
                    SensorWdcId = reader["SensorWdcId"].ToString(),
                    DateTimeRec = (DateTime)reader["DateofBirth"],
                    SensorNum = (int)reader["SensorNum"]
                });
            }
            reader.Close();
            return queSensor;
        }

        /// <summary>
        /// 查询停车位数量信息
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public List<ParkingInfo> queryParkingInfo(string dateTime1, string dateTime2)
        {
            //定义sql语句和组合条件
            string sql = $"select*from Parking where DateofBirth between '{dateTime1}' and '{dateTime2}'";

            //执行查询
            SQLiteDataReader reader = SQLiteHelper.GetReader(sql);
            List<ParkingInfo> parkingInfo = new List<ParkingInfo>();
            while (reader.Read())
            {
                parkingInfo.Add(new ParkingInfo
                {
                    ParkingSum = int.Parse(reader["ParkingSum"].ToString()),
                    ParkingOccupy = int.Parse(reader["ParkingOccupy"].ToString()),
                    ParkingVacant = int.Parse(reader["parkingVacant"].ToString()),
                    DateTimeUpdate = (DateTime)reader["DateofBirth"]
                });
            }
            reader.Close();
            return parkingInfo;
        }

        /// <summary>
        /// 查询所有节点和节点对应的编号的最新的一条数据
        /// </summary>
        /// <returns></returns>
        public List<Sensor> queryNewSensor()
        {
            string sql = "select * from SensorHeartData";
            sql += " inner join SensorList on SensorList.SensorID=SensorHeartData.SensorID ";
            //sql += " where SensorHeartData.DateofBirth in (select max(DateofBirth) from SensorHeartData group by SensorID)";
            sql += " where SensorList.DateofBirth in (select MAX(DateofBirth)from SensorList group by SensorID)";
            //执行查询
            SQLiteDataReader reader = SQLiteHelper.GetReader(sql);
            List<Sensor> newSensor = new List<Sensor>();
            while (reader.Read())
            {
                newSensor.Add(new Sensor
                {
                    SensorId = reader["SensorID"].ToString(),
                    SensorSerial = Convert.ToByte(reader["SensorSerial"]),
                    SensorState = reader["SensorState"].ToString(),
                    SensorModel = reader["SensorModel"].ToString(),
                    SensorHB = reader["SensorHB"].ToString(),
                    SensorRssi = reader["SensorRssi"].ToString(),
                    SensorVoltage = reader["SensorVoltage"].ToString(),
                    SensorSoftver = reader["SensorSoftver"].ToString(),
                    SensorHardver = reader["SensorHardver"].ToString(),
                    SensorWdcId = reader["SensorWdcId"].ToString(),
                    DateTimeRec = (DateTime)reader["DateofBirth"],
                    SensorNum = int.Parse(reader["SensorNum"].ToString())
                });
            }
            reader.Close();
            return newSensor;
        }

        /// <summary>
        /// 查询节点的信息，判断是否有该ID节点
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public int QueSen(Sensor sensor)
        {
            string sql = $"select * from SensorHeartData where  SensorID like '{sensor.SensorId}%'";
            SQLiteDataReader reader = SQLiteHelper.GetReader(sql);
            List<Sensor> newSensor = new List<Sensor>();
            while (reader.Read())
            {
                newSensor.Add(new Sensor
                {
                    SensorId = reader["SensorID"].ToString(),
                    SensorSerial = Convert.ToByte(reader["SensorSerial"]),
                    SensorState = reader["SensorState"].ToString(),
                    SensorModel = reader["SensorModel"].ToString(),
                    SensorHB = reader["SensorHB"].ToString(),
                    SensorRssi = reader["SensorRssi"].ToString(),
                    SensorVoltage = reader["SensorVoltage"].ToString(),
                    SensorSoftver = reader["SensorSoftver"].ToString(),
                    SensorHardver = reader["SensorHardver"].ToString(),
                    SensorWdcId = reader["SensorWdcId"].ToString(),
                    DateTimeRec = (DateTime)reader["DateofBirth"],
                    SensorNum = int.Parse(reader["SensorNum"].ToString())
                });
            }
            reader.Close();
            return newSensor.Count();
        }


        /// <summary>
        /// 查询节点对应车位号
        /// </summary>
        /// <returns></returns>
        public List<SensorList> QuesensorLists(string sensorId)
        {
            string sql = "select*from SensorList";
            if (sensorId != "")
            {
                sql += $" where SensorID like '{sensorId}%'";
            }
            SQLiteDataReader reader = SQLiteHelper.GetReader(sql);
            List<SensorList> sensorLists = new List<SensorList>();
            while (reader.Read())
            {
                sensorLists.Add(new SensorList
                {
                    SensorID = reader["SensorID"].ToString(),
                    SensorNum = int.Parse(reader["SensorNum"].ToString()),
                    AddDateTime = (DateTime)reader["DateofBirth"]
                });
            }
            reader.Close();
            return sensorLists;
        }
        #endregion

        #region 更新节点对应车位号
        public int UpdateSensorList(string sensorID, int sensorNum)
        {
            string sql = $"update SensorList set SensorNum='{sensorNum}' where SensorID like '{sensorID}%'";
            SQLiteParameter[] param = new SQLiteParameter[]
              {
                    new SQLiteParameter("@SensorNum",sensorNum),
                    new SQLiteParameter("@SensorID",sensorID),
              };
            return SQLiteHelper.Update(sql, param);
        }

        /// <summary>
        /// 更新实时数据
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public int UpdateSensor(Sensor sensor)
        {
            string sql = $"update SensorHeartData set SensorID=@SensorID,SensorSerial=@SensorSerial,SensorState=@SensorState,SensorModel=@SensorModel,SensorHB=@SensorHB,SensorRssi=@SensorRssi,SensorVoltage=@SensorVoltage,SensorSoftver=@SensorSoftver,SensorHardver=@SensorHardver,SensorWdcId=@SensorWdcId,DateofBirth=@DateofBirth";
            sql += " where SensorID=@SensorID";
            SQLiteParameter[] param = new SQLiteParameter[]
               {
                    new SQLiteParameter("@SensorID",sensor.SensorId),
                    new SQLiteParameter("@SensorSerial",sensor.SensorSerial),
                    new SQLiteParameter("@SensorState",sensor.SensorState),
                    new SQLiteParameter("@SensorModel",sensor.SensorModel),
                    new SQLiteParameter("@SensorHB",sensor.SensorHB),
                    new SQLiteParameter("@SensorRssi",sensor.SensorRssi),
                    new SQLiteParameter("@SensorVoltage",sensor.SensorVoltage),
                    new SQLiteParameter("@SensorSoftver",sensor.SensorSoftver),
                    new SQLiteParameter("@SensorHardver",sensor.SensorHardver),
                    new SQLiteParameter("@SensorWdcId",sensor.SensorWdcId),
                    new SQLiteParameter("@DateofBirth",sensor.DateTimeRec)
               };
            return SQLiteHelper.Update(sql, param);
        }
        #endregion
    }
}

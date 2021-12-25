using ParkingSys.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSystemDAL
{
    public class SensorDataService
    {
        #region 添加数据
        /// <summary>
        /// 添加检测数据
        /// </summary>
        /// <returns></returns>
        public int AddSensorData(Sensor sensor)
        {
            //定义sql语句，解析实体数据
            string sql = "insert into SensorData(SensorID,SensorState,DateofBirth)";
            sql += "values(@SensorID,@SensorState,@DateofBirth)";
            //封装SQL语句中的参数
            SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@SensorID",sensor.SensorId),
                    new SqlParameter("@SensorState",sensor.SensorState),
                    new SqlParameter("@DateofBirth",sensor.DateTimeRec)
                };
            return SQLHelper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 添加实时数据（心跳 检测等）
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public int AddHeartData(Sensor sensor)
        {
            string sql = $"if not exists(select '{sensor.SensorId}'from SensorHeartData where SensorID = '{sensor.SensorId}%')";
             sql += " insert into SensorHeartData(SensorID,SensorSerial,SensorState,SensorModel,SensorHB,SensorRssi,SensorVoltage,SensorSoftver,SensorHardver,SensorWdcId,DateofBirth)";
            sql += " values(@SensorID,@SensorSerial,@SensorState,@SensorModel,@SensorHB,@SensorRssi,@SensorVoltage,@SensorSoftver,@SensorHardver,@SensorWdcId,@DateofBirth)";
            sql+= $" else update SensorHeartData set SensorID = @SensorID, SensorSerial = @SensorSerial, SensorState = @SensorState, SensorModel = @SensorModel, SensorHB = @SensorHB, SensorRssi = @SensorRssi, SensorVoltage = @SensorVoltage, SensorSoftver = @SensorSoftver, SensorHardver = @SensorHardver, SensorWdcId = @SensorWdcId, DateofBirth = @DateofBirth";
            sql += " where SensorID=@SensorID"; 

            SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@SensorID",sensor.SensorId),
                    new SqlParameter("@SensorSerial",sensor.SensorSerial),
                    new SqlParameter("@SensorState",sensor.SensorState),
                    new SqlParameter("@SensorModel",sensor.SensorModel),
                    new SqlParameter("@SensorHB",sensor.SensorHB),
                    new SqlParameter("@SensorRssi",sensor.SensorRssi),
                    new SqlParameter("@SensorVoltage",sensor.SensorVoltage),
                    new SqlParameter("@SensorSoftver",sensor.SensorSoftver),
                    new SqlParameter("@SensorHardver",sensor.SensorHardver),
                    new SqlParameter("@SensorWdcId",sensor.SensorWdcId),
                    new SqlParameter("@DateofBirth",sensor.DateTimeRec)
                };
            return SQLHelper.ExecuteNonQuery(sql, param);
        }
        /// <summary>
        /// 插入停车场车位信息
        /// </summary>
        /// <param name="parkingInfo"></param>
        /// <returns></returns>
        public int AddParking(ParkingInfo parkingInfo)
        {
            string sql = "insert into Parking(ParkingSum,ParkingOccupy,Parkingvacant,DateofBirth)";
            sql += "values (@ParkingSum,@ParkingOccupy,@Parkingvacant,@DateofBirth)";
            SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@ParkingSum",parkingInfo.ParkingSum),
                    new SqlParameter("@ParkingOccupy",parkingInfo.ParkingOccupy),
                    new SqlParameter("@Parkingvacant",parkingInfo.ParkingVacant),
                    new SqlParameter("@DateofBirth",parkingInfo.DateTimeUpdate)
                };
            return SQLHelper.ExecuteNonQuery(sql, param);
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
            SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@SensorNum",sensorList.SensorNum),
                    new SqlParameter("@SensorID",sensorList.SensorID),
                     new SqlParameter("@DateofBirth",sensorList.AddDateTime)
                };
            return SQLHelper.ExecuteNonQuery(sql, param);
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
            SqlDataReader reader = SQLHelper.GetReader(sql);
            List<DetectInfo> detectInfos = new List<DetectInfo>();
            while (reader.Read())
            {
                detectInfos.Add(new DetectInfo
                {
                    SensorID = reader["SensorID"].ToString(),
                    SensorState = reader["SensorState"].ToString(),
                    DataTimeRec = (DateTime)reader["DateofBirth"]
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
            SqlDataReader reader = SQLHelper.GetReader(sql);
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
            SqlDataReader reader = SQLHelper.GetReader(sql);
            List<ParkingInfo> parkingInfo = new List<ParkingInfo>();
            while (reader.Read())
            {
                parkingInfo.Add(new ParkingInfo
                {
                    ParkingSum = (int)reader["ParkingSum"],
                    ParkingOccupy = (int)reader["ParkingOccupy"],
                    ParkingVacant = (int)reader["parkingVacant"],
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
            SqlDataReader reader = SQLHelper.GetReader(sql);
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
                    SensorNum = (int)reader["SensorNum"]
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
            SqlDataReader reader = SQLHelper.GetReader(sql);
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
                    SensorNum = (int)reader["SensorNum"]
                });
            }
            reader.Close();
            return newSensor.Count();
        }


        /// <summary>
        /// 查询节点对应车位号
        /// </summary>
        /// <returns></returns>
        public List<SensorList>QuesensorLists(string sensorId)
        {
            string sql = "select*from SensorList";
            if (sensorId != "")
            {
                sql += $" where SensorID like '{sensorId}%'";
            }
            SqlDataReader reader = SQLHelper.GetReader(sql);
            List<SensorList> sensorLists = new List<SensorList>();
            while (reader.Read())
            {
                sensorLists.Add(new SensorList
                {
                   SensorID= reader["SensorID"].ToString(),
                   SensorNum =int.Parse( reader["SensorNum"].ToString()),
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
            SqlParameter[] param = new SqlParameter[]
              {
                    new SqlParameter("@SensorNum",sensorNum),
                    new SqlParameter("@SensorID",sensorID),
              };
            return SQLHelper.ExecuteNonQuery(sql, param);
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
            SqlParameter[] param = new SqlParameter[]
               {
                    new SqlParameter("@SensorID",sensor.SensorId),
                    new SqlParameter("@SensorSerial",sensor.SensorSerial),
                    new SqlParameter("@SensorState",sensor.SensorState),
                    new SqlParameter("@SensorModel",sensor.SensorModel),
                    new SqlParameter("@SensorHB",sensor.SensorHB),
                    new SqlParameter("@SensorRssi",sensor.SensorRssi),
                    new SqlParameter("@SensorVoltage",sensor.SensorVoltage),
                    new SqlParameter("@SensorSoftver",sensor.SensorSoftver),
                    new SqlParameter("@SensorHardver",sensor.SensorHardver),
                    new SqlParameter("@SensorWdcId",sensor.SensorWdcId),
                    new SqlParameter("@DateofBirth",sensor.DateTimeRec)
               };
            return SQLHelper.ExecuteNonQuery(sql, param);
        }
        #endregion
    }
}

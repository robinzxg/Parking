using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParkingSystemDAL;
using ParkingSys.Models;

namespace ParkingSystem.BLL
{
    public class SensorData
    {
        private SensorDataService sensorDataSerice = new SensorDataService();
        private SqliteDataService  SqliteDataService = new SqliteDataService();

        /// <summary>
        /// 插入检测数据
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public bool AddSensorData(Sensor sensor)
        {
            //return sensorDataSerice.AddSensorData(sensor);
            return SqliteDataService.AddSensorData(sensor);
        }

        /// <summary>
        /// 插入实时数据
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public int AddSensorHeartData(Sensor sensor)
        {
            return SqliteDataService.AddHeartData(sensor);
        }

        /// <summary>
        /// 插入停车场数据
        /// </summary>
        /// <param name="parking"></param>
        /// <returns></returns>
        public int AddParkingData(ParkingInfo parkingInfo)
        {
            return SqliteDataService.AddParking(parkingInfo);
        }
        /// <summary>
        /// 插入节点编号信息
        /// </summary>
        /// <param name="sensorList"></param>
        /// <returns></returns>
        public int AddSensorList(SensorList sensorList)
        {
            return SqliteDataService.AddSensorList(sensorList);
        }

        /// <summary>
        /// 查询节点信息
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public int QueSen(Sensor sensor)
        {
            //return sensorDataSerice.QueSen(sensor);
            return SqliteDataService.QueSen(sensor);

        }


        /// <summary>
        /// 查询检测信息
        /// </summary>
        /// <param name="sensorID"></param>
        /// <returns></returns>
        public List<DetectInfo> QueryDetect(string sensorID, string dateTime1, string dateTime2)
        {
            // return sensorDataSerice.queryDetectInfo(sensorID, dateTime1, dateTime2);
            return SqliteDataService.queryDetectInfo(sensorID, dateTime1, dateTime2);
        }

        /// <summary>
        /// 查询停车场车位信息
        /// </summary>
        /// <returns></returns>
        public List<ParkingInfo> QueryParking(string dateTime1, string dateTime2)
        {
            //return sensorDataSerice.queryParkingInfo(dateTime1,dateTime2);
            return SqliteDataService.queryParkingInfo(dateTime1, dateTime2);
        }

        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <param name="sensorID"></param>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public List<Sensor> QueryData(string sensorID)
        {
           //return sensorDataSerice.queSensor(sensorID);
            return SqliteDataService.queSensor(sensorID);
        }

        /// <summary>
        /// 查询全部节点的最新一条数据
        /// </summary>
        /// <returns></returns>
        public List<Sensor>QueryNewSensor()
        {
            //return sensorDataSerice.queryNewSensor();
            return SqliteDataService.queryNewSensor();
        }

        /// <summary>
        /// 查询节点对应编号
        /// </summary>
        /// <returns></returns>
        public List<SensorList> SensorList(string sensorId)
        {
            //return sensorDataSerice.QuesensorLists(sensorId);
            return SqliteDataService.QuesensorLists(sensorId);
        }

        /// <summary>
        /// 更新单个传感器编号信息
        /// </summary>
        /// <param name="sensorID"></param>
        /// <param name="sensorNum"></param>
        /// <returns></returns>
        public int UpdateSensorList(string sensorID, int sensorNum)
        {
            return SqliteDataService.UpdateSensorList(sensorID, sensorNum);
        }

        /// <summary>
        /// 更新节点对应的实时数据
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public int UpdateSensor(Sensor sensor )
        {
            return SqliteDataService.UpdateSensor(sensor);
        }
    }
}

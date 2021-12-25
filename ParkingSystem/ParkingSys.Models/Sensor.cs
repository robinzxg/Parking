using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSys.Models
{
    public class Sensor
    {
        //数据包类型
        //public byte RecType { get; set; }

        //传感器序号
        public int SensorNum { get; set; }

        //节点ID
        public string SensorId { get; set; }

        //心跳包
        public byte SensorSerial { get; set; }

        //节点状态
        public string SensorState { get; set; }

        public byte sensorState
        {
            set
            {
                if (value == 0)
                {
                    SensorState = "vacant";
                }
                else
                {
                    SensorState = "occupy";
                }
            }
        }

        //节点型号
        public string SensorModel { get; set; } = "--";

        //心跳
        public string SensorHB { get; set; } = "--";

        //节点信号
        public string SensorRssi { get; set; } = "--";

        //节点电压
        public string SensorVoltage { get; set; } = "--";

        //硬件版本
        public string SensorHardver { get; set; } = "--";

        //软件版本
        public string SensorSoftver { get; set; } = "--";

        //对应的基站ID号码
        public string SensorWdcId { get; set; }

        //节点运行时间
        public string SensorRunTime { get; set; }

        //接收数据包时间
        public DateTime DateTimeRec { get; set; }

        //接收到的数据包
        //public string SensorMsg { get; set; }


    }
}

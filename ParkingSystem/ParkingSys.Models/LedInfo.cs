using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSys.Models
{
    public class LedInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 显示屏的IP
        /// </summary>
        public string LedIP { get; set; }

        /// <summary>
        /// 起始的号码
        /// </summary>
        public int StartNum { get; set; }

        /// <summary>
        /// 终止的号码
        /// </summary>
        public int EndNum { get; set; }

        /// <summary>
        /// 区域内节点数量总和
        /// </summary>
        public int parkingSum;     
        public int ParkingSum { get => parkingSum; set { parkingSum = value; this.SendChangeInfo("parkingSum"); } }


        /// <summary>
        /// 区域内占有车辆
        /// </summary>
        public int parkingOccupy;     
        public int ParkingOccupy
        {
            get => parkingOccupy; set
            {
                parkingOccupy = value; this.SendChangeInfo("parkingOccupy");
            }
        }


        /// <summary>
        /// 区域内空余车位
        /// </summary>
        public int parkingVacant;      
        public int ParkingVacant { get => parkingVacant; set { parkingVacant = value; this.SendChangeInfo("parkingVacant"); } }


        /// <summary>
        /// LED连接状态
        /// </summary>
        public bool LedConnect { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;
        private void SendChangeInfo(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}

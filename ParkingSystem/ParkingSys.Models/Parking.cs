using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSys.Models
{
    public class Parking
    {
        /// <summary>
        /// 停车场名称
        /// </summary>
        // public string  ParkingName { get; set; }

        /// <summary>
        /// 车位总数
        /// </summary>
        public int ParkingSum { get; set; } = 0;

        /// <summary>
        /// 占有车位数量
        /// </summary>
        public int ParkingOccupy { get; set; } = 0;

        /// <summary>
        /// 空余车位数量
        /// </summary>
        public int ParkingVacant { get; set; } = 0;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime DateTimeUpdate { get; set; }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSys.Models
{
    public class DetectInfo
    {
        public string SensorID { get; set; }
        public string SensorState { get; set; }
        public DateTime DataTimeRec { get; set; }
        public string RunTime { get; set; }


    }
}

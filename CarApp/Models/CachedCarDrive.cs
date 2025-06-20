using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarApp.Models
{
    public class CachedCarDrive
    {
        public CarDriveHeader Header { get; set; }
        public List<CarDriveDtList> Details { get; set; }
    }
}
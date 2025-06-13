using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarApp.Models
{
    public class CarDriveRequest
    {
        public CarDriveHeader Header { get; set; } = null!;
        public List<CarDriveDtList> Details { get; set; } = new();
    }
}
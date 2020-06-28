using OnlineMonitoringLog.Drivers.ModbusTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTcpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var df = new ModbusTCPUnitMOck(1,IPAddress.Parse("5.125.0.84"));
            var ers = new ModbusTCPUnitMOck(2, IPAddress.Parse("5.125.0.74"));
            Console.ReadKey();
        }
    }
}

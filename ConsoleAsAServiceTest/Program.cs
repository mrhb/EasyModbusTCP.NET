using OnlineMonitoringLog.Core;
using OnlineMonitoringLog.Core.Interfaces;
using OnlineMonitoringLog.Drivers;
using OnlineMonitoringLog.Drivers.ModbusTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAsAServiceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new LoggingContext())
            {
                var UnitEntities = db.UnitEntity.Where(p=>p.UnitId<2).ToList();

                List<IUnit> iunits = new List<IUnit>();
                foreach (var item in UnitEntities)
                {
                    iunits.Add(
                           UnitFactory.getUnit(item.ID, item.Type, item.Ip)
                           );
                }
            }
                Console.ReadKey();
        }
    }
}

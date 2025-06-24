using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartManufacturingLocal_HIP_PLC
{
    class ReadMachineData
    {
        public long MachineID { get; set; }
        public List<long> TotalCount { get; set; }
        public bool Break { get; set; }
        public bool Sched { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exemple1
{
    public class Machine 
    {
        public string MachineName { get; set; }
        public string MachineSystem { get; set; }
        public string MachineStockage { get; set; }
        public string MachineRam { get; set; }
        public string MachineStatuts { get; set; }

        public override string ToString()
        {
            return MachineName + ":" + MachineStatuts;
        }

        public void stop()
        {
            MachineStatuts = "stopped";
        }

        public void run() {
            MachineStatuts = "running";
        }

        public bool start()
        {
            return MachineStatuts.Equals("running");
        }
    }

   
}
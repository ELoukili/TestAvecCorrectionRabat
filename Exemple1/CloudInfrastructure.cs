using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Exemple1
{
    public class CloudInfrastructure 
    {
        //List of stock the stores
        public List<Store> store = new List<Store>();        

        //liste of machine
        public List<Machine> machines = new List<Machine>();   

        //test if store Exist
        public bool isContStore(string name)
        {
            bool i = false;
            var st = store.Find(p=>p.StoreName == name);
            if (st != null)
                return i=true;
            return i;
        }
        
        //methode for add the new store in store list
        public void CreateStore(string name)
        {
            if (isContStore(name))
                throw new StoreException();
            
            store.Add(new Store() { StoreName = name
                                    });
        }
        
        //methode for recuperer all store 
        public string ListStores()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var st in store)
            {
                sb.Append(st.ToString()+"||");
            }
        return sb.ToString().Substring(0, sb.Length - 2); ;
        }

        //upload file for name store
        public void UploadDocument(string name,params string[] file) {
            
            Store st = store.Find(p=>p.StoreName == name);
            foreach (string fileName in file) {
                st.addDocument(fileName);
            }
                    }       
        //delete Store
        public void DeleteStore(string nameStore)
        {
            var st = store.Single(p=>p.StoreName == nameStore);
            store.Remove(st);
            
        }

        //Empty Store
        public void EmptyStore(string nameStore)
        {
            Store st = store.Find(p=>p.StoreName == nameStore);
            st.deleteAllDocuments();

        }
                
        //create a new machine
        public void CreateMachine(string name,string systeme,string stockage,string ram) {
          
           machines.Add(new Machine { MachineName = name,
                                      MachineSystem = systeme,
                                      MachineStockage = stockage,
                                      MachineRam = ram,
                                      MachineStatuts = "Inactive"});

        }

        //test machine is Already running
        public bool isRun(string name)
        {
            bool i = false;
            var mach = machines.Find(p => p.MachineName == name);
            if (mach.start())
                return i = true;
            return i;
        }
        //run machine
        public void StartMachine(string Machinename)
        {
            if(isRun(Machinename))
                throw new StoreException();

            Machine m = machines.Find(p=>p.MachineName == Machinename);
            m.run();
        }

        //stoped Machine
        public void StopMachine(string Machinename) {
            Machine m = machines.Find(p => p.MachineName == Machinename);
            m.stop();
        }

        //List of Machine
        public string ListMachines() {
            StringBuilder sb = new StringBuilder();
            foreach(var mach in machines)
            {
                sb.Append(mach.ToString()+"||");
            }
            return sb.ToString().Substring(0,sb.Length-2);
        }

        //Memory used by Machine
        public double UsedMemoryMachine(string name)
        {
           Machine m = machines.Find(p => p.MachineName == name);
            if (m.start())
                return Convert.ToDouble(m.MachineRam.Substring(0, m.MachineRam.Length-2));
                
            return 0;
        }

        //Stockage used by Machine
        public double UsedDiskMachine(string name)
        {
            Machine m = machines.Find(p => p.MachineName == name);

            return Convert.ToDouble(m.MachineStockage.Substring(0, m.MachineStockage.Length-2));
             }

        //espace utilise pour les fichier d'un store
        public double UsedDiskStore(string StoreName)
        {
            double sizeUseed = 0;

            Store st = store.Find(p=>p.StoreName == StoreName);

            sizeUseed += Convert.ToDouble(st.documents.Count) * 0.1;

            return sizeUseed;
        }

        //Global disk used
        public double GlobalUsedDisk()
        {
            double diskTotal = 0;
            double totalFileSize = 0;
            foreach (var st in store)
            {
                totalFileSize += Convert.ToDouble(st.documents.Count) * 0.100;
            }
            foreach (var mach in machines) {
                diskTotal += Convert.ToDouble(mach.MachineStockage.Substring(0,mach.MachineStockage.Length-2));
            }
            return diskTotal+ totalFileSize;
        }

        //Global Memory Used
        public double GlobalUsedMemory()
        {
            double totalMemory = 0;
            
            foreach (var mach in machines)
            {
                if (mach.start())
                    totalMemory += Convert.ToDouble(mach.MachineRam.Substring(0, mach.MachineRam.Length - 2));

            }
            return totalMemory;
        }

    }
}
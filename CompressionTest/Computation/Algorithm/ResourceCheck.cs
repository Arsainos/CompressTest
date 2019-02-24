using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.IO;


namespace CompressionTest.Computation.Algorithm
{
    static class ResourceCheck
    {
        public static double checkRamSize()
        {
            double result = 0;

            var searcher = new ManagementObjectSearcher("select * from CIM_OperatingSystem");

            foreach(var t in searcher.Get())
            {
                result = Convert.ToDouble(t.Properties["FreePhysicalMemory"].Value);
            }

            return result;
        }

        public static double[] checkCPUUsage()
        {
            int NumberOfLogicalCores = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                NumberOfLogicalCores = Convert.ToInt32(item["NumberOfLogicalProcessors"]);
            }

            List<string> Cpus = new List<string>();
            for(int i=0;i<NumberOfLogicalCores;i++)
            {
                Cpus.Add(i.ToString());
            }

            double[] CPUsIdles = new double[NumberOfLogicalCores];

            var searcher = new ManagementObjectSearcher(@"\root\CIMV2", "select * from Win32_PerfFormattedData_PerfOS_Processor");

            foreach(var t in searcher.Get())
            {
                var res = t.Properties.Cast<PropertyData>().ToDictionary(p => p.Name, p => p.Value);
                if(Cpus.Contains(res["Name"].ToString()))
                {
                    CPUsIdles[Convert.ToInt32(res["Name"])] = Convert.ToDouble(res["PercentIdleTime"]);
                }
            }

            return CPUsIdles;
        }

        public static double checkDiskUsage(string diskName)
        {
            double result = 0;

            foreach(var drive in DriveInfo.GetDrives())
            {
                if (!drive.Name.Contains(diskName)) continue;
                //Console.WriteLine(drive.Name + " - " + drive.DriveType);

                if(drive.DriveType == DriveType.Fixed || drive.DriveType == DriveType.Removable)
                {
                    var Obj = new ManagementObject(String.Format("Win32_PerfFormattedData_PerfDisk_PhysicalDisk.Name='0 {0}:'",diskName));

                    result = Convert.ToDouble(Obj.Properties["PercentIdleTime"].Value);
                }
            }
            
            return result;
        }

    }
}

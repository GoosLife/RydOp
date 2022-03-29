using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace WMIApp
{
    class Program
    {
        // GUI
        static void Main(string[] args)
        {
            bool running = true;

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;

            do
            {
                Console.Clear();
                Console.WriteLine("  Choose an operation: ");
                Console.WriteLine("    1. Show harddisk metadata");
                Console.WriteLine("    2. Show harddisk serial number(s)");
                Console.WriteLine("    3. Show processor data");
                Console.WriteLine("    4. Show main storage information");
                Console.WriteLine("    5. Show user information");
                Console.WriteLine("    6. Show boot devices");
                Console.WriteLine("    7. List all services");
                Console.WriteLine("");
                Console.WriteLine("    8. Exit");

                string choice = Console.ReadLine();

                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine(GetHardDiskMetadata());
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.WriteLine(GetHardDiskSerialNumber());
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.WriteLine(ProcesorData());
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.WriteLine(MainStorage());
                        Console.ReadKey();
                        break;

                    case "5":
                        Console.WriteLine(UserInformation());
                        Console.ReadKey();
                        break;

                    case "6":
                        Console.WriteLine(ShowBootDevices());
                        Console.ReadKey();
                        break;

                    case "7":
                        Console.WriteLine("Process search: \n");
                        foreach (string s in ListAllServices())
                        {
                            Console.WriteLine(s);
                            if (Console.ReadKey().Key == ConsoleKey.Escape)
                                break;
                        }
                        break;

                    case "8":
                        Console.WriteLine("Thanks for using this mess.");
                        Console.ReadKey();
                        running = false;
                        break;

                    default:
                        break;
                }

            } while (running);

        } //Slut main


        // LOGIK LAG
        private static string ProcesorData()
        {
            string processorData = "\n--- PROCESSOR DATA ---\n";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                var usage = obj["PercentProcessorTime"];
                var name = obj["Name"];
                processorData += "CPU " + name + " : " + usage + "\n";
            }

            return processorData;
        }

        static string UserInformation()
        {
            string testOutput = "--- USER DATA ---\n";

            ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
            ManagementObjectCollection results = searcher.Get();
            foreach (ManagementObject result in results)
            {
                testOutput += "User:\t" + result["RegisteredUser"] + "\n";
                testOutput += "Org.:\t" + result["Organization"] + "\n";
                testOutput += string.Format("O/S :\t{0}", result["Name"]) + "\n";
            }

            return testOutput;
        }

        static string ShowBootDevices()
        {
            string bootDevices = "--- BOOT DEVICES ---\n";
            ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\cimv2");

            //create object query
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");

            //create object searcher
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            //get a collection of WMI objects
            ManagementObjectCollection queryCollection = searcher.Get();

            //enumerate the collection.
            foreach (ManagementObject m in queryCollection)
            {
                // access properties of the WMI object
                bootDevices += string.Format("BootDevice : {0}", m["BootDevice"]);
            }

            return bootDevices;
        }


        /*  Denne metode bliver ikke brugt ???

        static List<string> PopulateDisk()

        {

            List<string> disk = new List<string>();

            SelectQuery selectQuery = new SelectQuery("Win32_LogicalDisk");

            ManagementObjectSearcher mnagementObjectSearcher = new ManagementObjectSearcher(selectQuery);

            foreach (ManagementObject managementObject in mnagementObjectSearcher.Get())

            {

                disk.Add(managementObject.ToString());

            }

            return disk;

        } */

        static string MainStorage()
        {
            string mainStorageOutput = "--- MAIN STORAGE ---\n";

            ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
            ManagementObjectCollection results = searcher.Get();

            foreach (ManagementObject result in results)
            {
                mainStorageOutput += string.Format("Total Visible Memory: {0}KB", result["TotalVisibleMemorySize"]) + "\n";
                mainStorageOutput += string.Format("Free Physical Memory: {0}KB", result["FreePhysicalMemory"]) + "\n";
                mainStorageOutput += string.Format("Total Virtual Memory: {0}KB", result["TotalVirtualMemorySize"]) + "\n";
                mainStorageOutput += string.Format("Free Virtual Memory: {0}KB", result["FreeVirtualMemory"]) + "\n";
            }

            return mainStorageOutput;
        }

        static string GetHardDiskMetadata()
        {
            string metadata = "---DISK METADATA ---\n";

            ManagementScope managementScope = new ManagementScope();

            ObjectQuery objectQuery = new ObjectQuery("select FreeSpace,Size,Name from Win32_LogicalDisk where DriveType=3");

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, objectQuery);

            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();

            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                metadata += "Disk Name : " + managementObject["Name"].ToString() + "\n";
                metadata += "Free Space: " + managementObject["FreeSpace"].ToString() + "\n";
                metadata += "Disk Size : " + managementObject["Size"].ToString() + "\n";
                metadata += "---------------------------------------------------\n";
            }

            return metadata;
        }

        static string GetHardDiskSerialNumber(string drive = "C")
        {
            string hddSerialNumber = "--- HARD DISK SERIAL NUMBER ---\n";

            ManagementObject managementObject = new ManagementObject("Win32_LogicalDisk.DeviceID=\"" + drive + ":\"");
            managementObject.Get();

            return hddSerialNumber + "Volume serial number: " + managementObject["VolumeSerialNumber"].ToString() + "\n";
        }

        private static List<string> ListAllServices()
        {
            List<string> services = new List<string>();

            ManagementObjectSearcher windowsServicesSearcher = new ManagementObjectSearcher("root\\cimv2", "select * from Win32_Service");
            ManagementObjectCollection objectCollection = windowsServicesSearcher.Get();

            services.Add(string.Format("There are {0} Windows services: ", objectCollection.Count));

            foreach (ManagementObject windowsService in objectCollection)
            {
                PropertyDataCollection serviceProperties = windowsService.Properties;
                foreach (PropertyData serviceProperty in serviceProperties)
                {
                    if (serviceProperty.Value != null)
                    {
                        string s = string.Format("Windows service property name : {0}", serviceProperty.Name) + "\n";
                        s += string.Format("Windows service property value: {0}", serviceProperty.Value) + "\n";

                        services.Add(s);
                    }
                }
                services.Add("---------------------------------------");
            }

            return services;
        }
    }

}

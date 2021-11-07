using System.Text;
using System.Management;
using System;
using System.Security.Cryptography;
using P2P.Net;

namespace P2P
{
    class Program
    {
        public class SystemInfo
        {
            public static string id
            {
                get
                {
                    return GetMachineCodeString();
                }
            }
            private static string GetStringMD5(string str)
            {
                MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
                byte[] retVal = md5CSP.ComputeHash(Encoding.Default.GetBytes(str));
                string retStr = "";
                for (int i = 0; i < retVal.Length; i++)
                {
                    retStr += retVal[i].ToString("x2");
                }

                return retStr;
            }
            private static string GetMachineCodeString()
            {
                string machineCodeString = string.Empty;
                machineCodeString = "PC." + GetCpuInfo() + "." +
                                    GetHDid() + "." +
                                    GetMoAddress();
                return GetStringMD5(machineCodeString);
            }

            ///   <summary> 
            ///   获取cpu序列号     
            ///   </summary> 
            ///   <returns> string </returns> 
            private static string GetCpuInfo()
            {
                string cpuInfo = "";
                try
                {
                    using (ManagementClass cimobject = new ManagementClass("Win32_Processor"))
                    {
                        ManagementObjectCollection moc = cimobject.GetInstances();

                        foreach (ManagementObject mo in moc)
                        {
                            cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                            mo.Dispose();
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return cpuInfo.ToString();
            }

            ///   <summary> 
            ///   获取硬盘ID     
            ///   </summary> 
            ///   <returns> string </returns> 
            private static string GetHDid()
            {
                string HDid = "";
                try
                {
                    using (ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive"))
                    {
                        ManagementObjectCollection moc1 = cimobject1.GetInstances();
                        foreach (ManagementObject mo in moc1)
                        {
                            HDid = (string)mo.Properties["Model"].Value;
                            mo.Dispose();
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                return HDid.ToString();
            }

            ///   <summary> 
            ///   获取网卡硬件地址 
            ///   </summary> 
            ///   <returns> string </returns> 
            private static string GetMoAddress()
            {
                string MoAddress = "";
                try
                {
                    using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                    {
                        ManagementObjectCollection moc2 = mc.GetInstances();
                        foreach (ManagementObject mo in moc2)
                        {
                            if ((bool)mo["IPEnabled"] == true)
                                MoAddress = mo["MacAddress"].ToString();
                            mo.Dispose();
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return MoAddress.ToString();
            }
        }
        static void Main(string[] args)
        {
            string severIp = "127.0.0.1";
            int severPort = 5004;
            int buffersize = 4096;
            P2PClient client = new P2PClient(SystemInfo.id, severIp, severPort, buffersize,2000);
            P2PSever sever = new P2PSever(severPort, buffersize);
            while (true)
            {

            }
        }
    }
}

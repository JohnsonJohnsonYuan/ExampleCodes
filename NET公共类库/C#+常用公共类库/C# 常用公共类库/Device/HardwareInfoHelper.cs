using System.Runtime.InteropServices;
using System.Management;
using System.Collections.Generic;
using Microsoft.Win32;
using System;
using System.Text;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// HardDiskVal ��ժҪ˵����
    /// ��ȡָ���̷���Ӳ�����к�
    /// ���ܣ���ȡָ���̷���Ӳ�����к�
    /// </summary>
    public sealed class HardwareInfoHelper
    {
        #region Ӳ����Ϣ��ȡ

        [DllImport("kernel32.dll")]
        private static extern int GetVolumeInformation(
            string lpRootPathName,
            string lpVolumeNameBuffer,
            int nVolumeNameSize,
            ref int lpVolumeSerialNumber,
            int lpMaximumComponentLength,
            int lpFileSystemFlags,
            string lpFileSystemNameBuffer,
            int nFileSystemNameSize
            );

        /// <summary>
        /// ����̷�ΪdrvID��Ӳ�����кţ�ȱʡΪC
        /// </summary>
        /// <param name="drvID"></param>
        /// <returns></returns>
        public static string HDVal(string drvID)
        {
            const int MAX_FILENAME_LEN = 256;
            int retVal = 0;
            int a = 0;
            int b = 0;
            string str1 = null;
            string str2 = null;

            int i = GetVolumeInformation(
                drvID + @":\",
                str1,
                MAX_FILENAME_LEN,
                ref retVal,
                a,
                b,
                str2,
                MAX_FILENAME_LEN
                );

            return retVal.ToString();
        }

        public static string HDVal()
        {
            return HDVal("C");

        }

        /// <summary>
        /// ��ȡӲ��ID
        /// </summary>
        /// <returns></returns>
        public static string GetDiskID()
        {
            string HDid = "";
            ManagementClass mc = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                HDid = mo.Properties["signature"].Value.ToString();
            }
            return HDid;
        }

        #endregion

        #region CPU��Ϣ��ȡ

        /// <summary>
        /// ��ȡCPU��ID
        /// </summary>
        /// <returns></returns>
        public static string GetCPUId()
        {
            string strCpuID = "";
            try
            {
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }
            }
            catch
            {
                strCpuID = "078BFBFF00020FC1";//Ĭ�ϸ���һ��
            }
            return strCpuID;

        }

        /// <summary>
        /// ��ȡCPU������
        /// </summary>
        /// <returns></returns>
        public static string GetCPUName()
        {
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");

            object obj = rk.GetValue("ProcessorNameString");
            string CPUName = (string)obj;
            return CPUName.TrimStart();
        }
 
        #endregion

        #region USB�̷��б�
        // Credits of Team 2: SyncButler. With this method, it enables us to find all USB drives regardless of whether they are removable or fixed.

        /// <summary>
        /// Returns a List of drive letters of USB storage devices attached to the computer.
        /// Drive letter format is of the format X:
        /// </summary>
        /// <returns>List of USB Drive letters</returns>
        public static List<string> GetUSBDriveLetters()
        {
            List<string> list = new List<string>();
            ManagementObjectSearcher ddMgmtObjSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");

            foreach (ManagementObject ddObj in ddMgmtObjSearcher.Get())
            {
                foreach (ManagementObject dpObj in ddObj.GetRelated("Win32_DiskPartition"))
                {
                    foreach (ManagementObject ldObj in dpObj.GetRelated("Win32_LogicalDisk"))
                    {
                        list.Add(ldObj["DeviceID"].ToString());
                    }
                }
            }

            return list;
        } 
        #endregion

        /// <summary>
        /// ��ȡMAC��ַ
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            string mac = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    mac = mo["MacAddress"].ToString();
                    break;
                }
            }
            return mac;
        }

        /// <summary>
        /// ��ȡIP��ַ
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddress()
        {
            string st = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    //st=mo["IpAddress"].ToString();
                    System.Array ar;
                    ar = (System.Array)(mo.Properties["IpAddress"].Value);
                    st = ar.GetValue(0).ToString();
                    break;
                }
            }
            moc = null;
            mc = null;
            return st;
        }

        /// <summary>
        /// ��ȡ����ϵͳ�ĵ�¼�û���
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            return Environment.UserName;
        }
        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <returns></returns>
        public static string GetComputerName()
        {
            return System.Environment.MachineName;
        }
        /// <summary>
        /// ��ȡPC����
        /// </summary>
        /// <returns></returns>
        public static string GetSystemType()
        {
            string st = "";
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {

                st = mo["SystemType"].ToString();

            }
            return st;
        }
        /// <summary>
        /// ��ȡ�����ڴ�
        /// </summary>
        /// <returns></returns>
        public static string GetTotalPhysicalMemory()
        {
            string st = "";
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {

                st = mo["TotalPhysicalMemory"].ToString();

            }
            return st;
        }

        /// <summary>
        /// ���Ӳ����Ϣ
        /// </summary>
        public static HardDiskInfo GetHDInfo(byte driveIndex)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                    return GetHddInfo9x(driveIndex);
                case PlatformID.Win32NT:
                    return GetHddInfoNT(driveIndex);
                case PlatformID.Win32S:
                    throw new NotSupportedException("Win32s is not supported.");
                case PlatformID.WinCE:
                    throw new NotSupportedException("WinCE is not supported.");
                default:
                    throw new NotSupportedException("Unknown Platform.");
            }
        }

        #region ��ȡӲ����Ϣ��ʵ��

        #region �ṹ

        [Serializable]
        public struct HardDiskInfo
        {
            /// <summary>
            /// �ͺ�
            /// </summary>
            public string ModuleNumber;
            /// <summary>
            /// �̼��汾
            /// </summary>
            public string Firmware;
            /// <summary>
            /// ���к�
            /// </summary>
            public string SerialNumber;
            /// <summary>
            /// ��������MΪ��λ
            /// </summary>
            public uint Capacity;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct GetVersionOutParams
        {
            public byte bVersion;
            public byte bRevision;
            public byte bReserved;
            public byte bIDEDeviceMap;
            public uint fCapabilities;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] dwReserved; // For future use.
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct IdeRegs
        {
            public byte bFeaturesReg;
            public byte bSectorCountReg;
            public byte bSectorNumberReg;
            public byte bCylLowReg;
            public byte bCylHighReg;
            public byte bDriveHeadReg;
            public byte bCommandReg;
            public byte bReserved;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SendCmdInParams
        {
            public uint cBufferSize;
            public IdeRegs irDriveRegs;
            public byte bDriveNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] dwReserved;
            public byte bBuffer;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct DriverStatus
        {
            public byte bDriverError;
            public byte bIDEStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] dwReserved;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SendCmdOutParams
        {
            public uint cBufferSize;
            public DriverStatus DriverStatus;
            public IdSector bBuffer;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 512)]
        internal struct IdSector
        {
            public ushort wGenConfig;
            public ushort wNumCyls;
            public ushort wReserved;
            public ushort wNumHeads;
            public ushort wBytesPerTrack;
            public ushort wBytesPerSector;
            public ushort wSectorsPerTrack;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public ushort[] wVendorUnique;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] sSerialNumber;
            public ushort wBufferType;
            public ushort wBufferSize;
            public ushort wECCSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] sFirmwareRev;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public byte[] sModelNumber;
            public ushort wMoreVendorUnique;
            public ushort wDoubleWordIO;
            public ushort wCapabilities;
            public ushort wReserved1;
            public ushort wPIOTiming;
            public ushort wDMATiming;
            public ushort wBS;
            public ushort wNumCurrentCyls;
            public ushort wNumCurrentHeads;
            public ushort wNumCurrentSectorsPerTrack;
            public uint ulCurrentSectorCapacity;
            public ushort wMultSectorStuff;
            public uint ulTotalAddressableSectors;
            public ushort wSingleWordDMA;
            public ushort wMultiWordDMA;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] bReserved;
        }

        #endregion

        #region API

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFile(
         string lpFileName,
         uint dwDesiredAccess,
         uint dwShareMode,
         IntPtr lpSecurityAttributes,
         uint dwCreationDisposition,
         uint dwFlagsAndAttributes,
         IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        static extern int DeviceIoControl(
         IntPtr hDevice,
         uint dwIoControlCode,
         IntPtr lpInBuffer,
         uint nInBufferSize,
         ref GetVersionOutParams lpOutBuffer,
         uint nOutBufferSize,
         ref uint lpBytesReturned,
         [Out] IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        static extern int DeviceIoControl(
         IntPtr hDevice,
         uint dwIoControlCode,
         ref SendCmdInParams lpInBuffer,
         uint nInBufferSize,
         ref SendCmdOutParams lpOutBuffer,
         uint nOutBufferSize,
         ref uint lpBytesReturned,
         [Out] IntPtr lpOverlapped);


        const uint DFP_GET_VERSION = 0x00074080;
        const uint DFP_SEND_DRIVE_COMMAND = 0x0007c084;
        const uint DFP_RECEIVE_DRIVE_DATA = 0x0007c088;


        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint CREATE_NEW = 1;
        const uint OPEN_EXISTING = 3;


        #endregion

        /// <summary>
        /// ��ȡ9X�ܹ���Ӳ����Ϣ
        /// </summary>
        /// <param name="driveIndex"></param>
        /// <returns></returns>
        private static HardDiskInfo GetHddInfo9x(byte driveIndex)
        {
            GetVersionOutParams vers = new GetVersionOutParams();
            SendCmdInParams inParam = new SendCmdInParams();
            SendCmdOutParams outParam = new SendCmdOutParams();
            uint bytesReturned = 0;


            IntPtr hDevice = CreateFile(
             @"\\.\Smartvsd",
             0,
             0,
             IntPtr.Zero,
             CREATE_NEW,
             0,
             IntPtr.Zero);
            if (hDevice == IntPtr.Zero)
            {
                throw new Exception("Open smartvsd.vxd failed.");
            }
            if (0 == DeviceIoControl(
             hDevice,
             DFP_GET_VERSION,
             IntPtr.Zero,
             0,
             ref vers,
             (uint)Marshal.SizeOf(vers),
             ref bytesReturned,
             IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception("DeviceIoControl failed:DFP_GET_VERSION");
            }
            // If IDE identify command not supported, fails
            if (0 == (vers.fCapabilities & 1))
            {
                CloseHandle(hDevice);
                throw new Exception("Error: IDE identify command not supported.");
            }
            if (0 != (driveIndex & 1))
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xb0;
            }
            else
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xa0;
            }
            if (0 != (vers.fCapabilities & (16 >> driveIndex)))
            {
                // We don''t detect a ATAPI device.
                CloseHandle(hDevice);
                throw new Exception(string.Format("Drive {0} is a ATAPI device, we don''t detect it", driveIndex + 1));
            }
            else
            {
                inParam.irDriveRegs.bCommandReg = 0xec;
            }
            inParam.bDriveNumber = driveIndex;
            inParam.irDriveRegs.bSectorCountReg = 1;
            inParam.irDriveRegs.bSectorNumberReg = 1;
            inParam.cBufferSize = 512;
            if (0 == DeviceIoControl(
             hDevice,
             DFP_RECEIVE_DRIVE_DATA,
             ref inParam,
             (uint)Marshal.SizeOf(inParam),
             ref outParam,
             (uint)Marshal.SizeOf(outParam),
             ref bytesReturned,
             IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception("DeviceIoControl failed: DFP_RECEIVE_DRIVE_DATA");
            }
            CloseHandle(hDevice);


            return GetHardDiskInfo(outParam.bBuffer);
        }
        /// <summary>
        /// ��ȡNT�ܹ���Ӳ����Ϣ
        /// </summary>
        /// <param name="driveIndex"></param>
        /// <returns></returns>
        private static HardDiskInfo GetHddInfoNT(byte driveIndex)
        {
            GetVersionOutParams vers = new GetVersionOutParams();
            SendCmdInParams inParam = new SendCmdInParams();
            SendCmdOutParams outParam = new SendCmdOutParams();
            uint bytesReturned = 0;


            // We start in NT/Win2000
            IntPtr hDevice = CreateFile(
             string.Format(@"\\.\PhysicalDrive{0}", driveIndex),
             GENERIC_READ | GENERIC_WRITE,
             FILE_SHARE_READ | FILE_SHARE_WRITE,
             IntPtr.Zero,
             OPEN_EXISTING,
             0,
             IntPtr.Zero);
            if (hDevice == IntPtr.Zero)
            {
                throw new Exception("CreateFile faild.");
            }
            if (0 == DeviceIoControl(
             hDevice,
             DFP_GET_VERSION,
             IntPtr.Zero,
             0,
             ref vers,
             (uint)Marshal.SizeOf(vers),
             ref bytesReturned,
             IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception(string.Format("Drive {0} may not exists.", driveIndex + 1));
            }
            // If IDE identify command not supported, fails
            if (0 == (vers.fCapabilities & 1))
            {
                CloseHandle(hDevice);
                throw new Exception("Error: IDE identify command not supported.");
            }
            // Identify the IDE drives
            if (0 != (driveIndex & 1))
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xb0;
            }
            else
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xa0;
            }
            if (0 != (vers.fCapabilities & (16 >> driveIndex)))
            {
                CloseHandle(hDevice);
                throw new Exception(string.Format("Drive {0} is a ATAPI device, we don''t detect it.", driveIndex + 1));
            }
            else
            {
                inParam.irDriveRegs.bCommandReg = 0xec;
            }
            inParam.bDriveNumber = driveIndex;
            inParam.irDriveRegs.bSectorCountReg = 1;
            inParam.irDriveRegs.bSectorNumberReg = 1;
            inParam.cBufferSize = 512;


            if (0 == DeviceIoControl(
             hDevice,
             DFP_RECEIVE_DRIVE_DATA,
             ref inParam,
             (uint)Marshal.SizeOf(inParam),
             ref outParam,
             (uint)Marshal.SizeOf(outParam),
             ref bytesReturned,
             IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception("DeviceIoControl failed: DFP_RECEIVE_DRIVE_DATA");
            }
            CloseHandle(hDevice);


            return GetHardDiskInfo(outParam.bBuffer);
        }
        /// <summary>
        /// ��ȡӲ����Ϣ��ϸ��
        /// </summary>
        /// <param name="phdinfo"></param>
        /// <returns></returns>
        private static HardDiskInfo GetHardDiskInfo(IdSector phdinfo)
        {
            HardDiskInfo hddInfo = new HardDiskInfo();
            ChangeByteOrder(phdinfo.sModelNumber);
            hddInfo.ModuleNumber = Encoding.ASCII.GetString(phdinfo.sModelNumber).Trim();

            ChangeByteOrder(phdinfo.sFirmwareRev);
            hddInfo.Firmware = Encoding.ASCII.GetString(phdinfo.sFirmwareRev).Trim();

            ChangeByteOrder(phdinfo.sSerialNumber);
            hddInfo.SerialNumber = Encoding.ASCII.GetString(phdinfo.sSerialNumber).Trim();

            hddInfo.Capacity = phdinfo.ulTotalAddressableSectors / 2 / 1024;

            return hddInfo;
        }
        /// <summary>
        /// ��byte�����б������Ϣת�����ַ���
        /// </summary>
        /// <param name="charArray"></param>
        private static void ChangeByteOrder(byte[] charArray)
        {
            byte temp;
            for (int i = 0; i < charArray.Length; i += 2)
            {
                temp = charArray[i];
                charArray[i] = charArray[i + 1];
                charArray[i + 1] = temp;
            }
        }

        #endregion

    }
}
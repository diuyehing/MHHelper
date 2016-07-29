using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MHSucker
{
    class Memory
    {
        #region Windows Memory API

        public class MemoryAPI
        {
            [Flags]
            public enum ProcessAccessType
            {
                PROCESS_TERMINATE = (0x0001),
                PROCESS_CREATE_THREAD = (0x0002),
                PROCESS_SET_SESSIONID = (0x0004),
                PROCESS_VM_OPERATION = (0x0008),
                PROCESS_VM_READ = (0x0010),
                PROCESS_VM_WRITE = (0x0020),
                PROCESS_DUP_HANDLE = (0x0040),
                PROCESS_CREATE_PROCESS = (0x0080),
                PROCESS_SET_QUOTA = (0x0100),
                PROCESS_SET_INFORMATION = (0x0200),
                PROCESS_QUERY_INFORMATION = (0x0400)
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MEMORY_BASIC_INFORMATION64
            {
                public UInt64 BaseAddress;
                public UInt64 AllocationBase;
                public UInt32 AllocationProtect;
                public UInt32 __alignment1;
                public UInt64 RegionSize;
                public UInt32 State;
                public UInt32 Protect;
                public UInt32 lType;
                public UInt32 __alignment2;
            };

            [StructLayout(LayoutKind.Sequential)]
            public struct MEMORY_BASIC_INFORMATION32
            {
                public UInt64 BaseAddress;
                public UInt64 AllocationBase;
                public UInt32 AllocationProtect;
                public UInt64 RegionSize;
                public UInt32 State;
                public UInt32 Protect;
                public UInt32 Type;
            };

            [Flags]
            public enum MEMORY_STATE : int
            {
                COMMIT = 0x1000,
                FREE = 0x10000,
                RESERVE = 0x2000,
                RESET_UNDO = 0x1000000
            }

            [Flags]
            public enum MEMORY_TYPE : int
            {
                IMAGE = 0x1000000,
                MAPPED = 0x40000,
                PRIVATE = 0x20000
            }

            [Flags]
            public enum MemoryProtection : uint
            {
                PAGE_EXECUTE = 0x00000010,
                PAGE_EXECUTE_READ = 0x00000020,
                PAGE_EXECUTE_READWRITE = 0x00000040,
                PAGE_EXECUTE_WRITECOPY = 0x00000080,
                PAGE_NOACCESS = 0x00000001,
                PAGE_READONLY = 0x00000002,
                PAGE_READWRITE = 0x00000004,
                PAGE_WRITECOPY = 0x00000008,
                PAGE_GUARD = 0x00000100,
                PAGE_NOCACHE = 0x00000200,
                PAGE_WRITECOMBINE = 0x00000400
            }

            [DllImport("kernel32.dll")]
            public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

            [DllImport("kernel32.dll")]
            public static extern Int32 CloseHandle(IntPtr hObject);

            [DllImport("kernel32.dll")]
            public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out int nNumberOfBytesRead);

            [DllImport("kernel32.dll")]
            public static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out int nNumberOfBytesWritten);

            [DllImport("kernel32.dll")]
            public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION64 lpBuffer, uint dwLength);

            [DllImport("kernel32.dll")]
            public static extern int VirtualQueryEx(Int32 hProcess, Int32 lpAddress, out MEMORY_BASIC_INFORMATION32 lpBuffer, uint dwLength);

            [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsWow64Process([In] IntPtr processHandle,
                 [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);
        }

        #endregion

        #region Read Memory

        private static byte[] m_byteBuffer = new byte[32];
        private static int m_nBytesRead = 0;

        public static Byte ReadByte(IntPtr hProcess, UInt64 uMemoryAddress)
        {
            int nRetCode = MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)uMemoryAddress, m_byteBuffer, 1, out m_nBytesRead);

            if (nRetCode == 0)
                return 0;
            else
                return m_byteBuffer[0];
        }

        public static SByte ReadSByte(IntPtr hProcess, UInt64 uMemoryAddress)
        {
            int nRetCode = MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)uMemoryAddress, m_byteBuffer, 1, out m_nBytesRead);

            if (nRetCode == 0)
                return 0;
            else
                return (SByte)m_byteBuffer[0];
        }

        public static UInt16 ReadUInt16(IntPtr hProcess, UInt64 uMemoryAddress)
        {
            int nRetCode = MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)uMemoryAddress, m_byteBuffer, 2, out m_nBytesRead);

            if (nRetCode == 0)
                return 0;
            else
                return BitConverter.ToUInt16(m_byteBuffer, 0);
        }

        public static Int16 ReadInt16(IntPtr hProcess, UInt64 uMemoryAddress)
        {
            int nRetCode = MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)uMemoryAddress, m_byteBuffer, 2, out m_nBytesRead);

            if (nRetCode == 0)
                return 0;
            else
                return BitConverter.ToInt16(m_byteBuffer, 0);
        }

        public static UInt32 ReadUInt32(IntPtr hProcess, UInt64 uMemoryAddress)
        {
            int nRetCode = MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)uMemoryAddress, m_byteBuffer, 4, out m_nBytesRead);

            if (nRetCode == 0)
                return 0;
            else
                return BitConverter.ToUInt32(m_byteBuffer, 0);
        }

        public static Int32 ReadInt32(IntPtr hProcess, UInt64 uMemoryAddress)
        {
            int nRetCode = MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)uMemoryAddress, m_byteBuffer, 4, out m_nBytesRead);

            if (nRetCode == 0)
                return 0;
            else
                return BitConverter.ToInt32(m_byteBuffer, 0);
        }

        public static UInt64 ReadUInt64(IntPtr hProcess, UInt64 uMemoryAddress)
        {
            int nRetCode = MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)uMemoryAddress, m_byteBuffer, 8, out m_nBytesRead);

            if (nRetCode == 0)
                return 0;
            else
                return BitConverter.ToUInt64(m_byteBuffer, 0);
        }

        public static Int64 ReadInt64(IntPtr hProcess, UInt64 uMemoryAddress)
        {
            int nRetCode = MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)uMemoryAddress, m_byteBuffer, 8, out m_nBytesRead);

            if (nRetCode == 0)
                return 0;
            else
                return BitConverter.ToInt64(m_byteBuffer, 0);
        }

        public static float ReadSingle(IntPtr hProcess, UInt64 uMemoryAddress)
        {
            int nRetCode = MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)uMemoryAddress, m_byteBuffer, 4, out m_nBytesRead);

            if (nRetCode == 0)
                return 0;
            else
                return BitConverter.ToSingle(m_byteBuffer, 0);
        }

        public static double ReadDouble(IntPtr hProcess, UInt64 uMemoryAddress)
        {
            int nRetCode = MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)uMemoryAddress, m_byteBuffer, 8, out m_nBytesRead);

            if (nRetCode == 0)
                return 0;
            else
                return BitConverter.ToDouble(m_byteBuffer, 0);
        }

        public static String ReadString(IntPtr hProcess, UInt64 uMemoryAddress, uint iTextLength, int iMode = 0)
        {
            byte[] bBuffer = new byte[iTextLength];
           
            MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)uMemoryAddress, bBuffer, iTextLength, out m_nBytesRead);

            if (iMode == 0)
                return Encoding.UTF8.GetString(bBuffer);
            else if (iMode == 1)
                return BitConverter.ToString(bBuffer).Replace("-", "");
            else
                return "";
        }

        public static Byte[] ReadAOB(IntPtr hProcess, IntPtr uMemoryAddress, uint iBytesToRead, out int bytesRead)
        {
            byte[] bBuffer = new byte[iBytesToRead];
           
            MemoryAPI.ReadProcessMemory(hProcess, uMemoryAddress, bBuffer, iBytesToRead, out m_nBytesRead);
            bytesRead = m_nBytesRead;
            return bBuffer;
        }

        public static Byte[] ReadAOB(IntPtr hProcess, IntPtr uMemoryAddress, uint uBytesToRead)
        {
            byte[] bBuffer = new byte[uBytesToRead];
           
            MemoryAPI.ReadProcessMemory(hProcess, uMemoryAddress, bBuffer, uBytesToRead, out m_nBytesRead);
            return bBuffer;
        }

        #endregion
    }
}

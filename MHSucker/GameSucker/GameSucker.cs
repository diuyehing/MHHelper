using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MHSucker.GameSucker
{
    class GameSucker
    {
        private IntPtr m_hGameProcess;

        public void Init()
        {
            m_hGameProcess = IntPtr.Zero;
        }

        public void Uninit()
        {

        }

        private Process FindGameProcess()
        {
            string strGameProcessName = "mhmain";

            List<Process> ProcessList = new List<Process>(Process.GetProcessesByName(strGameProcessName));

            if (ProcessList.Count() == 0)
                return null;

            if (ProcessList.Count() > 1)
            {
                // printlog
            }

            return ProcessList[0];
        }

        private bool OpenGameProcess(Process process)
        {
            Memory.MemoryAPI.ProcessAccessType access;
            access = Memory.MemoryAPI.ProcessAccessType.PROCESS_VM_READ
                | Memory.MemoryAPI.ProcessAccessType.PROCESS_VM_WRITE
                | Memory.MemoryAPI.ProcessAccessType.PROCESS_VM_OPERATION;

            m_hGameProcess = Memory.MemoryAPI.OpenProcess((uint)access, 1, (uint)process.Id);
            if (m_hGameProcess == IntPtr.Zero)
            {
                // printlog
                return false;
            }

            return true;
        }

        private void CloseGameProcess()
        {
            try
            {
                if (m_hGameProcess == IntPtr.Zero)
                    return;

                int nRetCode = Memory.MemoryAPI.CloseHandle(m_hGameProcess);
                if (nRetCode == 0)
                {
                    throw new Exception("CloseHandle failed");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }
    }
}

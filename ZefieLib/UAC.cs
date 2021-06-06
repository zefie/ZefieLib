using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace ZefieLib
{
    public class UAC
    {
        /// <summary>
        /// Returns true if process has Administrative Access
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// Attempts to relaunch the current executable with Administrative Access
        /// </summary>
        /// <param name="args">CLI args to pass to ourselves</param>
        /// <returns>true if successful, so you can properly know to terminate your non-admin copy</returns>
        public static bool RunAsAdministrator(string args)
        {
            string executable = Process.GetCurrentProcess().MainModule.FileName;
            return RunAsAdministrator(executable, args);
        }

        /// <summary>
        /// Attempts to launch an executable with Administrative Access
        /// </summary>
        /// <param name="executable">full path to executable</param>
        /// <param name="args">arguments to pass to executable</param>
        /// <returns>true if successful, so you can properly know to terminate your non-admin copy</returns>
        public static bool RunAsAdministrator(string executable, string args)
        {
            if (IsAdmin == false)
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(executable)
                    {
                        Arguments = args,
                        Verb = "runas"
                    };
                    _ = Process.Start(startInfo);
                    return true;
                }
                catch
                {
                    DialogResult errormsg = MessageBox.Show("There was an error gaining administrative privileges", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (errormsg == DialogResult.Retry)
                    {
                        return RunAsAdministrator(executable, args);
                    }
                }
                return false;
            }
            return false;
        }
    }
}

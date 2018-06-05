using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SocketClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (DateTime.Now <= new DateTime(2018, 08, 1, 23, 59, 59))
            {
                Application.Run(new FrmTesting());
            }
        }
    }
}

using System;
using System.Windows.Forms;

namespace PBXEdit
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        private static FormMain _Main;

        public static FormMain GetMain()
        {
            return _Main;
        }

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _Main = new FormMain();
            Application.Run(_Main);
        }
    }
}

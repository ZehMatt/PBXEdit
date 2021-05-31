using System;
using System.Windows.Forms;

namespace PBXEdit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static FormMain _Main;

        public static FormMain GetMain()
        {
            return _Main;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _Main = new FormMain();
            Application.Run(_Main);
        }
    }
}

using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using SqlSugar;

namespace SXJL.GTCTK.UI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static Mutex run;

        [STAThread]
        private static void Main()
        {
            run = new Mutex(true, Process.GetCurrentProcess().ProcessName, out bool runone);
            if (runone)
            {
                Application app = new Application();
                MainWindow win = new MainWindow();
                app.MainWindow = win;
                win.Show();
                _ = app.Run();
            }
            else
            {
                _ = MessageBox.Show("程序已经运行，如果非正常关闭，并且不能再次运行，请检查【任务管理器】中是否有程序进程，或重启计算机！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                Current.Shutdown();
                Environment.Exit(0);
                Process.GetCurrentProcess().Kill();
            }
        }

        public App()
        {
            //using (ISqlSugarClient client = new SqlSugarClient(GetConnection()))
            //{
            //    client.CodeFirst.InitTables<TcRecords, PlcRecords>();
            //}
        }
        public static ConnectionConfig GetConnection()
        {
            ConnectionConfig connectionConfig = new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString,
                IsAutoCloseConnection = true,
            };
            return connectionConfig;
        }

        public static double byma4 = double.Parse(ConfigurationManager.AppSettings.Get("byma4"));
        public static double byma20 = double.Parse(ConfigurationManager.AppSettings.Get("byma20"));

        public static double byma4Real = double.Parse(ConfigurationManager.AppSettings.Get("byma4Real"));
        public static double byma20Real = double.Parse(ConfigurationManager.AppSettings.Get("byma20Real"));


        public static double dnma4 = double.Parse(ConfigurationManager.AppSettings.Get("dnma4"));
        public static double dnma20 = double.Parse(ConfigurationManager.AppSettings.Get("dnma20"));

        public static double dnma4Real = double.Parse(ConfigurationManager.AppSettings.Get("dnma4Real"));
        public static double dnma20Real = double.Parse(ConfigurationManager.AppSettings.Get("dnma20Real"));


        public static string State1 = ConfigurationManager.AppSettings.Get("State1");
        public static string State2 = ConfigurationManager.AppSettings.Get("State2");
        public static string State3 = ConfigurationManager.AppSettings.Get("State3");
        public static string State4 = ConfigurationManager.AppSettings.Get("State4");


        public static int DpmRefTime = int.Parse(ConfigurationManager.AppSettings.Get("DpmRefTime"));

        public static double Rj = double.Parse(ConfigurationManager.AppSettings.Get("Rj"));
        public static double Xc = double.Parse(ConfigurationManager.AppSettings.Get("Xc"));

        public static double Scale = double.Parse(ConfigurationManager.AppSettings.Get("Scale"));

        public static bool IsTestState = bool.Parse(ConfigurationManager.AppSettings.Get("IsTestState"));
        public static bool IsTestDn = bool.Parse(ConfigurationManager.AppSettings.Get("IsTestDn"));
    }
}

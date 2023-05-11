/*自己写的简单log类，结合缓存用，log4配置太多，而且使用的一些组件使用了特定的log4版本，多个版本的可能会发生冲突。比如现在的MemcachedHelper只能使用log4的1.x版本。。。
用法说明：
Log2(logType)
  */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RWS.Core
{
    public class LogType
    {
        /// <summary>
        /// Log级别
        /// </summary>
        public string LogLevelType
        {
            get; set;
        }

        /// <summary>
        /// Log文件名
        /// </summary>
        public string LogFileName
        {
            get; set;
        }

        /// <summary>
        /// Log消息
        /// </summary>
        public string LogMsg
        {
            get; set;
        }
    }

    public class Log2Helper
    {
        private static readonly Queue<LogType> LogQueue;
        private static readonly string path;

        static Log2Helper()
        {
            path = Process.GetCurrentProcess().MainModule.FileName;
            LogQueue = new Queue<LogType>();
            _ = ThreadPool.QueueUserWorkItem(h =>
            {
                while (true)
                {
                    try
                    {
                        if (LogQueue.Count > 0)
                        {
                            LogType logType = LogQueue.Dequeue();
                            Log(logType.LogLevelType, logType.LogFileName, logType.LogMsg);
                        }
                        else
                        {
                            Thread.Sleep(5000);
                        }
                    }
                    catch { }
                }
            });
        }

        public static void Log2(LogType logType)
        {
            LogQueue.Enqueue(logType);
        }

        private static void Log(string message)
        {
            Log(string.Empty, message);
        }

        private static void Log(string type, string message)
        {
            FileInfo FI = new FileInfo(path);
            string proName = FI.Name;
            string SName = proName.Substring(0, proName.LastIndexOf('.'));
            Log(type, SName, message);
        }

        private static void Log(string type, string logName, string message)
        {
            try
            {
                string m_szRunPath = AppDomain.CurrentDomain.BaseDirectory;
                string strLogPath = m_szRunPath + "\\log\\" + DateTime.Now.Year.ToString() + "\\" + DateTime.Now.Month.ToString() + "\\" + DateTime.Now.Day.ToString();
                if (!Directory.Exists(strLogPath))
                {
                    _ = Directory.CreateDirectory(strLogPath);
                }
                strLogPath = strLogPath + "\\" + logName + ".log";

                using (TextWriter tw = new StreamWriter(strLogPath, true))
                {
                    tw.WriteLine(type + "存档时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " ");
                    tw.WriteLine(message);
                    tw.WriteLine();
                    tw.Flush();
                    tw.Close();
                    //tw.Dispose();
                }
            }
            catch { }
        }
    }
}
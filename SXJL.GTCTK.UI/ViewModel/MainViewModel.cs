using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveCharts;
using RWS.Core;
using S7.Net;
using SqlSugar;
using SXJL.GTCTK.UI.Model;

namespace SXJL.GTCTK.UI.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        #region 基本信息
        /// <summary>
        /// 软件标题
        /// </summary>
        public string SoftTitle { get; } = ConfigurationManager.AppSettings.Get("SoftTitle");
        /// <summary>
        /// 版权
        /// </summary>
        public string Copyright { get; } = ConfigurationManager.AppSettings.Get("Copyright");

        public string ClientName { get; } = ConfigurationManager.AppSettings.Get("ClientName");

        public string IpAddress
        {
            get;
        }

        private DateTime _timeNow = DateTime.Now;
        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime TimeNow
        {
            get => _timeNow;
            set => SetProperty(ref _timeNow, value, nameof(TimeNow));
        }
        #endregion



        private readonly bool IsConnetDP = bool.Parse(ConfigurationManager.AppSettings.Get("IsConnectDP"));
        /// <summary>
        /// 大屏幕地址
        /// </summary>
        public string DpmIpAddress { get; } = ConfigurationManager.AppSettings.Get("DpmIpAddress");

        private string dpmViewTxt;

        public string DpmViewTxt
        {
            get => dpmViewTxt;
            set => SetProperty(ref dpmViewTxt, value, nameof(DpmViewTxt));
        }


        public string DbCon { get; } = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
        private Dpm5MK2Helper dpmHelper;
        private readonly StringBuilder dpmSb = new StringBuilder();
        private readonly Brush _dpmConnectedColor;
        public Brush DpmConnectedColor => _dpmConnectedColor;

        private Visibility isShowDnl = Visibility.Hidden;

        public Visibility IsShowDnl
        {
            get => isShowDnl;
            set => SetProperty(ref isShowDnl, value, nameof(IsShowDnl));
        }


        #region PLC

        private readonly List<PlcData> PlcDatas = new List<PlcData>();

        /// <summary>
        /// PLC地址
        /// </summary>
        public string PlcIpAddress { get; set; } = ConfigurationManager.AppSettings.Get("PlcIpAddress");
        public short Rack { get; private set; } = short.Parse(ConfigurationManager.AppSettings.Get("rack"));
        public short Slot { get; private set; } = short.Parse(ConfigurationManager.AppSettings.Get("slot"));
        public Plc Plc
        {
            get; private set;
        }

        private bool _isPlcOk;
        /// <summary>
        /// plc是否连接正常
        /// </summary>
        public bool IsPlcOk
        {
            get => _isPlcOk;
            set
            {
                _ = SetProperty(ref _isPlcOk, value, nameof(IsPlcOk));
                _ = SetProperty(ref _plcColor, value ? _trueColor : _falseColor, nameof(PlcColor));
            }
        }

        private Brush _plcColor;
        /// <summary>
        /// plc状态显示颜色
        /// </summary>
        public Brush PlcColor => _plcColor;

        private readonly Brush _trueColor = Brushes.Lime;
        private readonly Brush _falseColor = Brushes.Red;
        private readonly Brush _failColor = Brushes.DarkGray;
        /// <summary>
        /// 上炮	泥炮回转进--上炮	I0.0
        /// </summary>
        private readonly string sp = ConfigurationManager.AppSettings.Get("sp");
        private Brush _spColor;
        public Brush SpColor => _spColor;
        private bool _spValue;
        public bool SpValue
        {
            get => _spValue;
            set
            {
                _ = SetProperty(ref _spValue, value, nameof(SpValue));
                _ = SetProperty(ref _spColor, value ? _trueColor : _falseColor, nameof(SpColor));
            }
        }

        /// <summary>
        /// 回炮	泥炮回旋退--回炮	I0.1
        /// </summary>
        private readonly string hp = ConfigurationManager.AppSettings.Get("hp");
        private Brush _hpColor;
        public Brush HpColor => _hpColor;
        private bool _hpValue;
        public bool HpValue
        {
            get => _hpValue;
            set
            {
                _ = SetProperty(ref _hpValue, value, nameof(HpValue));
                _ = SetProperty(ref _hpColor, value ? _trueColor : _falseColor, nameof(HpColor));
            }
        }


        /// <summary>
        /// 打泥	泥炮打泥进入--打泥	I0.2
        /// </summary>
        private readonly string dn = ConfigurationManager.AppSettings.Get("dn");
        private Brush _dnColor;
        public Brush DnColor => _dnColor;
        private bool _dnValue;
        public bool DnValue
        {
            get => _dnValue;
            set
            {
                _ = SetProperty(ref _dnValue, value, nameof(DnValue));
                _ = SetProperty(ref _dnColor, value ? _trueColor : _falseColor, nameof(DnColor));
            }
        }


        /// <summary>
        /// 退泥	泥炮打泥退出--退泥	I0.3
        /// </summary>
        private readonly string tn = ConfigurationManager.AppSettings.Get("tn");
        private Brush _tnColor;
        public Brush TnColor => _tnColor;
        private bool _tnValue;
        public bool TnValue
        {
            get => _tnValue;
            set
            {
                _ = SetProperty(ref _tnValue, value, nameof(TnValue));
                _ = SetProperty(ref _tnColor, value ? _trueColor : _falseColor, nameof(TnColor));
            }
        }

        /// <summary>
        /// 转臂	开口机回转进--转臂	I0.4
        /// </summary>
        private readonly string zb = ConfigurationManager.AppSettings.Get("zb");
        private Brush _zbColor;
        public Brush ZbColor => _zbColor;
        private bool _zbValue;
        public bool ZbValue
        {
            get => _zbValue;
            set
            {
                _ = SetProperty(ref _zbValue, value, nameof(ZbValue));
                _ = SetProperty(ref _zbColor, value ? _trueColor : _falseColor, nameof(ZbColor));
            }
        }

        /// <summary>
        /// 回臂	开口机回转退出-回臂	I0.5
        /// </summary>
        private readonly string hb = ConfigurationManager.AppSettings.Get("hb");
        private Brush _hbColor;
        public Brush HbColor => _hbColor;
        private bool _hbValue;
        public bool HbValue
        {
            get => _hbValue;
            set
            {
                _ = SetProperty(ref _hbValue, value, nameof(HbValue));
                _ = SetProperty(ref _hbColor, value ? _trueColor : _falseColor, nameof(HbColor));
            }
        }

        /// <summary>
        /// 挂钩	开口机挂钩	I0.6
        /// </summary>
        private readonly string gg = ConfigurationManager.AppSettings.Get("gg");
        private Brush _ggColor;
        public Brush GgColor => _ggColor;
        private bool _ggValue;
        public bool GgValue
        {
            get => _ggValue;
            set
            {
                _ = SetProperty(ref _ggValue, value, nameof(GgValue));
                _ = SetProperty(ref _ggColor, value ? _trueColor : _falseColor, nameof(GgColor));
            }
        }

        /// <summary>
        /// 摘钩	开口机脱钩	I0.7
        /// </summary>
        private readonly string zg = ConfigurationManager.AppSettings.Get("zg");
        private Brush _zgColor;
        public Brush ZgColor => _zgColor;
        private bool _zgValue;
        public bool ZgValue
        {
            get => _zgValue;
            set
            {
                _ = SetProperty(ref _zgValue, value, nameof(ZgValue));
                _ = SetProperty(ref _zgColor, value ? _trueColor : _falseColor, nameof(ZgColor));
            }
        }

        /// <summary>
        /// 前进	开口机小车前进	I1.0
        /// </summary>
        private readonly string qj = ConfigurationManager.AppSettings.Get("qj");
        private Brush _qjColor;
        public Brush QjColor => _qjColor;
        private bool _qjValue;
        public bool QjValue
        {
            get => _qjValue;
            set
            {
                _ = SetProperty(ref _qjValue, value, nameof(QjValue));
                _ = SetProperty(ref _qjColor, value ? _trueColor : _falseColor, nameof(QjColor));
            }
        }

        /// <summary>
        /// 后退	开口机小车后退	I1.1
        /// </summary>
        private readonly string ht = ConfigurationManager.AppSettings.Get("ht");
        private Brush _htColor;
        public Brush HtColor => _htColor;
        private bool _htValue;
        public bool HtValue
        {
            get => _htValue;
            set
            {
                _ = SetProperty(ref _htValue, value, nameof(HtValue));
                _ = SetProperty(ref _htColor, value ? _trueColor : _falseColor, nameof(HtColor));
            }
        }

        /// <summary>
        /// 正转	开口机正转	I1.2
        /// </summary>
        private readonly string zz = ConfigurationManager.AppSettings.Get("zz");
        private Brush _zzColor;
        public Brush ZzColor => _zzColor;
        private bool _zzValue;
        public bool ZzValue
        {
            get => _zzValue;
            set
            {
                _ = SetProperty(ref _zzValue, value, nameof(ZzValue));
                _ = SetProperty(ref _zzColor, value ? _trueColor : _falseColor, nameof(ZzColor));
            }
        }

        /// <summary>
        /// 反转	开口机反转	I1.3
        /// </summary>
        private readonly string fz = ConfigurationManager.AppSettings.Get("fz");
        private Brush _fzColor;
        public Brush FzColor => _fzColor;
        private bool _fzValue;
        public bool FzValue
        {
            get => _fzValue;
            set
            {
                _ = SetProperty(ref _fzValue, value, nameof(FzValue));
                _ = SetProperty(ref _fzColor, value ? _trueColor : _falseColor, nameof(FzColor));
            }
        }

        /// <summary>
        /// 冲击	开口机冲击	I1.4
        /// </summary>
        private readonly string cj = ConfigurationManager.AppSettings.Get("cj");
        private Brush _cjColor;
        public Brush CjColor => _cjColor;
        private bool _cjValue;
        public bool CjValue
        {
            get => _cjValue;
            set
            {
                _ = SetProperty(ref _cjValue, value, nameof(CjValue));
                _ = SetProperty(ref _cjColor, value ? _trueColor : _falseColor, nameof(CjColor));
            }
        }

        /// <summary>
        /// 开口机建压	开口机建压	I1.5
        /// </summary>
        private readonly string jy = ConfigurationManager.AppSettings.Get("jy");
        private Brush _jyColor;
        public Brush JyColor => _jyColor;
        private bool _jyValue;
        public bool JyValue
        {
            get => _jyValue;
            set
            {
                _ = SetProperty(ref _jyValue, value, nameof(JyValue));
                _ = SetProperty(ref _jyColor, value ? _trueColor : _falseColor, nameof(JyColor));
            }
        }

        /// <summary>
        /// 泥炮补压	泥炮补压	I1.6
        /// </summary>
        private readonly string by = ConfigurationManager.AppSettings.Get("by");
        private Brush _byColor;
        public Brush ByColor => _byColor;
        private bool _byValue;
        public bool ByValue
        {
            get => _byValue;
            set
            {
                _ = SetProperty(ref _byValue, value, nameof(ByValue));
                _ = SetProperty(ref _byColor, value ? _trueColor : _falseColor, nameof(ByColor));
            }
        }


        /// <summary>
        /// 泥炮补压量
        /// </summary>
        private readonly string byv = ConfigurationManager.AppSettings.Get("byv");
        /// <summary>
        /// 泥炮补压数字量0-20毫安
        /// </summary>
        private double _byvSzl;
        public double ByvSzl
        {
            get => _byvSzl;
            set
            {
                _ = SetProperty(ref _byvSzl, value, nameof(ByvSzl));
                //计算

                _ = SetProperty(ref _byvReal, JsBy(value), nameof(ByvReal));
            }
        }

        private double JsBy(double value)
        {
            double result = value <= App.dnma4 ? App.dnma4Real : Math.Round((value - App.dnma4) / (App.dnma20 - App.dnma4) * (App.dnma20Real / 60), 2);
            return result;
        }

        private double _byvReal;
        /// <summary>
        /// 泥炮补压实际值
        /// </summary>
        public double ByvReal => _byvReal;

        /// <summary>
        /// 打泥量
        /// </summary>
        private readonly string dnv = ConfigurationManager.AppSettings.Get("dnv");
        /// <summary>
        /// 打泥量数字量0-20毫安
        /// </summary>
        private double _dnvSzl;
        public double DnvSzl
        {
            get => _dnvSzl;
            set
            {
                _ = SetProperty(ref _dnvSzl, value, nameof(DnvSzl));
                //计算
                _ = SetProperty(ref _dnvReal, JsDn(value), nameof(DnvReal));
            }
        }

        private double JsDn(double value)
        {
            double result;
            if (value <= App.dnma4)
            {
                result = App.dnma4Real;
            }
            else
            {
                double a = value - App.dnma4;
                double b = App.dnma20 - App.dnma4;
                double c = App.dnma20Real / 60;
                result = Math.Round(a / b * c, 2);
            }
            return result;
        }

        private double JsDn(double value, int second)
        {
            double result;
            if (value <= App.dnma4)
            {
                result = App.dnma4Real;
            }
            else
            {
                double a = value - (App.dnma4 * second);
                double b = App.dnma20 - App.dnma4;
                double c = App.dnma20Real / 60;
                result = Math.Round(a / b * c, 2);
            }
            return result;
        }

        private double GetCellByDnl(double value)
        {
            double a = JsDn(value);
            double b = App.Rj * 1000;
            return Math.Round(a * 5 / b, 2);
        }

        private double GetCellByDnl(double value, int second)
        {
            double a = JsDn(value, second);
            double b = App.Rj * 1000;
            return Math.Round(a * 5 / b, 2);
        }


        private double _dnvReal;
        /// <summary>
        /// 打泥量实际值
        /// </summary>
        public double DnvReal => _dnvReal;

        #endregion


        #region 计算逻辑用
        private GlState glState;

        private string _strState;
        /// <summary>
        /// 状态
        /// </summary>
        public string StrState
        {
            get => _strState;
            set => SetProperty(ref _strState, value, nameof(StrState));
        }

        private readonly StringBuilder _sbStateInfo = new StringBuilder();

        private string _strStateInfo;
        public string StrStateInfo
        {
            get => _strStateInfo;
            set => SetProperty(ref _strStateInfo, value, nameof(StrStateInfo));
        }

        private void SetStateInfo(string txt)
        {
            Log2Helper.Log2(new LogType { LogFileName = "PLC", LogLevelType = "info", LogMsg = $"当前状态：{glState}  PLC信号：{txt}" });
            //if (_sbStateInfo.Length > 5000)
            //{
            //    //Log2Helper.Log2(new LogType { LogFileName = "PLC", LogLevelType = "info", LogMsg = _sbStateInfo.ToString() });
            //    _sbStateInfo = new StringBuilder();
            //}
            //_ = _sbStateInfo.Insert(0, $"{TimeNow:yyyy-MM-dd HH:mm:ss}----{txt}{Environment.NewLine}");
            //StrStateInfo = _sbStateInfo.ToString();
        }


        #endregion

        #region 结果查询
        /// <summary>
        /// 查询记录
        /// </summary>
        public MyObservableCollection<Records> RecordsList { get; set; } = new MyObservableCollection<Records>();

        private int _recordMax = 100;
        public int RecordMax
        {
            get => _recordMax;
            set => SetProperty(ref _recordMax, value, nameof(RecordMax));
        }

        private int _recordIndex = 1;

        public int RecordIndex
        {
            get => _recordIndex < 1 ? 1 : _recordIndex;
            set
            {
                if (value > RecordMax)
                {
                    value = RecordMax;
                }

                _ = SetProperty(ref _recordIndex, value, nameof(RecordIndex));
            }
        }
        private readonly int pageSize = 11;
        /// <summary>
        /// 当前记录
        /// </summary>
        public RelayCommand GetNowRecords
        {
            get; set;
        }
        /// <summary>
        /// 前一页记录
        /// </summary>
        public RelayCommand GetPreviousRecords
        {
            get; set;
        }
        /// <summary>
        /// 后一页记录
        /// </summary>
        public RelayCommand GetNextRecords
        {
            get; set;
        }
        /// <summary>
        /// 获取记录，默认当前页
        /// </summary>
        private void GetRecords()
        {
            int totalCount = 0;
            using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
            {
                RecordMax = (int)Math.Ceiling((decimal)client.Queryable<Records>().Count() / pageSize);
                List<Records> list = client.Queryable<Records>().OrderBy(t => t.Createtime, OrderByType.Desc).ToPageList(RecordIndex, pageSize, ref totalCount).ToList();
                RecordsList.Clear();
                list.ForEach(item =>
                {
                    RecordsList.Add(item);
                });
            }
        }
        #endregion

        #region 铁次结果查询

        public MyObservableCollection<TcRecordsNew> TcRecordsViews { get; set; } = new MyObservableCollection<TcRecordsNew>();


        private int _recordMaxTc = 100;
        public int RecordMaxTc
        {
            get => _recordMaxTc;
            set => SetProperty(ref _recordMaxTc, value, nameof(RecordMaxTc));
        }


        private int _recordIndexTc = 1;
        public int RecordIndexTc
        {
            get => _recordIndexTc < 1 ? 1 : _recordIndexTc;
            set
            {
                if (value > RecordMaxTc)
                {
                    value = RecordMaxTc;
                }

                _ = SetProperty(ref _recordIndexTc, value, nameof(RecordIndexTc));
            }
        }
        private readonly int pageSizeTc = 9;
        /// <summary>
        /// 当前记录
        /// </summary>
        public RelayCommand GetNowRecordsTc
        {
            get; set;
        }
        /// <summary>
        /// 前一页记录
        /// </summary>
        public RelayCommand GetPreviousRecordsTc
        {
            get; set;
        }
        /// <summary>
        /// 后一页记录
        /// </summary>
        public RelayCommand GetNextRecordsTc
        {
            get; set;
        }
        /// <summary>
        /// 获取记录，默认当前页
        /// </summary>
        private void GetRecordsTc()
        {
            int totalCount = 0;
            TcRecordsViews.Clear();
            using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
            {
                int t1 = client.Queryable<TcRecordsNew>().Count();
                decimal t2 = decimal.Parse(t1.ToString());
                RecordMaxTc = (int)Math.Ceiling(t2 / pageSizeTc);
                List<TcRecordsNew> list = client.Queryable<TcRecordsNew>().OrderBy(r => r.TcNo, OrderByType.Desc).ToPageList(RecordIndexTc, pageSizeTc, ref totalCount).ToList();
                list.ForEach(item =>
                {
                    TcRecordsViews.Add(item);
                });
            }
        }
        #endregion

        #region 程序启动时需要获取当前状态，取数据库中的实时数据判断当前大概在什么状态
        /// <summary>
        /// 获取程序运行之前的实时数据，超过XX分钟的不要
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<PlcData> GetLastPlcDatas(int count, int lastMinutes, DateTime noTime)
        {
            DateTime lastTime = noTime - new TimeSpan(0, lastMinutes, 0);
            using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
            {
                return client.Queryable<PlcData>().Where(t => t.CreateTime >= lastTime && t.CreateTime <= noTime).OrderBy(t => t.CreateTime, OrderByType.Desc).Take(count).ToList().OrderBy(t => t.CreateTime).ToList();
            }
        }

        /// <summary>
        /// 获取最后的铁次记录，后面再结果plc数据判断程序启动前的状态
        /// </summary>
        /// <returns></returns>
        private TcRecordsNew GetLastTcRecord()
        {
            using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
            {
                return client.Queryable<TcRecordsNew>().Where(t => t.Time1KtkBegin != null).OrderBy(t => t.Time1KtkBegin, OrderByType.Desc).First();
            }
        }

        #endregion

        private void PlcOpen()
        {
            if (NetHelper.Ping(PlcIpAddress))
            {
                Plc = new Plc(CpuType.S7200Smart, PlcIpAddress, Rack, Slot);
                try
                {
                    Plc.Open();
                    IsPlcOk = true;
                }
                catch (Exception ex)
                {
                    IsPlcOk = false;
                    Log2Helper.Log2(new LogType() { LogFileName = "PLC通信错误", LogLevelType = "错误", LogMsg = ex.Message + Environment.NewLine + ex.StackTrace });
                }

            }
            else
            {
                IsPlcOk = false;
                Log2Helper.Log2(new LogType() { LogFileName = "PLC通信错误", LogLevelType = "错误", LogMsg = "Ping不通" });
            }
        }

        #region 打泥瞬时值曲线

        public ChartValues<double> DnQxCollection { get; set; } = new ChartValues<double>();

        private void AddDnQxCollection(double value)
        {
            if (DnQxCollection.Count > 100)
            {
                DnQxCollection.RemoveAt(0);
            }
            if (value >= 0)
            {
                DnQxCollection.Add(value);
            }
        }
        #endregion

        private bool isTestInitOk = false;

        #region 打泥测试，要求打泥值显示在大屏幕上，显示打泥量和打泥格数，测试时有打泥信号，没有上炮信号
        public Visibility IsTestDnVisibility => App.IsTestDn ? Visibility.Visible : Visibility.Hidden;
        private string testDnState = TestDn.打泥测试未开始.ToString();

        public string TestDnState
        {
            get => testDnState;
            set => SetProperty(ref testDnState, value, nameof(TestDnState));
        }

        private bool isTestDnBegin = false;
        private double testDnReal = 0;
        private double testDnCell = 0;
        public RelayCommand TestDnBegin
        {
            get; private set;
        }
        public RelayCommand TestDnEnd
        {
            get; private set;
        }
        private void DoTestDnBegin(object o)
        {
            isTestDnBegin = true;
            testDnReal = testDnCell = 0;
            TestDnState = TestDn.开始打泥测试.ToString();
        }
        private void DoTestDnEnd(object o)
        {
            isTestDnBegin = false;
            TestDnState = TestDn.打泥测试未开始.ToString();
        }
        #endregion

        public MainViewModel()
        {
            SetStateInfo("程序启动中……");
            #region 初始化
            #region 实时数据颜色初始化
            _spColor = _hpColor = _dnColor = _tnColor = _zbColor = _hbColor = _ggColor = _zgColor = _qjColor = _htColor = _zzColor = _fzColor = _cjColor = _jyColor = _byColor = _failColor;
            _dpmConnectedColor = _plcColor = _falseColor;
            #endregion
            string[] strs = new string[2];
            Array.Copy(PlcIpAddress.Split('.'), 0, strs, 0, 2);
            IpAddress = NetHelper.GetIP(string.Join(".", strs));
            InitRecords();
            #endregion

            #region 查询用

            RecordIndex = 1;
            GetRecords();

            GetNowRecords = new RelayCommand(o =>
            {
                RecordIndex = 1;
                GetRecords();
            });

            GetPreviousRecords = new RelayCommand(o =>
            {
                RecordIndex--;
                GetRecords();
            });

            GetNextRecords = new RelayCommand(o =>
            {
                RecordIndex++;
                GetRecords();
            });
            #endregion

            #region 铁次查询用

            RecordIndexTc = 1;
            GetRecordsTc();

            GetNowRecordsTc = new RelayCommand(o =>
            {
                RecordIndexTc = 1;
                GetRecordsTc();
            });

            GetPreviousRecordsTc = new RelayCommand(o =>
            {
                RecordIndexTc--;
                GetRecordsTc();
            });

            GetNextRecordsTc = new RelayCommand(o =>
            {
                RecordIndexTc++;
                GetRecordsTc();
            });
            #endregion

            if (IsConnetDP)
            {
                dpmHelper = new Dpm5MK2Helper(DpmIpAddress);
                if (!dpmHelper.IsConnect)
                {
                    Log2Helper.Log2(new LogType { LogFileName = "Dpm", LogLevelType = "Error", LogMsg = "连接大屏幕失败" });
                }
                _ = SetProperty(ref _dpmConnectedColor, dpmHelper.IsConnect ? _trueColor : _falseColor, nameof(DpmConnectedColor));
            }

            _ = Task.Run(() =>
            {
                while (true)
                {
                    if (!IsTest)
                    {
                        TimeNow = DateTime.Now;
                    }
                    Thread.Sleep(1000);
                }
            });
            PlcOpen();

            _ = Task.Run(() =>
            {
                while (true)
                {
                    if (PlcDatas.Count >= 1500)
                    {
                        PlcDatas.RemoveRange(0, 500);
                    }
                    if (IsTest && DateTime.TryParse(TestTime, out DateTime testDateTime))
                    {
                        if (!isTestInitOk)
                        {
                            PlcDatas.Clear();
                            PlcDatas.AddRange(GetLastPlcDatas(500, 20, testDateTime));
                            isTestInitOk = true;
                        }
                        PlcJs(testDateTime, out PlcData plcData);
                        PlcDatas.Add(plcData);
                        Thread.Sleep(1000);
                    }
                    else if (IsPlcOk && Plc.IsConnected)
                    {
                        Log2Helper.Log2(new LogType { LogFileName = "TimeTest", LogLevelType = "info", LogMsg = "plcBegin" });
                        PlcData plcData = new PlcData();
                        #region 读取PLC各个节点的数值
                        if (bool.TryParse(Plc.Read(sp).ToString(), out bool spV))
                        {
                            SpValue = spV;
                            plcData.SpValue = spV;
                        }

                        if (bool.TryParse(Plc.Read(hp).ToString(), out bool hpV))
                        {
                            HpValue = hpV;
                            plcData.HpValue = hpV;
                        }

                        if (bool.TryParse(Plc.Read(dn).ToString(), out bool dnV))
                        {
                            DnValue = dnV;
                            plcData.DnValue = dnV;
                        }

                        if (bool.TryParse(Plc.Read(tn).ToString(), out bool tnV))
                        {
                            TnValue = tnV;
                            plcData.TnValue = tnV;
                        }

                        if (bool.TryParse(Plc.Read(zb).ToString(), out bool zbV))
                        {
                            ZbValue = zbV;
                            plcData.ZbValue = zbV;
                        }

                        if (bool.TryParse(Plc.Read(hb).ToString(), out bool hbV))
                        {
                            HbValue = hbV;
                            plcData.HbValue = hbV;
                        }

                        if (bool.TryParse(Plc.Read(gg).ToString(), out bool ggV))
                        {
                            GgValue = ggV;
                            plcData.GgValue = ggV;
                        }

                        if (bool.TryParse(Plc.Read(zg).ToString(), out bool zgV))
                        {
                            ZgValue = zgV;
                            plcData.ZgValue = zgV;
                        }

                        if (bool.TryParse(Plc.Read(qj).ToString(), out bool qjV))
                        {
                            QjValue = qjV;
                            plcData.QjValue = qjV;
                        }

                        if (bool.TryParse(Plc.Read(ht).ToString(), out bool htV))
                        {
                            HtValue = htV;
                            plcData.HtValue = htV;
                        }

                        if (bool.TryParse(Plc.Read(zz).ToString(), out bool zzV))
                        {
                            ZzValue = zzV;
                            plcData.ZzValue = zzV;
                        }

                        if (bool.TryParse(Plc.Read(fz).ToString(), out bool fzV))
                        {
                            FzValue = fzV;
                            plcData.FzValue = fzV;
                        }

                        if (bool.TryParse(Plc.Read(cj).ToString(), out bool cjV))
                        {
                            CjValue = cjV;
                            plcData.CjValue = cjV;
                        }

                        if (bool.TryParse(Plc.Read(jy).ToString(), out bool jyV))
                        {
                            JyValue = jyV;
                            plcData.JyValue = jyV;
                        }

                        if (bool.TryParse(Plc.Read(by).ToString(), out bool byV))
                        {
                            ByValue = byV;
                            plcData.ByValue = byV;
                        }

                        if (double.TryParse(Plc.Read(byv).ToString(), out double byvV))
                        {
                            ByvSzl = byvV;
                            plcData.ByvSzlValue = byvV;
                            plcData.ByvRealValue = ByvReal;
                        }

                        if (double.TryParse(Plc.Read(dnv).ToString(), out double dnvV))
                        {
                            DnvSzl = dnvV;
                            plcData.DnvSzlValue = dnvV;
                            plcData.DnvRealValue = DnvReal;
                        }
                        #endregion
                        Log2Helper.Log2(new LogType { LogFileName = "TimeTest", LogLevelType = "info", LogMsg = "plcReadEnd" });
                        using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
                        {
                            _ = client.Insertable(plcData).ExecuteCommand();
                        }
                        Log2Helper.Log2(new LogType { LogFileName = "TimeTest", LogLevelType = "info", LogMsg = "plcSaveEnd" });
                        PlcJs(plcData);
                        Log2Helper.Log2(new LogType { LogFileName = "TimeTest", LogLevelType = "info", LogMsg = "plcJsEnd" });
                        PlcDatas.Add(plcData);
                        Log2Helper.Log2(new LogType { LogFileName = "TimeTest", LogLevelType = "info", LogMsg = "plcAddListEnd" });
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        //AddDnQxCollection(Math.Round((new Random().NextDouble() * 200), 3));
                        //报警plc连接错误报警
                        Log2Helper.Log2(new LogType { LogFileName = "PLC连接错误", LogLevelType = "Error", LogMsg = "plc连接错误报警" });
                        Thread.Sleep(1000 * 30);
                        PlcOpen();
                    }
                }
            });
            #region 大屏幕输出

            _ = Task.Run(() =>
            {
                while (true)
                {
                    string dpmTxt = string.Empty;
                    if (App.IsTestDn)
                    {
                        //打泥测试显示大屏幕
                        dpmTxt = $"{TestDnState}$打泥量{Math.Round(testDnReal, 2)}升$打泥格数{Math.Round(testDnCell, 2)}格";
                    }
                    else
                    {
                        if (glState == GlState.等铁)
                        {
                            /*
                            第3行  显示目前的状态
                            等铁：
                            第4行  堵铁口结束时刻
                            第5行  打泥时长（静态）
                            第6行  打泥升数（静态）
                            第7行  炮膛容积 250升
                            第8行  打泥格数（静态）
                            第9行  炮泥耗量（静态）
                             */
                            var dtkEnd = string.Empty;

                            if (tcRecords != null)
                            {
                                if (tcRecords.Time3DtkEnd.HasValue)
                                {
                                    dtkEnd=tcRecords.Time3DtkEnd.Value.ToString("HH:mm");
                                }
                                else
                                {
                                    using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
                                    {
                                        tcRecordsOld = client.Queryable<TcRecordsNew>().Where(t => t.Time3DtkEnd != null).OrderBy(t => t.TcNo, OrderByType.Desc).First();
                                        dtkEnd = tcRecordsOld.Time3DtkEnd.Value.ToString("HH:mm");
                                    }
                                }
                            }
                            dpmTxt = $"{glState}$堵铁口结束 {dtkEnd}$打泥时长 {ViewHoldtime}秒$打泥容积 {ViewUsageDN}L$炮膛容积 250L$打泥格数 {ViewCell}$炮泥耗量 {ViewWeight}Kg";
                        }
                        else
                        {
                            /*
                            第3行  显示目前的状态
                            堵铁口开始：
                            第4行  堵铁口开始时刻
                            第5行  打泥时长（随动）
                            第6行  打泥升数（随动）
                            第7行  炮膛容积 250升
                            第8行  打泥格数（随动）
                            第9行  炮泥耗量（随动）*/
                            var dtkBegin = string.Empty;
                            if (tcRecords != null)
                            {
                                if (tcRecords.Time3DtkBegin.HasValue)
                                {
                                    dtkBegin = tcRecords.Time3DtkBegin.Value.ToString("HH:mm");
                                }
                                else
                                {
                                    using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
                                    {
                                        tcRecordsOld = client.Queryable<TcRecordsNew>().Where(t => t.Time3DtkBegin != null).OrderBy(t => t.TcNo, OrderByType.Desc).First();
                                        dtkBegin = tcRecordsOld.Time3DtkBegin.Value.ToString("HH:mm");
                                    }
                                }
                            }
                            dpmTxt = $"{glState}$堵铁口开始 {dtkBegin}$打泥时长 {ViewHoldtime}秒$打泥容积 {ViewUsageDN}L$炮膛容积 250L$打泥格数 {ViewCell}$炮泥耗量 {ViewWeight}Kg";
                        }
                    }
                    bool result = dpmHelper.SendTxt(dpmTxt, out string tmp, 0);
                    if (!result)
                    {
                        dpmHelper.DisConnect();
                        Thread.Sleep(50);
                        dpmHelper = new Dpm5MK2Helper(DpmIpAddress);
                    }

                    StringBuilder vTmp = new StringBuilder();
                    for (int i = 0; i < tmp.Length / 12; i++)
                    {
                        _ = vTmp.AppendLine(tmp.Substring(i * 12, 12));
                    }
                    string strResult = result ? "成功" : "失败";
                    DpmViewTxt = vTmp.ToString() + $"发送屏幕结果：{strResult}";
                    Thread.Sleep(App.DpmRefTime);
                }
            });
            #endregion

            SetStateInfo("系统初始化完成，开始判断状态");
            #region 判断高炉当前状态
            TcRecordsNew lastTcRecord = GetLastTcRecord();
            List<PlcData> lastPlcDatas = GetLastPlcDatas(500, 5, TimeNow);
            PlcDatas.AddRange(lastPlcDatas);
            tcRecords = new TcRecordsNew() { TcNo = MakeTcNo(), DtkCout = 1 };
            if (lastTcRecord != null)
            {
                if (lastTcRecord.Time3DtkBegin != null)
                {
                    SetStateInfo("上次运行记录已经开始堵铁口，验证中");
                    if ((TimeNow - lastTcRecord.Time3DtkBegin.Value).TotalMinutes > 5)
                    {
                        SetStateInfo($"堵铁口时间差大于5分钟，状态【{GlState.待判断}】");
                        StrState = GlState.待判断.ToString();
                    }
                    else
                    {
                        SetStateInfo($"堵铁口时间差小于5分钟。状态【{GlState.等铁}】");
                        //需要plc实时数据详细判断
                        StrState = GlState.等铁.ToString();
                        tcRecords = lastTcRecord;
                    }
                }
                else if (lastTcRecord.Time2CtBegin != null)
                {
                    SetStateInfo("上次运行记录已经开始出铁，验证中");
                    if ((TimeNow - lastTcRecord.Time2CtBegin.Value).TotalMinutes > 60)
                    {
                        SetStateInfo($"出铁时间时间差大于60分钟，状态【{GlState.待判断}】");
                        StrState = GlState.待判断.ToString();
                    }
                    else
                    {
                        SetStateInfo($"出铁时间时间差小于60分钟。状态【{GlState.出铁}】");
                        StrState = GlState.出铁.ToString();
                        tcRecords = lastTcRecord;
                    }
                }
                else if (lastTcRecord.Time1KtkBegin != null)
                {
                    SetStateInfo("上次运行记录已开铁口，验证中");
                    if ((TimeNow - lastTcRecord.Time1KtkBegin.Value).TotalMinutes > 5)
                    {
                        SetStateInfo($"开铁口时间差大于5分钟，状态【{GlState.待判断}】");
                        StrState = GlState.待判断.ToString();
                    }
                    else
                    {
                        SetStateInfo($"开铁口时间差小于5分钟。状态为【{GlState.出铁}】");
                        StrState = GlState.出铁.ToString();
                        tcRecords = lastTcRecord;
                    }
                }
                else if (lastTcRecord.Time4DtBegin != null)
                {
                    SetStateInfo("上次运行记录等铁，验证中");
                    if ((TimeNow - lastTcRecord.Time4DtBegin.Value).TotalMinutes > 30)
                    {
                        SetStateInfo($"等铁时间时间差大于30分钟，状态为【{GlState.待判断}】");
                        StrState = GlState.待判断.ToString();
                    }
                    else
                    {
                        SetStateInfo($"等铁时间时间差小于30分钟。初步为【{GlState.等铁}】");
                        StrState = GlState.等铁.ToString();
                        tcRecords = lastTcRecord;
                    }
                }
            }
            else
            {
                SetStateInfo($"没有上次运行记录。当前状态【{GlState.待判断}】");
                StrState = GlState.待判断.ToString();
            }
            #endregion
            #region 测试打泥
            TestDnBegin = new RelayCommand(DoTestDnBegin);
            TestDnEnd = new RelayCommand(DoTestDnEnd);
            #endregion
        }
        /// <summary>
        /// 产生铁次，按照北/南侧+炉号+年月日+2位流水号。例如N12023030601
        /// </summary>
        /// <returns></returns>
        private string MakeTcNo()
        {
            string nb = string.Empty;
            nb = ClientName.Contains("北") ? "N" : "S";
            string lc = ClientName.Substring(0, 1);

            string tcBegin = $"{nb}{lc}{TimeNow:yyyyMMdd}";
            using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
            {
                TcRecordsNew last = client.Queryable<TcRecordsNew>().Where(t => t.TcNo.StartsWith(tcBegin)).OrderBy(t => t.TcNo, OrderByType.Desc).First();
                if (last == null)
                {
                    return tcBegin + "01";
                }
                else
                {
                    string oldTc = last.TcNo.Substring(tcBegin.Length, 2);
                    string oldTcEnd = (int.Parse(oldTc) + 1).ToString().PadLeft(2, '0');
                    return tcBegin + oldTcEnd;
                }
            }
        }

        private double _viewHoldtime;

        public double ViewHoldtime
        {
            get => _viewHoldtime;
            set => SetProperty(ref _viewHoldtime, value, nameof(ViewHoldtime));
        }

        private double _viewCell;

        public double ViewCell
        {
            get => _viewCell;
            set => SetProperty(ref _viewCell, value, nameof(ViewCell));
        }

        private double _viewUsageDN;

        public double ViewUsageDN
        {
            get => _viewUsageDN;
            set
            {
                SetProperty(ref _viewUsageDN, Math.Round(value), nameof(ViewUsageDN));
                SetProperty(ref _viewWeight, Math.Round(value * App.Scale, 0), nameof(ViewWeight));
            }
        }

        private double _viewWeight;

        public double ViewWeight
        {
            get => _viewWeight;
            set => SetProperty(ref _viewWeight, value, nameof(ViewWeight));
        }


        private void InitRecords()
        {
            using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
            {
                TcRecordsNew r = client.Queryable<TcRecordsNew>().OrderBy(t => t.TcNo, OrderByType.Desc).First();
                if (r != null)
                {
                    ViewHoldtime = r.Time3Dtk ?? 0;
                    ViewUsageDN = r.UsageDN ?? 0;
                    ViewCell = r.Cell ?? 0;
                }
                else
                {
                    ViewHoldtime = 0;
                    ViewUsageDN = 0;
                    ViewCell = 0;
                }
            }
        }
        private TcRecordsNew tcRecords;
        private TcRecordsNew tcRecordsOld;

        private DateTime timeDtk2Time;
        #region plc计算用的临时变量
        /// <summary>
        /// 是否有转臂
        /// </summary>
        private bool isZb = false;
        /// <summary>
        /// 是否有挂钩
        /// </summary>
        private bool isGg = false;

        /// <summary>
        /// 是否有回臂
        /// </summary>
        private bool isHb = false;
        /// <summary>
        /// 是否有摘钩
        /// </summary>
        private bool isZg = false;

        private void InitPlcTmpData()
        {
            isZb = isGg = isHb = isZg = false;
        }
        #endregion

        private void PlcJs(PlcData plcData)
        {
            #region 逻辑计算
            if (PlcDatas != null && PlcDatas.Count > 0)
            {
                PlcData oldData = PlcDatas[PlcDatas.Count - 1];
                if (oldData != null)
                {
                    #region 各种动作输出
                    if (oldData.SpValue != plcData.SpValue)
                    {
                        SetStateInfo(plcData.SpValue ? "上炮信号开始" : "上炮信号停止");
                    }
                    if (oldData.HpValue != plcData.HpValue)
                    {
                        SetStateInfo(plcData.HpValue ? "回炮信号开始" : "回炮信号停止");
                    }
                    if (oldData.DnValue != plcData.DnValue)
                    {
                        SetStateInfo(plcData.DnValue ? "打泥信号开始" : "打泥信号停止");
                    }
                    if (oldData.TnValue != plcData.TnValue)
                    {
                        SetStateInfo(plcData.TnValue ? "退泥信号开始" : "退泥信号停止");
                    }
                    if (oldData.ZbValue != plcData.ZbValue)
                    {
                        SetStateInfo(plcData.ZbValue ? "转臂信号开始" : "转臂信号停止");
                    }
                    if (oldData.HbValue != plcData.HbValue)
                    {
                        SetStateInfo(plcData.HbValue ? "回臂信号开始" : "回臂信号停止");
                    }
                    if (oldData.GgValue != plcData.GgValue)
                    {
                        SetStateInfo(plcData.GgValue ? "挂钩信号开始" : "挂钩信号停止");
                    }
                    if (oldData.ZgValue != plcData.ZgValue)
                    {
                        SetStateInfo(plcData.ZgValue ? "摘钩信号开始" : "摘钩信号停止");
                    }
                    if (oldData.QjValue != plcData.QjValue)
                    {
                        SetStateInfo(plcData.QjValue ? "前进信号开始" : "前进信号停止");
                    }
                    if (oldData.HtValue != plcData.HtValue)
                    {
                        SetStateInfo(plcData.HtValue ? "后退信号开始" : "后退信号停止");
                    }
                    if (oldData.ZzValue != plcData.ZzValue)
                    {
                        SetStateInfo(plcData.ZzValue ? "正转信号开始" : "正转信号停止");
                    }
                    if (oldData.FzValue != plcData.FzValue)
                    {
                        SetStateInfo(plcData.FzValue ? "反转信号开始" : "反转信号停止");
                    }
                    if (oldData.CjValue != plcData.CjValue)
                    {
                        SetStateInfo(plcData.CjValue ? "冲击信号开始" : "冲击信号停止");
                    }
                    if (oldData.JyValue != plcData.JyValue)
                    {
                        SetStateInfo(plcData.JyValue ? "建压信号开始" : "建压信号停止");
                    }
                    if (oldData.ByValue != plcData.ByValue)
                    {
                        SetStateInfo(plcData.ByValue ? "补压信号开始" : "补压信号停止");
                    }
                    #endregion

                    #region 等铁_预开铁口
                    try
                    {
                        if ((glState == GlState.等铁 && plcData.JyValue && !oldData.JyValue) ||
                            (glState == GlState.待判断 && plcData.JyValue && !oldData.JyValue))
                        {
                            glState = GlState.等铁_预开铁口;
                            StrState = glState.ToString();
                            SetStateInfo($"{glState}");
                            InitPlcTmpData();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log2Helper.Log2(new LogType { LogFileName = "等铁_预开铁口判断失败", LogLevelType = "error", LogMsg = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" });
                    }
                    #endregion

                    #region 开铁口开始
                    try
                    {
                        if (glState == GlState.等铁_预开铁口)
                        {
                            if (plcData.ZbValue)
                            {
                                isZb = true;
                            }
                            if (plcData.GgValue)
                            {
                                isGg = true;
                            }
                            if ((plcData.ZzValue && plcData.CjValue && plcData.JyValue && isZb && isGg) || (isZb && PlcDatas.Where(t => t.CreateTime < TimeNow.AddMinutes(-5)).Count(t => t.JyValue && t.ZzValue && t.CjValue) >= 5))
                            {
                                InitPlcTmpData();
                                glState = GlState.开铁口开始;
                                StrState = glState.ToString();
                                SetStateInfo($"{GlState.等铁}--结束");
                                SetStateInfo($"{glState}--开始");
                                using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
                                {
                                    TcRecordsNew oldTcRecords = client.Queryable<TcRecordsNew>().OrderBy(t => t.TcNo, OrderByType.Desc).First();
                                    if (oldTcRecords != null)
                                    {
                                        _ = client.Updateable<TcRecordsNew>().SetColumns(t => new TcRecordsNew { Time4DtEnd = TimeNow }).Where(t => t.Id == oldTcRecords.Id).ExecuteCommand();
                                    }
                                    tcRecords = new TcRecordsNew() { Time1KtkBegin = TimeNow, TcNo = MakeTcNo() };
                                    _ = client.Insertable(tcRecords).ExecuteCommand();
                                    tcRecords.DtkCout = 1;
                                }
                                GetRecordsTc();

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log2Helper.Log2(new LogType { LogFileName = "开铁口开始判断失败", LogLevelType = "error", LogMsg = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" });
                    }
                    #endregion

                    #region 开铁口成功
                    try
                    {
                        if (glState == GlState.开铁口开始)
                        {
                            if (plcData.ZgValue)
                            {
                                isZg = true;
                            }
                            if (plcData.HbValue)
                            {
                                isHb = true;
                            }
                            if (tcRecords.Time1KtkEnd == null && (isHb || (!plcData.CjValue && !plcData.ZzValue && isZg)))
                            {
                                InitPlcTmpData();
                                glState = GlState.开铁口成功;
                                StrState = glState.ToString();
                                SetStateInfo($"{GlState.开铁口开始}--结束");
                                SetStateInfo($"{glState}");
                                tcRecords.Time1KtkEnd = TimeNow;
                                using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
                                {
                                    _ = client.Updateable<TcRecordsNew>().SetColumns(t => new TcRecordsNew { Time1KtkEnd = TimeNow }).Where(t => t.Id == tcRecords.Id).ExecuteCommand();
                                }
                                GetRecordsTc();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log2Helper.Log2(new LogType { LogFileName = "开铁口成功判断失败", LogLevelType = "error", LogMsg = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" });
                    }
                    #endregion

                    #region 出铁(开铁口结束)
                    try
                    {
                        if (glState == GlState.开铁口成功)
                        {
                            if (plcData.HbValue)
                            {
                                isHb = true;
                            }
                            if (!plcData.JyValue && !plcData.CjValue && !plcData.ZzValue && tcRecords.Time2CtBegin == null && isHb)
                            {
                                glState = GlState.出铁;
                                StrState = glState.ToString();
                                SetStateInfo($"{GlState.开铁口开始}--结束");
                                SetStateInfo($"{glState}--开始");
                                tcRecords.Time2CtBegin = TimeNow;
                                using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
                                {
                                    _ = client.Updateable<TcRecordsNew>().SetColumns(t => new TcRecordsNew { Time2CtBegin = TimeNow }).Where(t => t.Id == tcRecords.Id).ExecuteCommand();
                                }
                                GetRecordsTc();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log2Helper.Log2(new LogType { LogFileName = "出铁判断失败", LogLevelType = "error", LogMsg = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" });
                    }
                    #endregion

                    #region 堵泥口测试
                    try
                    {
                        if (isTestDnBegin)
                        {
                            if (plcData.DnValue)
                            {
                                testDnReal += plcData.DnvRealValue;
                                testDnCell += GetCellByDnl(plcData.DnvSzlValue);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Log2Helper.Log2(new LogType { LogFileName = "堵泥口测试出现错误", LogLevelType = "error", LogMsg = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" });
                    }

                    #endregion

                    #region 堵泥口
                    try
                    {
                        if (glState == GlState.堵铁口 && plcData.SpValue && plcData.DnValue)
                        {
                            AddDnQxCollection(plcData.DnvRealValue);
                        }
                        if ((glState == GlState.出铁 || glState == GlState.待判断 || glState == GlState.堵铁口) && plcData.SpValue && tcRecords.Time3DtkEnd == null)
                        {
                            if (plcData.DnValue && tcRecords.Time3DtkBegin.HasValue)
                            {
                                List<PlcData> old = PlcDatas.Where(t => t.CreateTime >= tcRecords.Time3DtkBegin.Value && t.DnValue && t.SpValue).ToList();
                                double dnSum = Math.Round(old.Sum(t => t.DnvRealValue) + plcData.DnvRealValue, 2);
                                double dnSzlSum = old.Sum(t => t.DnvSzlValue) + plcData.DnvSzlValue;
                                double cell = GetCellByDnl(dnSzlSum, old.Count);

                                ViewHoldtime = old.Count;
                                ViewUsageDN = dnSum;
                                ViewCell = cell;
                            }
                            //第一次打泥
                            if (tcRecords.Time3DtkBegin == null && plcData.DnValue)
                            {
                                //第一次开始打泥
                                using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
                                {
                                    DnQxCollection.Clear();
                                    AddDnQxCollection(plcData.DnvRealValue);

                                    SetStateInfo($"{GlState.出铁}--结束");
                                    glState = GlState.堵铁口;
                                    StrState = glState.ToString();
                                    SetStateInfo($"{GlState.堵铁口}--开始");
                                    IsShowDnl = Visibility.Visible;
                                    tcRecords.Time3DtkBegin = tcRecords.Time2CtEnd = TimeNow;
                                    tcRecords.DtkCout = 1;


                                    ViewHoldtime = 1;
                                    ViewUsageDN = Math.Round(plcData.DnvRealValue, 2);
                                    ViewCell = GetCellByDnl(plcData.DnvSzlValue);

                                    TcRecordsNew oldTcRecord = client.Queryable<TcRecordsNew>().Where(t => t.Id == tcRecords.Id).First();
                                    _ = oldTcRecord != null
                                        ? client.Updateable<TcRecordsNew>().SetColumns(t => new TcRecordsNew { Time3DtkBegin = TimeNow, Time2CtEnd = TimeNow, DtkCout = 1 }).Where(t => t.Id == tcRecords.Id).ExecuteCommand()
                                        : client.Insertable(tcRecords).ExecuteCommand();
                                }
                                GetRecordsTc();
                            }
                            else if (glState == GlState.堵铁口)
                            {
                                if (!oldData.DnValue && plcData.DnValue)
                                {
                                    //开始打泥
                                    timeDtk2Time = TimeNow;
                                }
                                else if (!plcData.DnValue && oldData.DnValue)
                                {
                                    //停止打泥
                                    if (tcRecords.DtkCout == 1)
                                    {
                                        //第一次次停止打泥
                                        List<PlcData> old = PlcDatas.Where(t => t.CreateTime >= tcRecords.Time2CtEnd.Value && t.DnValue && t.SpValue).ToList();
                                        double dnSum = old.Sum(t => t.DnvRealValue) + plcData.DnvRealValue;
                                        double cell = GetCellByDnl(old.Sum(t => t.DnvSzlValue) + plcData.DnvSzlValue, old.Count + 1);
                                        using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
                                        {
                                            Records records = new Records { Id = Guid.NewGuid(), TcNo = tcRecords.TcNo, Createtime = tcRecords.Time3DtkBegin.Value, Stoptime = TimeNow, Holdtime = (int)(TimeNow - tcRecords.Time3DtkBegin.Value).TotalSeconds, UsageDN = dnSum, Cell = cell, Avgspeed = 1 };
                                            _ = client.Insertable(records).ExecuteCommand();
                                        }
                                        tcRecords.DtkCout = 2;
                                        GetRecords();
                                        GetRecordsTc();
                                    }
                                    else
                                    {
                                        //后面几次打泥
                                        using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
                                        {
                                            Records oldRecords = client.Queryable<Records>().Where(t => t.TcNo == tcRecords.TcNo).OrderBy(t => t.Createtime, OrderByType.Desc).First();

                                            List<PlcData> old = PlcDatas.Where(t => t.CreateTime > oldRecords.Stoptime && t.DnValue && t.SpValue).ToList();
                                            double dnSum = old.Sum(t => t.DnvRealValue);
                                            double cell = GetCellByDnl(old.Sum(t => t.DnvSzlValue), old.Count);
                                            Records records = new Records { Id = Guid.NewGuid(), TcNo = tcRecords.TcNo, Createtime = timeDtk2Time, Stoptime = TimeNow, Holdtime = (int)(TimeNow - timeDtk2Time).TotalSeconds, UsageDN = dnSum, Cell = cell, Avgspeed = 2 };
                                            _ = client.Insertable(records).ExecuteCommand();
                                        }
                                        GetRecords();
                                        GetRecordsTc();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log2Helper.Log2(new LogType { LogFileName = "堵泥口判断失败", LogLevelType = "error", LogMsg = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" });
                    }
                    #endregion

                    #region 等铁/堵铁口结束
                    try
                    {
                        if (glState == GlState.堵铁口 && !plcData.SpValue && !plcData.DnValue)
                        {
                            List<PlcData> old = PlcDatas.Where(t => t.CreateTime > TimeNow.AddHours(-1)).ToList();
                            if (old.Count(t => t.DnValue && t.SpValue) >= 5)
                            {
                                glState = GlState.等铁;
                                StrState = glState.ToString();
                                SetStateInfo($"{GlState.堵铁口}--结束");
                                SetStateInfo($"{glState}--开始");
                                IsShowDnl = Visibility.Hidden;
                                tcRecords.Time4DtBegin = tcRecords.Time3DtkEnd = TimeNow;
                                using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
                                {
                                    List<Records> recordsJs = client.Queryable<Records>().Where(t => t.TcNo == tcRecords.TcNo).ToList();

                                    ViewHoldtime = recordsJs.Sum(x => x.Holdtime);
                                    ViewUsageDN = Math.Round(recordsJs.Sum(x => x.UsageDN), 2);
                                    ViewCell = Math.Round(recordsJs.Sum(x => x.Cell), 2);

                                    _ = client.Updateable<TcRecordsNew>().SetColumns(t => new TcRecordsNew { Time4DtBegin = TimeNow, Time3DtkEnd = TimeNow, Time3Dtk = recordsJs.Sum(x => x.Holdtime), DtkCout = recordsJs.Count, Cell = recordsJs.Sum(x => x.Cell), UsageDN = recordsJs.Sum(x => x.UsageDN) }).Where(t => t.Id == tcRecords.Id).ExecuteCommand();
                                }
                                RecordIndexTc = 1;
                                GetRecordsTc();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log2Helper.Log2(new LogType { LogFileName = "等铁判断失败", LogLevelType = "error", LogMsg = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" });
                    }
                    #endregion
                }

            }
            #endregion
        }

        private bool isTest;

        public bool IsTest
        {
            get => isTest;
            set => SetProperty(ref isTest, value, nameof(IsTest));
        }

        private string testTime;

        public Visibility IsTestState => App.IsTestState ? Visibility.Visible : Visibility.Hidden;

        public string TestTime
        {
            get => testTime;
            set => SetProperty(ref testTime, value, nameof(TestTime));
        }

        /// <summary>
        /// 调试计算过程
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="plcData"></param>
        private void PlcJs(DateTime beginTime, out PlcData plcData)
        {
            DateTime time = DateTime.Parse(TestTime);
            TimeNow = time;
            using (ISqlSugarClient client = new SqlSugarClient(App.GetConnection()))
            {
                plcData = client.SqlQueryable<PlcData>($"select top 1 * from PlcData where CreateTime>'{TimeNow:yyyy-MM-dd HH:mm:ss}' order by CreateTime").First();
                if (plcData == null)
                {
                    return;
                }
                plcData.DnvRealValue = JsDn(plcData.DnvSzlValue);
            }
            PlcJs(plcData);
            time = time.AddSeconds(1);
            TestTime = time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void Exit()
        {
            Log2Helper.Log2(new LogType { LogFileName = "程序退出", LogLevelType = "info", LogMsg = _sbStateInfo.ToString() });
            if (dpmHelper != null && dpmHelper.IsConnect)
            {
                dpmHelper.DisConnect();
            }
            if (Plc != null && Plc.IsConnected)
            {
                IsPlcOk = false;
                Plc.Close();
            }
        }
    }
}

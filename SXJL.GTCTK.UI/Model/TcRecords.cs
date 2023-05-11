using System;

namespace SXJL.GTCTK.UI.Model
{
    internal class TcRecordsNew
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 铁次，按照北/南侧+炉号+年月日+2位流水号
        /// </summary>
        public string TcNo
        {
            get; set;
        }
        /// <summary>
        /// 开铁口开始时间
        /// </summary>
        public DateTime? Time1KtkBegin
        {
            get; set;
        }

        /// <summary>
        /// 开铁口结束时间
        /// </summary>
        public DateTime? Time1KtkEnd
        {
            get; set;
        }

        /// <summary>
        /// 开铁口时长
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string Time1Ktk => Time1KtkBegin.HasValue && Time1KtkEnd.HasValue
                    ? $"{Math.Round((Time1KtkEnd.Value - Time1KtkBegin.Value).TotalSeconds)}秒"
                    : string.Empty;

        /// <summary>
        /// 出铁开始时间
        /// </summary>
        public DateTime? Time2CtBegin
        {
            get; set;
        }

        /// <summary>
        /// 出铁结束时间
        /// </summary>
        public DateTime? Time2CtEnd
        {
            get; set;
        }

        /// <summary>
        /// 出铁时长
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string Time2Ct => Time2CtBegin.HasValue && Time2CtEnd.HasValue
                    ? $"{Math.Round((Time2CtEnd.Value - Time2CtBegin.Value).TotalMinutes)}分钟"
                    : string.Empty;
        /// <summary>
        /// 堵铁口开始时间
        /// </summary>
        public DateTime? Time3DtkBegin
        {
            get; set;
        }

        /// <summary>
        /// 堵铁口结束时间
        /// </summary>
        public DateTime? Time3DtkEnd
        {
            get; set;
        }

        /// <summary>
        /// 堵铁口（打泥）时长
        /// </summary>
        public double? Time3Dtk
        {
            get; set;
        }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string Time3DtkStr
        {
            get
            {
                string tmp = $"{Time3Dtk}秒";
                return tmp == "秒" ? string.Empty : tmp;
            }
        }

        /// <summary>
        /// 等铁开始时间
        /// </summary>
        public DateTime? Time4DtBegin
        {
            get; set;
        }

        /// <summary>
        /// 等铁结束时间
        /// </summary>
        public DateTime? Time4DtEnd
        {
            get; set;
        }


        /// <summary>
        /// 等铁时长
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string Time4Dt => Time4DtEnd.HasValue && Time3DtkBegin.HasValue ? $"{Math.Round((Time4DtEnd.Value - Time3DtkBegin.Value).TotalMinutes)}分钟" : string.Empty;

        /// <summary>
        /// 打泥次数
        /// </summary>
        public int? DtkCout
        {
            get; set;
        }
        /// <summary>
        /// 打泥量
        /// </summary>
        public double? UsageDN
        {
            get; set;
        }
        /// <summary>
        /// 打泥格数
        /// </summary>
        public double? Cell
        {
            get; set;
        }
    }

}

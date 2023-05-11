using System;

namespace SXJL.GTCTK.UI
{
    internal class Records
    {
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public int RowIndex
        {
            get; set;
        }
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 铁次号
        /// </summary>
        public string TcNo
        {
            get; set;
        }
        /// <summary>
        /// 打泥开始时间
        /// </summary>
        public DateTime Createtime
        {
            get; set;
        }
        /// <summary>
        /// 打泥时长
        /// </summary>
        public double Holdtime
        {
            get; set;
        }
        /// <summary>
        /// 打泥停止时间
        /// </summary>
        public DateTime Stoptime
        {
            get; set;
        }
        /// <summary>
        /// 打泥量
        /// </summary>
        public double UsageDN
        {
            get; set;
        }

        /// <summary>
        /// 小格
        /// </summary>
        public double Cell
        {
            get; set;
        }
        /// <summary>
        /// 均速
        /// </summary>
        public double Avgspeed
        {
            get; set;
        }
        public string Remark
        {
            get; set;
        }
        public string Remark_change
        {
            get; set;
        }
    }
}

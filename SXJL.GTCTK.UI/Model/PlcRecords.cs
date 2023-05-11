using System;

namespace SXJL.GTCTK.UI.Model
{
    /// <summary>
    /// plc实时数据推算结果
    /// </summary>
    internal class PlcRecords
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
        /// 动作类型
        /// </summary>
        public string RecordName
        {
            get; set;
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime RecordBeginTime
        {
            get; set;
        }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? RecordEndTime
        {
            get; set;
        }
        /// <summary>
        /// 状态，属于4个状态中的那个状态（等铁、开铁口、出铁、堵铁口）
        /// </summary>
        public string TheState
        {
            get; set;
        }
    }
}

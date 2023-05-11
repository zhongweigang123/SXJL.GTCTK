using System;

namespace SXJL.GTCTK.UI.Model
{
    public class PlcData
    {
        public PlcData()
        {
            Id = Guid.NewGuid();
            CreateTime = DateTime.Now;
        }
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public Guid Id
        {
            get; set;
        }
        public DateTime CreateTime
        {
            get; private set;
        }
        /// <summary>
        /// 上炮	泥炮回转进--上炮	I0.0
        /// </summary>
        public bool SpValue
        {
            get; set;
        }
        /// <summary>
        /// 回炮	泥炮回旋退--回炮	I0.1
        /// </summary>
        public bool HpValue
        {
            get; set;
        }
        /// <summary>
        /// 打泥	泥炮打泥进入--打泥	I0.2
        /// </summary>
        public bool DnValue
        {
            get; set;
        }
        /// <summary>
        /// 退泥	泥炮打泥退出--退泥	I0.3
        /// </summary>
        public bool TnValue
        {
            get; set;
        }
        /// <summary>
        /// 转臂	开口机回转进--转臂	I0.4
        /// </summary>
        public bool ZbValue
        {
            get; set;
        }
        /// <summary>
        /// 回臂	开口机回转退出-回臂	I0.5
        /// </summary>
        public bool HbValue
        {
            get; set;
        }
        /// <summary>
        /// 挂钩	开口机挂钩	I0.6
        /// </summary>
        public bool GgValue
        {
            get; set;
        }
        /// <summary>
        /// 摘钩	开口机脱钩	I0.7
        /// </summary>
        public bool ZgValue
        {
            get; set;
        }
        /// <summary>
        /// 前进	开口机小车前进	I1.0
        /// </summary>
        public bool QjValue
        {
            get; set;
        }
        /// <summary>
        /// 后退	开口机小车后退	I1.1
        /// </summary>
        public bool HtValue
        {
            get; set;
        }
        /// <summary>
        /// 正转	开口机正转	I1.2
        /// </summary>
        public bool ZzValue
        {
            get; set;
        }
        /// <summary>
        /// 反转	开口机反转	I1.3
        /// </summary>
        public bool FzValue
        {
            get; set;
        }
        /// <summary>
        /// 冲击	开口机冲击	I1.4
        /// </summary>
        public bool CjValue
        {
            get; set;
        }
        /// <summary>
        /// 开口机建压	开口机建压	I1.5
        /// </summary>
        public bool JyValue
        {
            get; set;
        }
        /// <summary>
        /// 泥炮补压	泥炮补压	I1.6
        /// </summary>
        public bool ByValue
        {
            get; set;
        }
        /// <summary>
        /// 泥炮补压		PIW1（数字量）
        /// </summary>
        public double ByvSzlValue
        {
            get; set;
        }
        /// <summary>
        /// 泥炮补压		PIW1（换算量）
        /// </summary>
        public double ByvRealValue
        {
            get; set;
        }
        /// <summary>
        /// 打泥量		PIW2（数字量）
        /// </summary>
        public double DnvSzlValue
        {
            get; set;
        }
        /// <summary>
        /// 打泥量		PIW2（换算量）
        /// </summary>
        public double DnvRealValue
        {
            get; set;
        }
    }
}

namespace SXJL.GTCTK.UI.Model
{
    /// <summary>
    /// 一个周期，4个状态，具体名字写在配置文件，参考：等铁---开铁口---出铁---堵铁口
    /// </summary>
    internal enum GlState
    {
        待判断 = 0, 等铁 = 1, 开铁口开始 = 2, 出铁 = 3, 堵铁口 = 4, 等铁_预开铁口 = 6, 开铁口成功 = 7
    }

    internal enum GlStateNew
    {
        待判断 = 0, 开铁口开始 = 1, 开铁口结束 = 2, 出铁开始 = 3, 出铁结束 = 4, 堵铁口开始 = 5, 堵铁口结束 = 6, 等铁开始 = 7, 等铁结束 = 8
    }

    internal enum TestDn
    {
        打泥测试未开始 = 0, 开始打泥测试, 测试中有打泥信号
    }
}

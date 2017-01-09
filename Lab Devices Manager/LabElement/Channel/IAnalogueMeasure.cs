using System;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 表示包含采集器可测量、采集的接口。
    /// </summary>
    public interface IAnalogueMeasure : IUnitRange
    {
        /// <summary>
        /// 获取/设置采集的采样率。
        /// </summary>
        int Frequence { get; set; }
        /// <summary>
        /// 获取/设置采集数据值。
        /// </summary>
        double MeasureValue { get; set; }
        /// <summary>
        /// 获取/设置采集器对象。
        /// </summary>
        object Collector { get; set; }
        /// <summary>
        /// 当可测量对象的采集器改变时发生。
        /// </summary>
        event Action<object, MesureCollectorChangedEventArgs> CollectorChanged;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.EException
{
    /// <summary>
    /// 异常状态类型，状态的改变只能从Active到另一种状态的改变。
    /// </summary>
    [Serializable]
    public enum EExcepState
    {
        /// <summary>
        /// 试验异常活跃，尝未进行处理。
        /// </summary>
        Active,

        /// <summary>
        /// 已经处理。
        /// </summary>
        Deal,

        /// <summary>
        /// 已终止。
        /// </summary>
        Abort,

        /// <summary>
        /// 忽视本次异常。
        /// </summary>
        Ignore,
    }

    /// <summary>
    /// 异常发生的情况。
    /// </summary>
    [Serializable]
    public enum EExcepStatus
    {
        /// <summary>
        /// 已发生一次。
        /// </summary>
        Onece,

        /// <summary>
        /// 已发生多次。
        /// </summary>        
        Many,

        /// <summary>
        /// 表示异常一直连续发生。
        /// </summary>
        Continue,
    }

    /// <summary>
    /// 异常的处理方式。
    /// </summary>
    public enum EExcepDealStyle
    {
        /// <summary>
        /// 未知的处理方式。
        /// </summary>
        Unknown,

        /// <summary>
        /// 异常发现源自身逻辑处理。
        /// </summary>
        SourceLogic,

        /// <summary>
        /// 人为处理。
        /// </summary>
        Factitious,
    }

    /// <summary>
    /// 异常发生事件传递参数。
    /// </summary>
    public class EExceptionHappenedEventArgs : EventArgs, IEExceptionAppear
    {
        public EExceptionHappenedEventArgs(EException eexc)
        {
            EExceptionInfor = (EException)eexc.Clone();
            AppearTime = DateTime.Now;
        }

        /// <summary>
        /// 获取产生的基本异常信息。
        /// </summary>
        public EException EExceptionInfor { get; private set; }

        /// <summary>
        /// 异常源。
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// 异常发生的位置。
        /// </summary>
        public object Position { get; set; }

        /// <summary>
        /// 异常的初始源头，如异常检测通道。
        /// </summary>
        public object OriginalSource { get; set; }

        /// <summary>
        /// 异常的发生时间。
        /// </summary>
        public DateTime AppearTime { get; private set; }

    }

    /// <summary>
    /// 包含试验异常出现信息。
    /// </summary>
    [Serializable]
    public class EExcepAppearInfor : IEExceptionAppear, ISerializable
    {
        public EExcepAppearInfor(DateTime appearTime)
        {
            AppearTime = appearTime;
        }

        public EExcepAppearInfor()
        {
            AppearTime = DateTime.Now;
        }

        #region Properties

        /// <summary>
        /// 异常的发生时间。
        /// </summary>
        public DateTime AppearTime
        {
            get;
            private set;
        }
        /// <summary>
        /// 异常的初始源头，如异常检测通道。
        /// </summary>
        public object OriginalSource
        {
            get;
            set;
        }

        /// <summary>
        /// 异常发生的位置。
        /// </summary>
        public object Position
        {
            get;
            set;
        }
        /// <summary>
        /// 异常源。
        /// </summary>
        public object Source
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 从一个IEExceptionAppear复制信息内容。
        /// </summary>
        /// <param name="src">源对象。</param>
        public void CopyFrom(IEExceptionAppear src)
        {
            AppearTime = src.AppearTime;
            Position = src.Position;
            Source = src.Source;
            OriginalSource = src.OriginalSource;
        }

        #region ISerializable

        protected EExcepAppearInfor(SerializationInfo infor, StreamingContext context)
        {
            AppearTime = infor?.GetDateTime("AppearTime") ?? DateTime.Now;

            Position = infor?.GetString("Position");
            Source = infor?.GetString("Source");
            OriginalSource = infor?.GetString("OriginalSource");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info?.AddValue("AppearTime", AppearTime);
            // 在序列化过程中只将对象的格式化字符串形式。
            info?.AddValue("Position", Position.ToString());
            info?.AddValue("Source", Source.ToString());
            info?.AddValue("OrginalSource", OriginalSource.ToString());
        } 
        #endregion
    }
}

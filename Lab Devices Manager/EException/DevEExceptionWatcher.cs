using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.EException
{
    /// <summary>
    /// 监测器报告异常事件参数。
    /// </summary>
    public class WatcherReportEExceptionEventArgs : EExceptionHappenedEventArgs
    {
        public WatcherReportEExceptionEventArgs(EExceptionHappenedEventArgs e, object dealLogic = null)
            : base(e.EExceptionInfor)
        {
            DealLogic = dealLogic;
            Source = e.Source;
            OriginalSource = e.OriginalSource;
            Position = e.Position;
        }

        /// <summary>
        /// 获取通过DevEExceptionWatcher为异常指定的处理逻辑对象。
        /// </summary>
        public object DealLogic { get; private set; }
    }

    /// <summary>
    /// 用于设备对象中的试验异常监测逻辑，管理设备对象中的所有ChannelExceptionSrc对象。
    /// </summary>
    public class DevEExceptionWatcher
    {
        public DevEExceptionWatcher()
        {
            _cheSrcs = new Dictionary<ChannelExceptionSrc, object>();
        }

        // 用于保存ChannelExceptionSrc与其处理逻辑对象的集合。
        Dictionary<ChannelExceptionSrc, object> _cheSrcs;

        /// <summary>
        /// 增加一个通道异常源，并可为其指定一个处理逻辑对象。
        /// </summary>
        /// <param name="ces">通道异常源。</param>
        /// <param name="dealLogic">指定的处理逻辑。</param>
        public void AddExceptionSrc(ChannelExceptionSrc ces, object dealLogic = null)
        {
            _cheSrcs.Add(ces, dealLogic);
            ces.EExcepHappened += EExcepHappened;
        }

        private void EExcepHappened(object sender, EExceptionHappenedEventArgs e)
        {
            // 如果事件源不在集合中则不产生事件。
            object logic;
            if (_cheSrcs.TryGetValue(sender as ChannelExceptionSrc, out logic))
            {
                e.Source = sender;
                ReportEException?.Invoke
                    (this,
                    new WatcherReportEExceptionEventArgs(e, logic)
                    );
            }
        }

        /// <summary>
        /// 当有异常出现时发生。
        /// </summary>
        public event EventHandler<WatcherReportEExceptionEventArgs> ReportEException;
    }
}

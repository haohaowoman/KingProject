using LabMCESystem.LabElement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.EException
{    
    /// <summary>
    /// 带有通道异常触发器异常源，实现对异常的检测。
    /// </summary>
    public class ChannelExceptionSrc : EException
    {
        public ChannelExceptionSrc(EException ebase):this()
        {
            ebase.CopyTo(this);
        }

        public ChannelExceptionSrc()
        {
            Triggers = new ObservableCollection<ExcepTrigger>();
            Triggers.CollectionChanged += Triggers_CollectionChanged;
        }

        private void Triggers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        // 添加事件
                        foreach (var nItem in e.NewItems)
                        {
                            ExcepTrigger et = nItem as ExcepTrigger;
                            if (et != null)
                            {
                                et.Triggered += Triggered;
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        // 移除事件
                        foreach (var oItem in e.OldItems)
                        {
                            ExcepTrigger et = oItem as ExcepTrigger;
                            if (et != null)
                            {
                                et.Triggered -= Triggered;
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    {
                        // 移除事件
                        foreach (var oItem in e.OldItems)
                        {
                            ExcepTrigger et = oItem as ExcepTrigger;
                            if (et != null)
                            {
                                et.Triggered -= Triggered;
                            }
                        }
                        // 添加事件
                        foreach (var nItem in e.NewItems)
                        {
                            ExcepTrigger et = nItem as ExcepTrigger;
                            if (et != null)
                            {
                                et.Triggered += Triggered;
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                        // 移除事件
                        foreach (var oItem in e.OldItems)
                        {
                            ExcepTrigger et = oItem as ExcepTrigger;
                            if (et != null)
                            {
                                et.Triggered -= Triggered;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Triggered(object sender, ExceptTriggeredEventArgs e)
        {
            EExcepHappened?.Invoke(this,
                new EExceptionHappenedEventArgs(this)
                {
                    Source = sender,
                    OriginalSource = e.Source,
                    Position = (e.Source as Channel)?.OwnerDevice
                });
        }

        /// <summary>
        /// 获取/设置试验异常的触发器集合。
        /// </summary>
        public ObservableCollection<ExcepTrigger> Triggers { get; private set; }

        /// <summary>
        /// 异常发生时调用。
        /// </summary>
        public event EventHandler<EExceptionHappenedEventArgs> EExcepHappened;
    }
}

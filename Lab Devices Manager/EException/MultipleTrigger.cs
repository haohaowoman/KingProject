using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace LabMCESystem.EException
{
    /// <summary>
    /// 需要满足多个触发条件的触发器。
    /// </summary>
    public sealed class MultipleTrigger : ExcepTrigger
    {
        public MultipleTrigger()
        {
            Predicates = new ObservableCollection<ExcepTrigger>();
            Predicates.CollectionChanged += Predicates_CollectionChanged;
        }

        public ObservableCollection<ExcepTrigger> Predicates { get; private set; }

        public override bool IsAction
        {
            get
            {
                bool action = Predicates.Count == 0 ? false : true;

                foreach (var p in Predicates)
                {
                    action &= p.IsAction;
                }

                return action;
            }
        }

        #region Operators

        private void Predicates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
                                et.Triggered += Predicates_Triggered;
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
                                et.Triggered -= Predicates_Triggered;
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
                                et.Triggered -= Predicates_Triggered;
                            }
                        }
                        // 添加事件
                        foreach (var nItem in e.NewItems)
                        {
                            ExcepTrigger et = nItem as ExcepTrigger;
                            if (et != null)
                            {
                                et.Triggered += Predicates_Triggered;
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
                                et.Triggered -= Predicates_Triggered;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Predicates_Triggered(object sender, ExceptTriggeredEventArgs e)
        {
            OnTrigger(e.Source);
        }

        #endregion
    }
}

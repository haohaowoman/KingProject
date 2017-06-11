using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.EException;
namespace ExpRuntimeApp.Modules
{
    class ExceptionActionModule : IEExcepAction, INotifyPropertyChanged
    {
        #region Properties

        private EExcepAction _action;
        /// <summary>
        /// 获取/设置异常行为。
        /// </summary>
        public EExcepAction Action
        {
            get { return _action; }
            set
            {
                if (_action != null)
                {
                    _action.AppearedAgain -= ExcepAction_AppearedAgain;
                    _action.StateChanged -= ExcepAction_StateChanged;
                }

                if (value != null)
                {
                    value.AppearedAgain += ExcepAction_AppearedAgain;
                    value.StateChanged += ExcepAction_StateChanged;
                }

                NotifyUpdateProperties();
                _action = value;
            }
        }

        // 实现基本属性。
        public string Label
        {
            get
            {
                return _action?.Label;
            }
        }

        public string Summary
        {
            get
            {
                return _action?.Summary;
            }
        }

        public string DealOpinion
        {
            get
            {
                return _action?.DealOpinion;
            }
        }

        public EExcepType ExcepType
        {
            get
            {
                return _action?.ExcepType ?? EExcepType.Warning;
            }
        }

        public string ExcepTypeStr
        {
            get
            {
                if (_action != null)
                {
                    string str = null;
                    switch (_action.ExcepType)
                    {
                        case EExcepType.Warning:
                            str = "警告";
                            break;
                        case EExcepType.Fault:
                            str = "故障";
                            break;
                        default:
                            break;
                    }
                    return str;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int AppearCount
        {
            get
            {
                return ((IEExcepAction)Action)?.AppearCount ?? 0;
            }
        }

        public IReadOnlyList<IEExceptionAppear> Appears
        {
            get
            {
                return ((IEExcepAction)Action)?.Appears;
            }
        }

        public EExcepDealStyle DealStyle
        {
            get
            {
                return ((IEExcepAction)Action)?.DealStyle ?? EExcepDealStyle.Unknown;
            }
        }

        public string DealStyleStr
        {
            get
            {
                if (_action != null)
                {
                    string str = string.Empty;
                    switch (DealStyle)
                    {
                        case EExcepDealStyle.Unknown:
                            str = "未知";
                            break;
                        case EExcepDealStyle.SourceLogic:
                            str = "程序逻辑处理";
                            break;
                        case EExcepDealStyle.Factitious:
                            str = "人为处理";
                            break;
                        default:
                            break;
                    }
                    return str;
                }
                else
                {
                    return null;
                }
            }
        }

        public DateTime DealTime
        {
            get
            {
                return ((IEExcepAction)Action)?.DealTime ?? DateTime.UtcNow;
            }
        }

        public DateTime FirstAppearTime
        {
            get
            {
                return ((IEExcepAction)Action)?.FirstAppearTime ?? DateTime.UtcNow;
            }
        }

        public bool IsActive
        {
            get
            {
                return ((IEExcepAction)Action)?.IsActive ?? false;
            }
        }

        public IEExceptionAppear LastAppearInfo
        {
            get
            {
                return ((IEExcepAction)Action)?.LastAppearInfo;
            }
        }

        public EExcepState State
        {
            get
            {
                return ((IEExcepAction)Action)?.State ?? EExcepState.Abort;
            }
        }

        public string StateStr
        {
            get
            {
                if (_action != null)
                {
                    string str = null;
                    switch (State)
                    {
                        case EExcepState.Active:
                            str = "活动";
                            break;
                        case EExcepState.Deal:
                            str = "已处理";
                            break;
                        case EExcepState.Abort:
                            str = "终止";
                            break;
                        case EExcepState.Ignore:
                            str = "已忽略";
                            break;
                        default:
                            break;
                    }
                    return str;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public EExcepStatus Status
        {
            get
            {
                return ((IEExcepAction)Action)?.Status ?? EExcepStatus.Onece;
            }
        }

        public string StatusStr
        {
            get
            {
                if (_action != null)
                {
                    string str = null;
                    switch (Status)
                    {
                        case EExcepStatus.Onece:
                            str = "一次";
                            break;
                        case EExcepStatus.Many:
                            str = "多次出现";
                            break;
                        case EExcepStatus.Continue:
                            str = "连续出现";
                            break;
                        default:
                            break;
                    }
                    return str;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        #endregion

        #region Operators

        private void ExcepAction_StateChanged(object sender, EExcepStateChangedEventArgs e)
        {
            NotifyUpdateProperties();
        }

        private void ExcepAction_AppearedAgain(object sender, EExcepAppearAgainEventArgs e)
        {
            NotifyUpdateProperties();
        }

        public void NotifyUpdateProperties()
        {
            if (_action == null)
            {
                return;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Label)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Summary)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DealOpinion)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppearCount)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Appears)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DealStyle)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DealTime)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstAppearTime)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsActive)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastAppearInfo)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExcepTypeStr)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StateStr)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusStr)));
        }

        #endregion
    }
}

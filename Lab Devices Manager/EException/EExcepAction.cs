using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.EException
{
    /// <summary>
    /// 试验异常状态改变事件参数。
    /// </summary>
    public class EExcepStateChangedEventArgs : EventArgs
    {
        public EExcepStateChangedEventArgs(EExcepState nState, EExcepDealStyle dealStyle = EExcepDealStyle.SourceLogic)
        {
            NewState = nState;
            DealStyle = dealStyle;
        }
        /// <summary>
        /// 获取异常的新状态。
        /// </summary>
        public EExcepState NewState { get; private set; }

        /// <summary>
        /// 获取改变异常状态的处理方式。
        /// </summary>
        public EExcepDealStyle DealStyle { get; private set; }

    }

    /// <summary>
    /// 异常出现事件参数。
    /// </summary>
    public class EExcepAppearAgainEventArgs : EventArgs
    {
        public EExcepAppearAgainEventArgs(IEExceptionAppear nApInfo)
        {
            NewEExcepAppearInfo = nApInfo;
        }

        public IEExceptionAppear NewEExcepAppearInfo { get; private set; }
    }

    /// <summary>
    /// 试验异常行为，包括有试验异常基本信息，产生信息，处理方式等行为。
    /// </summary>
    [Serializable]
    public class EExcepAction : EException, IEExcepAction
    {
        public EExcepAction(IEExceptionAppear eaInfo = null)
        {
            _appears = new List<IEExceptionAppear>(MaxCapacity);
            if (eaInfo == null)
            {
                AppearAgain(new EExcepAppearInfor());
            }
            else
            {
                AppearAgain(eaInfo);
            }

            State = EExcepState.Active;
        }

        public EExcepAction(EException ee, IEExceptionAppear eaInfo = null) : this(eaInfo)
        {
            ee.CopyTo(this);
        }

        #region Fields

        /// <summary>
        /// 状态改变锁。
        /// </summary>
        [NonSerialized]
        private object _stateLocker = new object();

        /// <summary>
        /// 异常出现锁。
        /// </summary>
        [NonSerialized]
        private object _appearLocker = new object();
        #endregion

        #region Properties

        private EExcepState _state;
        /// <summary>
        /// 获取异常的当前状态。
        /// </summary>
        public EExcepState State
        {
            get { return _state; }
            private set { _state = value; }
        }

        private EExcepStatus _status;
        /// <summary>
        /// 获取异常的出现情形。
        /// </summary>
        public EExcepStatus Status
        {
            get { return _status; }
            private set { _status = value; }
        }

        private EExcepDealStyle _dealStyle;
        /// <summary>
        /// 设置/获取异常的处理方式。
        /// </summary>
        public EExcepDealStyle DealStyle
        {
            get { return _dealStyle; }
            private set { _dealStyle = value; }
        }

        private List<IEExceptionAppear> _appears;
        /// <summary>
        /// 获取已出现异常产生信息集合。
        /// </summary>
        public IReadOnlyList<IEExceptionAppear> Appears
        {
            get { return _appears.AsReadOnly(); }
        }

        private int _appearCount;
        /// <summary>
        /// 获取异常的出现次数。
        /// </summary>
        public int AppearCount
        {
            get { return _appearCount; }
            private set { _appearCount = value; }
        }

        private int _maxCapacity = _contineStatusCritical;
        /// <summary>
        /// 获取/设置最大的异常产生信息储存量。
        /// </summary>
        public int MaxCapacity
        {
            get { return _maxCapacity; }
            set { _maxCapacity = value; }
        }

        /// <summary>
        /// 获取最近一次异常的产生信息。
        /// </summary>
        public IEExceptionAppear LastAppearInfo
        {
            get
            {
                return _appears.Last();
            }
        }

        /// <summary>
        /// 确定异常是否处于激活状态。
        /// </summary>
        public bool IsActive
        {
            get
            {
                return State == EExcepState.Active;
            }
        }

        /// <summary>
        /// 获取异常第一次出现的时间。
        /// </summary>
        public DateTime FirstAppearTime { get; private set; }

        /// <summary>
        /// 获取异常被处理的时间。
        /// </summary>
        public DateTime DealTime { get; private set; }

        private static int _contineStatusCritical = 15;
        /// <summary>
        /// 获取/设置异常情形转化为Continue的边界值。
        /// </summary>
        public static int ContinueStatusCritical
        {
            set
            {
                _contineStatusCritical = Math.Max(2, value);
            }
            get
            {
                return _contineStatusCritical;
            }
        }

        /// <summary>
        /// 当异常的状态改变时发生。
        /// </summary>
        public event EventHandler<EExcepStateChangedEventArgs> StateChanged;

        /// <summary>
        /// 当此异常再次出现时发生。
        /// </summary>
        public event EventHandler<EExcepAppearAgainEventArgs> AppearedAgain;
        #endregion

        #region Operators

        /// <summary>
        /// 当异常再次出现时调用。
        /// </summary>
        /// <param name="eea"></param>
        public void AppearAgain(IEExceptionAppear eea)
        {
            if (IsActive)
            {
                lock (_appearLocker)
                {
                    int count = _appears.Count;

                    if (count >= MaxCapacity)
                    {
                        for (int i = count - 1; i >= MaxCapacity - 1; i--)
                        {
                            _appears.RemoveAt(i);
                        }
                    }

                    var nInfo = new EExcepAppearInfor();
                    if (eea != null)
                    {
                        nInfo.CopyFrom(eea);
                    }
                    
                    _appears.Add(nInfo);

                    _appearCount++;
                    if (_appearCount == 1)
                    {
                        _status = EExcepStatus.Onece;
                        FirstAppearTime = eea.AppearTime;
                    }
                    else if (_appearCount > 1 && _appearCount < _contineStatusCritical)
                    {
                        _status = EExcepStatus.Many;
                    }
                    else
                    {
                        _status = EExcepStatus.Continue;
                    }

                    AppearedAgain?.Invoke(this, new EExcepAppearAgainEventArgs(nInfo));
                }
            }
        }

        /// <summary>
        /// 调用以处理异常。
        /// </summary>
        /// <param name="dealStyle">指定异常的处理方式。</param>
        public bool DealEException(EExcepDealStyle dealStyle = EExcepDealStyle.SourceLogic)
        {
            return ChangeEExcepState(EExcepState.Deal, dealStyle);
        }

        /// <summary>
        /// 调用以终止异常。
        /// </summary>
        /// <param name="dealStyle">指定异常的处理方式。</param>
        public bool AbortEException(EExcepDealStyle dealStyle = EExcepDealStyle.SourceLogic)
        {
            return ChangeEExcepState(EExcepState.Abort, dealStyle);
        }

        /// <summary>
        /// 调用以忽略异常。
        /// </summary>
        /// <param name="dealStyle"></param>
        public bool IgnorEException(EExcepDealStyle dealStyle = EExcepDealStyle.SourceLogic)
        {
            return ChangeEExcepState(EExcepState.Ignore, dealStyle);
        }

        /// <summary>
        /// 改变异常的当前状态，当异常处理了一次之后将不能再次处理。
        /// </summary>
        /// <param name="nState">指定的新状态。</param>
        /// <param name="dealStyle">处理方式。</param>
        public bool ChangeEExcepState(EExcepState nState, EExcepDealStyle dealStyle = EExcepDealStyle.SourceLogic)
        {
            bool rb = true;
            lock (_stateLocker)
            {
                if (IsActive)
                {
                    State = nState;
                    DealStyle = dealStyle;

                    DealTime = DateTime.Now;

                    StateChanged?.Invoke(this, new EExcepStateChangedEventArgs(State, dealStyle));
                }
                else
                {
                    rb = false;
                }
            }
            return rb;
        }

        #endregion
    }
}

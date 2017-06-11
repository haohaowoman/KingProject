using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.EException
{
    /// <summary>
    /// 包含了试验运行时的试验常管理任务。
    /// </summary>
    public class EExceptionManager : IExcepManagement
    {
        public EExceptionManager()
        {
            _activeExceps = new List<EExcepAction>();
            _handledExceps = new List<EExcepAction>();
        }
        #region Fields

        // 活跃的异常集合
        private List<EExcepAction> _activeExceps;

        // 已处理过的异常集合
        private List<EExcepAction> _handledExceps;

        public event EventHandler<ActivatedEExceptionEventArgs> ActivatedEException;
        public event EventHandler<HandledEExceptionEventArgs> HandledEException;

        #endregion

        #region IExcepManagement


        public int ActiveEExcepCount
        {
            get
            {
                return _activeExceps.Count;
            }
        }

        public int HandledEExcepCount
        {
            get
            {
                return _handledExceps.Count;
            }
        }

        public List<EExcepAction> ActiveEExceptions
        {
            get
            {
                return new List<EExcepAction>(_activeExceps);
            }
        }

        public List<EExcepAction> HandledEExceptions
        {
            get
            {
                return new List<EExcepAction>(_handledExceps);
            }
        }

        public List<EExcepAction> AppearedEExceptions
        {
            get
            {
                var nl = new List<EExcepAction>(_activeExceps);
                nl.AddRange(_handledExceps);
                return nl;
            }
        }


        public int ActivateEException(EException e, IEExceptionAppear eap)
        {
            // 在活跃异常中如果没有则新建
            var ae = _activeExceps.Find(
                o => o.Equals(e)
                );
            if (ae == null)
            {
                ae = new EExcepAction(e, eap);
                ae.AppearedAgain += ExcepAppearedAgain;
                ae.StateChanged += ExcepStateChanged;
                _activeExceps.Add(ae);

                ActivatedEException?.Invoke(this, new ActivatedEExceptionEventArgs(e, eap));
            }
            else
            {
                ae.AppearAgain(eap);
            }

            return ae.AppearCount;
        }

        public IEExcepAction GetActiveExcepAction(EException e)
        {
            return _activeExceps.Find(o => o.Equals(e));
        }

        public IReadOnlyList<IEExceptionAppear> GetActiveExcepAppears(EException e)
        {
            return _activeExceps.Find(o => o.Equals(e))?.Appears;
        }

        public bool HandleEException(EException e, EExcepState hState, EExcepDealStyle dealStyle)
        {
            bool rb = false;
            var ea = _activeExceps.Find(o => o.Equals(e));

            if (ea != null)
            {
                rb = ea.ChangeEExcepState(hState, dealStyle);
            }

            return rb;
        }

        #endregion

        #region Operators

        private void ExcepAppearedAgain(object sender, EExcepAppearAgainEventArgs e)
        {
            ActivatedEException?.Invoke(this, new ActivatedEExceptionEventArgs(sender as EExcepAction, e.NewEExcepAppearInfo));
        }

        private void ExcepStateChanged(object sender, EExcepStateChangedEventArgs e)
        {
            var ea = sender as EExcepAction;
            if (ea != null)
            {
                _handledExceps.Add(ea);
                ea.AppearedAgain -= ExcepAppearedAgain;
                ea.StateChanged -= ExcepStateChanged;
                _activeExceps.Remove(ea);

                HandledEException?.Invoke(this, new HandledEExceptionEventArgs(ea, e));
            }
        }

        #endregion
    }
}

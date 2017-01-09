using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.EException
{
    /// <summary>
    /// 试验异常管理器异常激活事件参数。
    /// </summary>
    public class ActivatedEExceptionEventArgs : EventArgs
    {
        public ActivatedEExceptionEventArgs(EException e, IEExceptionAppear aInfo)
        {
            ActivatedEException = (EException)e.Clone();
            AppearInfo = aInfo;
        }

        public EException ActivatedEException { get; private set; }

        public IEExceptionAppear AppearInfo { get; private set; }

    }

    /// <summary>
    /// 试验异常管理器异常处理事件参数。
    /// </summary>
    public class HandledEExceptionEventArgs : EventArgs
    {
        public HandledEExceptionEventArgs(EException e, EExcepState nState, EExcepDealStyle dealStyle)
        {
            HandledEException = (EException)e.Clone();
            NewState = nState;
            DealStyle = dealStyle;
        }
        public HandledEExceptionEventArgs(EException e, EExcepStateChangedEventArgs args)
            :this(e, args.NewState, args.DealStyle)
        {

        }

        public EException HandledEException { get; private set; }

        public EExcepState NewState { get; private set; }

        public EExcepDealStyle DealStyle { get; private set; }
    }

    /// <summary>
    /// 封装了异常管理的方法。
    /// </summary>
    public interface IExcepManagement
    {
        /// <summary>
        /// 激活一个异常。
        /// </summary>
        /// <param name="e">指定的异常信息。</param>
        /// <param name="eap">异常出现的前置信息。</param>
        /// <returns>返回此异常在活跃次数。</returns>
        int ActivateEException(EException e, IEExceptionAppear eap);

        /// <summary>
        /// 获取当前活跃的异常数。
        /// </summary>
        int ActiveEExcepCount { get; }

        /// <summary>
        /// 在异常管理对象中激活一个试验异常时发生。
        /// </summary>
        event EventHandler<ActivatedEExceptionEventArgs> ActivatedEException;

        /// <summary>
        /// 对活跃异常进行处理。
        /// </summary>
        /// <param name="e">指定异常。</param>
        /// <param name="hState">需要处理的新状态。</param>
        /// <param name="dealStyle">指定的处理方式。</param>
        /// <returns>成功返回true。</returns>
        bool HandleEException(EException e, EExcepState hState, EExcepDealStyle dealStyle);

        /// <summary>
        /// 获取已处理的异常数。
        /// </summary>
        int HandledEExcepCount { get; }

        /// <summary>
        /// 在异常处理中其状态改变后发生。
        /// </summary>
        event EventHandler<HandledEExceptionEventArgs> HandledEException;

        /// <summary>
        /// 获取指定的活跃试验异常的活动。
        /// </summary>
        /// <param name="e">指定的异常。</param>
        /// <returns>异常活动，如果没有指定活跃的异常则返回null。</returns>
        IEExcepAction GetActiveExcepAction(EException e);

        /// <summary>
        /// 获取指定的活跃异常的前位信息。
        /// </summary>
        /// <param name="e">指定的异常。</param>
        /// <returns>异常前位信息，如果没有指定活跃的异常则返回null。</returns>
        IReadOnlyList<IEExceptionAppear> GetActiveExcepAppears(EException e);

        /// <summary>
        /// 获取所有活跃异常。
        /// </summary>
        List<EExcepAction> ActiveEExceptions { get; }

        /// <summary>
        /// 获取所有已处理的异常。
        /// </summary>
        List<EExcepAction> HandledEExceptions { get; }

        /// <summary>
        /// 获取所有出现过的异常。
        /// </summary>
        List<EExcepAction> AppearedEExceptions { get; }
    }
}

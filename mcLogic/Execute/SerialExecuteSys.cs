using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Execute
{
    /// <summary>
    /// 表示串行执行器系统
    /// 拥有一个主执行器和一系列次执行器，主执行器输出时由次执行器共同完成对主执行器的控制
    /// </summary>
    public class SerialExecuteSys
    {

        /// <summary>
        /// 指定主执行器和次执行器列表创建串行执行器系统
        /// </summary>
        /// <param name="mExe">主执行器</param>
        /// <param name="subExes">次执行器集合</param>
        public SerialExecuteSys(Executer mExe, List<Executer> subExes)
        {
            _mainExe = mExe;

            _mainExe.ExecuteChanged += _mainExe_ExecuteChanged;

            _subExe = subExes;
        }

        /// <summary>
        /// 指定主执行器创建串行执行器系统
        /// </summary>
        /// <param name="mExe">主执行器</param>
        public SerialExecuteSys(Executer mExe) : this(mExe, new List<Executer>())
        {

        }

        private Executer _mainExe;
        /// <summary>
        /// 获取/设置主执行器
        /// </summary>
        public Executer MainExecuter
        {
            get { return _mainExe; }
            set { _mainExe = value; }
        }

        private List<Executer> _subExe;
        /// <summary>
        /// 获取/设置次执行器集合
        /// </summary>
        public List<Executer> SubExecuters
        {
            get { return _subExe; }
            set { _subExe = value; }
        }

        /// <summary>
        /// 获取/设置是否强制主执行器等待子执行器的执行状态
        /// 如果仍有子执行器的状态为Executing，则不进行执行操作直到所有执行操作已完成
        /// </summary>
        public bool WaitForSubExecuting { get; set; } = false;


        /// <summary>
        /// 主执行器产生执行操作时发生
        /// </summary>
        public event Action<object, double> MainExecuterExecuted;

        private void _mainExe_ExecuteChanged(object sender, double executedVal)
        {
            if (WaitForSubExecuting)
            {
                foreach (var sub in _subExe)
                {
                    // 如果仍有执行正在进行 则退出
                    if (sub.Status == ExecuteStatus.Executing)
                    {
                        return;
                    }
                }
            }

            MainExecuterExecuted?.Invoke(this, executedVal);

            // 主执行器产生执行操作交由次执行器集合执行
            foreach (var sub in _subExe)
            {
                sub.ExecuteBegin();
            }
        }

    }
}

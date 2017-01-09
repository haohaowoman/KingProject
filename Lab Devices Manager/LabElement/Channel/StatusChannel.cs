using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 表示状态通道。
    /// </summary>
    [Serializable]
    public class StatusChannel : Channel, IStatusExpress
    {
        /// <summary>
        /// 创建状态通道。
        /// </summary>
        /// <param name="owner">通道所属的设备对象。</param>
        /// <exception cref="ArgumentNullException">通道不接受空的拥有者设备对象。</exception>
        internal StatusChannel(LabDevice owner, string label) : base(owner, label, ExperimentStyle.Status)
        {

        }

        /// <summary>
        /// 获取/设置通道数据值。
        /// 在此为Status的转换。
        /// </summary>
        public override object Value
        {
            get
            {
                return _status;
            }

            set
            {
                _status = (bool)value;
                NotifyValueUpdate();
            }
        }

        private bool _status;
        /// <summary>
        /// 获取/设置状态值。
        /// </summary>        
        public bool Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
                NotifyValueUpdate();
            }
        }
    }
}

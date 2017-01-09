using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 试验段/试验区域
    /// </summary>
    [Serializable]
    public class ExperimentalArea : GroupElement<ExperimentalPoint>
    {
        public ExperimentalArea() : base()
        {

        }

        public ExperimentalArea(string label) : base(label)
        {

        }

        #region Properties

        #endregion

        #region Override

        #endregion

        /// <summary>
        /// 创建一个子元素。
        /// </summary>
        /// <param name="label">为子元素指定Label属性。</param>
        /// <returns>创建成功返回对象，否则返回null。</returns>
        /// <remarks>此方法只进行ExperimentalPoint的创建，并不将其添加进行组，如要如此做请再使用AddElement函数。</remarks>
        public override ExperimentalPoint CreateChild(string label)
        {
            return new ExperimentalPoint(this, label);
        }

        /// <summary>
        /// 于试验段中创建一个试验点。
        /// </summary>
        /// <param name="label">试验点的Label属性。</param>
        /// <returns>创建成功返回对象，否则返回null。</returns>
        public ExperimentalPoint CreatePointIn(string label)
        {
            return CreateChildInto(this, label);
        }
    }
}


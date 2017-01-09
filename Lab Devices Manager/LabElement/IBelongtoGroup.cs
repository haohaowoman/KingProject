using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 提供属于组元素的子元素的方法。
    /// </summary>
    internal interface IBelongtoGroup
    {
        /// <summary>
        /// 获取/设置属的组元素。
        /// </summary>
        LabElement Group { get; }
    }

    /// <summary>
    /// 表示组元素所拥有的泛型方法。
    /// </summary>
    /// <typeparam name="T">子元素类型。</typeparam>
    internal interface IOwnedGroup<T> where T : ChildElement
    {
        /// <summary>
        /// 获取子元素的只读列表集合。
        /// </summary>
        IReadOnlyList<T> Children { get; }

        /// <summary>
        /// 在组中添加一个子元素。
        /// </summary>
        /// <param name="el">所添加的新子元素。</param>
        /// <returns>Successful status.</returns>
        bool AddElement(T el);

        /// <summary>
        /// 从组中删除只定的子元素。
        /// </summary>
        /// <param name="el">元素对象。</param>
        /// <returns></returns>
        bool RemoveElement(T el);

        /// <summary>
        /// 通过索引删除子元素。
        /// </summary>
        /// <param name="index">元素索引。</param>
        /// <returns>成功删除为true，否则false。</returns>
        bool RemoveElementAt(int index);

        /// <summary>
        /// 移除组中的所有元素。
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// 确定组中是否包含元素el。
        /// </summary>
        /// <param name="el">指定的元素。</param>
        /// <returns>如果包含则返回true，否则返回false。</returns>
        bool Contains(T el);

        /// <summary>
        /// 通过子元素的Label属性在组中查找指定元素。
        /// </summary>
        /// <param name="label">子元素的Label属性字符串。</param>
        /// <returns>如果查找到则返回对象，否则返回null。</returns>
        T GetElementAsLabel(string label);

        T this[string label] { get; }

        /// <summary>
        /// 通过子元素在组中的索引获取元素对象。
        /// </summary>
        /// <param name="index">子元素索引。</param>
        /// <returns>如果查找到则返回对象，否则返回null。</returns>
        T GetElementAsIndex(int index);

        T this[int index] { get; }

        /// <summary>
        /// 获取元素在组中的索引。
        /// </summary>
        /// <param name="el">元素。</param>
        /// <returns>如果组中不包含指定元素则返回-1。</returns>
        int ElementIndexOf(T el);

        /// <summary>
        /// 当元素组集合发生改变时发生。
        /// </summary>
        event NotifyCollectionChangedEventHandler GroupChanged;

        /// <summary>
        /// 判断组中是否包含Label属性相同的子元素。
        /// </summary>
        /// <param name="label">子元素的Label属性字符串。</param>
        /// <returns>包含返回true。</returns>
        bool ContainsAsLable(string label);

    }
}

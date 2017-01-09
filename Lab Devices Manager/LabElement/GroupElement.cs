using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Diagnostics;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 组元素的抽象类。
    /// </summary>
    /// <typeparam name="T">派生自ChildElement的子元素类。</typeparam>
    [Serializable]
    public abstract class GroupElement<T> : LabElement, IOwnedGroup<T>
        where T : ChildElement
    {
        public GroupElement()
        {
            _children = new List<T>();
        }

        public GroupElement(string label) : base(label)
        {
            _children = new List<T>();
        }

        #region Fields

        // sub element list.
        protected List<T> _children;

        // SubElement collection changed event.
        public event NotifyCollectionChangedEventHandler GroupChanged;

        #endregion

        #region Operators

        #region IOwnedGroup

        /// <summary>
        /// 获取子元素的只读列表集合。
        /// </summary>
        public IReadOnlyList<T> Children
        {
            get
            {
                return _children.AsReadOnly();
            }
        }


        public T this[int index]
        {
            get
            {
                return _children[index];
            }
        }

        /// <summary>
        /// 在组中添加一个子元素。
        /// </summary>
        /// <param name="el">所添加的新子元素。</param>
        /// <returns>Successful status.</returns>
        /// <exception cref="ArgumentException">子元素的Group属性无效，请确定子元素是由本组元素创建。</exception>
        public bool AddElement(T el)
        {
            if (el != null && GetElementAsLabel(el.Label) != null)
            {
                return false;
            }

            if (_children.Contains(el))
            {
                return false;
            }

            else
            {
                if (el.Group != this)
                {
                    throw new ArgumentException("子元素的Group属性无效，请确定子元素是由本组元素创建。", nameof(el));
                }

                _children.Add(el);

                GroupChanged?.Invoke
                    (this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, el)
                    );
            }
            return true;
        }

        public bool Contains(T el)
        {
            return _children.Contains(el);
        }

        public T GetElementAsLabel(string label)
        {
            return _children.Find(o => o.Label == label);
        }

        public T this[string label]
        {
            get
            {
                return GetElementAsLabel(label);
            }
        }

        public bool ContainsAsLable(string label)
        {
            return GetElementAsLabel(label) == null ? false : true;
        }

        public bool RemoveElement(T el)
        {
            bool br = _children.Remove(el);
            if (br)
            {
                GroupChanged?.Invoke
                    (this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, el)
                    );
            }
            return br;
        }

        public bool RemoveElementAt(int index)
        {
            bool br = false;
            try
            {
                object oldItem = _children[index];

                _children.RemoveAt(index);

                br = true;

                GroupChanged?.Invoke
                    (this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index)
                    );
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return br;
        }

        public void RemoveAll()
        {
            _children.Clear();
            GroupChanged?.Invoke
                    (this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove)
                    );
        }

        public T GetElementAsIndex(int index)
        {
            return _children.ElementAt(index);
        }

        public int ElementIndexOf(T el)
        {
            return _children.IndexOf(el);
        }

        #endregion
        
        [OnDeserialized]
        private void OnDeSerializedMethod(StreamingContext strContext)
        {

        }

        /// <summary>
        /// 抽象，由派生类实现。
        /// 创建一个子元素。
        /// </summary>
        /// <param name="label">为子元素指定Label属性。</param>
        /// <returns>创建成功返回子元素对象，否则返回null。</returns>
        /// <remarks>此方法只进行对象的创建，并不将其添加进行组，如要如此做请再使用AddElement函数。</remarks>
        public abstract T CreateChild(string label);

        /// <summary>
        /// 静态，在GroupElement中创建一个子元素。
        /// </summary>
        /// <param name="group">组元素。</param>
        /// <param name="label">为子元素指定Label属性。</param>
        /// <returns>创建成功返回子元素对象，否则返回null。</returns>
        /// <remarks>此方法只进行对象的创建，并不将其添加进行组，如要如此做请再使用group.AddElement函数。</remarks>
        public static T CreateChild(GroupElement<T> group, string label)
        {
            return group?.CreateChild(label);
        }

        /// <summary>
        /// 静态，在GroupElement中创建一个子元素，并尝试将其添加进组。
        /// </summary>
        /// <param name="group">组元素。</param>
        /// <param name="label">为子元素指定Label属性。</param>
        /// <returns>创建并添加成功返回子元素对象，否则返回null。</returns>
        public static T CreateChildInto(GroupElement<T> group, string label)
        {
            T ce = CreateChild(group, label);
            return group?.AddElement(ce) == true ? ce : null;
        }

        #endregion


    }

}

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
    [Serializable]
    public abstract class LabGroupElement<T> : LabElement, IOwnedGroup<T>
        where T : LabGroupSubElement
    {
        #region Fields

        // sub element list.
        protected List<T> _subElements;

        // SubElement collection changed event.
        public event NotifyCollectionChangedEventHandler ElementGroupChanged;

        #endregion

        #region Operators

        #region IOwnedGroup

        /// <summary>
        /// 获取子元素集合的只读列表
        /// </summary>
        public IReadOnlyList<T> SubElements
        {
            get
            {
                return _subElements.AsReadOnly();
            }
        }

        /// <summary>
        ///  Group add a new element that must be only in group.
        /// </summary>
        /// <param name="el">A new element.</param>
        /// <returns>Successful status.</returns>
        public bool AddElement(T el)
        {
            if (_subElements.Contains(el))
            {
                return false;
            }
            else
            {
                _subElements.Add(el);
                el.LabGroup = this;

                ElementGroupChanged?.Invoke
                    (this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, el)
                    );
            }
            return true;
        }

        public T Contains(T el)
        {
            return _subElements.Find(o => o == el);
        }

        public T GetElementAsLabel(string label)
        {
            return _subElements.Find(o => o.Label == label);
        }

        /// <summary>
        /// 获取实例中指定Label属性的LabElement对象
        /// </summary>
        /// <param name="label">LabElement 的 Label</param>
        /// <returns>本实例中不存在Label的LabElement，返回null</returns>
        public T this[string label]
        {
            get
            {
                return GetElementAsLabel(label);
            }
        }

        public bool RemoveElement(T el)
        {
            bool br = _subElements.Remove(el);
            if (br)
            {
                ElementGroupChanged?.Invoke
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
                object oldItem = _subElements[index];

                _subElements.RemoveAt(index);

                br = true;

                ElementGroupChanged?.Invoke
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
            _subElements.Clear();
            ElementGroupChanged?.Invoke
                    (this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove)
                    );
        }

        public T GetElementAsIndex(int index)
        {
            return _subElements.ElementAt(index);
        }

        public int ElementIndexOf(T el)
        {
            return _subElements.IndexOf(el);
        }

        /// <summary>
        /// Refresh and sort the subcollection.
        /// </summary>
        public virtual void RefreshSubElements()
        {
            foreach (var item in _subElements)
            {
                item.LabGroup = this;
            }
            _subElements.Sort();
        }

        #endregion

        public LabGroupElement()
        {
            _subElements = new List<T>();
        }

        [OnDeserialized]
        private void OnDeSerializedMethod(StreamingContext strContext)
        {
            // This will refresh this subelements group property.
            RefreshSubElements();
        }

        #endregion


    }

}

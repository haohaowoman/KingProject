using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    internal interface IBeGrouped
    {
        /// <summary>
        /// Get/set the group.
        /// </summary>
         LabElement LabGroup { get; set; }

        /// <summary>
        /// Get this index of LabGroup.
        /// </summary>
        int IndexInGroup { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The subclass LabElement type of this group.</typeparam>
    internal interface IOwnedGroup<T> where T : LabGroupSubElement
    {
        /// <summary>
        /// Get the readonly list of sub elements.
        /// </summary>
        IReadOnlyList<T> SubElements { get; }

        /// <summary>
        /// Add a element into group.
        /// </summary>
        /// <param name="el">New element.</param>
        /// <returns>Successful status.</returns>
        bool AddElement(T el);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        bool RemoveElement(T el);
        bool RemoveElementAt(int index);
        void RemoveAll();

        T Contains(T el);
        T GetElementAsLabel(string label);
        T GetElementAsIndex(int index);
        int ElementIndexOf(T el);

        void RefreshSubElements();
        
        // Group collection changed event.
        event NotifyCollectionChangedEventHandler ElementGroupChanged;

    }
}

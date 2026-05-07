using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace ChessFinalProject.Models
{
    public class ObservableDictionary<Tkey, Tvalue> : Dictionary<Tkey, Tvalue> ,INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public new void Add(Tkey key, Tvalue value)
        {
            base.Add(key, value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<Tkey, Tvalue>(key, value)));
        }
    }
}

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace SXJL.GTCTK.UI
{
    public class MyObservableCollection<T> : Collection<T>, INotifyCollectionChanged
    {
        protected override void ClearItems()
        {
            base.ClearItems();
            _ = Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CollectionChanged?.Invoke(this,
              new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            _ = Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CollectionChanged?.Invoke(this,
              new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            });
        }

        protected override void RemoveItem(int index)
        {
            T item = this[index];

            base.RemoveItem(index);

            _ = Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CollectionChanged?.Invoke(this,
              new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            });
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = this[index];
            base.SetItem(index, item);
            _ = Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CollectionChanged?.Invoke(this,
              new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
            });
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}

using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using BrokenHouse.Internal;

namespace BrokenHouse.Windows.Data
{
    /// <summary>
    /// Provides a <see cref="System.Windows.Data.CollectionView"/> that can be internally managed or have its content provided 
    /// by an <see cref="System.Collections.IEnumerable"/>.
    /// </summary>
    /// <remarks>
    /// Can be used as a collection for an <see cref="System.Windows.FrameworkElement"/> that supports multiple children that
    /// are either an internal collection or sourced from another <see cref="System.Collections.IEnumerable"/>. It is very
    /// similar to the <see cref="System.Windows.Controls.ItemCollection"/> but this implementation does not support any
    /// sorting or filtering.
    /// </remarks>
    public class CompoundCollectionView : ICollectionView, IList, IEnumerable, IWeakEventListener, INotifyPropertyChanged
    {
        private SimpleListCollectionView     m_ListView;
        private CollectionView               m_BasicView;
        private CollectionView               m_CurrentView;
        
        #region --- Constructors ---

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundCollectionView"/> class that is managed internally.
        /// </summary>
        public CompoundCollectionView()
        {
            CurrentView = m_ListView = new SimpleListCollectionView(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundCollectionView"/> class that represents a view of the specified collection. 
        /// </summary>
        /// <param name="source">The underlying collection.</param>
        public CompoundCollectionView( IEnumerable source )
        {
            CurrentView = m_BasicView = new CollectionView(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundCollectionView"/> class that is managed internally.
        /// </summary>
        /// <remarks>
        /// By supplying the owner of the collection any child that is added to this collection is automatically
        /// added as a logical child of the owner.
        /// </remarks>
        /// <param name="owner">The element whose items that are being managed.</param>
        internal CompoundCollectionView( ICollectionViewModelParent owner )
        {
            CurrentView = m_ListView = new SimpleListCollectionView(owner);
        }
     
        #endregion
            
        #region --- Private Helpers ---

        /// <summary>
        /// Ensures that the current view is a <see cref="SimpleListCollectionView"/>. If not an exception is thrown.
        /// </summary>
        private void EnsureListView()
        {
            if (m_ListView == null)
            {
                throw new InvalidOperationException("Current SourceList does not support List operations");
            }
        }

        /// <summary>
        /// Gets or sets the source of the data we represent
        /// </summary>
        private CollectionView CurrentView
        {
            get { return m_CurrentView; }
            set 
            {
                if (m_CurrentView != null)
                {
                    CollectionChangedEventManager.RemoveListener(m_CurrentView, this);
                    CurrentChangingEventManager.RemoveListener(m_CurrentView, this);
                    CurrentChangedEventManager.RemoveListener(m_CurrentView, this);
                    PropertyChangedEventManager.RemoveListener(m_CurrentView, this, string.Empty);
                }
                m_CurrentView = value;
                if (m_CurrentView != null)
                {
                    CollectionChangedEventManager.AddListener(m_CurrentView, this);
                    CurrentChangingEventManager.AddListener(m_CurrentView, this);
                    CurrentChangedEventManager.AddListener(m_CurrentView, this);
                    PropertyChangedEventManager.AddListener(m_CurrentView, this, string.Empty);
                }
            }
        }
 
        bool IWeakEventListener.ReceiveWeakEvent( Type managerType, object sender, EventArgs e )
        {
            bool result = true;

            if (managerType == typeof(PropertyChangedEventManager))
            {
                OnPropertyChanged((e as PropertyChangedEventArgs).PropertyName);
            }
            else if (managerType == typeof(CollectionChangedEventManager))
            {
                NotifyCollectionChangedEventArgs args = e as NotifyCollectionChangedEventArgs;

                
                {
                    //base.InvalidateEnumerableWrapper();
                    OnCollectionChanged(args);
                }
            }
            else if (managerType == typeof(CurrentChangingEventManager))
            {
                OnCurrentChanging(e as CurrentChangingEventArgs);   
            }
            else if (managerType == typeof(CurrentChangedEventManager))
            {
                OnCurrentChanged();
            }
            else
            {
                result = false;
            }
            
            return result;
        }

 

 

        #endregion
                   
        #region --- Protected Event Triggers ---
  
        /// <summary>
        /// Triggers the property changed event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected virtual void OnPropertyChanged( string propertyName )
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
      
        /// <summary>
        /// Triggers the current changing event.
        /// </summary>
        /// <param name="e">Details of the proposed current item</param>
        protected virtual void OnCurrentChanging( CurrentChangingEventArgs e )
        {
            if (CurrentChanging != null)
            {
                CurrentChanging(this, e);
            }        
        }

        /// <summary>
        /// Triggers the current changed event.
        /// </summary>
        protected virtual void OnCurrentChanged()
        {
            if (CurrentChanged != null)
            {
                CurrentChanged(this, EventArgs.Empty);
            }        
        }

        /// <summary>
        /// Triggers the collection changed event.
        /// </summary>
        /// <param name="e">Details of the what has changed in the collection.</param>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }
          
        #endregion

        #region --- Public Interface ---

        /// <summary>
        /// Changes the source of this <see cref="CompoundCollectionView"/> to be based on the supplied source.
        /// </summary>
        /// <param name="source">The new source of this <see cref="CompoundCollectionView"/>.</param>
        public void SetSource( IEnumerable source )
        {
            OnCurrentChanging(new CurrentChangingEventArgs(false));

            m_BasicView = new CollectionView((source == null)? new object[0] : source);
            m_ListView  = null;

            CurrentView = m_BasicView;

            OnCurrentChanged();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            if (source != null)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, source.Cast<object>().ToList()));
            }
            OnPropertyChanged("Count");
        }
          
        #endregion

        #region --- ICollectionView Members ---

        /// <summary>
        /// Gets a value that indicates whether the underlying view supports filtering.
        /// </summary>
        public bool CanFilter
        { 
            get { return CurrentView.CanFilter; } 
        }

        /// <summary>
        /// Gets a value that indicates whether the underlying view supports grouping.
        /// </summary>
        public bool CanGroup
        { 
            get { return CurrentView.CanGroup; } 
        } 

        /// <summary>
        /// Gets a value that indicates whether the underlying view supports sorting.
        /// </summary>
        public bool CanSort
        { 
            get { return CurrentView.CanSort; } 
        } 

        /// <summary>
        /// Gets or sets the cultural info for actions like sorting.
        /// </summary>
        public CultureInfo Culture
        { 
            get { return CurrentView.Culture; } 
            set { CurrentView.Culture = value; } 
        } 

        /// <summary>
        /// Gets the current item within the view.
        /// </summary>
        public object CurrentItem
        { 
            get { return CurrentView.CurrentItem; } 
        }
 
        /// <summary>
        /// Gets the ordinal position of the <see cref="CurrentItem"/> within the view.
        /// </summary>
        public int CurrentPosition
        { 
            get { return CurrentView.CurrentPosition; } 
        } 

        /// <summary>
        /// Returns a value that indicates whether a given item belongs to this collection view.
        /// </summary>
        /// <param name="item">The object to check.</param>
        /// <returns><b>true</b> if the item belongs to this collection view; otherwise, <b>false</b>.</returns>
        public bool Contains( object item )
        { 
            return CurrentView.Contains(item); 
        }

        /// <summary>
        /// Gets or sets a callback used to determine if an item is suitable for inclusion in the view.
        /// </summary>
        public Predicate<object> Filter                     
        { 
            get { return CurrentView.Filter; } 
            set { CurrentView.Filter = value; } 
        } 

        /// <summary>
        /// Gets a collection of <see cref="System.ComponentModel.GroupDescription"/> objects that describe how the items 
        /// in the collection are grouped in the view.
        /// </summary>
        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return CurrentView.GroupDescriptions; }
        }

        /// <summary>
        /// Gets the top-level groups.
        /// </summary>
        public ReadOnlyObservableCollection<object> Groups
        {
            get { return CurrentView.Groups; }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="CurrentItem"/> of the view is beyond the end of the collection.
        /// </summary>
        public bool IsCurrentAfterLast
        {
            get { return CurrentView.IsCurrentAfterLast; }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="CurrentItem"/> of the view is before the begining of the collection.
        /// </summary>
        public bool IsCurrentBeforeFirst
        {
            get { return CurrentView.IsCurrentBeforeFirst; }
        }

        /// <summary>
        /// Gets a value that indicates that the view is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return CurrentView.IsEmpty; }
        }
        
        /// <summary>
        /// Gets the number of items in the view.
        /// </summary>
        public int Count
        {
            get { return CurrentView.Count; }
        }

        /// <summary>
        /// Gets the sort descriptions used by the view.
        /// </summary>
        public SortDescriptionCollection SortDescriptions
        {
            get { return CurrentView.SortDescriptions; }
        }

        /// <summary>
        /// Gets the IEnumerable that is the source of this collection.
        /// </summary>
        public IEnumerable SourceCollection
        {
            get { return CurrentView.SourceCollection; }
        }

        /// <summary>
        /// Sets the specified item to be the 
        /// </summary>
        /// <param name="item">The new value for the current item.</param>
        /// <returns><b>true</b> if the move was successful.</returns>
        public bool MoveCurrentTo(object item)
        {
            return CurrentView.MoveCurrentTo(item);
        }

        /// <summary>
        /// Sets the <see cref="CurrentItem"/> to be the first item in the view.
        /// </summary>
        /// <returns><b>true</b> if the move was successful.</returns>
        public bool MoveCurrentToFirst()
        {
            return CurrentView.MoveCurrentToFirst();
        }

        /// <summary>
        /// Sets the <see cref="CurrentItem"/> to be the last item in the view.
        /// </summary>
        /// <returns><b>true</b> if the move was successful.</returns>
        public bool MoveCurrentToLast()
        {
            return CurrentView.MoveCurrentToLast();
        }

        /// <summary>
        /// Sets the <see cref="CurrentItem"/> to be the next item in the view.
        /// </summary>
        /// <returns><b>true</b> if the move was successful.</returns>
        public bool MoveCurrentToNext()
        {
            return CurrentView.MoveCurrentToNext();
        }

        /// <summary>
        /// Sets the <see cref="CurrentItem"/> to be the item at the specified position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns><b>true</b> if the move was successful.</returns>
        public bool MoveCurrentToPosition(int position)
        {
            return CurrentView.MoveCurrentToPosition(position);
        }

        /// <summary>
        /// Sets the <see cref="CurrentItem"/> to be the previous item in the view.
        /// </summary>
        /// <returns><b>true</b> if the move was successful.</returns>
        public bool MoveCurrentToPrevious()
        {
            return CurrentView.MoveCurrentToPrevious();
        }

        /// <summary>
        /// Refreshs the view.
        /// </summary>
        public void Refresh()
        {
            CurrentView.Refresh();
        }

        /// <summary>
        /// Allows the automatic refresh to be deferred.
        /// </summary>
        /// <returns>An IDisposable object that when disposed will trigger the refresh.</returns>
        public IDisposable DeferRefresh()                      
        { 
            return CurrentView.DeferRefresh(); 
        }

        /// <summary>
        /// Returns an enumerator that iterates through the view.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)CurrentView).GetEnumerator();
        }
        
        /// <summary>
        /// Returns the index of an item in the view.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The iondex of the intem in the view; if it is not found then <b>-1</b> is returned.</returns>
        public int IndexOf(object value)
        {
            return CurrentView.IndexOf(value);
        }

        /// <summary>
        /// Occurs when the <see cref="CurrentItem"/> has changed.
        /// </summary>
        public event EventHandler CurrentChanged;

        /// <summary>
        /// Occurs when the <see cref="CurrentItem"/> is about to change.
        /// </summary>
        public event CurrentChangingEventHandler CurrentChanging;

        /// <summary>
        /// Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region --- IList Members ---

        /// <summary>
        /// Adds an item into the view. 
        /// </summary>
        /// <remarks>
        /// If the source of this <see cref="CompoundCollectionView"/> is an IEnumerable
        /// then an exception will be thrown.
        /// </remarks>
        /// <param name="value">The value to add to the compound view.</param>
        /// <returns>The index where the item was added.</returns>
        public int Add( object value )
        {
            EnsureListView();

            return m_ListView.Add(value);
        }

        /// <summary>
        /// Clears all items in the view.
        /// </summary>
        /// <remarks>
        /// If the source of this <see cref="CompoundCollectionView"/> is an IEnumerable
        /// then an exception will be thrown.
        /// </remarks>
        public void Clear()
        {
            EnsureListView();

            m_ListView.Clear();
        }

        /// <summary>
        /// Inserts an item in to the view
        /// </summary>
        /// <remarks>
        /// If the source of this <see cref="CompoundCollectionView"/> is an IEnumerable
        /// then an exception will be thrown.
        /// </remarks>
        /// <param name="index">The index to insert at.</param>
        /// <param name="value">The item to insert.</param>
        public void Insert(int index, object value)
        {
            EnsureListView();
            
            m_ListView.Insert(index, value);
        }

        /// <summary>
        /// Gets whether the view is based on a list of fixed size.
        /// </summary>
        /// <remarks>
        /// If the source of this <see cref="CompoundCollectionView"/> is an IEnumerable
        /// then an exception will be thrown.
        /// </remarks>
        public bool IsFixedSize
        {
            get { EnsureListView(); return m_ListView.IsFixedSize; }
        }

        /// <summary>
        /// Gets whether the view is based on a list that is read only.
        /// </summary>
        /// <remarks>
        /// If the source of this <see cref="CompoundCollectionView"/> is an IEnumerable
        /// then an exception will be thrown.
        /// </remarks>
        public bool IsReadOnly
        {
            get { EnsureListView(); return m_ListView.IsReadOnly; }
        }

        /// <summary>
        /// Removes an item from the view.
        /// </summary>
        /// <remarks>
        /// If the source of this <see cref="CompoundCollectionView"/> is an IEnumerable
        /// then an exception will be thrown.
        /// </remarks>
        /// <param name="value">The item to remove.</param>
        public void Remove( object value )
        {
            EnsureListView();
            
            m_ListView.Remove(value);
        }

        /// <summary>
        /// Removes the item at the supplied index.
        /// </summary>
        /// <remarks>
        /// If the source of this <see cref="CompoundCollectionView"/> is an IEnumerable
        /// then an exception will be thrown.
        /// </remarks>
        /// <param name="index">The index at which the item is removed.</param>
        public void RemoveAt(int index)
        {
            EnsureListView();
            
            m_ListView.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <remarks>
        /// If the source of this <see cref="CompoundCollectionView"/> is an IEnumerable
        /// then an exception will be thrown.
        /// </remarks>
        /// <param name="index">The index of the item to get or set.</param>
        /// <returns>The item at the specific index.</returns>
        public object this[int index]
        {
            get { return CurrentView.GetItemAt(index); }
            set
            {
                EnsureListView();

                m_ListView[index] = value;
            }
        }

        #endregion

        #region --- ICollection Members ---

        /// <summary>
        /// Copy the contents of the view into the supplied array.
        /// </summary>
        /// <remarks>
        /// If the source of this <see cref="CompoundCollectionView"/> is an IEnumerable
        /// then an exception will be thrown.
        /// </remarks>
        /// <param name="array">The array into which the items should be placed.</param>
        /// <param name="index">The index to start the copy.</param>
        public void CopyTo(Array array, int index)
        {
            EnsureListView();

            m_ListView.CopyTo(array, index);
        }

        /// <summary>
        /// Gets whether the list is synchronised.
        /// </summary>
        /// <remarks>
        /// If the source of this <see cref="CompoundCollectionView"/> is an IEnumerable
        /// then an exception will be thrown.
        /// </remarks>
        public bool IsSynchronized
        {
            get { EnsureListView(); return m_ListView.IsSynchronized; }
        }

        /// <summary>
        /// Returns an object that can be used to synchronize access to the <see cref="CompoundCollectionView"/>.
        /// </summary>
        /// <remarks>
        /// If the source of this <see cref="CompoundCollectionView"/> is an IEnumerable
        /// then an exception will be thrown.
        /// </remarks>
        public object SyncRoot
        {
            get { EnsureListView(); return m_ListView.SyncRoot; }
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using BrokenHouse.Extensions;

namespace BrokenHouse.Windows.Data
{
    /// <summary>
    /// Provides a simple CollectionView that supports adding and removing.
    /// </summary>
    public class SimpleListCollectionView : CollectionView, IList
    {
        private List<object>               m_List     = null;
        private ICollectionViewModelParent m_Parent   = null;

        #region --- Constructors ---

        /// <summary>
        /// Construct the collection view
        /// </summary>
        internal SimpleListCollectionView( ICollectionViewModelParent collectionParent ) : base(new object[0])
        {
            // Save the collections parent
            m_Parent = collectionParent;

            // Initialise the list and the view to be the same
            m_List = new List<object>();
        }
        
        #endregion

        #region --- Private Helpers ---

        /// <summary>
        /// Trigger the property change event
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged( string propertyName )
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Ensure that the model parent is valid
        /// </summary>
        /// <param name="newValue">The new value in the collection.</param>
        /// <param name="oldValue">The old value in the collection.</param>
        /// <param name="undoAction">The action to perform if the update fails</param>
        private void UpdateModelParent( object newValue, object oldValue, Action undoAction )
        {
            bool   succeeded = true;

            // Do we have a valid new item
            if (m_Parent != null)
            {
                if (newValue != null)
                {
                    // Yes - tassume the update will fail
                    succeeded = false;

                    // Adjsut the model parents
                    try
                    {
                        m_Parent.AddModelItem(newValue);
                        succeeded = true;
                    }
                    finally
                    {
                        // If the add did not succeed - do the undo
                        if (!succeeded && (undoAction != null))
                        {
                            undoAction();
                        }
                    }
                }
     
                if (succeeded && (oldValue != null))
                {
                    m_Parent.RemoveModelItem(oldValue);
                }
            }
        }

        /// <summary>
        /// Do the common post remove actions
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        private void PostRemoveItemAt( int index, object item )
        {
            // Reset the model parent
            UpdateModelParent(item, null, null);
           
            // Trigger update
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));

            // Update the position if it would be changed
            if (index < CurrentPosition)
            {
                int newPosition = CurrentPosition - 1;

                // Update the position
                SetCurrent(m_List[newPosition], newPosition);
            }
            else if (index == CurrentPosition)
            {
                // Ensure that the new position does not extend beyond the view.
                int newPosition = Math.Min(CurrentPosition, m_List.Count - 1);

                // Update the position because the current item is actually changing
                OnCurrentChanging();
                SetCurrent(m_List[newPosition], newPosition);
                OnCurrentChanged();
            }
            else
            {
                // No change to the current element
            }
        }
      
        #endregion

        #region --- Private Properties ---

        /// <summary>
        /// Determine if the current item is in the view
        /// </summary>
        private bool IsCurrentInView
        {
            get { return (CurrentPosition >= 0) || (CurrentPosition < m_List.Count); }
        }
 

        #endregion

        #region --- CollectionView Overrides ---

        /// <summary>
        /// Gets whether this <see cref="SimpleListCollectionView"/> is empty.
        /// </summary>
        public override bool IsEmpty
        {
            get { return (Count == 0); }
        }

        /// <summary>
        /// Gets whether this <see cref="SimpleListCollectionView"/> can do any filtering.
        /// </summary>
        public override bool CanFilter
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the number of items in this <see cref="SimpleListCollectionView"/>
        /// </summary>
        public override int Count
        {
            get { return m_List.Count; }
        }

        /// <summary>
        /// Determines if this <see cref="SimpleListCollectionView"/> contains the supplied element.
        /// </summary>
        /// <param name="item">The element to search for</param>
        /// <returns><b>true</b> is the <see cref="SimpleListCollectionView"/> contains the element.</returns>
        public override bool Contains( object item )
        {
            return m_List.Contains(item);
        }

        /// <summary>
        /// Obtains the element at the specified index.
        /// </summary>
        /// <param name="index">The index into the <see cref="SimpleListCollectionView"/>.</param>
        /// <returns>The element at the supplied index.</returns>
        public override object GetItemAt( int index )
        {
            return m_List[index];
        }

        /// <summary>
        /// Obtains the enumerator for this <see cref="SimpleListCollectionView"/>.
        /// </summary>
        /// <returns>The enumerator.</returns>
        protected override System.Collections.IEnumerator GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        /// <summary>
        /// Obtains the index of an element in this <see cref="SimpleListCollectionView"/>.
        /// </summary>
        /// <param name="item">The element to search for.</param>
        /// <returns>The index of the requested element or -1 if the element could not be found.</returns>
        public override int IndexOf(object item)
        {
            return m_List.IndexOf(item);
        }

        /// <summary>
        /// Positions the view so that the current item matches the supplied item
        /// </summary>
        /// <param name="item">The item to set as the current item</param>
        /// <returns><b>true</b> if the item was found and the move was successful.</returns>
        public override bool MoveCurrentTo( object item )
        {
            bool result = false;

            if (!object.Equals(CurrentItem, item))
            {
               result = this.MoveCurrentToPosition(IndexOf(item));
            }

            return result;
        }

        /// <summary>
        /// Positions the view so that the current item matches the supplied index
        /// </summary>
        /// <param name="position">The new position of the current item</param>
        /// <returns><b>true</b> if the new position is within the view.</returns>
        public override bool MoveCurrentToPosition( int position )
        {
            // Check the input
            if ((position < -1) || (position > m_List.Count))
            {
                throw new ArgumentOutOfRangeException("position");
            }

            // Are we really moving and can we move
            if ((position != this.CurrentPosition) && base.OKToChangeCurrent())
            {
                bool oldIsCurrentAfterLast   = IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;

                // Move to the new position
                if ((position < 0) || (position >= m_List.Count))
                {
                    base.SetCurrent(null, Math.Max(-1, Math.Min(m_List.Count, position)));
                }
                else
                {
                    base.SetCurrent(m_List[position], position);
                }

                // The current item has changed
                OnCurrentChanged();

                // Trigger the events of properties that we know have changed
                OnPropertyChanged("CurrentPosition");
                OnPropertyChanged("CurrentItem");

                // Trigger other events only if the property has changed
                if (IsCurrentAfterLast != oldIsCurrentAfterLast)
                {
                    OnPropertyChanged("IsCurrentAfterLast");
                }
                if (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst)
                {
                    OnPropertyChanged("IsCurrentBeforeFirst");
                }
            }

            return IsCurrentInView;
        }

        /// <summary>
        /// Refresh the view and trigger any appropriate property change events.
        /// </summary>
        protected override void RefreshOverride()
        {
            bool   isEmpty              = this.IsEmpty;
            object currentItem          = this.CurrentItem;
            bool   isCurrentAfterLast   = this.IsCurrentAfterLast;
            bool   isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
            int    currentPosition      = this.CurrentPosition;

            // We assume that the current is going to change
            base.OnCurrentChanging();

            // Move to the correct position
            if (IsEmpty || isCurrentBeforeFirst)
            {
                MoveCurrentToPosition(-1);
            }
            else if (isCurrentAfterLast)
            {
                MoveCurrentToPosition(Count);
            }
            else if (currentItem != null)
            {
                MoveCurrentToPosition(Math.Max(0, m_List.IndexOf(currentItem)));
            }

            // Trigger a collection change
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            // The current has changed
            this.OnCurrentChanged();

            // Trigger any appropraite porperty changes
            if (this.IsCurrentAfterLast != isCurrentAfterLast)
            {
                this.OnPropertyChanged("IsCurrentAfterLast");
            }
            if (this.IsCurrentBeforeFirst != isCurrentBeforeFirst)
            {
                this.OnPropertyChanged("IsCurrentBeforeFirst");
            }
            if (currentPosition != this.CurrentPosition)
            {
                this.OnPropertyChanged("CurrentPosition");
            }
            if (currentItem != this.CurrentItem)
            {
                this.OnPropertyChanged("CurrentItem");
            }
        }

        #endregion

        #region --- IList & ICollection Members ---

        /// <summary>
        /// Adds an element to the <see cref="SimpleListCollectionView"/>
        /// </summary>
        /// <param name="value">The element to add to the <see cref="SimpleListCollectionView"/></param>
        /// <returns>The index in the view of the new item.</returns>
        public int Add( object value )
        {
            int index = m_List.Count;

            Insert(m_List.Count, value);

            return index;
        }

        /// <summary>
        /// Clears all the items from the list.
        /// </summary>
        public void Clear()
        {
            try
            {
                if (m_Parent != null)
                {
                    m_List.ForEach(i => m_Parent.RemoveModelItem(i));
                }
            }
            finally
            {
                m_List.Clear();
                RefreshOrDefer();
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="SimpleListCollectionView"/> at the specified index.
        /// </summary>
        /// <param name="index">The index into the view where the element should be inserted.</param>
        /// <param name="value">The element to insert.</param>
        public void Insert( int index, object value )
        {
            // Insert the value into the list
            if (index < m_List.Count)
            {
                m_List.Insert(index, value);
            }
            else
            {
               index = m_List.Count;

               m_List.Add(value);
            }

            // Define the undo action
            Action undoAction = delegate 
            { 
                m_List.RemoveAt(index);
            };

            // Do the model update
            UpdateModelParent(value, null, undoAction);
           
            // Trigger update
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));

            // Update the position if it would be changed
            if (index <= CurrentPosition)
            {
                int newPosition = CurrentPosition + 1;

                // Update the position
                SetCurrent(m_List[newPosition], newPosition);
            }
        }

        /// <summary>
        /// Gets whether the list is a fixed size
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// The view is not read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="SimpleListCollectionView"/>.
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="SimpleListCollectionView"/>.</param>
        public void Remove( object value )
        {
            int index = m_List.IndexOf(value);

            // Attempt to remove the item from the view
            if (index >= 0)
            {
                m_List.RemoveAt(index);
            }

            // Post process the removal
            PostRemoveItemAt(index, value);
        }

        /// <summary>
        /// Removes an element at a specific index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt( int index )
        {
            // Check the index
            if ((0 > index) || (index >= Count))
            {
                throw new ArgumentOutOfRangeException("index", "Index is less than zero or greater than or equal to Count.");
            }

            // Get the item that we intend to use
            object itemToRemove = m_List[index];

            // Attempt to remove the item from the view
            m_List.RemoveAt(index);

            // Post process the removal
            PostRemoveItemAt(index, itemToRemove);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public object this[int index]
        {
            get { return GetItemAt(index); }
            set 
            { 
                object oldValue = m_List[index];

                // Define the action that will act as redo and undo...
                Action<object> processAction = delegate( object updateValue )
                {
                    m_List[index] = updateValue;
                };

                // Define the undo action
                Action undoAction = delegate 
                { 
                    processAction(oldValue); 
                };

                // Perform the action to set the value
                processAction(value);

                // Process the update
                UpdateModelParent(value, oldValue, undoAction);
                
                // Trigger the event
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue, index));
            }
        }

        /// <summary>
        /// Provide a mechanism to copy the filtered and sorted contents to an array.
        /// </summary>
        /// <param name="array">The destination array</param>
        /// <param name="index">The starting point in the destination array to start the copy</param>
        public void CopyTo( Array array, int index )
        {
            (m_List as System.Collections.IList).CopyTo(array, index);
        }

        /// <summary>
        /// The collection is not synchronised
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Provide access to the sync root of the list
        /// </summary>
        public object SyncRoot
        {
            get { return (m_List as System.Collections.IList).SyncRoot; }
        }

        #endregion
    }
}

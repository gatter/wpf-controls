using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrokenHouse.Windows.Controls;
using BrokenHouse.Windows.Parts.Transition;
using BrokenHouse.Windows.Parts.Transition.Effects;

namespace DemoApplication.Demos.Transition
{
    /// <summary>
    /// Interaction logic for TransitionItemsControl.xaml
    /// </summary>
    public partial class TransitionItemsControl : DemoItem, INotifyPropertyChanged
    {
        /// <summary>
        /// Our private data for the current index
        /// </summary>
        private int m_CurrentIndex = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public TransitionItemsControl()
        {
            // Set up the context
            DataContext = this;

            // Load uo the XAML
            InitializeComponent();
        }
 
        /// <summary>
        /// The current index
        /// </summary>
        public int CurrentIndex   
        { 
            get { return m_CurrentIndex; }
            set 
            { 
                m_CurrentIndex = value;

                OnPropertyChanged("CurrentIndex");
                OnPropertyChanged("CanMoveNext");
                OnPropertyChanged("CanMovePrev");
            }
        }

        /// <summary>
        /// Can we move next
        /// </summary>
        public bool CanMoveNext    
        { 
            get { return m_CurrentIndex + 1 < TransitionControl.Items.Count; } 
        }

        /// <summary>
        /// Can we move next
        /// </summary>
        public bool CanMovePrev
        { 
            get { return m_CurrentIndex >= 1; } 
        }

        /// <summary>
        /// Prev button has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMovePrev(object sender, RoutedEventArgs e)
        {
            CurrentIndex--;
        }

        /// <summary>
        /// Next button has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoveNext(object sender, RoutedEventArgs e)
        {
            CurrentIndex++;
        }

        #region --- INotifyPropertyChanged Implementation ---

        /// <summary>
        /// Event that is triggered when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Trigger the property changed event handler
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged( string propertyName )
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

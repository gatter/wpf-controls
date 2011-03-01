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
    /// Interaction logic for TransitionControl.xaml
    /// </summary>
    public partial class TransitionControl : DemoItem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TransitionControl()
        {
            // Set up the context
            DataContext = this;

            // Load uo the XAML
            InitializeComponent();
        }

        /// <summary>
        /// Prev button has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectContent(object sender, RoutedEventArgs e)
        {
            VisualBrush brush = new VisualBrush((sender as Button).Content as Visual) { Stretch = Stretch.Fill };

            m_TransitionControl.Content = new Rectangle { Fill = brush };
        }
    }
}

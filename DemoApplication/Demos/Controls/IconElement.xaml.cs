using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BrokenHouse.Windows.Controls;

namespace DemoApplication.Demos.Controls
{
    /// <summary>
    /// Interaction logic for IconElement.xaml
    /// </summary>
    public partial class IconElement : DemoItem, INotifyPropertyChanged
    {
        private Thickness   m_OffsetMargin = new Thickness(0.0, 0.0, 1.0, 1.0);

        /// <summary>
        /// Constructor
        /// </summary>
        public IconElement()
        {
            BitmapDecoder decoder = (TaskIcons.Information as BitmapFrame).Decoder;

            // Get the frames from the decoder
            Icons = decoder.Frames.OfType<BitmapSource>().ToArray();

            // Set the data context
            DataContext = this;

            // Initialise the XAML
            InitializeComponent();
        }

        /// <summary>
        /// The amount of vertical offset to apply to the margin
        /// </summary>
        public double VerticalOffset
        {
            get { return m_OffsetMargin.Top; }
            set
            {
                double roundedValue = Math.Round(value * 100.0) / 100.0;

                m_OffsetMargin.Top = roundedValue;
                m_OffsetMargin.Bottom = 1.0 - roundedValue;

                OnPropertyChanged("VerticalOffset");
                OnPropertyChanged("OffsetMargin");
            }
        }

        /// <summary>
        /// The amount of horizontal offset to apply to the margin
        /// </summary>
        public double HorizontalOffset
        {
            get { return m_OffsetMargin.Left; }
            set
            {
                double roundedValue = Math.Round(value * 100.0) / 100.0;

                m_OffsetMargin.Left = roundedValue;
                m_OffsetMargin.Right = 1.0 - roundedValue;

                OnPropertyChanged("HorizontalOffset");
                OnPropertyChanged("OffsetMargin");
            }
        }

        /// <summary>
        /// The margin used to offset the content of an item
        /// </summary>
        public Thickness OffsetMargin
        {
            get { return m_OffsetMargin; }
        }

        /// <summary>
        /// A list of frames in a sample icon. We use this in the display
        /// to allow the standard image element use the larger icons found in the icon decoder.
        /// </summary>
        public  BitmapSource[]   Icons { get; set; }


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

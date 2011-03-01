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
    /// Interaction logic for IconColorTransforms.xaml
    /// </summary>
    public partial class IconColorTransforms : DemoItem
    {
        /// <summary>
        /// Helper class to hold transformation data
        /// </summary>
        public class ColorTransformData
        {
            public double DesaturationAmount { get; set; }
            public double GammaCorrection  { get; set; }   
        }

        /// <summary>
        /// An array of an array of colour transforms
        /// </summary>
        public ColorTransformData[][]  TransformData { get; set; }

        /// <summary>
        /// Initialise our data
        /// </summary>
        public IconColorTransforms()
        {
            // Initialise the transform data
            TransformData = new ColorTransformData[5][];

            // Build up the 2 dimensional array
            for (int i = 0; i < 5; i++)
            {
                double desaturation = (1.0 / 4.0) * i;

                TransformData[i] = new ColorTransformData[5];

                for (int j = 0; j < 5; j++)
                {
                    double gamma = 1.0 - (j * (0.4 / 4.0));

                    TransformData[i][j] = new ColorTransformData { DesaturationAmount = desaturation, GammaCorrection = gamma };
                }
            }

            // Initialise the context
            DataContext = this;

            // Initialise the window
            InitializeComponent();
        }
    }
}

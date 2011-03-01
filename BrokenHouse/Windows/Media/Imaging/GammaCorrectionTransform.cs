using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BrokenHouse.Windows.Media.Imaging
{
    /// <summary>
    /// Defines a color transform that will modify the Gamma of an underlying image
    /// </summary>
    public class GammaCorrectionTransform : ColorTransform
    {
        /// <summary>
        /// Identifies the <see cref="Gamma"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty GammaProperty = DependencyProperty.Register("Gamma", typeof(double), typeof(GammaCorrectionTransform), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Journal, OnGammaChangedThunk), null);

        /// <summary>
        /// Cached caclculated values of inverse gamma to help improve performance
        /// </summary>
        private double InverseGamma { get; set; }

        /// <summary>
        /// Cached caclculated values of rescale gamma to help improve performance
        /// </summary>
        private double RescaleGamma { get; set; }
                
        #region --- Freezable ---

        /// <summary>
        /// Create the transformable bitmap
        /// </summary>
        /// <returns></returns>
        [SecuritySafeCritical]
        protected override Freezable CreateInstanceCore()
        {
            return new GammaCorrectionTransform();
        }

        #endregion
        
        #region --- Dependency Property change handlers ---
                
        /// <summary>
        /// The Gamma has changed - trigger a change notification
        /// </summary>
        /// <param name="oldValue">The old value of the Gamma</param>
        /// <param name="newValue">The new value of the Gamma</param>
        protected virtual void OnGammaChanged( double oldValue, double newValue )
        {
            InverseGamma = 1.0 / newValue;
			RescaleGamma = 255.0 / Math.Pow(255.0, InverseGamma);
        }
               
        /// <summary>
        /// Static method to trigger the instance <see cref="OnGammaChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnGammaChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as GammaCorrectionTransform).OnGammaChanged((double)args.OldValue, (double)args.NewValue);
        }

        #endregion

        #region --- Trnasform ---

        /// <summary>
        /// Change the color based on the required Gamma.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [SecurityCritical]
        internal override Color TransformColor(Color color)
        {
            if (InverseGamma != 1.0)
            {
                color.R = (byte)(Math.Pow((double)color.R, InverseGamma) * RescaleGamma);
                color.G = (byte)(Math.Pow((double)color.G, InverseGamma) * RescaleGamma);
                color.B = (byte)(Math.Pow((double)color.B, InverseGamma) * RescaleGamma);
            }

            return color;
        }

        #endregion 
        
        #region --- Properties ---

        /// <summary>
        /// Return <b>true</b> if the <see cref="Gamma"/> means that there will be no change to the underlying image.
        /// </summary>
        internal override bool IsIdentity
        {
            [SecurityCritical]
            get { return Gamma == 1.0; }
        }

        /// <summary>
        /// Gets or sets the gamma correction that will be applied to the underlying image. This is a dependency property. 
        /// </summary>
        public double Gamma
        {
            get { return (double) base.GetValue(GammaProperty); }
            set 
            { 
                base.SetValue(GammaProperty, value); 

                WritePreamble();
                WritePostscript();
            }
        }

        #endregion 
    }
}

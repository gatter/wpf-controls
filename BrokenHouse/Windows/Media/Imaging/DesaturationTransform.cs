using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Windows;
using System.Windows.Media;

namespace BrokenHouse.Windows.Media.Imaging
{
    /// <summary>
    /// Defines a color transform that will desaturate a bitmap.
    /// </summary>
    public class DesaturationTransform : ColorTransform
    {
        /// <summary>
        /// Identifies the <see cref="Amount"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount", typeof(double), typeof(DesaturationTransform), new FrameworkPropertyMetadata(0.0, OnAmountChangedThunk), null);

        /// <summary>
        /// Take a copy of the dependancy property - effectively caching it
        /// </summary>
        private double CachedAmount { get; set; }
                
        #region --- Freezable ---

        /// <summary>
        /// Create the transformable bitmap
        /// </summary>
        /// <returns></returns>
        [SecuritySafeCritical]
        protected override Freezable CreateInstanceCore()
        {
            return new DesaturationTransform();
        }

        #endregion
        
        #region --- Dependency Property change handlers ---
                
        /// <summary>
        /// The amount of desaturation has changed - trigger a change notification
        /// </summary>
        /// <param name="oldValue">The old value of the amount</param>
        /// <param name="newValue">The new value of the amount</param>
        protected virtual void OnAmountChanged( double oldValue, double newValue )
        {
            // Cache the value
            CachedAmount = newValue;
        }
        
        /// <summary>
        /// Static method to trigger the instance <see cref="OnAmountChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnAmountChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as DesaturationTransform).OnAmountChanged((double)args.OldValue, (double)args.NewValue);
        }

        #endregion

        #region --- Transform ---

        /// <summary>
        /// Desaturate the colour
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [SecurityCritical]
        internal override Color  TransformColor(Color color)
        {
            double amount = Amount;

            // Do we have to do the transform
            if (amount != 0.0)
            {
                // Calculate the gray value
                float value  = (color.ScR * 0.30f) + (color.ScG * 0.59f) + (color.ScB * 0.11f);
                float red    = color.ScR + (float)(amount * (value - color.ScR));
                float green  = color.ScG + (float)(amount * (value - color.ScG));
                float blue   = color.ScB + (float)(amount * (value - color.ScB));

                // Calculate the gray colour
                color = Color.FromScRgb(color.ScA, red, green, blue);
            }

            return color;
        }

        #endregion 
        
        #region --- Properties ---

        /// <summary>
        /// Return <b>true</b> if the desaturation amount means that we would be no color change.
        /// </summary>
        internal override bool IsIdentity
        {
            [SecurityCritical]
            get { return Amount == 0.0; }
        }

        /// <summary>
        /// Gets or sets the amount that underlying image is desaturated. This is a dependency property. 
        /// </summary>
        public double Amount
        {
            get { return (double) base.GetValue(AmountProperty); }
            set { base.SetValue(AmountProperty, value); }
        }

        #endregion 
    }
}

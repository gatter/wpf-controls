using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Effects;
using System.Windows;
using System.Windows.Media;

namespace BrokenHouse.Windows.Media.Effects
{
    /// <summary>
    /// A bitmap effect that desaturates the target.
    /// </summary>
    public class DesaturateEffect : ShaderEffect
    {
        /// <summary>
        /// This is the pixel shader that performs the desaturation
        /// </summary>
        private static PixelShader s_pixelShader;
 
        /// <summary>
        /// Identifies the <see cref="Input"/> dependency property. 
        /// </summary>        
        public static readonly DependencyProperty InputProperty;
 
        /// <summary>
        /// Identifies the <see cref="Amount"/> dependency property. 
        /// </summary>        
         public static readonly DependencyProperty AmountProperty;

        /// <summary>
        /// Do the WPF stuff to register the effects
        /// </summary>
        static DesaturateEffect()
        {
            InputProperty  = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(DesaturateEffect), 0);
            AmountProperty = DependencyProperty.Register("Amount", typeof(double), typeof(DesaturateEffect), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0), CoerceAmount));

            s_pixelShader  = new PixelShader() { UriSource = new Uri(@"pack://application:,,,/BrokenHouse;;component/Windows/Media/Effects/DesaturationEffect.ps") };
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DesaturateEffect()
        {
            PixelShader = s_pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(AmountProperty);
        }

        /// <summary>
        /// Gets or sets the input into the effect.
        /// This is a dependency property. 
        /// </summary>
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        /// <summary>
        /// Gets or sets the amount by which the target will be desaturated.
        /// This is a dependency property. 
        /// </summary>
        public double Amount
        {
            get { return (double)GetValue(AmountProperty); }
            set { SetValue(AmountProperty, value); }
        }

        /// <summary>
        /// Helper function to ensure that the supplied value for the amount is between 0 and 1.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceAmount(DependencyObject d, object value)
        {
            DesaturateEffect effect = (DesaturateEffect)d;
            double           amount = (double)value;

            return ((amount >= 0.0) && (amount <= 1.0))? amount : effect.Amount;
        }
    }
}

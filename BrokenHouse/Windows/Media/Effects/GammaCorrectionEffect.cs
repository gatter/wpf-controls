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
    /// A bitmap effect that changes the gamma the target.
    /// </summary>
    public class GammaCorrectionEffect : ShaderEffect
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
        /// Identifies the <see cref="Gamma"/> dependency property. 
        /// </summary>        
        public static readonly DependencyProperty GammaProperty;

        /// <summary>
        /// Do the WPF stuff to register the effects
        /// </summary>
        static GammaCorrectionEffect()
        {
            InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(GammaCorrectionEffect), 0);
            GammaProperty = DependencyProperty.Register("Gamma", typeof(double), typeof(GammaCorrectionEffect), new UIPropertyMetadata(1.0, PixelShaderConstantCallback(0)));

            s_pixelShader = new PixelShader() { UriSource = new Uri(@"pack://application:,,,/BrokenHouse;;component/Windows/Media/Effects/GammaCorrectionEffect.ps") };
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GammaCorrectionEffect()
        {
            PixelShader = s_pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(GammaProperty);
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
        /// Gets or sets the gamma that will be applied to the target.
        /// This is a dependency property. 
        /// </summary>
        public double Gamma
        {
            get { return (double)GetValue(GammaProperty); }
            set { SetValue(GammaProperty, value); }
        }

    }
}

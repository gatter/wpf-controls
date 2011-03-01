using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BrokenHouse.Windows.Media.Imaging
{
    /// <summary>
    /// This is a helper class that is used to perform bitmap transformations
    /// </summary>
    public class FixedFormatBitmap : CustomBitmap
    {
        /// <summary>
        /// A list of pixel formats that support an alpha channel
        /// </summary>
        private static PixelFormat[]       s_FormatsWithAlpha = new PixelFormat[] { PixelFormats.Bgra32,       PixelFormats.Prgba64,      
                                                                                    PixelFormats.Pbgra32,      PixelFormats.Prgba128Float, 
                                                                                    PixelFormats.Rgba128Float, PixelFormats.Rgba64};

        /// <summary>
        /// The actual source of pixels, can either be a FormatConvertedBitmap or the source
        /// </summary>
        private BitmapSource               FormattedBitmapSource { get; set; }

        /// <summary>
        /// An optional converter used to convert the source to the correct pixel format
        /// </summary>
        private FormatConvertedBitmap      BitmapSourceConverter { get; set; }

        #region --- Freezable ---

        /// <summary>
        /// Create the transformable bitmap
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new FixedFormatBitmap();
        }

        /// <summary>
        /// Makes an instance of BitmapSource or a derived class immutable.
        /// </summary>
        /// <param name="isChecking"><b>true</b> if this instance should actually freeze itself when this method is called; otherwise, <b>false</b>.</param>
        /// <returns><b>true</b> if we cn actually be frozen</returns>
        protected override bool FreezeCore( bool isChecking )
        {
            bool canFreeze = true;

            if (BitmapSourceConverter != null)
            {
                if (isChecking)
                {
                    canFreeze = BitmapSourceConverter.CanFreeze;
                }
                else
                {
                    BitmapSourceConverter.Freeze();
                }
            }

            return canFreeze;
        }
           
        /// <summary>
        /// Copies data into a cloned instance.
        /// </summary>
        /// <param name="source">The source of the information to clone</param>
        protected sealed override void CloneCore( Freezable source )
        {
            FixedFormatBitmap original = source as FixedFormatBitmap;

            // Call the default
            base.CloneCore(source);

            // Clone the converter
            if (original.BitmapSourceConverter != null)
            {
                BitmapSourceConverter = (FormatConvertedBitmap)original.BitmapSourceConverter.Clone();
            }
        }

        /// <summary>
        /// Copies current value data into a cloned instance.
        /// </summary>
        /// <param name="source">The source of the information to clone</param>
        protected sealed override void CloneCurrentValueCore( Freezable source )
        {
            FixedFormatBitmap original = source as FixedFormatBitmap;

            // Call the default
            base.CloneCurrentValueCore(source);

            // Clone the converter
            if (original.BitmapSourceConverter != null)
            {
                BitmapSourceConverter = (FormatConvertedBitmap)original.BitmapSourceConverter.CloneCurrentValue();
            }
        }

        /// <summary>
        /// Makes this instance a clone of the specified TransformingBitmap object. 
        /// </summary>
        /// <param name="source">The source of the information to clone</param>
        protected sealed override void GetAsFrozenCore( Freezable source )
        {
            FixedFormatBitmap original = source as FixedFormatBitmap;

            // Call the default
            base.GetAsFrozenCore(source);

            // Clone the converter
            if (original.BitmapSourceConverter != null)
            {
                BitmapSourceConverter = (FormatConvertedBitmap)original.BitmapSourceConverter.GetAsFrozen();
            }
        }

        /// <summary>
        /// Makes this instance a frozen clone of the specified BitmapSource.
        /// Resource references, data bindings, and animations are not copied, but their current values are.
        /// </summary>
        /// <param name="source">The source of the information to clone</param>
        protected sealed override void GetCurrentValueAsFrozenCore( Freezable source )
        {
            FixedFormatBitmap original = source as FixedFormatBitmap;

            // Call the default
            base.GetCurrentValueAsFrozenCore(source);

            // Clone the converter
            if (original.BitmapSourceConverter != null)
            {
                BitmapSourceConverter = (FormatConvertedBitmap)original.BitmapSourceConverter.GetCurrentValueAsFrozen();
            }
        }

        #endregion


        #region --- Private Event Handlers ---

        /// <summary>
        /// The source has changed
        /// </summary>
        /// <param name="oldValue">The old bitmap source value</param>
        /// <param name="newValue">The new bitmap source value</param>
        protected override void OnSourceChanged( BitmapSource oldValue, BitmapSource newValue )
        {
            // Clear the old format converter
            if (oldValue != null)
            {
                FormattedBitmapSource = null;
                BitmapSourceConverter = null;
            }

            // Attach the new ones
            if (newValue != null)
            {
                // Determine the required format
                PixelFormat requiredFormat = s_FormatsWithAlpha.Contains(newValue.Format)? PixelFormats.Bgra32 : PixelFormats.Bgr32;

                // Create the format converter bitmap
                if (newValue.Format == requiredFormat)
                {
                    FormattedBitmapSource = newValue;
                }
                else
                {
                    FormattedBitmapSource = BitmapSourceConverter = new FormatConvertedBitmap(newValue, requiredFormat, null, 0.0);
                }
            }
        }

        #endregion

        #region --- BitmapSource Properties --- 

        /// <summary>
        /// Gets the horizontal dots per inch (dpi) of the image. 
        /// </summary>
        public override double DpiX
        {
            get
            {
                BitmapSource source = FormattedBitmapSource;
                
                return (source != null)? source.DpiX : base.DpiX;
            }
        }

        /// <summary>
        /// Gets the vertical dots per inch (dpi) of the image. 
        /// </summary>
        public override double DpiY
        {
            get
            {
                BitmapSource source = FormattedBitmapSource;
                
                return (source != null)? source.DpiY : base.DpiY;
            }
        }

        /// <summary>
        /// Gets the native PixelFormat of the bitmap data. 
        /// </summary>
        public override PixelFormat Format
        {
            get
            {
                BitmapSource source = FormattedBitmapSource;
                
                return (source != null)? source.Format : base.Format;
            }
        }

        /// <summary>
        /// Gets the width of the bitmap in pixels. 
        /// </summary>
        public override int PixelWidth
        {
            get
            {
                BitmapSource source = FormattedBitmapSource;
                
                return (source != null)? source.PixelWidth : base.PixelWidth;
            }
        }

        /// <summary>
        /// Gets the height of the bitmap in pixels. 
        /// </summary>
        public override int PixelHeight
        {
            get
            {
                BitmapSource source = FormattedBitmapSource;
                
                return (source != null)? source.PixelHeight : base.PixelHeight;
            }
        }

        /// <summary>
        /// Gets the color palette of the bitmap, if one is specified. 
        /// </summary>
        public override BitmapPalette Palette
        {
            get
            {
                BitmapSource source = FormattedBitmapSource;
                
                return (source != null)? source.Palette : base.Palette;
            }
        }

        #endregion

        #region --- CopyPixels ---
        
        /// <summary>
        /// Copies the bitmap pixel data within the specified rectangle into an array of 
        /// pixels that has the specified stride starting at the specified offset.
        /// </summary>
        /// <param name="sourceRect">The source rectangle to copy. An Empty value specifies the entire bitmap.</param>
        /// <param name="pixels">The destination array.</param>
        /// <param name="stride">The stride of the bitmap.</param>
        /// <param name="offset">The pixel location where copying begins.</param>
        public override void CopyPixels(Int32Rect sourceRect, Array pixels, int stride, int offset)
        {
            // Ensure that the source rect is not empty
            if (sourceRect.IsEmpty)
            {
                sourceRect.Width = PixelWidth;
                sourceRect.Height = PixelHeight;
            }

            // Copy from the formatted butmap
            if (FormattedBitmapSource != null)
            {
                FormattedBitmapSource.CopyPixels(sourceRect, pixels, stride, offset);
            }
        }

        #endregion 

    }
}


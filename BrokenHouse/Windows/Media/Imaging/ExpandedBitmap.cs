using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BrokenHouse;

namespace BrokenHouse.Windows.Media.Imaging
{
    /// <summary>
    /// This is a helper class used to expand a bitmap
    /// </summary>
    internal class ExpandedBitmap : CustomBitmap
    {
        /// <summary>
        /// Identifies the <see cref="Margin"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty MarginProperty = DependencyProperty.Register("Margin", typeof(Int32Thickness), typeof(ExpandedBitmap), new FrameworkPropertyMetadata(new Int32Thickness(0), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, null), null);
      
        #region --- Freezable ---

        /// <summary>
        /// Create the transformable bitmap
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new ExpandedBitmap();
        }

        #endregion
        
        #region --- BitmapSource Properties --- 

        /// <summary>
        /// Gets the width of the bitmap in pixels. 
        /// </summary>
        /// <remarks>
        /// We simply use the underlying <c>PixelWidth</c> and add the <see cref="Margin"/> (if defined).
        /// </remarks>
        public override int PixelWidth
        {
            get
            {
                BitmapSource source = Source;
                
                return (source != null)? source.PixelWidth + Margin.Size.Width : base.PixelWidth;
            }
        }

        /// <summary>
        /// Gets the height of the bitmap in pixels. 
        /// </summary>
        /// <remarks>
        /// We simply use the underlying <c>PixelHeight</c> and add the <see cref="Margin"/> (if defined).
        /// </remarks>
        public override int PixelHeight
        {
            get
            {
                BitmapSource source = Source;
                
                return (source != null)? source.PixelHeight + Margin.Size.Height : base.PixelHeight;
            }
        }

        #endregion

        #region --- CopyPixels ---

        
        /// <summary>
        /// Copies the bitmap pixel data within the specified rectangle 
        /// </summary>
        /// <param name="sourceRect">The source rectangle to transform</param>
        /// <param name="stride">The stride of the bitmap.</param>
        /// <param name="bufferSize">The size of destination buffer.</param>
        /// <param name="buffer">The destination buffer.</param>
        public  override void CopyPixels( Int32Rect sourceRect, IntPtr buffer, int bufferSize, int stride )
        {

            // Ensure that the source rect is not empty
            if (sourceRect.IsEmpty)
            {
                sourceRect.Width = PixelWidth;
                sourceRect.Height = PixelHeight;
            }

            // Adjust the source rect to exclude the margin
            Int32Thickness margin = Margin;

            // Clamp the values
            if (sourceRect.X < margin.Left)
            {
                sourceRect.Width -= (margin.Left - sourceRect.X);
                sourceRect.X = margin.Left;
            }
            if (sourceRect.Y < margin.Top)
            {
                sourceRect.Height -= (margin.Top - sourceRect.Y);
                sourceRect.Y = margin.Top;
            }
            if (sourceRect.X + sourceRect.Width > PixelWidth - margin.Right)
            {
                sourceRect.Width = PixelWidth - margin.Right - sourceRect.X;
            }
            if (sourceRect.Y + sourceRect.Height > PixelHeight - margin.Bottom)
            {
                sourceRect.Height = PixelHeight - margin.Bottom - sourceRect.Y;
            }

            // Reposition the rect
            sourceRect.X -= margin.Left;
            sourceRect.Y -= margin.Top;

            // Work out the new buffer
            int    newOffset = (4 * margin.Left) + (margin.Top * stride);
            IntPtr newBuffer = new IntPtr(buffer.ToInt64() + newOffset);

            // Adjust the buffer size
            bufferSize -= newOffset;

            // Call the base
            Source.CopyPixels(sourceRect, newBuffer, bufferSize, stride);
        }

        /// <summary>
        /// Copies the bitmap pixel data within the specified rectangle into an array of 
        /// pixels that has the specified stride starting at the specified offset.
        /// </summary>
        /// <param name="sourceRect">The source rectangle to copy. An Empty value specifies the entire bitmap.</param>
        /// <param name="pixels">The destination array.</param>
        /// <param name="stride">The stride of the bitmap.</param>
        /// <param name="offset">The pixel location where copying begins.</param>
        public sealed override void CopyPixels(Int32Rect sourceRect, Array pixels, int stride, int offset)
        {
            // Ensure that the source rect is not empty
            if (sourceRect.IsEmpty)
            {
                sourceRect.Width = PixelWidth;
                sourceRect.Height = PixelHeight;
            }

            // Adjust the source rect to exclude the margin
            Int32Thickness margin = Margin;

            // Clamp the values
            if (sourceRect.X < margin.Left)
            {
                sourceRect.Width -= (margin.Left - sourceRect.X);
                sourceRect.X = margin.Left;
            }
            if (sourceRect.Y < margin.Top)
            {
                sourceRect.Height -= (margin.Top - sourceRect.Y);
                sourceRect.Y = margin.Top;
            }
            if (sourceRect.X + sourceRect.Width > PixelWidth - margin.Right)
            {
                sourceRect.Width = PixelWidth - margin.Right - sourceRect.X;
            }
            if (sourceRect.Y + sourceRect.Height > PixelHeight - margin.Bottom)
            {
                sourceRect.Height = PixelHeight - margin.Bottom - sourceRect.Y;
            }

            // Reposition the rect
            sourceRect.X -= margin.Left;
            sourceRect.Y -= margin.Top;

            // Adjust the offset
            offset += (4 * margin.Left) + (margin.Top * stride);

            // Do any transfrom
            Source.CopyPixels(sourceRect, pixels, stride, offset);
        }


        #endregion 
        
        #region --- Dependency Properties ---

        /// <summary>
        /// Gets or sets the margin that will be added to the underlying image. This is a dependency property. 
        /// </summary>
        public Int32Thickness Margin
        {
            get { return (Int32Thickness) base.GetValue(MarginProperty); }
            set { base.SetValue(MarginProperty, value); }
        }

        #endregion 

    }
}


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
    public class CustomBitmap : BitmapSource
    {
        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(BitmapSource), typeof(CustomBitmap), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSourceChangedThunk), null), null);
    
        #region --- Freezable ---

        /// <summary>
        /// Create the transformable bitmap
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new CustomBitmap();
        }

        /// <summary>
        /// Makes an instance of BitmapSource or a derived class immutable.
        /// </summary>
        /// <param name="isChecking"><b>true</b> if this instance should actually freeze itself when this method is called; otherwise, <b>false</b>.</param>
        /// <returns><b>true</b> if we cn actually be frozen</returns>
        protected override bool FreezeCore( bool isChecking )
        {
            return true;
        }

        #endregion

        #region --- Events ---

        /// <summary>
        /// The download has completed
        /// </summary>
        public override event EventHandler                            DownloadCompleted;

        /// <summary>
        /// We have download progress
        /// </summary>
        public override event EventHandler<DownloadProgressEventArgs> DownloadProgress;

        /// <summary>
        /// The download failed
        /// </summary>
        public override event EventHandler<ExceptionEventArgs>        DownloadFailed;

        /// <summary>
        /// The deconde failed
        /// </summary>
        public override event EventHandler<ExceptionEventArgs>        DecodeFailed;

        #endregion 

        #region --- Overridable Event Handlers ---

        /// <summary>
        /// Raise the DownloadCompleted event
        /// </summary>
        protected void OnDownloadCompleted()
        {
            if (DownloadCompleted != null)
            {
                DownloadCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raise the DownloadProgress event
        /// </summary>
        /// <param name="args"></param>
        protected void OnDownloadProgress( DownloadProgressEventArgs args )
        {
            if (DownloadProgress != null)
            {
                DownloadProgress(this, args);
            }
        }

        /// <summary>
        /// Raise the DownloadFailed event
        /// </summary>
        /// <param name="args"></param>
        protected void OnDownloadFailed( ExceptionEventArgs args )
        {
            if (DownloadFailed != null)
            {
                DownloadFailed(this, args);
            }
        }
        
        /// <summary>
        /// Raise the DecodeFailed event
        /// </summary>
        /// <param name="args"></param>
        protected void OnDecodeFailed( ExceptionEventArgs args )
        {
            if (DecodeFailed != null)
            {
                DecodeFailed(this, args);
            }
        }

        #endregion 

        #region --- Private Event Handlers ---

        /// <summary>
        /// Raise the DownloadCompleted event
        /// </summary>
        private void OnSourceDownloadCompleted( object source, EventArgs args )
        {
            OnDownloadCompleted();
        }

        /// <summary>
        /// The source download has prgressed
        /// </summary>
        private void OnSourceDownloadProgress( object source, DownloadProgressEventArgs args )
        {
            OnDownloadProgress(args);
        }

        /// <summary>
        /// The source download failed
        /// </summary>
        private void OnSourceDownloadFailed( object source, ExceptionEventArgs args )
        {
            OnDownloadFailed(args);
        }
        
        /// <summary>
        /// The source decoding failed
        /// </summary>
        private void OnSourceDecodeFailed( object source, ExceptionEventArgs args )
        {
            OnDecodeFailed(args);
        }

        /// <summary>
        /// The source has changed
        /// </summary>
        /// <param name="oldValue">The old bitmap source value</param>
        /// <param name="newValue">The new bitmap source value</param>
        protected virtual void OnSourceChanged( BitmapSource oldValue, BitmapSource newValue )
        {
            // Detatch the old events
            if (oldValue != null)
            {
                // If the old value is not frozen then detatch from it
                if (!oldValue.IsFrozen)
                {
                    oldValue.DownloadCompleted -= OnSourceDownloadCompleted;
                    oldValue.DownloadProgress -= OnSourceDownloadProgress;
                    oldValue.DownloadFailed -= OnSourceDownloadFailed;
                    oldValue.DecodeFailed -= OnSourceDecodeFailed;
                }
            }

            // Attach the new ones
            if (newValue != null)
            {
                // If the new value is not frozen that attach to it
                if (!newValue.IsFrozen)
                {
                    newValue.DownloadCompleted += OnSourceDownloadCompleted;
                    newValue.DownloadProgress += OnSourceDownloadProgress;
                    newValue.DownloadFailed += OnSourceDownloadFailed;
                    newValue.DecodeFailed += OnSourceDecodeFailed;
                }
            }
        }
        
        /// <summary>
        /// The source has changed. This is a thunk that will call the instance OnSourceChanged.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSourceChangedThunk( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            CustomBitmap transformingBitmap = d as CustomBitmap;

            transformingBitmap.OnSourceChanged(e.OldValue as BitmapSource, e.NewValue as BitmapSource);
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
                BitmapSource source = Source;
                
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
                BitmapSource source = Source;
                
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
                BitmapSource source = Source;
                
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
                BitmapSource source = Source;
                
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
                BitmapSource source = Source;
                
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
                BitmapSource source = Source;
                
                return (source != null)? source.Palette : base.Palette;
            }
        }

        #endregion

        #region --- CopyPixels ---

        /// <summary>
        /// Copies the bitmap pixel data into an array of pixels with the specified stride, starting at the specified offset.
        /// </summary>
        /// <remarks>
        /// This call is a simple wrapper to other Copy Pixel classes
        /// </remarks>
        /// <param name="pixels">The destination array.</param>
        /// <param name="stride">The stride of the bitmap.</param>
        /// <param name="offset">The pixel location where copying starts.</param>
        public sealed override void CopyPixels( Array pixels, int stride, int offset )
        {
            CopyPixels(Int32Rect.Empty, pixels, stride, offset);
        }
        
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
            if (Source != null)
            {
                Source.CopyPixels(sourceRect, pixels, stride, offset);
            }
        }

        /// <summary>
        /// Copies the bitmap pixel data within the specified rectangle 
        /// </summary>
        /// <param name="sourceRect">The source rectangle to transform</param>
        /// <param name="stride">The stride of the bitmap.</param>
        /// <param name="bufferSize">The size of destination buffer.</param>
        /// <param name="buffer">The destination buffer.</param>
        public  override void CopyPixels( Int32Rect sourceRect, IntPtr buffer, int bufferSize, int stride )
        {
            throw new NotImplementedException("Unsafe bitmap operations are not supported");
        }

        #endregion 
        
        #region --- Dependency Properties ---

        /// <summary>
        /// Gets or sets the <see cref="System.Windows.Media.Imaging.BitmapSource"/> for the underlying image. This is a dependency property. 
        /// </summary>
        public BitmapSource Source
        {
            get { return (BitmapSource) base.GetValue(SourceProperty); }
            set { base.SetValue(SourceProperty, value); }
        }

        #endregion 

    }
}


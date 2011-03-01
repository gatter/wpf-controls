using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Diagnostics.CodeAnalysis;

namespace BrokenHouse.Windows.Media.Imaging
{
    /// <summary>
    /// Represents a group of <see cref="ColorTransform"/> objects that is used to apply multiple 
    /// transforms to a <see cref="System.Windows.Media.Imaging.BitmapSource"/> using the 
    /// <see cref="ColorTransformedBitmap"/>.
    /// </summary>
    [ContentProperty("Children")]
    public class ColorTransformGroup : ColorTransform
    {
        /// <summary>
        /// Identifies the <see cref="Children"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children", typeof(ColorTransformCollection), typeof(ColorTransformGroup), new FrameworkPropertyMetadata(null, OnChildrenChangedThunk), null);
 
        #region --- Constructor ---

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ColorTransformGroup()
        {
            Children = new ColorTransformCollection();
        }

        #endregion

        #region --- Freezable ---

        /// <summary>
        /// Create a new instance of the group
        /// </summary>
        /// <returns></returns>
        [SecuritySafeCritical]
        protected override Freezable CreateInstanceCore()
        {
            return new ColorTransformGroup();
        }

        #endregion
        
        #region --- Dependency Property change handlers ---
                
        /// <summary>
        /// The children have changed.
        /// </summary>
        /// <remarks>
        /// When the children change we need to make sure that the change notification progresses
        /// up the object tree. 
        /// </remarks>
        /// <param name="oldValue">The old value of the amount</param>
        /// <param name="newValue">The new value of the amount</param>
        protected virtual void OnChildrenChanged( ColorTransformCollection oldValue, ColorTransformCollection newValue )
        {
            WritePreamble();
            WritePostscript();

            OnFreezablePropertyChanged(oldValue, newValue, ChildrenProperty);
        }
               
        /// <summary>
        /// The Children have changed. This is a thunk that will call the OnAmountChanged instance method.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnChildrenChangedThunk( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            (d as ColorTransformGroup).OnChildrenChanged((ColorTransformCollection)e.OldValue, (ColorTransformCollection)e.NewValue);
        }

        #endregion

        #region --- Implemetation ---

        /// <summary>
        /// Transform the colour using all the transforms in the group
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [SecurityCritical]
        internal override Color TransformColor(Color color)
        {
            if (Children != null)
            {
                foreach (ColorTransform transform in Children)
                {
                    if (!transform.IsIdentity)
                    {
                        color = transform.TransformColor(color);
                    }
                }
            }

            return color;
        }


        #endregion
            
        #region --- Properties ---
        
        /// <summary>
        /// Gets whether this transform is an identity transform. This will be <c>true</c> if all the child
        /// transforms are identity transforms.
        /// </summary>
        internal override bool  IsIdentity
        {
            [SecurityCritical]
            get { return Children.Aggregate(true, (f, i) => f & i.IsIdentity); }
        }

        /// <summary>
        /// Gets or sets the children of the <see cref="ColorTransformGroup"/>. This is a dependency property. 
        /// </summary>
        public ColorTransformCollection Children
        {
            get { return (ColorTransformCollection) base.GetValue(ChildrenProperty); }
            set { base.SetValue(ChildrenProperty, value); }
        }

        #endregion         
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BrokenHouse.Windows.Parts.Task
{
    /// <summary>
    /// The role of this class is to provide keys for all aspects of the styles and colours
    /// used in the Task Dialogs.
    /// </summary>
    public static class TaskElements
    {
        /// <summary>
        /// Gets the key used to identify the resource used for task dialog control
        /// </summary>
        public static ComponentResourceKey DialogControlStyleKey     {get; private set;}
        /// <summary>
        /// Gets the key used to identify the resource used for task dialog control
        /// </summary>
        public static ComponentResourceKey TaskButtonBarControlStyleKey     {get; private set;}
        /// <summary>
        /// Gets the key used to identify the style for the classic task dialog expander toggle button
        /// </summary>
        public static ComponentResourceKey ClassicDialogExpanderStyleKey     {get; private set;}
        /// <summary>
        /// Gets the key used to identify the style for the classic task dialog check box button
        /// </summary>
        public static ComponentResourceKey ClassicDialogCheckBoxStyleKey     {get; private set;}
        /// <summary>
        /// Gets the key used to identify the background brush used for the content of the classic task dialog control
        /// </summary>
        public static ComponentResourceKey ClassicDialogContentBackgroundKey     {get; private set;}
        /// <summary>
        /// Gets the key used to identify the background brush used for the footer of the classic task dialog control
        /// </summary>
        public static ComponentResourceKey ClassicDialogFooterBackgroundKey     {get; private set;}
        /// <summary>
        /// Gets the key used to identify the background brush used for the separator for sections in the footer.
        /// </summary>
        public static ComponentResourceKey ClassicDialogSeparatorKey    {get; private set;}
        /// <summary>
        /// Gets the key used to identify the style for the Aero task dialog expander toggle button
        /// </summary>
        public static ComponentResourceKey AeroDialogExpanderStyleKey     {get; private set;}
        /// <summary>
        /// Gets the key used to identify the style for the Aero task dialog check box toggle button
        /// </summary>
        public static ComponentResourceKey AeroDialogCheckBoxStyleKey     {get; private set;}
        /// <summary>
        /// Gets the key used to identify the background brush used for the content of the Aero task dialog control
        /// </summary>
        public static ComponentResourceKey AeroDialogContentBackgroundKey     {get; private set;}
        /// <summary>
        /// Gets the key used to identify the background brush used for the footer of the Aero task dialog control
        /// </summary>
        public static ComponentResourceKey AeroDialogFooterBackgroundKey     {get; private set;}
        /// <summary>
        /// Gets the key used to identify the brush used for the separator highlight for sections in the footer.
        /// </summary>
        public static ComponentResourceKey AeroDialogSeparatorHighlightKey    {get; private set;}
        /// <summary>
        /// Gets the key used to identify the brush used for the separator lowlight for sections in the footer.
        /// </summary>
        public static ComponentResourceKey AeroDialogSeparatorLowlightKey    {get; private set;}

        
        /// <summary>
        /// Create all the resource keys so that they can be used in the XAML.
        /// </summary>
        static TaskElements()
        {
            DialogControlStyleKey             = new ComponentResourceKey(typeof(TaskDialogControl),   "DialogControlStyle");
            TaskButtonBarControlStyleKey      = new ComponentResourceKey(typeof(TaskDialogControl),   "TaskButtonBarControlStyle");

            ClassicDialogCheckBoxStyleKey     = new ComponentResourceKey(typeof(TaskDialogControl),   "AeroDialogCheckBoxStyle");
            ClassicDialogExpanderStyleKey     = new ComponentResourceKey(typeof(TaskDialogControl),   "ClassicDialogExpanderStyle");
            ClassicDialogFooterBackgroundKey  = new ComponentResourceKey(typeof(TaskDialogControl),   "ClassicDialogFooterBackground");
            ClassicDialogContentBackgroundKey = new ComponentResourceKey(typeof(TaskDialogControl),   "ClassicDialogContentBackground");
            ClassicDialogSeparatorKey         = new ComponentResourceKey(typeof(TaskDialogControl),   "ClassicDialogSeparator");

            AeroDialogCheckBoxStyleKey        = new ComponentResourceKey(typeof(TaskDialogControl),   "AeroDialogCheckBoxStyle");
            AeroDialogExpanderStyleKey        = new ComponentResourceKey(typeof(TaskDialogControl),   "AeroDialogExpanderStyle");
            AeroDialogFooterBackgroundKey     = new ComponentResourceKey(typeof(TaskDialogControl),   "AeroDialogFooterBackground");
            AeroDialogContentBackgroundKey    = new ComponentResourceKey(typeof(TaskDialogControl),   "AeroDialogContentBackground");
            AeroDialogSeparatorHighlightKey   = new ComponentResourceKey(typeof(TaskDialogControl),   "AeroDialogSeparatorHighlight");
            AeroDialogSeparatorLowlightKey    = new ComponentResourceKey(typeof(TaskDialogControl),   "AeroDialogSeparatorLowlight");
        }
    }
}

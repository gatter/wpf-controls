using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenHouse.Windows.Parts.Task
{
    /// <summary>
    /// This class encapsulates the result from a <see cref="TaskDialog"/>.
    /// </summary>
    /// <remarks>
    /// A <see cref="TaskDialog"/> can be closed by either clicking on one of the standard
    /// buttons defined in <see cref="TaskButtonSet"/>, by clicking on a custom 
    /// <see cref="BrokenHouse.Windows.Controls.CommandLink"/> or by closing the dialog.
    /// This class can be used to represent all three scenarios. In all cases 
    /// <see cref="TaskButton"/> will indicate which button was pressed. If the dialog
    /// was closed by pressing the escape key then the button will depend
    /// on the <see cref="TaskButtonSet"/> being used. However, 
    /// if a custom button was clicked then <see cref="TaskButton"/> will be set to 
    /// <see cref="BrokenHouse.Windows.Parts.Task.TaskButton.Custom"/> and <see cref="ButtonName"/> will be set to the name 
    /// assigned to the button that was clicked.
    /// </remarks>
    public class TaskDialogResult
    {
        /// <summary>
        /// Gets the button that was clicked.
        /// </summary>
        /// <remarks>
        /// If the button was a custom button then this property will be 
        /// <see cref="BrokenHouse.Windows.Parts.Task.TaskButton.Custom"/>
        /// and the name of the button will contained in <ee cref="ButtonName"/>.
        /// </remarks>
        public TaskButton   TaskButton { get; private set; }

        /// <summary>
        /// Gets the name of the button that triggered this result.
        /// </summary>
        /// <remarks>
        /// If the button clicked was a standard task button then this property will
        /// be set to the string representation of the <see cref="TaskButton"/> property.
        /// </remarks>
        public string       ButtonName { get; private set; }

        /// <summary>
        /// Internal constructor to create an empty result
        /// </summary>
        internal TaskDialogResult()
        {
            TaskButton = TaskButton.None;
            ButtonName = null;
        }

        /// <summary>
        /// Internal constructor to create a result beased on a standard button being clicked
        /// </summary>
        /// <param name="button"></param>
        internal TaskDialogResult( TaskButton button )
        {
            TaskButton = button;
            ButtonName = button.ToString();
        }

        /// <summary>
        /// Internal construct to create a result based on a custom button being clicked
        /// </summary>
        /// <param name="buttonName"></param>
        internal TaskDialogResult( string buttonName )
        {
            TaskButton = TaskButton.Custom;
            ButtonName = buttonName;
        }

        /// <summary>
        /// Gets a flag indicating that the button clicked was a custom button
        /// </summary>
        public bool IsCustomButton
        {
            get { return (TaskButton == TaskButton.Custom); }
        }

        /// <summary>
        /// Provides a way of obtaining the name of the button that was clicked
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ButtonName;
        }

        /// <summary>
        /// Provide a way of comparing this object with another object. 
        /// </summary>
        /// <remarks>
        /// This object can be compared with another <see cref="TaskDialogResult"/>
        /// or a string. If a string is used then the comparison is made with the 
        /// <see cref="ButtonName"/> property.
        /// </remarks>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            bool result = false;

            // Do the comparison based on strings - if we can
            if (obj != null)
            {
                result = (obj.ToString() == ToString());
            }

            // Return the string
            return result;
        }

        /// <summary>
        /// Provide a way ovtaining the custom hashcode of this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return IsCustomButton? ButtonName.GetHashCode() : TaskButton.GetHashCode();
        }


        /// <summary>
        /// Provide a comparison operator so that this object can be compared to a string.
        /// </summary>
        /// <param name="left">The <see cref="TaskDialogResult"/> to compare.</param>
        /// <param name="right">The string to compare</param>
        /// <returns><c>true</c> if the supplied string matches the value 
        /// contained in the <see cref="ButtonName"/> property</returns>
        static public bool operator==( TaskDialogResult left, string right )
        {
            return (left.ButtonName == right);
        }

        /// <summary>
        /// Provides an equality operator so thatthis object can be compared to a string. 
        /// </summary>
        /// <param name="left">The <see cref="TaskDialogResult"/> to compare.</param>
        /// <param name="right">The string to compare</param>
        /// <returns><c>true</c> if the supplied string does not match the value 
        /// contained in the <see cref="ButtonName"/> property</returns>
         static public bool operator!=( TaskDialogResult left, string right )
        {
            return (left.ButtonName != right);
        }

        /// <summary>
        /// Provide a comparison operator so that this object can be compared to a <see cref="TaskButton"/>.
        /// </summary>
        /// <param name="left">The <see cref="TaskDialogResult"/> to compare.</param>
        /// <param name="right">The <see cref="TaskButton"/> to compare</param>
        /// <returns><c>true</c> if the supplied task button matches the value 
        /// contained in the <see cref="TaskButton"/> property</returns>
        static public bool operator==( TaskDialogResult left, TaskButton right )
        {
            return (left.TaskButton == right);
        }

        /// <summary>
        /// Provides an equality operator so thatthis object can be compared to a <see cref="TaskButton"/>. 
        /// </summary>
        /// <param name="left">The <see cref="TaskDialogResult"/> to compare.</param>
        /// <param name="right">The <see cref="TaskButton"/> to compare</param>
        /// <returns><c>true</c> if the supplied string does not match the value 
        /// contained in the <see cref="TaskButton"/> property</returns>
        static public bool operator!=( TaskDialogResult left, TaskButton right )
        {
            return (left.TaskButton != right);
        }
    }
}

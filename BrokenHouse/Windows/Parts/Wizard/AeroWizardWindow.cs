using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Interop;
using BrokenHouse.Windows.Interop;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows.Parts.Wizard
{
    /// <summary>
    /// A special window to be used inconjunction with the <see cref="AeroWizardControl"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation of the <see cref="AeroWindow"/> is designed to provide the default
    /// styles and options to match the Aero wizard style. For example, this window will not
    /// have an icon or caption on the window frame if glass is enabled.
    /// </para>
    /// </remarks>
    public class AeroWizardWindow : AeroWindow
    {
        /// <summary>
        /// Public constructor for the aero wizard window
        /// </summary>
        public AeroWizardWindow()
        {
            NativeWindowStyles.SetCanMaximize(this, false);
            NativeWindowStyles.SetCanMinimize(this, false);
            NativeWindowStyles.SetIsWindowCaptionVisible(this, false);
            NativeWindowStyles.SetIsWindowIconVisible(this, false);
        }

        /// <summary>
        /// Show the dialog centered on the supplied owner
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public bool? ShowDialog( Window owner )
        {
            Window                oldOwner    = Owner;
            WindowStartupLocation oldLocation = WindowStartupLocation;

            // Set the tempoary values
            Owner                 = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Show the dialog
            bool? result = ShowDialog();

            // Reset the owner and location
            Owner                 = oldOwner;
            WindowStartupLocation = oldLocation;

            // Return the result
            return result;
        }
    }

}

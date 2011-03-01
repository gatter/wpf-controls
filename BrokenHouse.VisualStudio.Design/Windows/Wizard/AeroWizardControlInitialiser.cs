using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design;
using BrokenHouse.Windows.Controls;
using BrokenHouse.Windows.Parts.Wizard;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.VisualStudio.Design.Windows.Wizard
{
    internal class AeroWizardControlInitializer : DefaultInitializer
    {
        /// <summary>
        /// Create the aero wizard initialiser
        /// </summary>
        /// <param name="page"></param>
        /// <param name="context"></param>
        public override void InitializeDefaults( ModelItem control, EditingContext context )
        {
            // Create the content
            ModelItem contentPage = ModelFactory.CreateItem(context, typeof(AeroWizardPage), CreateOptions.InitializeDefaults, new object[0]);

            // Adjust to meet our requirements
            contentPage.Properties["Header"].SetValue("Content page");
            contentPage.Properties["Width"].ClearValue();
            contentPage.Properties["Height"].ClearValue();

            // Set the content
            control.Content.Collection.Add(contentPage);
        }
    }

 

}

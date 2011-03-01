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
    internal class ClassicWizardControlInitializer : DefaultInitializer
    {
        /// <summary>
        /// Initialise the classic wizard
        /// </summary>
        /// <param name="control"></param>
        /// <param name="context"></param>
        public override void InitializeDefaults( ModelItem control, EditingContext context )
        {
            // Create the content
            ModelItem startPage   = ModelFactory.CreateItem(context, typeof(ClassicWizardTitlePage), CreateOptions.InitializeDefaults, new object[0]);
            ModelItem contentPage = ModelFactory.CreateItem(context, typeof(ClassicWizardContentPage), CreateOptions.InitializeDefaults, new object[0]);
            ModelItem endPage     = ModelFactory.CreateItem(context, typeof(ClassicWizardTitlePage), CreateOptions.InitializeDefaults, new object[0]);

            // Adjust to meet our requirements
            startPage.Properties["Title"].SetValue("Start Page");
            contentPage.Properties["Title"].SetValue("Content page");
            endPage.Properties["Title"].SetValue("End Page");
            startPage.Properties["Name"].ClearValue();
            contentPage.Properties["Name"].ClearValue();
            endPage.Properties["Name"].ClearValue();

            control.Properties["Width"].ClearValue();
            control.Properties["Height"].ClearValue();

            // Setthe content
            control.Content.Collection.Add(startPage);
            control.Content.Collection.Add(contentPage);
            control.Content.Collection.Add(endPage);
        }
    }

 

}

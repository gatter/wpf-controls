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
    internal class ClassicWizardContentPageInitializer : DefaultInitializer
    {
        /// <summary>
        /// Create the classic page initialiser
        /// </summary>
        /// <param name="page"></param>
        /// <param name="context"></param>
        public override void InitializeDefaults( ModelItem page, EditingContext context )
        {
            // Create the content
            ModelItem content = ModelFactory.CreateItem(context, typeof(Grid), CreateOptions.InitializeDefaults, new object[0]);

            // Adjust to meet our requirements
            content.Properties["Height"].ClearValue();
            content.Properties["Width"].ClearValue();
            content.Properties["Name"].ClearValue();
            content.Properties["HorizontalAlignment"].ClearValue();
            content.Properties["VerticalAlignment"].ClearValue();

            // Update the page
            page.Properties[Metadata.ClassicWizardContentPageTitlePropertyId].SetValue(page.Name);
            page.Properties[Metadata.ClassicWizardContentPageDescriptionPropertyId].SetValue("Description");
            page.Properties[Metadata.FrameworkElementHeightPropertyId].ClearValue();
            page.Properties[Metadata.FrameworkElementWidthPropertyId].ClearValue();  
            page.Properties[Metadata.FrameworkElementHorizontalAlignmentPropertyId].ClearValue();
            page.Properties[Metadata.FrameworkElementVerticalAlignmentPropertyId].ClearValue();

            // Setthe content
            page.Content.SetValue(content);
        }
    }

 

}

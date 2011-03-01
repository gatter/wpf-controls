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

namespace BrokenHouse.Windows.VisualStudio.Design.Wizard
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

            // Update the page
            page.Properties[FrameworkElement.WidthProperty].ClearValue();
            page.Properties[FrameworkElement.HeightProperty].ClearValue();
            page.Properties[ClassicWizardContentPage.TitleProperty].SetValue(page.Name);
            page.Properties[ClassicWizardContentPage.DescriptionProperty].SetValue("Description");

            // Setthe content
            page.Content.SetValue(content);
        }
    }

 

}

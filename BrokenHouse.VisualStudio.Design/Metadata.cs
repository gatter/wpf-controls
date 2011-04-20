using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using BrokenHouse.Windows.Parts.Wizard;
using BrokenHouse.VisualStudio.Design.Windows.Wizard;

    
namespace BrokenHouse.VisualStudio.Design
{
    /// <summary>
    /// MetadataRegistration class.
    /// </summary>
    public class MetadataRegistration : IProvideAttributeTable
    {
        public AttributeTable  AttributeTable
        {
	        get 
            { 
                AttributeTableBuilder builder = new AttributeTableBuilder();

                // This will only be called in Cider.  Hence can reference the other Cider assemblies here as well.
                // Since MenuActions are a Cider only feature, the metadata for adding MenuActions are here.

                builder.AddCustomAttributes(typeof(WizardControl), new FeatureAttribute(typeof(CustomContextMenuProvider)));
                builder.AddCustomAttributes(typeof(WizardControl), new FeatureAttribute(typeof(WizardControlAdornerProvider)), new FeatureAttribute(typeof(WizardControlParentAdapter)));
                builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(WizardControlAdornerProvider)), new FeatureAttribute(typeof(WizardPageParentAdapter)));
                builder.AddCustomAttributes(typeof(ClassicWizardContentPage), new FeatureAttribute(typeof(ClassicWizardContentPageInitializer)));
                builder.AddCustomAttributes(typeof(ClassicWizardTitlePage), new FeatureAttribute(typeof(ClassicWizardTitlePageInitializer)));
                builder.AddCustomAttributes(typeof(AeroWizardPage), new FeatureAttribute(typeof(AeroWizardPageInitializer)));
                builder.AddCustomAttributes(typeof(ClassicWizardControl), new FeatureAttribute(typeof(ClassicWizardControlInitializer)));
                builder.AddCustomAttributes(typeof(AeroWizardControl), new FeatureAttribute(typeof(AeroWizardControlInitializer)));

                return builder.CreateTable();

            }
        }
    }

     public static class Metadata
     {
        public static PropertyIdentifier WizardPageIsFinalPagePropertyId               = new PropertyIdentifier(typeof(WizardPage), "IsFinalPage");
        public static PropertyIdentifier WizardPageIsNextEnabledPropertyId             = new PropertyIdentifier(typeof(WizardPage), "IsNextEnabled");
        public static PropertyIdentifier AeroWizardPageHeaderPropertyId                = new PropertyIdentifier(typeof(AeroWizardPage), "Header");
        public static PropertyIdentifier ClassicWizardTitlePageTitlePropertyId         = new PropertyIdentifier(typeof(ClassicWizardTitlePage), "Title");
        public static PropertyIdentifier FrameworkElementWidthPropertyId               = new PropertyIdentifier(typeof(FrameworkElement), "Width");
        public static PropertyIdentifier FrameworkElementHeightPropertyId              = new PropertyIdentifier(typeof(FrameworkElement), "Height");
        public static PropertyIdentifier FrameworkElementMarginPropertyId              = new PropertyIdentifier(typeof(FrameworkElement), "Margin");
        public static PropertyIdentifier FrameworkElementHorizontalAlignmentPropertyId = new PropertyIdentifier(typeof(FrameworkElement), "HorizontalAlignment");
        public static PropertyIdentifier FrameworkElementVerticalAlignmentPropertyId   = new PropertyIdentifier(typeof(FrameworkElement), "VerticalAlignment");
        public static PropertyIdentifier ClassicWizardContentPageTitlePropertyId       = new PropertyIdentifier(typeof(ClassicWizardContentPage), "Title");
        public static PropertyIdentifier ClassicWizardContentPageDescriptionPropertyId = new PropertyIdentifier(typeof(ClassicWizardContentPage), "Description");

     }
}

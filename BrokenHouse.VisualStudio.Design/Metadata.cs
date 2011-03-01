using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using BrokenHouse.Windows.Parts.Wizard;
using BrokenHouse.VisualStudio.Design.Windows.Wizard;

namespace BrokenHouse.VisualStudio.Design
{
    internal class Metadata : IRegisterMetadata
    {
        public void Register()
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

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}


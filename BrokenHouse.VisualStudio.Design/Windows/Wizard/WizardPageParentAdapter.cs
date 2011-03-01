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
using BrokenHouse.Windows.Controls;
using BrokenHouse.Windows.Parts.Wizard;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.VisualStudio.Design.Windows.Wizard
{
    public class WizardPageParentAdapter : ParentAdapter
    {
        public override bool IsParent(ModelItem parent, ModelItem child)
        {
            return base.IsParent(parent, child);
        }
        /// <summary>
        /// Common check function
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childType"></param>
        /// <returns></returns>
        private ModelItem CheckRedirectParent( ModelItem parent, Type childType )
        {
            WizardPage    wizardPage    = parent.View.PlatformObject as WizardPage;
            WizardControl wizardControl = parent.Parent.View.PlatformObject as WizardControl;
            ModelItem     checkedParent = parent;

            if ((wizardPage != null) && ((wizardControl == null) || !wizardPage.IsActive))
            {
                // Cannot parent suggested parent is not the active parent
                checkedParent = null;
            }
            else if (typeof(WizardPage).IsAssignableFrom(childType))
            {
                // Cannot parent a wizard page to a wizard page
                checkedParent = null;
            }
            else if ((parent.Content != null) && parent.Content.IsSet)
            {
                // The new parent has content
                ModelItem contentItem    = parent.Content.Value;
                UIElement contentElement = (contentItem == null)? null : contentItem.View.PlatformObject as UIElement;

                // Is the conent element valid
                if (contentElement != null)
                {
                    // Is the page active and the content not visible?
                    if (wizardPage.IsActive && !contentElement.IsVisible)
                    {
                        // Yes - triger a relayout
                        wizardPage.UpdateLayout();
                    }

                    // Is the content a panel or content control
                    if (contentElement.IsVisible && ((contentElement is Panel) || (contentElement is ContentControl)))
                    {
                        checkedParent = contentItem;
                    }
                }
            }
            else
            {
                checkedParent = parent;
            }

            return checkedParent;
        }

        /// <summary>
        // The parent has changed for the child
        /// </summary>
        /// <param name="newParent"></param>
        /// <param name="child"></param>
        public override void Parent( ModelItem newParent, ModelItem child )
        {
            if (newParent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            // Ensure that the child has the appropriate propreties set
            child.Properties[Metadata.FrameworkElementMarginPropertyId].ClearValue();
            child.Properties[Metadata.FrameworkElementHeightPropertyId].ClearValue();
            child.Properties[Metadata.FrameworkElementWidthPropertyId].ClearValue();

            // Add the item
            newParent.Content.SetValue(child);
        }

        /// <summary>
        /// Determine if we can parent the child type
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childType"></param>
        /// <returns></returns>
        public override bool CanParent(ModelItem parent, Type childType)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (childType == null)
            {
                throw new ArgumentNullException("childType");
            }

            return (CheckRedirectParent(parent, childType) != null);
        }

        /// <summary>
        /// Work out the parent to redirect to
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childType"></param>
        /// <returns></returns>
        public override ModelItem RedirectParent(ModelItem parent, Type childType)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (childType == null)
            {
                throw new ArgumentNullException("childType");
            }

            // Obtain the parent
            return CheckRedirectParent(parent, childType) ?? parent;
        }

        /// <summary>
        /// The parent has been removed
        /// </summary>
        /// <param name="currentParent"></param>
        /// <param name="newParent"></param>
        /// <param name="child"></param>
        public override void RemoveParent( ModelItem currentParent, ModelItem newParent, ModelItem child )
        {
            if (currentParent == null)
            {
                throw new ArgumentNullException("currentParent");
            }
            if (newParent == null)
            {
                throw new ArgumentNullException("newParent");
            }
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            // Remove the page from the wizard
            currentParent.Content.ClearValue();
        }
    }
}

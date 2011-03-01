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

namespace BrokenHouse.Windows.VisualStudio.Design.Wizard
{
    internal class WizardControlParentAdapter : ParentAdapter
    {
        private WizardPage m_LastPageAdded;

        /// <summary>
        /// Helper class to see if we can parent and redirect to the correct parent
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childType"></param>
        /// <returns></returns>
        private ModelItem CheckRedirectParent( ModelItem parent, Type childType )
        {
            WizardControl wizardControl = parent.View as WizardControl;
            DesignerView  designerView  = DesignerView.GetDesignerView(wizardControl);
            ModelItem     checkedParent = null;

            // Check what we can do
            if (typeof(WizardPage).IsAssignableFrom(childType))
            {
                // Simple case
                checkedParent = parent;
            }
            else if (((wizardControl == null) || (parent.Content == null)) || (parent.Content.Collection.Count <= 0))
            {
                // Its a simple add
                checkedParent = parent;
            }
            else if ((designerView != null) && (designerView.Context != null))
            {
                Tool tool        = designerView.Context.Items.GetValue<Tool>();
                int  activeIndex = wizardControl.ActiveIndex;

                // Do we have a valid index
                if (activeIndex >= 0)
                {
                    // Are we focused
                    if ((tool == null) || (tool.FocusedTask != null))
                    {
                        checkedParent = parent.Content.Collection[activeIndex];
                    }
                    else
                    {
                        AdapterService adapterService = designerView.Context.Services.GetService<AdapterService>();
                        
                        checkedParent = FindSuitableParent(adapterService, parent, childType, activeIndex) ?? parent;
                    }
                }
            }
            else
            {
                // Did not find anything
            }
        
            return checkedParent;
        }

        /// <summary>
        /// Helper to find a suitable parent
        /// </summary>
        /// <param name="adapterService"></param>
        /// <param name="parent"></param>
        /// <param name="childType"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private ModelItem FindSuitableParent( AdapterService adapterService, ModelItem parent, Type childType, int index )
        {
            ModelItem item = null;

            // Do we have a valid service?
            if (adapterService != null)
            {
                ModelItem       pageItem       = parent.Content.Collection[index];
                ParentAdapter   adapter        = adapterService.GetAdapter<ParentAdapter>(pageItem.ItemType);
                List<ModelItem> processedItems = new List<ModelItem>();

                ModelItem thisItem = pageItem;
                ModelItem nextItem = adapter.RedirectParent(thisItem, childType);

                // Add the items to the processed list
                processedItems.Add(thisItem);
                processedItems.Add(nextItem);

                // Keep redirecting until we stop.
                while (thisItem != nextItem)
                {
                    // Update this item
                    thisItem = nextItem;

                    // Obtain the new adapter
                    adapter  = adapterService.GetAdapter<ParentAdapter>(thisItem.ItemType);

                    // Redirect the parent
                    nextItem = adapter.RedirectParent(thisItem, childType);

                    // If we have already hit this item then stop
                    if (processedItems.Contains(nextItem))
                    {
                        break;
                    }

                    // Add the next item so that we do not hit it again
                    processedItems.Add(nextItem);
                }

                // Can we actually parent
                if (adapter.CanParent(thisItem, childType))
                {
                    item = thisItem;
                }
            }

            // Return the most suitable parent
            return item;
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
            child.Properties[FrameworkElement.MarginProperty].ClearValue();
            child.Properties[FrameworkElement.HeightProperty].ClearValue();
            child.Properties[FrameworkElement.WidthProperty].ClearValue();
            child.Properties[WizardPage.IsNextEnabledProperty].SetValue(true);

            // Obtain the wizard control and page
            WizardControl wizardControl = newParent.View as WizardControl;
            WizardPage    wizardPage    = child.View as WizardPage;
            bool          isWizardEmpty = (newParent.Content.Collection.Count == 0);
            DesignerView  designerView  = DesignerView.GetDesignerView(wizardControl);

            // Save the last page added
            m_LastPageAdded = wizardPage;

            // Do we have a valid designer view
            if ((designerView != null) && (designerView.Context != null))
            {
                // Are we adding the correct item
                if (wizardPage != null)
                {
                    bool allowAdd = false;

                    // Check the types
                    if ((wizardControl is AeroWizardControl) && (wizardPage is AeroWizardPage))
                    {
                        allowAdd = true;
                    }
                    else if ((wizardControl is ClassicWizardControl) && (wizardPage is ClassicWizardContentPage))
                    {
                        allowAdd = true;
                    }
                    else if ((wizardControl is ClassicWizardControl) && (wizardPage is ClassicWizardTitlePage))
                    {
                        allowAdd = true;
                    }
                    else
                    {
                        // Page and wizard are mismatched
                    }

                    // Can we add
                    if (allowAdd)
                    {
                        Tool tool = designerView.Context.Items.GetValue<Tool>();

                        // Are we focused
                        if ((tool != null) && (tool.FocusedTask != null))
                        {
                            // Yes - we have to make the page the active page after the focus
                            tool.FocusedTask.Completed += OnFocusedTaskCompleted;
                        }

                        // Add he child
                        newParent.Content.Collection.Add(child);
                    }
                }
                else
                {
                    // Lets try to create the content for the item
                    try
                    {
                        Type      newPageType  = null;

                        // What page are we creating
                        if (wizardControl is AeroWizardControl)
                        {
                            newPageType = typeof(AeroWizardPage);
                        }
                        else
                        {
                            newPageType = isWizardEmpty? typeof(ClassicWizardTitlePage) : typeof(ClassicWizardContentPage);
                        }

                        // Create the new page
                        ModelItem newPageItem  = ModelFactory.CreateItem(designerView.Context, newPageType, CreateOptions.InitializeDefaults, new object[0]);
                        int       newPageIndex = newParent.Content.Collection.Count;

                        // Add this new item to the parent
                        newParent.Content.Collection.Add(newPageItem);

                        // Make this the active item
                        wizardControl.ActivePage = newPageItem.View as WizardPage;

                        // If we have an element then invalidate the control
                        if (wizardControl != null)
                        {
                            wizardControl.InvalidateArrange();
                            wizardControl.InvalidateMeasure();
                            wizardControl.UpdateLayout();
                        }

                        // Find the adapter service through which we can find a suitable parent
                        AdapterService adapterService    = designerView.Context.Services.GetService<AdapterService>();
                        ModelItem      newSuitableParent = this.FindSuitableParent(adapterService, newParent, child.ItemType, newPageIndex);

                        // Did we find it
                        if (newSuitableParent != null)
                        {
                            // Set the parent of the child
                            adapterService.GetAdapter<ParentAdapter>(newSuitableParent.ItemType).Parent(newSuitableParent, child);
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new InvalidOperationException("Parenting Failed", exception.InnerException);
                    }
                }
            }
        }

        /// <summary>
        /// A focus task has completed - select the page we have just added
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFocusedTaskCompleted(object sender, EventArgs e)
        {
            Task task = sender as Task;

            // Was it a task - if so clean up
            if (task != null)
            {
                task.Completed -= OnFocusedTaskCompleted;
            }
            if (m_LastPageAdded != null)
            {
                WizardControl wizardControl = m_LastPageAdded.FindVisualAncestor<WizardControl>();

                if (wizardControl != null)
                {
                    wizardControl.ActivePage = m_LastPageAdded;
                }
            }
        }
 
        /// <summary>
        /// Determine if we can parent the child type
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childType"></param>
        /// <returns></returns>
        public override bool CanParent( ModelItem parent, Type childType )
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (childType == null)
            {
                throw new ArgumentNullException("childType");
            }

            return (CheckRedirectParent(parent, childType) == parent);
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
            return CheckRedirectParent(parent, childType) ?? base.RedirectParent(parent, childType);
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
            currentParent.Content.Collection.Remove(child);
        }
    }
}

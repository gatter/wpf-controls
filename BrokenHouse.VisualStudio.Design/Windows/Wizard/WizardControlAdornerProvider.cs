using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Model;
using BrokenHouse.Windows.Controls;
using BrokenHouse.Windows.Parts.Wizard;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.VisualStudio.Design.Windows.Wizard
{
    [UsesItemPolicy(typeof(SelectedWizardPagePolicy))]
    internal class WizardControlAdornerProvider : PrimarySelectionAdornerProvider
    {
        private const float                    c_NudgeAmount       = 20;
        private AdornerPanel                   m_PopupAdornerPanel = new AdornerPanel() { IsContentFocusable = true };
        private static WizardControlOptions    s_WizardOptions     = new WizardControlOptions();

        /// <summary>
        /// Construct the adorner provider
        /// </summary>
        public WizardControlAdornerProvider()
        {
            // Position the adoner
            AdornerPlacementCollection popupPlacement = new AdornerPlacementCollection();

            // these ones are Relative to Adorner since the size and position is based on the
            // size of the adorner itself (i.e. I want it to be the desired size and position it
            // so that it is on the outside corner of the adorned element plus some nudge amount
            // NOTE: Coordinate space starts at 0,0 for the adorned element
            popupPlacement.SizeRelativeToAdornerDesiredHeight(1.0, 0);
            popupPlacement.SizeRelativeToAdornerDesiredWidth(1.0, 0);
            popupPlacement.PositionRelativeToContentWidth(1.0, c_NudgeAmount);
            AdornerPanel.SetPlacements(s_WizardOptions, popupPlacement);
            AdornerProperties.SetOrder(m_PopupAdornerPanel, AdornerOrder.Foreground);

            // Detath from the old parent
            if (s_WizardOptions.Parent != null)
            {
                (s_WizardOptions.Parent as AdornerPanel).Children.Remove(s_WizardOptions);
            }

            // And add it to the list of AdornerPanels provided by this AdornerProvider
            m_PopupAdornerPanel.Children.Add(s_WizardOptions);

            // Finally, add our panel to the Adorners collection
            Adorners.Add(m_PopupAdornerPanel);
        }

        /// <summary>
        /// Our control has been activated
        /// </summary>
        /// <param name="item"></param>
        /// <param name="view"></param>
        protected override void Activate( ModelItem item )
        {
            // What type is it
            if (item.ItemType.IsSubclassOf(typeof(WizardControl)))
            {
                // Its the wizard control
                s_WizardOptions.Activate(Context, item, new Point(0, 0));
            }
            else if ((item.Parent != null) && item.ItemType.IsSubclassOf(typeof(WizardPage)) && item.Parent.ItemType.IsSubclassOf(typeof(WizardControl)))
            {
                WizardPage    wizardPage    = item.View.PlatformObject as WizardPage;
                WizardControl wizardControl = wizardPage.ParentWizard;
                WizardPage    originalPage  = wizardControl.ActivePage;
                DesignerView  designerView  = DesignerView.GetDesignerView(wizardControl);

                // Do we have a designer view
                if ((designerView != null) && (designerView.Context != null))
                {
                    Tool tool = designerView.Context.Items.GetValue<Tool>();

                    // Is it to do with focusing
                    if ((tool == null) || (tool.FocusedTask == null))
                    {
                        wizardControl.ActivePage = wizardPage;
                    }
                }

                // Ensure everything is ready
                wizardControl.InvalidateArrange();
                wizardControl.InvalidateMeasure();
                wizardControl.InvalidateVisual();
                wizardControl.UpdateLayout();

                // Only active if we can
                if (wizardPage.IsDescendantOf(wizardControl))
                {
                    // Determine the change in offset
                    Rect controlBounds = new Rect(0.0, 0.0, wizardControl.ActualWidth, wizardControl.ActualHeight);
                    Rect transformBounds = wizardControl.TransformToDescendant(wizardPage).TransformBounds(controlBounds);

                    // Activate the parent
                    s_WizardOptions.Activate(Context, item.Parent, new Point(transformBounds.Right - wizardPage.ActualWidth, transformBounds.Top));
                }
            }

            // Call the base
            base.Activate(item);
        }

        /// <summary>
        /// Our Control has been deactivated
        /// </summary>
        protected override void Deactivate()
        {
            base.Deactivate();

            // Call the base
            s_WizardOptions.Deactivate();
        }
    }
}

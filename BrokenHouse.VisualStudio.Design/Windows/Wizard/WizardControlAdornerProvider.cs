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
        private const float                           c_NudgeAmount       = 20;
        private AdornerPanel                          m_PopupAdornerPanel = new AdornerPanel() { IsContentFocusable = true };
        private static WizardControlOptions           s_WizardOptions     = null;
        private static AdornerPanel                   s_OptionsPanel      = null;

        static WizardControlAdornerProvider()
        {
            // Create the static wizard control
            s_WizardOptions = new WizardControlOptions();
        
            // Position the adorner
            AdornerPlacementCollection popupPlacement = new AdornerPlacementCollection();
            
            // Define the placement
            popupPlacement.SizeRelativeToAdornerDesiredHeight(1.0, 0);
            popupPlacement.SizeRelativeToAdornerDesiredWidth(1.0, 0);
            popupPlacement.PositionRelativeToContentWidth(1.0, c_NudgeAmount);

            // Associate the placement with the options
            AdornerPanel.SetPlacements(s_WizardOptions, popupPlacement);
        }

        /// <summary>
        /// Construct the adorner provider
        /// </summary>
        public WizardControlAdornerProvider()
        {
            AdornerProperties.SetOrder(m_PopupAdornerPanel, AdornerOrder.Foreground);
 
            Adorners.Add(m_PopupAdornerPanel);
        }

        /// <summary>
        /// Our control has been activated
        /// </summary>
        /// <param name="item"></param>
        /// <param name="view"></param>
        protected override void Activate( ModelItem item )
        {
            if ( item.View == null)
            {
                // Nothing to activate
            }
            else if (item.ItemType.IsSubclassOf(typeof(WizardControl)))
            {
                WizardControl    wizardControl    = item.View.PlatformObject as WizardControl;

                // Its the wizard control
                if (wizardControl != null)
                {
                    ActivateOptions(item, new Point(0, 0));
                }
            }
            else if ((item.Parent != null) && item.ItemType.IsSubclassOf(typeof(WizardPage)) && item.Parent.ItemType.IsSubclassOf(typeof(WizardControl)))
            {
                ModelItem     wizardControlItem = item.Parent;
                WizardPage    wizardPage        = item.View.PlatformObject as WizardPage;
                WizardControl wizardControl     = wizardControlItem.View.PlatformObject as WizardControl;
                WizardPage    originalPage      = wizardControl.ActivePage;
                DesignerView  designerView      = DesignerView.GetDesignerView(wizardControl);
                int           newIndex          = wizardControl.Pages.IndexOf(wizardPage);
                
                // Do we have a designer view
                if ((designerView != null) && (designerView.Context != null))
                {
                    Tool tool = designerView.Context.Items.GetValue<Tool>();

                    // Is it to do with focusing
                    if ((tool == null) || (tool.FocusedTask == null))
                    {
                        if (wizardControl.ActiveIndex != newIndex)
                        {
                            wizardControl.ActiveIndex = newIndex;

                            wizardControlItem.Properties["ActiveIndex"].SetValue(newIndex);
                        }
                    }
                }
               

                // Ensure everything is ready
                wizardControl.InvalidateArrange();
                wizardControl.InvalidateMeasure();
                wizardControl.InvalidateVisual();
                wizardControl.UpdateLayout();


                if (newIndex != -1)
                {
                    WizardPage targetPage = wizardControl.Pages[newIndex] as WizardPage;

                    // Determine the change in offset
                    Rect controlBounds = new Rect(0.0, 0.0, wizardControl.ActualWidth, wizardControl.ActualHeight);
                    Rect transformBounds = wizardControl.TransformToDescendant(wizardPage).TransformBounds(controlBounds);

                    // Activate the parent
                    ActivateOptions(item.Parent, new Point(transformBounds.Right - wizardPage.ActualWidth, transformBounds.Top));
                }
            }

            // Call the base
            base.Activate(item);
        }

        private void ActivateOptions( ModelItem item, Point position )
        {  
            // If we are activated on another item - remove the options from that panel
            if (s_OptionsPanel != null)
            {
                s_OptionsPanel.Children.Remove(s_WizardOptions);
                s_OptionsPanel = null;
                s_WizardOptions.Deactivate();
            }

            // And add it to the list of AdornerPanels provided by this AdornerProvider
            m_PopupAdornerPanel.Children.Add(s_WizardOptions);
            s_OptionsPanel = m_PopupAdornerPanel;
            
            // Activate the options with this context
            s_WizardOptions.Activate(Context, item, position);
        }

        /// <summary>
        /// Our Control has been deactivated
        /// </summary>
        protected override void Deactivate()
        {
            // Call the base
            base.Deactivate();

            // Only remove the options if it is associated with our panel
            if (s_OptionsPanel == m_PopupAdornerPanel)
            {
                s_OptionsPanel.Children.Remove(s_WizardOptions);
                s_OptionsPanel = null;
                s_WizardOptions.Deactivate();
            }

        }
    }
}

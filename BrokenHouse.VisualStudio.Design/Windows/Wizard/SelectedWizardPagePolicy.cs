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

namespace BrokenHouse.VisualStudio.Design.Windows
{
    internal class SelectedWizardPagePolicy : SelectionPolicy
    {
        protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection)
        {
            ModelItem primarySelection = selection.PrimarySelection;
            List<ModelItem> list = new List<ModelItem>();

            if (primarySelection != null)
            {
                for (ModelItem item2 = primarySelection; item2 != null; item2 = item2.Parent)
                {
                    if (typeof(WizardPage).IsAssignableFrom(item2.ItemType))
                    {
                        list.Add(item2);
                        break;
                    }
                    else if (typeof(WizardControl).IsAssignableFrom(item2.ItemType))
                    {
                        list.Add(item2);
                        break;
                    }
                    else
                    {
                        // Ignore the item
                    }
                }
            }
            return list;
        }
    }
}

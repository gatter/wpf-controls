using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System.Windows.Controls;
using System.Windows.Media;

namespace BrokenHouse.VisualStudio.Design
{
    public class CustomContextMenuProvider : PrimarySelectionContextMenuProvider
    {
        public CustomContextMenuProvider()
        {
            MenuAction setBackgroundToBlueMenuAction = new MenuAction("Blue");
            //setBackgroundToBlueMenuAction.Execute += new EventHandler<MenuActionEventArgs>(SetBackgroundToBlue_Execute);

            MenuAction clearBackgroundMenuAction = new MenuAction("Cleared");
            //clearBackgroundMenuAction.Execute += new EventHandler<MenuActionEventArgs>(ClearBackground_Execute);


            // Flyouts with actions
            MenuGroup backgroundFlyoutGroup = new MenuGroup("SetBackgroundsGroup", "Set Background");

            // if this is false, this group will not show up as a flyout but inline
            backgroundFlyoutGroup.HasDropDown = true;
            backgroundFlyoutGroup.Items.Add(setBackgroundToBlueMenuAction);
            backgroundFlyoutGroup.Items.Add(clearBackgroundMenuAction);
            this.Items.Add(backgroundFlyoutGroup);

            // Called right before this provider shows its tabs, opportunity to set states
            // UpdateItemStatus += new EventHandler<MenuActionEventArgs>(CustomContextMenuProvider_UpdateItemStatus);    
        }
    }
}

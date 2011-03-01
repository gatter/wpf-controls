using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BrokenHouse.Windows.Parts.Wizard.Primitives;

namespace BrokenHouse.Windows.Parts.Wizard
{
    /// <summary>
    /// The role of this class is to provide keys for all aspects of the sytles and colours
    /// used in the Wizards.
    /// </summary>
    public static class WizardElements
    {
       /// <summary>
        /// Gets the key used to identify the resource used for the background of the classic wizard title page.
        /// </summary>
        public static ComponentResourceKey ClassicTitlePageBackgroundKey     {get; private set;}

        /// <summary>
        /// Gets the key used to identify the style used for the classic wizard title page.
        /// </summary>
        public static ComponentResourceKey ClassicTitlePageStyleKey          {get; private set;}

        /// <summary>
        /// Gets the key used to identify the brush used for the background of the classic wizard page.
        /// </summary>
        public static ComponentResourceKey ClassicContentPageBackgroundKey    {get; private set;}

        /// <summary>
        /// Gets the key used to identify the brush used for the border of the classic wizard page.
        /// </summary>
        public static ComponentResourceKey ClassicWizardPageBorderKey        {get; private set;}
        
        /// <summary>
        /// Gets the key used to identify the style of the classic wizard page.
        /// </summary>
        public static ComponentResourceKey ClassicContentPageStyleKey         {get; private set;}

        /// <summary>
        /// Gets the key used to identify the style for the classic wizard.
        /// </summary>
        public static ComponentResourceKey ClassicWizardStyleKey             {get; private set;}

        /// <summary>
        /// Gets the key used to identify the brush used for the footer of the classic wizard.
        /// </summary>
        public static ComponentResourceKey ClassicWizardFooterBackgroundKey  {get; private set;}

        /// <summary>
        /// Gets the key used to identify the brush used for the header of the classic wizard.
        /// </summary>
        public static ComponentResourceKey ClassicWizardHeaderBackgroundKey  {get; private set;}

        /// <summary>
        /// Gets the key used to identify the style used for the header of the classic wizard.
        /// </summary>
        public static ComponentResourceKey ClassicWizardHeaderStyleKey       {get; private set;}

        /// <summary>
        /// Gets the key used to identify the brush used for the background of the aero wizard page.
        /// </summary>
        public static ComponentResourceKey AeroWizardPageBackgroundKey       {get; private set;}

        /// <summary>
        /// Gets the key used to identify the style used for the aero wizard page.
        /// </summary>
        public static ComponentResourceKey AeroWizardPageStyleKey            {get; private set;}

        /// <summary>
        /// Gets the key used to identify the style used for the aero wizard.
        /// </summary>
        public static ComponentResourceKey AeroWizardStyleKey                {get; private set;}

        /// <summary>
        /// Gets the key used to identify the style used for the back button in the aero wizard
        /// </summary>
        public static ComponentResourceKey AeroWizardBackButtonStyleKey      {get; private set;}

        /// <summary>
        /// Gets the key used to identify the background used for the button bar in the aero wizard
        /// </summary>
        public static ComponentResourceKey AeroWizardButtonBarBackgroundKey  {get; private set;}

        /// <summary>
        /// Gets the key used to identify the brush used to separate key areas of the control
        /// </summary>
        public static ComponentResourceKey AeroWizardPageBorderKey  {get; private set;}

        /// <summary>
        /// Gets the key used to identify the border brush used in the error section of the control
        /// </summary>
        public static ComponentResourceKey AeroWizardErrorBorderKey  {get; private set;}

        /// <summary>
        /// Gets the key used to identify the background of the error section of the control
        /// </summary>
        public static ComponentResourceKey AeroWizardErrorBackgroundKey  {get; private set;}

        /// <summary>
        /// Gets the key used to identify the background of the error section of the control
        /// </summary>
        public static ComponentResourceKey AeroWizardErrorForegroundKey  {get; private set;}

        /// <summary>
        /// Gets the key used to identify the background used for the classic themed aero wizard
        /// </summary>
        public static ComponentResourceKey AeroWizardClassicPageBackgroundKey  {get; private set;}
       
        /// <summary>
        /// Create all the resource keys so that they can be used in the XAML.
        /// </summary>
        static WizardElements()
        {
            ClassicTitlePageBackgroundKey      = new ComponentResourceKey(typeof(ClassicWizardTitlePage),    "ClassicTitlePageBackground");
            ClassicTitlePageStyleKey           = new ComponentResourceKey(typeof(ClassicWizardTitlePage),    "ClassicTitlePageStyle");
            ClassicWizardPageBorderKey         = new ComponentResourceKey(typeof(ClassicWizardContentPage),    "ClassicTitlePageBorder");
            ClassicContentPageBackgroundKey    = new ComponentResourceKey(typeof(ClassicWizardContentPage),   "ClassicContentPageBackground");
            ClassicContentPageStyleKey         = new ComponentResourceKey(typeof(ClassicWizardContentPage),   "ClassicContentPageStyle");
            ClassicWizardStyleKey              = new ComponentResourceKey(typeof(ClassicWizardControl),"ClassicWizardStyle");
            ClassicWizardHeaderBackgroundKey   = new ComponentResourceKey(typeof(ClassicWizardHeader), "ClassicWizardHeaderBackground");
            ClassicWizardHeaderStyleKey        = new ComponentResourceKey(typeof(ClassicWizardHeader), "ClassicWizardHeaderStyle");
            ClassicWizardFooterBackgroundKey   = new ComponentResourceKey(typeof(ClassicWizardHeader), "ClassicWizardFooterBackground");
            AeroWizardClassicPageBackgroundKey = new ComponentResourceKey(typeof(AeroWizardControl),   "AeroWizardClassicPageBackground");
            AeroWizardPageBackgroundKey        = new ComponentResourceKey(typeof(AeroWizardControl),   "AeroWizardPageBackground");
            AeroWizardPageStyleKey             = new ComponentResourceKey(typeof(AeroWizardControl),   "AeroWizardPageStyle");
            AeroWizardStyleKey                 = new ComponentResourceKey(typeof(AeroWizardControl),   "AeroWizardStyle");
            AeroWizardBackButtonStyleKey       = new ComponentResourceKey(typeof(AeroWizardControl),   "AeroWizardBackButtonStyle");
            AeroWizardButtonBarBackgroundKey   = new ComponentResourceKey(typeof(AeroWizardControl),   "AeroWizardButtonBarBackground");
            AeroWizardPageBorderKey            = new ComponentResourceKey(typeof(AeroWizardControl),   "AeroWizardPageBorder");
            AeroWizardErrorBorderKey           = new ComponentResourceKey(typeof(AeroWizardControl),   "AeroWizardErrorBorder");
            AeroWizardErrorBackgroundKey       = new ComponentResourceKey(typeof(AeroWizardControl),   "AeroWizardErrorBackground");
            AeroWizardErrorForegroundKey       = new ComponentResourceKey(typeof(AeroWizardControl),   "AeroWizardErrorForeground");
        }
    }
}

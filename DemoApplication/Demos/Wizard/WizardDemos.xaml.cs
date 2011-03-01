using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrokenHouse.Windows.Extensions;
using DemoApplication.Demos.Wizard.Registration;
using DemoApplication.Demos.Wizard.Connection;

namespace DemoApplication.Demos.Wizard
{
    /// <summary>
    /// Interaction logic for WizardDemos.xaml
    /// </summary>
    public partial class WizardDemos : DemoItem
    {
        public WizardDemos()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Show the Classic Registration dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowClassicRegistraionWizard(object sender, RoutedEventArgs e)
        {
            ClassicRegistrationWizard dialog = new ClassicRegistrationWizard{ Owner = this.FindVisualAncestor<Window>() };

            dialog.ShowDialog();            
        }

        /// <summary>
        /// Show the Aero Registrations dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowAeroRegistraionWizard(object sender, RoutedEventArgs e)
        {
            AeroRegistrationWizard dialog = new AeroRegistrationWizard{ Owner = this.FindVisualAncestor<Window>() };

            dialog.ShowDialog();            
        }

           
        /// <summary>
        /// Show the Aero Connection wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowAeroConnectionWizard(object sender, RoutedEventArgs e)
        {
            AeroConnectionWizard dialog = new AeroConnectionWizard{ Owner = this.FindVisualAncestor<Window>() };

            dialog.ShowDialog();
        }       
        
        /// <summary>
        /// Show the Custom Aero Connection wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowCustomConnectionWizard(object sender, RoutedEventArgs e)
        {
            CustomConnectionWizard dialog = new CustomConnectionWizard{ Owner = this.FindVisualAncestor<Window>() };

            dialog.ShowDialog();
        }
        
        /// <summary>
        /// Show the Custom Aero Connection wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowSequencedConnectionWizard(object sender, RoutedEventArgs e)
        {
            SequencedConnectionWizard dialog = new SequencedConnectionWizard{ Owner = this.FindVisualAncestor<Window>() };

            dialog.ShowDialog();
        }
    }
}

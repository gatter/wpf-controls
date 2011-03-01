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
using BrokenHouse.Windows.Parts.Wizard;

namespace DemoApplication.Demos.Wizard.Connection
{
    /// <summary>
    /// Interaction logic for CheckAuthenticationPage.xaml
    /// </summary>
    public partial class CheckAuthenticationPage : AeroWizardPage
    {
        public CheckAuthenticationPage()
        {
            // Initialise the dialog
            InitializeComponent();

            // Set the password in the dialog
            if (Model != null)
            {   
                m_Password.Password = Model.Password;
            }
        }
 
        /// <summary>
        /// The data context has the details so just use the context
        /// </summary>
        public ConnectionModel Model
        {
            get { return DataContext as ConnectionModel; }
        }
 
        /// <summary>
        /// We want to know when the page is deactivating
        /// </summary>
        /// <param name="args"></param>
        protected override void OnPageDeactivating( WizardPageChangingEventArgs args )
        {
            if (args.ChangeType != WizardPageChangeType.NavigateBack)
            {
                MockAuthenticationService service = new MockAuthenticationService(Model.Hostname);

                if (!service.CheckAuthentication(Model.Credential))
                {
                    args.CancelChange("Authentication failed, please check credentials and try again.");
                }
            }

            // Call the default
            base.OnPageDeactivating(args);
        }

        /// <summary>
        /// The password box does not allow binding so we have to populate the model
        /// ourselves. As seeon as it is set it will become encrypted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Model.Password = m_Password.Password;
        }
    }
}

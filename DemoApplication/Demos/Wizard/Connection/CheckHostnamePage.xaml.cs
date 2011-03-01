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
    /// Interaction logic for CheckHostnamePage.xaml
    /// </summary>
    public partial class CheckHostnamePage : AeroWizardPage
    {
        /// <summary>
        /// Constructor for the hostname page
        /// </summary>
        public CheckHostnamePage()
        {
            InitializeComponent();
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
                MockHandshakeService service = new MockHandshakeService(Model.Hostname);
                Version              version = service.GetVersion();
            }

            // Call the default
            base.OnPageDeactivating(args);
        }

    }
}

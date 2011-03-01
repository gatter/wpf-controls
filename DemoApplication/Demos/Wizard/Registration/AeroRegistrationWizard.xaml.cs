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
using System.Windows.Shapes;
using BrokenHouse.Windows;
using BrokenHouse.Windows.Parts.Wizard;

namespace DemoApplication.Demos.Wizard.Registration
{
    /// <summary>
    /// Interaction logic for AeroRegistrationWizard.xaml
    /// </summary>
    public partial class AeroRegistrationWizard : AeroWizardWindow
    {
        public RegistrationModel  Model { get; set; }

        public AeroRegistrationWizard()
        {
            DataContext = Model = new RegistrationModel();

            InitializeComponent();
        }
    }
}

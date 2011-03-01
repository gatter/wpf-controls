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

namespace DemoApplication.Demos.Wizard.Connection
{
    /// <summary>
    /// Interaction logic for TestAeroWizard.xaml
    /// </summary>
    public partial class CustomConnectionWizard : Window
    {
        public CustomConnectionWizard()
        {
            DataContext = new ConnectionModel();

            InitializeComponent();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using DemoApplication.Demos;
using DemoApplication.Demos.Controls;
using DemoApplication.Demos.Task;
using DemoApplication.Demos.Wizard;
using DemoApplication.Demos.Transition;

namespace DemoApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<DemoItem>   AllItems { get; private set; }

        public MainWindow()
        {
            DataContext = this;

            AllItems = new List<DemoItem> { new Introduction(), new Releases(),
                                            new ControlsOverview(), new ControlsFeatures(), new IconElement(), new ActiveIconElement(),
                                            new CommandLinkElement(), new IconColorTransforms(), new StockIcons(), new SnapDecoratorElement(),
                                            new EdgeScrollViewer(),
                                            new TaskDialogOverview(), new TaskDialogFeatures(), new TaskDialogDemos(),
                                            new WizardOverview(), new WizardFeatures(), new WizardDemos(),
                                            new TransitionOverview(), new TransitionFeatures(), new TransitionControl(), new TransitionItemsControl()
            };
            
            InitializeComponent();


        }

    }
}

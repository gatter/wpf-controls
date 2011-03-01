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
using DemoApplication.Demos.Task;

namespace DemoApplication.Demos.Task
{
    /// <summary>
    /// Interaction logic for TaskDialogTests.xaml
    /// </summary>
    public partial class TaskDialogDemos : DemoItem
    {
        public TaskDialogDemos()
        {
            InitializeComponent();
        }

        
        /// <summary>
        /// Show the copy dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowCopyProgressDialog(object sender, RoutedEventArgs e)
        {
            CopyProgressDialog dialog = new CopyProgressDialog{ Owner = this.FindVisualAncestor<Window>() };

            dialog.ShowDialog();
        }

        /// <summary>
        /// Show the move confirmation dialog
        /// </summary>
        /// <param name="sender"></param>Confirmation
        /// <param name="e"></param>
        private void OnShowMoveConfirmationDialog(object sender, RoutedEventArgs e)
        {
            MoveConfirmationDialog dialog = new MoveConfirmationDialog{ Owner = this.FindVisualAncestor<Window>() };

            dialog.ShowDialog();
        }

        /// <summary>
        /// Show simple dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowTaskDialogCreator(object sender, RoutedEventArgs e)
        {
            TaskDialogCreator dialog = new TaskDialogCreator { Owner = this.FindVisualAncestor<Window>() };

            dialog.ShowDialog();
        }

           
        /// <summary>
        /// Show resizing dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowResizingTaskDialog(object sender, RoutedEventArgs e)
        {
            ResizingTaskDialog dialog = new ResizingTaskDialog{ Owner = this.FindVisualAncestor<Window>() };

            dialog.ShowDialog();
        }
    }
}

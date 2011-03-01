using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using BrokenHouse.Extensions;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Parts.Task;

namespace DemoApplication.Demos.Task
{
    /// <summary>
    /// Interaction logic for CopyDialog.xaml
    /// </summary>
    public partial class CopyProgressDialog : TaskDialogWindow, INotifyPropertyChanged
    {
        public CopyProgressDialog()
        {
            // Initialise the start time
            StartTime = DateTime.Now;
            Timer = new DispatcherTimer(new TimeSpan(10000 * 500), DispatcherPriority.Background, OnTimerTick, Dispatcher);
            OriginalItems = new ObservableCollection<double>();
            RemainingItems = new ObservableCollection<double>();            
            Random = new Random();

            // Initialise the number of items
            int    numItems = (int)((Random.NextDouble() + Random.NextDouble()) * 50.0);

            // Build up the items
            for (int i = 0; i < numItems; i++)
            {
                double size = Random.NextDouble() * 100.0 * 1024.0;

                OriginalItems.Add(size);
                RemainingItems.Add(size);
            }

            // Determine the instruction text
            if (numItems > 1)
            {
                InstructionText = string.Format("Copying {0} Items ({1})", numItems, FormatSize(OriginalItems.Sum()));
            }
            else
            {
                InstructionText = string.Format("Copying 1 Item ({0})", FormatSize(OriginalItems.Sum()));
            }

            // Update the text properties
            UpdateTextProperties();

            // Set up the context for the view
            DataContext = this;

            // Initialise things
            InitializeComponent();

       }

        /// <summary>
        /// The timer has ticked - we update our amount copied.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick( object sender, EventArgs e )
        {
            double amount = (Random.NextDouble() + Random.NextDouble()) * 40.0 * 1024.0;

            // Process the amount
            while ((amount > 0.0) && (RemainingItems.Count > 0))
            {
                if (RemainingItems[0] >= amount)
                {
                    RemainingItems[0] -= amount;
                    amount = 0.0;
                }
                else
                {
                    amount -= RemainingItems[0];
                    RemainingItems.RemoveAt(0);
                }
            }

            // Update the text
            UpdateTextProperties();

            // Check the exit condition
            if (RemainingItems.Count == 0)
            {
                DialogResult = true;
                Close();
            }
        }

        /// <summary>
        /// The dialog is closing - stop the timer.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            Timer.Stop();
            base.OnClosed(e);
        }

        /// <summary>
        /// Update thetext properties based on the current state
        /// </summary>
        private void UpdateTextProperties()
        {
            // Update all the details
            double   remain         = RemainingItems.Sum();
            double   original       = OriginalItems.Sum();
            double   copied         = original - remain;
            long     ticksTaken     = DateTime.Now.Ticks - StartTime.Ticks;
            TimeSpan timeTaken      = new TimeSpan(ticksTaken);
            double   speed          = (timeTaken.TotalSeconds > 0)? copied / timeTaken.TotalSeconds : 0.0;

            // Determine the speed text
            SpeedText          = FormatSize(speed) + "/sec";
            ItemsRemainingText = string.Format("{0} ({1})", RemainingItems.Count, FormatSize(remain));
            PercentageDone     = (original > 0.0)? (copied / original) * 100.0 : 0.0;
            
            // Home much habve we copied.
            if (copied > 0)
            {
                TimeSpan timeRemaining = new TimeSpan((long)((ticksTaken / copied) * original) - ticksTaken);
                List<string> timeItems = new List<string>();

                if (timeRemaining.Hours > 0)
                {
                    timeItems.Add(string.Format("{0:F0} Hours", timeRemaining.Hours));
                }
                if (timeRemaining.Minutes > 0)
                {
                    timeItems.Add(string.Format("{0:F0} Minutes", timeRemaining.Minutes));
                }
                if (timeRemaining.Seconds > 0)
                {
                    timeItems.Add(string.Format("{0:F0} Seconds", timeRemaining.Seconds));
                }

                if (timeItems.Count > 0)
                {
                    Func<int, string> getSeparator = delegate(int i)
                    {
                        return (i == 0)? string.Empty : ((i == 1) ? " and " : ", ");
                    };

                    TimeRemainingText = timeItems.Reverse<string>().Select((s, i) => new { Separator = getSeparator(i), Value = s })
                                                 .Aggregate("", (s, a) => a.Value + a.Separator + s);
                }
                else
                {
                    TimeRemainingText = "Less than 1 second";
                }
            }
            else
            {
                TimeRemainingText = "Calculating...";
            }

            OnPropertyChanged("TimeRemainingText");
            OnPropertyChanged("ItemsRemainingText");
            OnPropertyChanged("PercentageDone");
            OnPropertyChanged("SpeedText");
        }

        /// <summary>
        /// Simple function to format the size of a double based on bytes/kilo/mega etc.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private string FormatSize( double size )
        {
            double       comparison = 100.0;
            double       factor     = 1.0;
            List<string> prefixes   = new List<string> { "B", "KB", "MB", "GB", "TB" };
            string       text       = "";
            
            for (int i = 0; i < prefixes.Count; i++)
            {
                if (size < (comparison * factor))
                {
                    text = string.Format("{0:F2} {1}", size / factor, prefixes[i]);
                    break;
                }

                factor *= 1024.0;
            }

            return text;
        }

        /// <summary>
        /// The timer used to update the data
        /// </summary>
        private DispatcherTimer Timer { get; set; }

        /// <summary>
        /// The time the copy started
        /// </summary>
        private DateTime StartTime { get; set; }

        /// <summary>
        /// The randomiser used to generate the random data used to update the amount of data copied
        /// </summary>
        private Random Random { get; set; }
        
        /// <summary>
        /// The list of original items
        /// </summary>
        public ObservableCollection<double> OriginalItems { get; set; }

        /// <summary>
        /// The list of remaining items
        /// </summary>
        public ObservableCollection<double> RemainingItems{ get; set; } 

        /// <summary>
        /// The current speed
        /// </summary>
        public string SpeedText { get; private set; } 

        /// <summary>
        /// The item remaining
        /// </summary>
        public string ItemsRemainingText { get; private set; } 

        /// <summary>
        /// The instruction text
        /// </summary>
        public string InstructionText { get; private set; } 

        /// <summary>
        /// The time remaining
        /// </summary>
        public string TimeRemainingText { get; private set; } 

        /// <summary>
        /// The amount done
        /// </summary>
        public double PercentageDone { get; private set; }    

        #region --- INotifyPropertyChanged Implementation ---

        /// <summary>
        /// Event that is triggered when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Trigger the property changed event handler
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged( string propertyName )
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion        
    }
}

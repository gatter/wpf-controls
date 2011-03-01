using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using BrokenHouse.Windows.Controls;
using BrokenHouse.Windows.Parts.Task;
using BrokenHouse.Windows.Parts.Transition;
using BrokenHouse.Windows.Parts.Transition.Effects;

namespace DemoApplication.Demos
{
    /// <summary>
    /// Simple details about a transition effect
    /// </summary>
    public class TransitionEffectInfo
    {
        public TransitionEffect Effect          { get; set; }
        public string           Label           { get; set; }

        /// <summary>
        /// Quick Constructor
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <param name="label"></param>
        public TransitionEffectInfo( TransitionEffect effect, string label )
        {
            Effect = effect;
            Label = label;
        }
    }
    /// <summary>
    /// Simple details of a button set
    /// </summary>
    public class ButtonSetInfo
    {
        public TaskButtonSet    ButtonSet       { get; set; }
        public string           Label           { get; set; }

        /// <summary>
        /// Quick Constructor
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <param name="label"></param>
        public ButtonSetInfo( TaskButtonSet buttonSet, string label )
        {
            ButtonSet = buttonSet;
            Label = label;
        }
    }

    /// <summary>
    /// Helper class to hold a bitmap source and a label
    /// </summary>
    public class StockIconInfo
    {
        public BitmapSource BitmapSource { get; private set; }
        public string Label { get; set; }
        public string Type { get; set; }

        /// <summary>
        /// Quick Constructor
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <param name="label"></param>
        public StockIconInfo( BitmapSource bitmapSource, string label, string type )
        {
            BitmapSource = bitmapSource;
            Label = label;
            Type = type;
        }
    }
 
    /// <summary>
    /// Helper class to hold a button set value and a label
    /// </summary>
    public class DataProvider
    {
        /// <summary>
        /// Initialise all the static button set info
        /// </summary>
        static DataProvider()
        {
            ButtonSetInfos = new List<ButtonSetInfo> { new ButtonSetInfo(TaskButtonSet.Cancel, "Cancel"),
                                                       new ButtonSetInfo(TaskButtonSet.Close, "Close"),
                                                       new ButtonSetInfo(TaskButtonSet.Ok, "Ok"),
                                                       new ButtonSetInfo(TaskButtonSet.OkCancel, "Ok & Cancel"),
                                                       new ButtonSetInfo(TaskButtonSet.RetryCancel, "Retry & Cancel"),
                                                       new ButtonSetInfo(TaskButtonSet.Yes, "Yes"),
                                                       new ButtonSetInfo(TaskButtonSet.YesNo, "Yes & No"),
                                                       new ButtonSetInfo(TaskButtonSet.YesNoCancel, "Yes, No & Cancel")};
            StockIconInfos = new List<StockIconInfo> { new StockIconInfo(TaskIcons.AltTick, "Alternative Tick", "TaskIcons.AltTick"),
                                                       new StockIconInfo(TaskIcons.Arrow, "Arrow", "TaskIcons.Arrow"),
                                                       new StockIconInfo(TaskIcons.Error, "Error", "TaskIcons.Error"),
                                                       new StockIconInfo(TaskIcons.Information, "Information", "TaskIcons.Information"),
                                                       new StockIconInfo(TaskIcons.NoEntry, "No Entry", "TaskIcons.NoEntry"),
                                                       new StockIconInfo(TaskIcons.Question, "Question", "TaskIcons.Question"),
                                                       new StockIconInfo(TaskIcons.Tick, "Tick", "TaskIcons.Tick"),
                                                       new StockIconInfo(TaskIcons.Warning, "Warning", "TaskIcons.Warning"),
                                                       new StockIconInfo(ShieldIcons.Error, "Error Shield", "ShieldIcons.Error"),
                                                       new StockIconInfo(ShieldIcons.Question, "Question Shield", "ShieldIcons.Question"),
                                                       new StockIconInfo(ShieldIcons.Tick, "Tick Shield", "ShieldIcons.Tick"),
                                                       new StockIconInfo(ShieldIcons.Warning, "Warning Shield", "ShieldIcons.Warning"),
                                                       new StockIconInfo(ShieldIcons.Windows, "Windows Shield", "ShieldIcons.Windows")};
            TransitionEffectInfos = new List<TransitionEffectInfo> { new TransitionEffectInfo(new CheckerBoardTransitionEffect(), "Checkerboard"),
                                                                     new TransitionEffectInfo(new SimpleFadeTransitionEffect(), "Simple Fade"),
                                                                     new TransitionEffectInfo(new DiamondTransitionEffect(), "Diamond"),
                                                                     new TransitionEffectInfo(new GrowAndFadeTransitionEffect(), "Grow and Fade"),
                                                                     new TransitionEffectInfo(new BlurAndFadeTransitionEffect(), "Blur and Fade"),
                                                                     new TransitionEffectInfo(new WipeTransitionEffect{Angle = 0.0}, "Overlapped Basic Wipe"),
                                                                     new TransitionEffectInfo(new WipeTransitionEffect{Angle = 45.0}, "Overlapped Angled Wipe"),
                                                                     new TransitionEffectInfo(new WipeTransitionEffect{Angle = 45.0, TransitionType = TransitionType.Sequential}, "Sequential Angled Wipe"),
                                                                     new TransitionEffectInfo(new SlideTransitionEffect{Direction = TransitionMovement.LeftToRight, TransitionType = TransitionType.Overlapped}, "Overlapped Left To Right Swipe"),
                                                                     new TransitionEffectInfo(new SlideTransitionEffect{Direction = TransitionMovement.LeftToRight, TransitionType = TransitionType.Sequential}, "Sequential Left To Right Swipe"),
                                                                     new TransitionEffectInfo(new SlideTransitionEffect{Direction = TransitionMovement.TopToBottom, TransitionType = TransitionType.Overlapped}, "Overlapped Top To Bottom Swipe"),
                                                                     new TransitionEffectInfo(new SlideTransitionEffect{Direction = TransitionMovement.TopToBottom, TransitionType = TransitionType.Sequential}, "Sequential Top To Bottom Swipe")};

        }

        /// <summary>
        /// A list of transition effects
        /// </summary>
        static public List<TransitionEffectInfo> TransitionEffectInfos { get; private set; }

        /// <summary>
        /// A list of stock icon infos that describe all the stock icons
        /// </summary>
        static public List<StockIconInfo> StockIconInfos { get; private set; }

        /// <summary>
        /// A list of button set infos that describe the buttons sets that can be used in
        /// task dialogs
        /// </summary>
        static public List<ButtonSetInfo> ButtonSetInfos { get; private set; }

    }
}
   
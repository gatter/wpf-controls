using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;

namespace BrokenHouse.Windows.Extensions
{
    /// <summary>
    /// Provides a set of static methods that extend the <see cref="System.Windows.Threading.DispatcherObject"/> class.
    /// </summary>
    /// <remarks>
    /// The main role of these extensions is to provide mechanisms to invoke actions
    /// via the DispatcherObject's defined dispatcher. These are very important when
    /// working in a multithreaded environment.
    /// </remarks>
    public static class DispatcherObjectExtensions
    {
        /// <summary>
        /// Safely invokes an action even if the calling thread is not associated with the 
        /// object's dispatcher.
        /// </summary>
        /// <remarks>
        /// If the calling thread is associated with the object's dispatcher then the action is invoked
        /// directly because it is safe to do so. If the dispatcher is associated with a different thread then 
        /// the action is invoked asyncronously with a <see cref="System.Windows.Threading.DispatcherPriority.Background"/> priority in the
        /// dispathers thread.
        /// </remarks>
        /// <param name="target">The target that is used to identify the dispatcher</param>
        /// <param name="action">The action that will be invoked</param>
        public static void SafeInvoke( this DispatcherObject target, Action action )
        {
            SafeInvoke(target, DispatcherPriority.Background, action);
        }

        /// <summary>
        /// Safely invokes an action even if the calling thread is not associated with the 
        /// object's dispatcher.S
        /// </summary>
        /// <remarks>
        /// If the calling thread is associated with the object's dispatcher then the action is invoked
        /// directly because it is safe to do so. If the dispatcher is associated with a different thread then 
        /// the action is invoked asyncronously with a requested priority in the dispatchers thread.
        /// </remarks>
        /// <param name="target">The target that is used to identify the dispatcher</param>
        /// <param name="priority">The priority with which the action is invoked when the calling thread cannot directly access the dispatcher.</param>
        /// <param name="action">The action that will be invoked</param>
        public static void SafeInvoke( this DispatcherObject target, DispatcherPriority priority, Action action )
        {
            if (target.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                target.Dispatcher.BeginInvoke(priority, action);
            }
        }

        /// <summary>
        /// Executes a specific action at a later point in time.
        /// </summary>
        /// <param name="target">The target that is used to identify the dispatcher</param>
        /// <param name="seconds">The minimum time in seconds before the action should be invoked </param>
        /// <param name="action">The action to be performed</param>
        public static void InvokeLater( this DispatcherObject target, double seconds, Action action  )
        {
            // Create the timer
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan((long)(seconds * 10000.0)), DispatcherPriority.Background, 
                                                        OnInvokeLaterTick, target.Dispatcher) { Tag = action };

            // Start the timer
            timer.Start();
        }

        /// <summary>
        /// The handler for the invoke later method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnInvokeLaterTick( object sender, EventArgs e )
        {
            DispatcherTimer timer  = sender as DispatcherTimer;
            Action          action = timer.Tag as Action;

            // Stop the timer and remove the event
            timer.Stop();
            timer.Tick -= OnInvokeLaterTick;

            // Invoke the action
            action();
        }
    }
}

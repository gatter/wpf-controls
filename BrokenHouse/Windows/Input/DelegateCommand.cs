using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BrokenHouse.Windows.Input
{
    public class DelegateCommand : ICommand
    {
        private Action         m_ExecuteDelegate;
        private Func<bool>     m_CanExecuteDelegate;
        private EventHandler   m_RequeryHandler;

        public DelegateCommand( Action executeDelegate, Func<bool> canExecuteDelegate = null )
        {
            m_CanExecuteDelegate = canExecuteDelegate;
            m_ExecuteDelegate = executeDelegate;
            m_RequeryHandler = (o, e) => TriggerCanExecuteChanged();

            CommandManager.RequerySuggested += m_RequeryHandler;
        }

        public void TriggerCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return (m_CanExecuteDelegate == null)? true : m_CanExecuteDelegate();
        }

        public void Execute(object parameter)
        {
            m_ExecuteDelegate();
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private Action<T>      m_ExecuteDelegate;
        private Func<T, bool>  m_CanExecuteDelegate;
        private EventHandler   m_RequeryHandler;

        public DelegateCommand( Action<T> executeDelegate, Func<T, bool> canExecuteDelegate = null )
        {
            m_CanExecuteDelegate = canExecuteDelegate;
            m_ExecuteDelegate = executeDelegate;
            m_RequeryHandler = (o, e) => TriggerCanExecuteChanged();

            CommandManager.RequerySuggested += m_RequeryHandler;
        }

        public void TriggerCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return (m_CanExecuteDelegate == null)? true : m_CanExecuteDelegate((T)parameter);
        }

        public void Execute(object parameter)
        {
            m_ExecuteDelegate((T)parameter);
        }

    }
}

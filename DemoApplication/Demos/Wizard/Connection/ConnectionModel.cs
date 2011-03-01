using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using BrokenHouse.Utils;

namespace DemoApplication.Demos.Wizard.Connection
{
    /// <summary>
    /// This is the model associated will all pages of the connection wizard.
    /// </summary>
    public class ConnectionModel : INotifyPropertyChanged
    {
        private PropertyChangedNotifier<ConnectionModel> m_Notifier;
        private string                                   m_Hostname;
        private NetworkCredential                        m_Credential = new NetworkCredential();

        #region --- Constructors ---

        /// <summary>
        /// Register the property dependencies
        /// </summary>
        static ConnectionModel()
        {
            PropertyChangedNotifier<ConnectionModel>.RegisterDependency("Hostname", "IsHostNameValid");
            PropertyChangedNotifier<ConnectionModel>.RegisterDependency("UserName", "IsCredentialValid", "Credential");
            PropertyChangedNotifier<ConnectionModel>.RegisterDependency("Password", "IsCredentialValid", "Credential");
        } 

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConnectionModel()
        {
            m_Notifier = new PropertyChangedNotifier<ConnectionModel>(OnPropertyChanged);
        }
        
        #endregion
        
        #region --- Properties ---
  
        /// <summary>
        /// The hostname of the server we will connect to
        /// </summary>
        public string Hostname
        {
            get { return m_Hostname; }
            set { m_Hostname = value; m_Notifier.Invoke("Hostname"); }
        }

        /// <summary>
        /// Provide access to the combined credential
        /// </summary>
        public NetworkCredential Credential
        {
            get { return m_Credential; }
        }

        /// <summary>
        /// The username used to connect to the server
        /// </summary>
        public string UserName
        {
            get { return m_Credential.UserName; }
            set { m_Credential.UserName = value; m_Notifier.Invoke("UserName"); }
        }

        /// <summary>
        /// The password used to connect to the server
        /// </summary>
        public string Password
        {
            get { return m_Credential.Password; }
            set { m_Credential.Password = value; m_Notifier.Invoke("Password"); }
        }

        /// <summary>
        /// Return true if the hostname is valid
        /// </summary>
        public bool IsHostnameValid
        {
            get { return !string.IsNullOrEmpty(Hostname); }
        }

        /// <summary>
        /// Return true if the credential details are valid
        /// </summary>
        public bool IsCredentialValid
        {
            get { return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password); }
        }

        #endregion
        
        #region --- INotifyPropertyChanged Implementation ---

        /// <summary>
        /// Event that is triggered when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Trigger the property changed event handler
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged( PropertyChangedEventArgs args )
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, args);
            }
        }

        #endregion
    }
}

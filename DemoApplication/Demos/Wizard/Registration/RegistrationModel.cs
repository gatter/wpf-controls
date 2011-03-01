using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrokenHouse.Utils;

namespace DemoApplication.Demos.Wizard.Registration
{
    public class RegistrationModel : INotifyPropertyChanged
    {
        private PropertyChangedNotifier<RegistrationModel> m_Notifier;

        private string    m_Address1;
        private string    m_Address2;
        private string    m_FirstName;
        private string    m_LastName;
        private string    m_City;
        private string    m_State;
        private string    m_PostalCode;
        private string    m_Country;
        private string[]  m_Countries = new string[] { "United Kingdom", "United States", "Australia",
                                                       "New Zealand", "Belgium", "Canada",
                                                       "Denmark", "Germany", "Ireland",
                                                       "Italy", "Netherlands", "Switzerland",
                                                       "Austria", "Spain", "Luxemberg",
                                                       "Mexico", "Norway", "Sweeden",
                                                       "Portugal", "South Africa", "India", "France"};
                
        #region --- Constructors ---

        /// <summary>
        /// Register the property dependencies
        /// </summary>
        static RegistrationModel()
        {
            PropertyChangedNotifier<RegistrationModel>.RegisterDependency("Address1",   "IsValid");
            PropertyChangedNotifier<RegistrationModel>.RegisterDependency("Address2",   "IsValid");
            PropertyChangedNotifier<RegistrationModel>.RegisterDependency("FirstName",  "IsValid");
            PropertyChangedNotifier<RegistrationModel>.RegisterDependency("LastName",   "IsValid");
            PropertyChangedNotifier<RegistrationModel>.RegisterDependency("City",       "IsValid");
            PropertyChangedNotifier<RegistrationModel>.RegisterDependency("State",      "IsValid");
            PropertyChangedNotifier<RegistrationModel>.RegisterDependency("PostalCode", "IsValid");
            PropertyChangedNotifier<RegistrationModel>.RegisterDependency("Country",    "IsValid");
        } 

        /// <summary>
        /// Default constructor
        /// </summary>
        public RegistrationModel()
        {
            m_Notifier = new PropertyChangedNotifier<RegistrationModel>(OnPropertyChanged);
        }
        
        #endregion
        
        #region --- Properties ---
        
        /// <summary>
        /// A list of countries sorted by name
        /// </summary>
        public string[] AvailableCountries
        {
            get { return m_Countries.OrderBy(i => i).ToArray(); }
        }

        /// <summary>
        /// The country
        /// </summary>
        public string Country
        {
            get { return m_Country; }
            set { m_Country = value; m_Notifier.Invoke("Country"); }
        }

        /// <summary>
        /// The first line of the address
        /// </summary>
        public string Address1
        {
            get { return m_Address1; }
            set { m_Address1 = value; m_Notifier.Invoke("Address1"); }
        }

        /// <summary>
        /// The second line of the address
        /// </summary>
        public string Address2
        {
            get { return m_Address2; }
            set { m_Address2 = value; m_Notifier.Invoke("Address2"); }
        }

        /// <summary>
        /// The first name
        /// </summary>
        public string FirstName
        {
            get { return m_FirstName; }
            set { m_FirstName = value; m_Notifier.Invoke("FirstName"); }
        }

        /// <summary>
        /// The last name
        /// </summary>
        public string LastName
        {
            get { return m_LastName; }
            set { m_LastName = value; m_Notifier.Invoke("LastName"); }
        }

        /// <summary>
        /// The city
        /// </summary>
        public string City
        {
            get { return m_City; }
            set { m_City = value; m_Notifier.Invoke("City"); }
        }

        /// <summary>
        /// The state
        /// </summary>
        public string State
        {
            get { return m_State; }
            set { m_State = value; m_Notifier.Invoke("State"); }
        }

        /// <summary>
        /// The postal code
        /// </summary>
        public string PostalCode
        {
            get { return m_PostalCode; }
            set { m_PostalCode = value; m_Notifier.Invoke("PostalCode"); }
        }

        /// <summary>
        /// Return true if all the required properties are valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                string[] requiredProperties = new string[] { m_FirstName, m_LastName, m_Address1, m_City, m_State, m_PostalCode, m_Country };

                return requiredProperties.All(i => !string.IsNullOrEmpty(i));
            }
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Reflection;

namespace BrokenHouse.Utils
{
    /// <summary>
    /// Provides a way of triggering multiple <c>PropertyChanged</c> events for dependent properties with
    /// a single call.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It’s common that the value of one property of a class is dependent on the value of another property of the same class. 
    /// The <c>PropertyChangedNotifier</c> class provides a way of declaring these dependencies so that <c>PropertyChanged</c> events
    /// can be triggered for the root property as well as its dependent properties.
    /// </para>
    /// <para>
    /// For example, consider a <c>Rectangle</c> with properties <c>Width</c>, <c>Height</c>, <c>Perimeter</c> and <c>Area</c>.
    /// Whenever <c>Width</c> or <c>Height</c> change you would want any clients to be notified that <c>Perimeter</c> and <c>Area</c> 
    /// have also changed. To normally achieve this, the <c>Width</c> property would have to trigger change events for not only the <c>Width</c> 
    /// property but also <c>Perimeter</c> and <c>Area</c> properties. With more complex classes there can be a significant amount of 
    /// property change events that need
    /// to be triggered for each property, this can become very cumbersome and hard to maintain.
    /// </para>
    /// <para>
    /// It would be easy to provide a base class that allowed dependent properties to be registered and a method that would trigger
    /// all of the required <c>PropertyChanged</c> events. However, as C# only allows single inheritance this base class could only be used 
    /// in a small sub small subset of cases.
    /// </para>
    /// <para>
    /// Another solution, and the one implemented here, is to provide the same functionality in a helper class.
    /// Unfortunately, C# has a restriction in that event delegates can only be invoked 
    /// from within the class or struct where they are declared (the publisher class). This means that the
    /// <c>PropertyChangedNotifier</c> cannot trigger the <c>PropertyChanged</c> event directly. To workaround
    /// this each instance of the <c>PropertyChangedNotifier</c> must be provided with an action
    /// that will actually trigger the event. It is left to the <c>PropertyChangedNotifier</c> to manage the
    /// the dependencies.
    /// </para>
    /// <para>
    /// To allow the property depndencies to be registered statically we have designed the <c>PropertyChangedNotifier</c>
    /// to take a type parameter. This type parameter must be the class that is triggereing the properties, by doing
    /// this we can define the dependencies in a very simple without the need to pass in the type for which we are 
    /// registering the dependencies.
    /// </para>
    /// </remarks>
    /// <example>
    /// This example shows how to use the <c>PropertyChangedNotifier</c>
    /// <code>
    /// public class Rect : A, INotifyPropertyChanged
    /// {
    ///     private PropertyChangedNotifier&lt;Rect&gt;   m_Notifier;
    ///     private double                          m_Width;
    ///     private double                          m_Height;
    ///      
    ///     // Register the property dependencies
    ///     static Rect()
    ///     {
    ///         PropertyChangedNotifier&lt;Rect&gt;.RegisterDependency("Width", "Perimeter", "Area");
    ///         PropertyChangedNotifier&lt;Rect&gt;.RegisterDependency("Height", "Perimeter", "Area");
    ///     }
    /// 
    ///     // Create the notifier for this instance
    ///     public Rect()
    ///     {
    ///        m_Notifier = new PropertyChangedNotifier&lt;Rect&gt;(OnPropertyChanged);
    ///     }
    ///      
    ///     public double Area
    ///     {
    ///         get { return (m_Width * m_Height); }
    ///     }
    ///
    ///     public double Perimeter
    ///     {
    ///         get { return 2.0 * (m_Width + m_Height); }
    ///     }
    ///
    ///     public double Width
    ///     {
    ///         get { return m_Width; }
    ///         set { m_Width = value; s_Notifier.Invoke("Width"); }
    ///     }
    ///    
    ///     public double Height
    ///     {
    ///         get { return m_Height; }
    ///         set { m_Height = value; s_Notifier.Invoke("Height"); }
    ///     }
    ///     
    ///     // The method that will trigger the property changed event
    ///     protected void OnPropertyChanged( PropertyChagnedEventArgs args )
    ///     {
    ///         if (PropertyChanged != null)
    ///         {
    ///             PropertyChanged(this, args);
    ///         }
    ///     }
    ///     
    ///     // The property changed event
    ///     public event PropertyChangedEventHandler    PropertyChanged;
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="T">The type of the object that this class will manage the events for.</typeparam>
    public class PropertyChangedNotifier<T>
    {
        private Action<PropertyChangedEventArgs>         m_Raiser;
        private static Dictionary<string, List<string>>  s_Dependencies = new Dictionary<string, List<string>>();

        /// <summary>
        /// Register a list of dependent property names associated with a root property name.
        /// </summary>
        /// <param name="rootProperty">The root property name.</param> 
        /// <param name="dependentProperties">A list of dependent property names.</param>
        public static void RegisterDependency( string rootProperty, params string[] dependentProperties )
        {
            RegisterDependency(rootProperty, dependentProperties as IEnumerable<string>);
        }

        /// <summary>
        /// Register a list of dependent properties associated with a root property name.
        /// </summary>
        /// <param name="rootProperty">The root property name</param>
        /// <param name="dependentProperties">A list of dependent property names.</param>
        public static void RegisterDependency( string rootProperty, IEnumerable<string> dependentProperties )
        {
            List<string>    dependencies = null;

            if (s_Dependencies.TryGetValue(rootProperty, out dependencies))
            {
                s_Dependencies[rootProperty] = dependencies.Union(dependentProperties).ToList();
            }
            else
            {
                s_Dependencies[rootProperty] = dependentProperties.ToList();
            }
        }
        
        /// <summary>
        /// Initialises a new instance of the <c>PropertyChangedNotifier</c>.
        /// </summary>
        /// <remarks>
        /// The action supplied to this constructor will be called to trigger the actual <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
        /// event.
        /// </remarks>
        /// <param name="raiser">The action to perform the raising of the 
        /// <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> event.</param>
        public PropertyChangedNotifier( Action<PropertyChangedEventArgs> raiser )
        {
            m_Raiser = raiser;
        }

        /// <summary>
        /// Raises the property changed event for the supplied property name as well as all its registered dependent
        /// property names.
        /// </summary>
        /// <remarks>
        /// If due to a complex property hierarchy a dependent property is listed more than once it will only be triggered
        /// once per call to <see cref="Invoke(string)"/>.
        /// </remarks>
        /// <param name="propertyName">The root property name</param>
        public void Invoke( string propertyName ) 
        {
            Invoke(propertyName, new List<string>());
        }

        /// <summary>
        /// Trigger a proeprty name change - but keep a list of the property names we have already triggered
        /// </summary>
        /// <param name="propertyName">The property name to trigger.</param>
        /// <param name="triggeredNames">The property names that have already been triggered.</param>
        private void Invoke( string propertyName, List<string> triggeredNames )
        {
            List<string>    dependencies = null;

            // Fire the key property
            m_Raiser(new PropertyChangedEventArgs(propertyName));

            // Are the any dependencies for this property
            if (s_Dependencies.TryGetValue(propertyName, out dependencies))
            {
                foreach (var dependentName in dependencies)
                {
                    if (!triggeredNames.Contains(dependentName))
                    {
                        // Add the triggered name first
                        triggeredNames.Add(dependentName);

                        // Invoke the method
                        Invoke(dependentName, triggeredNames);
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DemoApplication.Demos.Wizard.Connection
{
    /// <summary>
    /// This is a mock implementation of the IHandshakeService.
    /// </summary>
    /// <remarks>
    /// Because in the context of this demo application we cannot connect
    /// to a server we mock one up. This class will do an dns lookup
    /// to check the server is valid. Only a username and password of 
    /// guest will authenticate successfully.
    /// </remarks>
    public class MockAuthenticationService 
    {
        /// <summary>
        /// The hostname we are connection to
        /// </summary>
        public string ServerHostname { get; private set; }

        /// <summary>
        /// Constructor for the authentication service
        /// </summary>
        /// <param name="serverHostname"></param>
        public MockAuthenticationService( string serverHostname )
        {
            ServerHostname = serverHostname;
        }

        /// <summary>
        /// Simple mock authentication - return true if the username and password are equal to guest.
        /// </summary>
        /// <param name="serverHostname"></param>
        /// <param name="credential"></param>
        /// <returns></returns>
        public bool CheckAuthentication( NetworkCredential credential )
        {
            return (credential.UserName == credential.Password) && (credential.Password == "guest");
        }

    }
}

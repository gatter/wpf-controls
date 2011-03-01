using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DemoApplication.Demos.Wizard.Connection
{
    /// <summary>
    /// This is a mock implementation of a HandshakeService.
    /// </summary>
    /// <remarks>
    /// Because in the context of this demo application we cannot connect
    /// to a server we mock one up. This class will do an dns lookup
    /// to check the server is valid. Only a username and password of 
    /// guest will authenticate successfully.
    /// </remarks>
    public class MockHandshakeService
    {
        /// <summary>
        /// The hostname we are connection to
        /// </summary>
        public string ServerHostname { get; private set; }

        /// <summary>
        /// Constructor for the handshake service
        /// </summary>
        /// <param name="serverHostname"></param>
        public MockHandshakeService( string serverHostname )
        {
            ServerHostname = serverHostname;
        }

        /// <summary>
        /// Simple mock check server. Check the server has an ip address and if so return a version of 1.0
        /// </summary>
        /// <param name="serverHostname"></param>
        /// <returns></returns>
        public Version GetVersion()
        {
            if (string.IsNullOrEmpty(ServerHostname))
            {
                throw new InvalidOperationException("Hostname cannot be empty");
            }
            
            // Try to lookup the host
            IPHostEntry hostInfo = Dns.GetHostEntry(ServerHostname);
            Version     version  = null;

            if (hostInfo.AddressList.Length > 0)
            {
                version = new Version(1, 0);
            }
            else
            {
                version = new Version(0, 0);
            }

            return version;
        }

    }
}

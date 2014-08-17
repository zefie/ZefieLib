﻿using System.Net;
using System.Net.NetworkInformation;

namespace Zefie
{
    public class Networking
    {
        /// <summary>
        /// Checks if the specified port is in use
        /// </summary>
        /// <param name="port"></param>
        /// <param name="address">IPAdddress, defaults to localhost</param>
        /// <returns>True if the port is avaible, false if it is in use</returns>
        public static bool isPortAvailable(int port, IPAddress address = null)
        {
            // 127.0.0.1
            if (address == null)
                address = new IPAddress(16777343);

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Address == address && tcpi.LocalEndPoint.Port == port)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
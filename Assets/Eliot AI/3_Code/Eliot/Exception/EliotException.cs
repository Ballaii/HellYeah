using System;
using UnityEngine;

namespace Eliot
{
    /// <summary>
    /// Base class for all the exceptions recognised internally in Eliot.
    /// </summary>
    public class EliotException : Exception
    {
        /// <summary>
        /// Log the message.
        /// </summary>
        /// <param name="message"></param>
        public EliotException(string message) : base(message)
        {
            DebugErrorBlue(message);
        }

        /// <summary>
        /// Display the message in blue color.
        /// </summary>
        /// <param name="message"></param>
        protected static void DebugErrorBlue(string message)
        {
            Debug.LogErrorFormat("<color=blue>" + message + "</color>");
        }
    }
}


///-----------------------------------------------------------------
///   File:         Logger.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 16:46:57
///-----------------------------------------------------------------

namespace Nanomite.Core.Server.Helper
{
    using System;

    /// <summary>
    /// Defines the <see cref="Logger" />
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Initializes the loggingservice
        /// </summary>
        /// <param name="cloudId">The cloudId<see cref="string"/></param>
        public static void Initialize(string cloudId)
        {
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="level">The level.</param>
        public static void Log(string sender, string msg, NLog.LogLevel level)
        {
            if (level == NLog.LogLevel.Trace)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(DateTime.Now.ToString() + ": " + msg);
                Console.ResetColor();
            }
            else if (level == NLog.LogLevel.Debug)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(DateTime.Now.ToString() + ": " + msg);
                Console.ResetColor();
            }
            else if (level == NLog.LogLevel.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(DateTime.Now.ToString() + ": " + msg);
                Console.ResetColor();
            }
            else if (level == NLog.LogLevel.Info)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": " + msg);
            }
            else if (level == NLog.LogLevel.Warn)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(DateTime.Now.ToString() + ": " + msg);
                Console.ResetColor();
            }
        }
    }
}

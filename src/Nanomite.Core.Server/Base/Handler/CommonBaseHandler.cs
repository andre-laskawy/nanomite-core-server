///-----------------------------------------------------------------
///   File:         CommonBaseHandler.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 17:08:24
///-----------------------------------------------------------------

namespace Nanomite.Core.Server.Base.Handler
{
    using NLog;
    using System;

    /// <summary>
    /// Defines the <see cref="CommonBaseHandler"/>
    /// </summary>
    public abstract class CommonBaseHandler
    {
        /// <summary>
        /// Gets or sets the logging method
        /// </summary>
        public static Action<string, string, LogLevel> OnLog { get; set; }

        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="srcComponent">The source component.</param>
        /// <param name="code">The code.</param>
        /// <param name="ex">The ex.</param>
        public static void Log(string srcComponent, string code, Exception ex)
        {
            Log(srcComponent, ex.ToText(code), LogLevel.Error);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="srcComponent">The source component.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="level">The level.</param>
        public static void Log(string srcComponent, string msg, LogLevel level)
        {
            OnLog?.Invoke(srcComponent, msg, level);
        }

        /// <summary>
        /// Logs the specified source identifier.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="ex">The ex.</param>
        protected virtual void Log(string code, Exception ex)
        {
            Log(ex.ToText(code), LogLevel.Error);
        }

        /// <summary>
        /// Logs the specified sender.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="level">The level.</param>
        protected virtual void Log(string msg, LogLevel level)
        {
            OnLog?.Invoke(this.GetType().ToString(), msg, level);
        }
    }
}

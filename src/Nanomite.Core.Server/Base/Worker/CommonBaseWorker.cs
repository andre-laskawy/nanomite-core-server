///-----------------------------------------------------------------
///   File:         CommonBaseWorker.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 18:21:12
///-----------------------------------------------------------------

namespace Nanomite.Core.Server.Base.Worker
{
    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;
    using Nanomite.Core.Server.Base.Handler;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nanomite.Core.Network.Common;

    /// <summary>
    /// Defines the <see cref="CommonBaseWorker"/>
    /// </summary>
    public abstract class CommonBaseWorker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonBaseWorker"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        public CommonBaseWorker()
        { }

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="ex">The ex.</param>
        protected virtual void Log(string code, Exception ex)
        {
            CommonBaseHandler.Log(this.GetType().ToString(), ex.ToText(code), LogLevel.Error);
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="level">The level.</param>
        protected virtual void Log(string msg, LogLevel level)
        {
            CommonBaseHandler.Log(this.GetType().ToString(), msg, level);
        }

        /// <summary>
        /// Creates a grpc repsonse as an unauthorization result.
        /// </summary>
        /// <returns>a grpc response</returns>
        protected GrpcResponse Unauthorized()
        {
            GrpcResponse result = new GrpcResponse();
            result.Result = ResultCode.Error;
            result.Message = "Access denied";
            return result;
        }

        /// <summary>
        /// Creates a grpc repsonse as a bad request result.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        /// <returns>a grpc response</returns>
        protected GrpcResponse BadRequest(Exception ex, string message = null)
        {
            GrpcResponse result = new GrpcResponse();
            result.Result = ResultCode.Error;
            if (!string.IsNullOrEmpty(message))
            {
                result.Message = message;
            }
            else
            {
                result.Message = ex.Message;
            }
            result.Data.Add(Any.Pack(ex.ToProtoException()));
            return result;
        }

        /// <summary>
        /// Creates a grpc repsonse as a bad request result.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>a grpc response</returns>
        protected GrpcResponse BadRequest(string message)
        {
            GrpcResponse result = new GrpcResponse();
            result.Result = ResultCode.Error;
            result.Message = message;
            return result;
        }

        /// <summary>
        /// Creates a grpc repsonse as an ok result.
        /// </summary>
        /// <param name="proto">The proto.</param>
        /// <param name="message">The message.</param>
        /// <returns>a grpc response</returns>
        protected GrpcResponse Ok(IMessage proto, string message = null)
        {
            GrpcResponse result = new GrpcResponse();
            result.Result = ResultCode.Ok;
            if (!string.IsNullOrEmpty(message))
            {
                result.Message = message;
            }
            if (proto != null)
            {
                result.Data.Add(Any.Pack(proto));
            }
            return result;
        }

        /// <summary>
        /// Creates a grpc repsonse as an ok result.
        /// </summary>
        /// <param name="proto">The proto.</param>
        /// <param name="message">The message.</param>
        /// <returns>a grpc response</returns>
        protected GrpcResponse Ok(List<IMessage> proto, string message = null)
        {
            GrpcResponse result = new GrpcResponse();
            result.Result = ResultCode.Ok;
            if (!string.IsNullOrEmpty(message))
            {
                result.Message = message;
            }
            if (proto != null)
            {
                result.Data.AddRange(proto.Select(p => Any.Pack(p)));
            }
            return result;
        }

        /// <summary>
        /// Creates a grpc repsonse as an ok result.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>a grpc response</returns>
        protected GrpcResponse Ok(string message = null)
        {
            GrpcResponse result = new GrpcResponse();
            result.Result = ResultCode.Ok;
            if (!string.IsNullOrEmpty(message))
            {
                result.Message = message;
            }
            return result;
        }
    }
}

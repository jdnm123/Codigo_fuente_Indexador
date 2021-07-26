using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Indexai.Services
{
    /// <summary>
    /// Telemetría para eventos y errores.
    /// </summary>
    public static class Telemetry
    {
        private static TelemetryClient _client;

        /// <summary>
        /// Inicia los clientes de telemetría.
        /// </summary>
        public static void Initialize()
        {
            try
            {
                var config = new TelemetryConfiguration
                {
                    InstrumentationKey = "04c1bd07-c09e-4833-9872-b83030a3f067"
                };
                //config.TelemetryChannel = new Microsoft.ApplicationInsights.Channel.InMemoryChannel(); // Default channel
                config.TelemetryChannel.DeveloperMode = Debugger.IsAttached;
#if DEBUG
                config.TelemetryChannel.DeveloperMode = true;
#endif
                _client = new TelemetryClient(config);
                _client.Context.Component.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
                _client.Context.Session.Id = Guid.NewGuid().ToString();
                _client.Context.User.Id = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();
                _client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
            }
            catch (Exception)
            {
                _client = null;
            }
        }

        /// <summary>
        /// Agrega un error a la telematría.
        /// </summary>
        /// <param name="exception">Exception</param>
        internal static void TrackException(Exception exception)
        {
            if (_client != null)
            {
                _client.TrackException(exception);
            }
            else
            {
                throw exception;
            }
        }

        /// <summary>
        /// Libera los recursos de los clientes de la telemetría.
        /// </summary>
        internal static void Flush()
        {
            if (_client != null)
            {
                _client.Flush();
            }
        }

        /// <summary>
        /// Indica el ususario actual para registro de eventos.
        /// </summary>
        /// <param name="user">Usuario actual.</param>
        internal static void SetUser(string user)
        {
            if (_client != null)
            {
                _client.Context.User.AuthenticatedUserId = user;
            }
        }

        /// <summary>
        /// Agrega un evento a la telemetría.
        /// </summary>
        /// <param name="eventName">Nombre del evento.</param>
        internal static void TrackEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentException("message", nameof(eventName));
            }

            if (_client != null)
            {
                _client.TrackEvent(eventName);
            }
        }
    }
}
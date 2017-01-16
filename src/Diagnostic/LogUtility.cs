// ----------------------------------------------------------------------------
// <copyright file="LogUtility.cs" company="ABC Software Ltd">
//    Copyright © 2015 ABC Software Ltd. All rights reserved.
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License  as published by the Free Software Foundation, either 
//    version 3 of the License, or (at your option) any later version. 
//
//    This library is distributed in the hope that it will be useful, 
//    but WITHOUT ANY WARRANTY; without even the implied warranty of 
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with the library. If not, see http://www.gnu.org/licenses/.
// </copyright>
// ----------------------------------------------------------------------------

#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic {
#else
namespace Abc.Diagnostics {
#endif
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security;

    /// <summary>
    /// Represent tracing utility.
    /// </summary>
    public class LogUtility {
        /// <summary>
        /// Category name for logging exception.
        /// </summary>
        public const string GeneralCategory = "General";

        /// <summary>
        /// Category name for logging activities.
        /// </summary>
        public const string LogActivityCategory = "Activity";

        internal const int DefaultPriority = -1;
        internal const int DefaultEventId = 0;
        private const TraceEventType DefaultSeverity = TraceEventType.Verbose;
        private const TraceEventType DefaultExceptionSeverity = TraceEventType.Error;
        private const string EventSourceName = "Abc.Diagnostics";

        private static readonly ICollection<string> EmptyCategoriesList = new List<string>(0);

        private static object sync = new object();
        private static volatile ILogWriter writer;
        private static string logSourceName;

        private readonly string sourceName;
        private bool calledShutdown;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogUtility"/> class.
        /// </summary>
        [SecurityCritical]
        public LogUtility()
            : this(LogUtility.LogSourceName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogUtility"/> class.
        /// </summary>
        /// <param name="sourceName">Name of the source.</param>
        [SecurityCritical]
        public LogUtility(string sourceName) {
            this.sourceName = sourceName;
            this.UnsafeAddDomainEventHandlersForCleanup();
        }

        #region Properteis 
        /// <summary>
        /// Gets a value indicating whether logging enabled.
        /// </summary>
        /// <value><c>true</c> if logging enabled; otherwise, <c>false</c>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Is a class meber")]
        public bool LoggingEnabled {
            get { return LogUtility.Writer.IsLoggingEnabled; } 
        }

#if !NETSTANDARD
        /// <summary>
        /// Gets or sets the activity id.
        /// </summary>
        /// <value>
        /// The activity id.
        /// </value>
        internal static Guid ActivityId {
            [SecurityCritical]
#if NET20 || NET30 || NET35 || NET40
            [SecurityTreatAsSafe]
#else
            [SecuritySafeCritical]
#endif
            get { return Trace.CorrelationManager.ActivityId; }

            [SecurityCritical]
#if NET20 || NET30 || NET35 || NET40
            [SecurityTreatAsSafe]
#else
            [SecuritySafeCritical]
#endif
            set { Trace.CorrelationManager.ActivityId = value; }
        }
#endif

        /// <summary>
        /// Gets the instance of <see cref="ILogWriter"/> used by the facade.
        /// </summary>
        /// <remarks>
        /// The lifetime of this instance is managed by the facade.
        /// </remarks>
        internal static ILogWriter Writer {
            get {
                if (writer == null) {
                    lock (sync) {
                        if (writer == null) {
#if NETSTANDARD
                            writer = new DefaultLogWriter();
#else
                            try {
                                writer = Configuration.DiagnosticSettings.Current.CreateLogWriter();    
                            }
                            catch (System.Configuration.ConfigurationErrorsException configurationException) {
                                LogUtility.TryLogFailure(configurationException);

                                throw;
                            }
#endif
                        }
                    }
                }

                return writer;
            }
        }

        /// <summary>
        /// Gets the name of the log source.
        /// </summary>
        /// <value>
        /// The name of the log source.
        /// </value>
        internal static string LogSourceName {
            get {
                if (logSourceName == null) {
                    lock (sync) {
                        if (logSourceName == null) {
                            var logSource = LogUtility.Writer as ILogSource;
                            if (logSource != null) {
                                logSourceName = logSource.SourceName;
                            }
                            else {
#if !NETSTANDARD
                                logSourceName = System.IO.Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
#else
                                logSourceName = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationName;
#endif
                            }
                        }
                    }
                }

                return logSourceName;
            }
        }

        internal string SourceName {
            get { return this.sourceName; }
        }

#endregion

#region Write Static
        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="title">Additional description of the log entry message.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public static void Write(string message, string category, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties) {
            LogUtility.Write(message, new string[] { category }, priority, eventId, severity, title, properties, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="categories">Category names used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="title">Additional description of the log entry message.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public static void Write(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties) {
            LogUtility.Write(message, categories, priority, eventId, severity, title, properties, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="title">Additional description of the log entry message.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        public static void Write(string message, string category, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties, Guid activityId) {
            LogUtility.Write(message, new string[] { category }, priority, eventId, severity, title, properties, activityId);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="categories">Category names used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="title">Additional description of the log entry message.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        public static void Write(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties, Guid activityId) {
            LogUtility.Write(message, categories, priority, eventId, severity, title, properties, null, activityId);   
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title, dictionary of extended properties and exception.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="categories">Category names used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="title">Additional description of the log entry message.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        public static void Write(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties, Exception exception, Guid activityId) {
#if !NETSTANDARD
            using (activityId == Guid.Empty ? null : Activity.CreateActivity(activityId)) {
                LogUtility.Writer.Write(
                    message, categories, priority, eventId, severity, title, properties, exception, LogUtility.ActivityId, null);
            }
#else
            LogUtility.Writer.Write(
                    message, categories, priority, eventId, severity, title, properties, exception, Guid.Empty, null);
#endif
        }

        #endregion

        #region Write
        /// <overloads>
        /// Write a new log entry to the default category.
        /// </overloads>
        /// <summary>
        /// Write a new log entry to the default category.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// one required parameter, message.
        /// <code>Logger.Write("My message body");</code></example>
        /// <param name="message">Message body to log.</param>
        public void Write(string message) {
            this.WriteCore(message, EmptyCategoriesList, DefaultPriority, DefaultEventId, DefaultSeverity, null, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry to a specific category.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        public void Write(string message, string category) {
            this.WriteCore(message, new string[] { category }, DefaultPriority, DefaultEventId, DefaultSeverity, null, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category and priority.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        public void Write(string message, string category, int priority) {
            this.WriteCore(message, new string[] { category }, priority, DefaultEventId, DefaultSeverity, null, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority and event id.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        public void Write(string message, string category, int priority, int eventId) {
            this.WriteCore(message, new string[] { category }, priority, eventId, DefaultSeverity, null, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event id and severity.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log entry severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        public void Write(string message, string category, int priority, int eventId, TraceEventType severity) {
            this.WriteCore(message, new string[] { category }, priority, eventId, severity, null, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event id and severity.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log entry severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="activityId">The qualified name of activity.</param>
        public void Write(string message, string category, int priority, int eventId, TraceEventType severity, Guid activityId) {
            this.WriteCore(message, new string[] { category }, priority, eventId, severity, null, activityId);
        }
#endregion

#region Write with Properties
        /// <summary>
        /// Write a new log entry and a dictionary of extended properties.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public void Write(string message, IDictionary<string, object> properties) {
            this.WriteCore(message, EmptyCategoriesList, DefaultPriority, DefaultEventId, DefaultSeverity, properties, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry to a specific category with a dictionary 
        /// of extended properties.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public void Write(string message, string category, IDictionary<string, object> properties) {
            this.WriteCore(message, new string[] { category }, DefaultPriority, DefaultEventId, DefaultSeverity, properties, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry to with a specific category, priority and a dictionary 
        /// of extended properties.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public void Write(string message, string category, int priority, IDictionary<string, object> properties) {
            this.WriteCore(message, new string[] { category }, priority, DefaultEventId, DefaultSeverity, properties, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public void Write(string message, string category, int priority, int eventId, TraceEventType severity, IDictionary<string, object> properties) {
            this.WriteCore(message, new string[] { category }, priority, eventId, severity, properties, Guid.Empty);
        }
        
        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="categories">Category names used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public void Write(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, IDictionary<string, object> properties) {
            this.WriteCore(message, categories, priority, eventId, severity, properties, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="categories">Category names used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        public void Write(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, IDictionary<string, object> properties, Guid activityId) {
            this.WriteCore(message, categories, priority, eventId, severity, properties, activityId);
        }
#endregion

#region Write with Exception
        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="exception">Exception to log.</param>
        public void Write(string message, Exception exception) {
            this.WriteCore(message, EmptyCategoriesList, DefaultPriority, DefaultEventId, DefaultExceptionSeverity, null, exception, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="exception">Exception to log.</param>
        public void Write(string message, string category, Exception exception) {
            this.Write(message, category, DefaultPriority, DefaultEventId, DefaultExceptionSeverity, exception, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="exception">Exception to log.</param>
        public void Write(string message, string category, int priority, Exception exception) {
            this.Write(message, category, priority, DefaultEventId, DefaultExceptionSeverity, exception, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="exception">Exception to log.</param>
        public void Write(string message, string category, int priority, int eventId, TraceEventType severity, Exception exception) {
            this.Write(message, category, priority, eventId, severity, exception, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        public void Write(string message, string category, int priority, int eventId, TraceEventType severity, Exception exception, Guid activityId) {
            this.WriteCore(message, new string[] { category }, priority, eventId, severity, null, exception, activityId);
        }
#endregion

        /// <summary>
        /// Public for testing purposes.
        /// Reset the writer used by the <see cref="LogUtility"/> facade.
        /// </summary>
        /// <remarks>
        /// Threads that already acquired the reference to the old writer will fail when it gets disposed.
        /// </remarks>
        internal static void Reset() {
            lock (sync) {
                IDisposable oldWriter = writer as IDisposable;

                // this will be seen by threads requesting the writer (because of the double check locking pattern the query is outside the lock).
                // these threads should be stopped when trying to lock to create the writer.
                writer = null;

                // the old writer is disposed inside the lock to avoid having two instances with the same configuration.
                if (oldWriter != null) {
                    oldWriter.Dispose();
                }
            }
        }

#region Protected Methods
        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="categories">Category names used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        protected virtual void WriteCore(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, IDictionary<string, object> properties, Guid activityId) {
            this.WriteCore(message, categories, priority, eventId, severity, properties, null, activityId); 
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title, dictionary of extended properties and exception.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="message">Message body to log.</param>
        /// <param name="categories">Category names used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        protected void WriteCore(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, IDictionary<string, object> properties, Exception exception, Guid activityId) {
            LogUtility.Write(message, categories, priority, eventId, severity, this.sourceName, properties, exception, activityId);
        }
#endregion

#region Private Methods
        private static void TryLogFailure(Exception exception) {
            try {
#if !NETSTANDARD
                //// if (!EventLog.SourceExists(eventSourceName)) {
                ////     EventLog.CreateEventSource(eventSourceName, "Application");
                //// }

                EventLog.WriteEntry(EventSourceName, exception.ToString(), EventLogEntryType.Error, 0, 4);
#endif
            }
            catch (Exception ex) { 
                if (ExceptionUtility.IsFatal(ex)) {
                    throw; 
                }

                //// LogFailureToEventLog(ex);
            }
        }

#if !NETSTANDARD
        [SecurityCritical]
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
#endif
        private void UnsafeAddDomainEventHandlersForCleanup() {
            try {
                bool tracingEnabled = LogUtility.Writer.IsLoggingEnabled;
                if (tracingEnabled) {
#if !NETSTANDARD
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    currentDomain.DomainUnload += (sender, e) => {
                        this.ShutdownTracing();
                    };

                    currentDomain.ProcessExit += (sender, e) => {
                        this.ShutdownTracing();
                    };

                    currentDomain.UnhandledException += (sender, e) => {
                        Exception exception = (Exception)e.ExceptionObject;
                        this.Write(Abc.Diagnostics.SR.UnhandledException, GeneralCategory, DefaultPriority, DefaultEventId, TraceEventType.Critical, exception);
                        this.ShutdownTracing();
                    };
#elif NETSTANDARD1_5 || NETSTANDARD1_6
                    System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += context => this.ShutdownTracing();
#endif
                }
            }
            catch (Exception exception) {
                if (ExceptionUtility.IsFatal(exception)) {
                    throw;
                }

                LogUtility.TryLogFailure(exception);
            }
        }

        private void ShutdownTracing() {
            if (!this.calledShutdown) {
                try {
                    this.Write(Abc.Diagnostics.SR.TraceCodeAppDomainUnload, GeneralCategory, DefaultPriority, DefaultEventId, TraceEventType.Information);

                    this.calledShutdown = true;
                    LogUtility.Writer.Flush();
                }
                catch (Exception exception) {
                    if (ExceptionUtility.IsFatal(exception)) {
                        throw;
                    }

                    LogUtility.TryLogFailure(exception);
                }
            }
        }
#endregion
    }
}

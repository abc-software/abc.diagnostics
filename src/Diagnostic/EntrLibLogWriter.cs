// ----------------------------------------------------------------------------
// <copyright file="EntrLibLogWriter.cs" company="ABC Software Ltd">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Proxy class for Enterprise Library LogWriter class.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Entr", Justification = "Contraction.")]
    public abstract class EntrLibLogWriter : ILogWriter, IDisposable {
        private Assembly loggingAssembly;
        private object logWriter;
        private readonly bool exceptionAsDisctionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntrLibLogWriter"/> class.
        /// </summary>
        /// <param name="initializeData">The initialize data.</param>
        protected EntrLibLogWriter(string initializeData) {
            this.exceptionAsDisctionary = string.Equals(
                initializeData, "exceptionAsDictionary", StringComparison.OrdinalIgnoreCase); 
        }

        /// <summary>
        /// Gets a value indicating whether this instance is logging enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is logging enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoggingEnabled {
            get {
                if (this.logWriter != null) {
                    ////
                    //// return logWriter.IsLoggingEnabled();
                    ////
                    MethodInfo loggingEnabledMethod = this.logWriter.GetType().GetMethod("IsLoggingEnabled", new Type[0]);
                    return (bool)loggingEnabledMethod.Invoke(this.logWriter, new object[0]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is tracing enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tracing enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsTracingEnabled {
            get {
                if (this.logWriter != null) {
                    ////
                    ////  return logWriter.IsTracingEnabled();
                    ////
                    MethodInfo loggingEnabledMethod = this.logWriter.GetType().GetMethod("IsTracingEnabled", new Type[0]);
                    return (bool)loggingEnabledMethod.Invoke(this.logWriter, new object[0]);
                }

                return false;
            }
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="categories">Category names used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="T:System.Diagnostics.TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="title">Additional description of the log entry message.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        /// <param name="relatedActivityId">The qualified name of related activity.</param>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        public void Write(string message, ICollection<string> categories, int priority, int eventId, System.Diagnostics.TraceEventType severity, string title, IDictionary<string, object> properties, Exception exception, Guid activityId, Guid? relatedActivityId) {
            if (this.logWriter != null) {
                // Log to flat files do not support
                if (exception != null && this.exceptionAsDisctionary) {
                    if (properties == null) {
                        properties = new Dictionary<string, object>(5);
                    }

                    ExceptionInformationProvider exceptionProvider = new ExceptionInformationProvider(exception);
                    exceptionProvider.PopulateDictionary(properties);
                }

                ////
                //// Microsoft.Practices.EnterpriseLibrary.Logging.XmlLogEntry log = new Microsoft.Practices.EnterpriseLibrary.Logging.XmlLogEntry();
                //// log.Message = message;
                //// log.Categories = categories;
                //// log.Priority = priority;
                //// log.EventId = eventId;
                //// log.Severity = severity;
                //// log.Title = title;
                //// log.ExtendedProperties = properties;
                //// log.ActivityId = activityId;
                //// log.RelatedActivityId = relatedActivityId;
                //// log.Xml = DefaultLogWriter.BuildTraceRecord(message, priority, severity, title, properties, exception);
                ////
                Type logEntryType = this.loggingAssembly.GetType("Microsoft.Practices.EnterpriseLibrary.Logging.XmlLogEntry");
                object logEntry = Activator.CreateInstance(logEntryType);
                logEntryType.GetProperty("Message").SetValue(logEntry, message, new object[0]);
                logEntryType.GetProperty("Categories").SetValue(logEntry, categories, new object[0]);
                logEntryType.GetProperty("Priority").SetValue(logEntry, priority, new object[0]);
                logEntryType.GetProperty("EventId").SetValue(logEntry, eventId, new object[0]);
                logEntryType.GetProperty("Severity").SetValue(logEntry, severity, new object[0]);
                logEntryType.GetProperty("Title").SetValue(logEntry, title, new object[0]);
                logEntryType.GetProperty("ExtendedProperties").SetValue(logEntry, properties, new object[0]);
                logEntryType.GetProperty("ActivityId").SetValue(logEntry, activityId, new object[0]);
                logEntryType.GetProperty("RelatedActivityId").SetValue(logEntry, relatedActivityId, new object[0]);
                logEntryType.GetProperty("Xml").SetValue(logEntry, DefaultLogWriter.BuildTraceRecord(message, priority, severity, title, properties, exception), new object[0]); 

                //// 
                //// this.logWriter.Write(log); 
                ////
                MethodInfo writeMethod = this.logWriter.GetType().GetMethod("Write", new Type[] { logEntryType });
                writeMethod.Invoke(this.logWriter, new object[] { logEntry });
            }
        }

        /// <summary>
        /// Flushes log writer.
        /// </summary>
        public void Flush() {
            ////
            //// this.logWriter.FlushContextItems();
            //// 
            if (this.logWriter != null) {
                MethodInfo flushContextItemsMethod = this.logWriter.GetType().GetMethod("FlushContextItems", new Type[0]);
                flushContextItemsMethod.Invoke(this.logWriter, new object[0]);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            this.Dispose(true); 
            GC.SuppressFinalize(this); 
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                IDisposable writer = this.logWriter as IDisposable;
                if (writer != null) {
                    writer.Dispose();
                }
            }
        }

        /// <summary>
        /// Initializes the instance of the <see cref="EntrLibLogWriter"/>.
        /// </summary>
        /// <param name="loggingAssembly">The logging assembly.</param>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "LogWriterFactory", Justification = "A class name")]
        protected void Initialize(Assembly loggingAssembly) {
            this.loggingAssembly = loggingAssembly;

            if (this.loggingAssembly != null) {
                ////
                //// this.logWriter = new Microsoft.Practices.EnterpriseLibrary.Logging.LogWriterFactory().Create();
                ////
                Type factoryType =
                 loggingAssembly.GetType("Microsoft.Practices.EnterpriseLibrary.Logging.LogWriterFactory");

                object factory = Activator.CreateInstance(factoryType);
                MethodInfo createMethod = factoryType.GetMethod("Create", new Type[0]); // Get method without paramters
                if (createMethod == null) {
                    throw new InvalidOperationException("Method Create must be present in a class 'LogWriterFactory'.");
                }

                this.logWriter = createMethod.Invoke(factory, new object[0]);
            }
        }

        #region Private Class
        /// <summary>
        /// Represent exception information provider.
        /// </summary>
        private class ExceptionInformationProvider : ExtraInformation.IExtraInformationProvider {
            private readonly Exception exception;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExceptionInformationProvider"/> class.
            /// </summary>
            /// <param name="exception">The exception.</param>
            public ExceptionInformationProvider(Exception exception) {
                if (exception == null) {
                    throw new ArgumentNullException("exception");
                }

                this.exception = exception;
            }

            /// <summary>
            /// Populates an <see cref="T:System.Collections.Generic.IDictionary`2"/> with helpful diagnostic information.
            /// </summary>
            /// <param name="dict">Dictionary containing extra information used to initialize the <see cref="T:Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.IExtraInformationProvider"></see> instance</param>
            public void PopulateDictionary(IDictionary<string, object> dict) {
                if (dict == null) {
                    throw new ArgumentNullException("dict");
                }

                dict.Add(SR.ExtraInformation_ExceptionType, this.exception.GetType().AssemblyQualifiedName);
                dict.Add(SR.ExtraInformation_ExceptionMessage, this.exception.Message);
                dict.Add(SR.ExtraInformation_ExceptionString, this.exception.ToString());

                // Stack Trace
                dict.Add(SR.ExtraInformation_StackTrace, DefaultLogWriter.StackTraceString(this.exception));

                // Win32 Exception
                System.ComponentModel.Win32Exception win32Exception = this.exception as System.ComponentModel.Win32Exception;
                if (win32Exception != null) {
                    dict.Add(SR.ExtraInformation_NativeErrorCode, win32Exception.NativeErrorCode.ToString("X", CultureInfo.InvariantCulture));
                }
            }
        }
        #endregion
    }
}

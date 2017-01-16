// ----------------------------------------------------------------------------
// <copyright file="DefaultLogWriter.cs" company="ABC Software Ltd">
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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml;
    using System.Xml.XPath;

    /// <summary>
    /// Default Log Writer
    /// </summary>
    public class DefaultLogWriter : ILogWriter {
#if !NETSTANDARD
        private static readonly string AppDomainName = AppDomain.CurrentDomain.FriendlyName;
#endif

        /// <summary>
        /// Gets a value indicating whether this instance is logging enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is logging enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoggingEnabled {
            get {
                return true;
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
                return HasTraceListeners(new TraceSource(Abc.Diagnostics.SR.TraceAsTraceSource));
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
        public void Write(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties, Exception exception, Guid activityId, Guid? relatedActivityId) {
            if (categories != null && categories.Count > 0) {
                foreach (string category in categories) {
                    TraceSource ts = new TraceSource(category);
                    if (!HasTraceListeners(ts)) {
                        ts = new TraceSource(Abc.Diagnostics.SR.TraceAsTraceSource); 
                    }
#if !NETSTANDARD
                    if (severity == TraceEventType.Transfer && relatedActivityId.HasValue) {
                        ts.TraceTransfer(eventId, message, relatedActivityId.Value);
                    }
                    else {
#else               
                    {
#endif
                        ts.TraceData(severity, eventId, BuildTraceRecord(message, priority, severity, title, properties, exception));
                    }
                }
            }
            else {
                TraceSource ts = new TraceSource(LogUtility.GeneralCategory);
#if !NETSTANDARD
                if (severity == TraceEventType.Transfer && relatedActivityId.HasValue) {
                    ts.TraceTransfer(eventId, message, relatedActivityId.Value);
                }
                else {
#else
                {
#endif
                    ts.TraceData(severity, eventId, BuildTraceRecord(message, priority, severity, title, properties, exception));
                }
            }
        }

        /// <summary>
        /// Flushes log writer.
        /// </summary>
        public void Flush() {
        }

#region BuildTraceRecord
        internal static XPathNavigator BuildTraceRecord(string message, int priority, TraceEventType severity, string title, IDictionary<string, object> properties, Exception exception) {
            const string TraceRecordNamespaceURI = "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord";
            const string DictionaryTraceRecordNamespaceURI = "http://schemas.microsoft.com/2006/08/ServiceModel/DictionaryTraceRecord";

#if !NETSTANDARD
            XmlDocument doc = new XmlDocument();
            XPathNavigator navigator = doc.CreateNavigator();
#else
            XPathNavigator navigator = new XPathDocument(new System.IO.MemoryStream(2048)).CreateNavigator();
#endif
            using (XmlWriter writer = navigator.AppendChild()) {
                writer.WriteStartElement("TraceRecord", TraceRecordNamespaceURI);
                
                writer.WriteAttributeString("Severity", severity.ToString());

                string traceId = string.Format(CultureInfo.InvariantCulture, "http://www.abcsoftware.lv/{0}/library/Diagnostic", CultureInfo.CurrentCulture.Name);
                writer.WriteElementString("TraceIdentifier", traceId);

                if (message != null) {
                    writer.WriteElementString("Description", message);
                }

#if !NETSTANDARD
                writer.WriteElementString("AppDomain", AppDomainName);
#endif

                if (title != null) {
                    writer.WriteElementString("Source", title);
                }

                writer.WriteElementString("Priority", priority.ToString(CultureInfo.InvariantCulture)); 

                if (properties != null && properties.Count > 0) {
                    writer.WriteStartElement("ExtendedData", DictionaryTraceRecordNamespaceURI);

                    foreach (KeyValuePair<string, object> pair in properties) {
                        string value = pair.Value != null ? pair.Value.ToString() : string.Empty;
                        writer.WriteElementString(XmlConvert.EncodeLocalName(pair.Key), value); 
                    }

                    writer.WriteEndElement();
                }

                if (exception != null) {
                    writer.WriteStartElement("Exception");
                    BuildExceptionRecord(writer, exception);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            return navigator; 
        }

        internal static string StackTraceString(Exception exception) {
            string stackTrace = exception.StackTrace;
            if (!string.IsNullOrEmpty(stackTrace)) {
                return stackTrace;
            }

#if !NETSTANDARD
            try {
                StackFrame[] frames = new StackTrace(false).GetFrames();
                int skipFrames = 0;
                foreach (StackFrame frame in frames) {
                    string name = frame.GetMethod().Module.ScopeName;
                    if (name != null && (name == typeof(LogUtility).Module.ScopeName)) {
                        skipFrames++;
                    }
                    else {
                        break;
                    }
                }

                StackTrace trace = new StackTrace(skipFrames, false);
                stackTrace = trace.ToString();
            }
            catch (System.Security.SecurityException) {
                stackTrace = string.Format(
                    Abc.Diagnostics.SR.Culture, Abc.Diagnostics.SR.ExtraInformation_PropertyError, Abc.Diagnostics.SR.ExtraInformation_StackTraceSecurityException);
            }
            catch (Exception ex) {
                if (ExceptionUtility.IsFatal(ex)) {
                    throw;
                }

                stackTrace = string.Format(
                    Abc.Diagnostics.SR.Culture, Abc.Diagnostics.SR.ExtraInformation_PropertyError, Abc.Diagnostics.SR.ExtraInformation_StackTraceException);
            }
#else
                stackTrace = string.Format(
                    Abc.Diagnostics.SR.Culture, Abc.Diagnostics.SR.ExtraInformation_PropertyError, Abc.Diagnostics.SR.ExtraInformation_StackTraceException);
#endif

            return stackTrace; 
        }

        private static void BuildExceptionRecord(XmlWriter writer, Exception exception) {
            writer.WriteElementString("ExceptionType", exception.GetType().AssemblyQualifiedName);
            writer.WriteElementString("Message", exception.Message);
            writer.WriteElementString("StackTrace", StackTraceString(exception));
            writer.WriteElementString("ExceptionString", exception.ToString());

            Win32Exception win32Exception = exception as Win32Exception;
            if (win32Exception != null) {
                writer.WriteElementString("NativeErrorCode", win32Exception.NativeErrorCode.ToString("X", CultureInfo.InvariantCulture));
            }

            if (exception.Data != null && exception.Data.Count > 0) {
                writer.WriteStartElement("DataItems");
                foreach (object key in exception.Data.Keys) {
                    writer.WriteStartElement("Data");
                    writer.WriteElementString("Key", key.ToString());
                    writer.WriteElementString("Value", exception.Data[key].ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            if (exception.InnerException != null) {
                writer.WriteStartElement("InnerException");
                BuildExceptionRecord(writer, exception.InnerException);
                writer.WriteEndElement();
            }
        }
#endregion

        private static bool HasTraceListeners(TraceSource traceSource) {
            foreach (var item in traceSource.Listeners) {
                if (item is DefaultTraceListener) {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}
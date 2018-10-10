// ----------------------------------------------------------------------------
// <copyright file="RoutedLogWriter.cs" company="ABC Software Ltd">
//    Copyright © 2018 ABC Software Ltd. All rights reserved.
//
//    This library is free software; you can redistribute it and/or.
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

#if !NETSTANDARD
#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic {
#else
namespace Abc.Diagnostics {
#endif
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Routed log writer
    /// </summary>
    /// <seealso cref="ILogWriter" />
    public class RoutedLogWriter : ILogWriter, ILogWriterCustomAttributes {
        private const string DefaultCategoryAttributeName = "defaultCategory";
        private readonly Dictionary<string[], ILogWriter> logWriters = new Dictionary<string[], ILogWriter>();
        private string defaultCategory = LogUtility.GeneralCategory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutedLogWriter"/> class.
        /// </summary>
        public RoutedLogWriter() {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is logging enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is logging enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoggingEnabled {
            get {
                foreach (var item in this.logWriters.Values) {
                    if (item.IsLoggingEnabled) {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is tracing enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is tracing enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsTracingEnabled {
            get {
                foreach (var item in this.logWriters.Values) {
                    if (item.IsTracingEnabled) {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Flushes log writer.
        /// </summary>
        public void Flush() {
            foreach (var logWriter in this.logWriters.Values) {
                logWriter.Flush();
            }
        }

        /// <summary>
        /// Gets the custom attributes supported by the Log writer.
        /// </summary>
        /// <returns>
        /// A naming enumeration the custom attributes supported by the trace listener, or <c>null</c> if there are no custom attributes
        /// </returns>
        public IEnumerable<string> GetSupportedAttributes() {
            return new string[] { DefaultCategoryAttributeName };
        }

        /// <summary>
        /// Sets the attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        public void SetAttributes(IDictionary<string, string> attributes) {
            if (attributes == null) {
                throw new ArgumentNullException("attributes");
            }

            if (attributes.ContainsKey(DefaultCategoryAttributeName)) {
                this.defaultCategory = attributes[DefaultCategoryAttributeName];
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
        /// <param name="severity">Log message severity as a <see cref="T:System.Diagnostics.TraceEventType" /> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="title">Additional description of the log entry message.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        /// <param name="relatedActivityId">The qualified name of related activity.</param>
        public void Write(
            string message,
            ICollection<string> categories,
            int priority,
            int eventId,
            TraceEventType severity,
            string title,
            IDictionary<string, object> properties,
            Exception exception,
            Guid activityId,
            Guid? relatedActivityId) {
            if (categories != null && categories.Count > 0) {
                foreach (var category in categories) {
                    this.Write(message, category, priority, eventId, severity, title, properties, exception, activityId, relatedActivityId);
                }
            }
            else {
                this.Write(message, this.defaultCategory, priority, eventId, severity, title, properties, exception, activityId, relatedActivityId);
            }
        }

#pragma warning disable S107 // Methods should not have too many parameters
        internal void Write(
            string message,
            string category,
            int priority,
            int eventId,
            TraceEventType severity,
            string title,
            IDictionary<string, object> properties,
            Exception exception,
            Guid activityId,
            Guid? relatedActivityId) {
#pragma warning restore S107 // Methods should not have too many parameters
            var writers = new List<ILogWriter>();
            foreach (var item in this.logWriters) {
                if (Array.IndexOf(item.Key, category) > -1) {
                    writers.Add(item.Value);
                }
            }

            if (writers.Count == 0) {
                foreach (var item in this.logWriters) {
                    if (Array.IndexOf(item.Key, "*") > -1) {
                        writers.Add(item.Value);
                    }
                }
            }

            foreach (var writer in writers) {
                writer.Write(message, new string[] { category }, priority, eventId, severity, title, properties, exception, activityId, relatedActivityId);
            }
        }

        internal void SetFilters(Configuration.FilterElementCollection filters) {
            if (filters == null) {
                return;
            }

            foreach (Configuration.FilterElement filter in filters) {
                var categories = new string[filter.Categories.Count];
                filter.Categories.CopyTo(categories, 0);

                this.logWriters.Add(categories, Configuration.LogWriterFactory.CreteLogWriter(filter));
            }
        }
    }
}
#endif
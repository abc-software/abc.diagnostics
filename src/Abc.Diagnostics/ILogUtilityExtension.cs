// ----------------------------------------------------------------------------
// <copyright file="ILogUtilityExtension.cs" company="ABC Software Ltd">
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

#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic {
#else
namespace Abc.Diagnostics {
#endif
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// <see cref="ILogUtility"/> extension.
    /// </summary>
    public static class ILogUtilityExtension {
        internal const int DefaultPriority = -1;
        internal const int DefaultEventId = 0;
        private const TraceEventType DefaultSeverity = TraceEventType.Verbose;
        private const TraceEventType DefaultExceptionSeverity = TraceEventType.Error;
        private static readonly ICollection<string> EmptyCategoriesList = new List<string>(0);

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
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        public static void Write(this ILogUtility logUtility, string message) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, EmptyCategoriesList, DefaultPriority, DefaultEventId, DefaultSeverity, null, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry to a specific category.
        /// </summary>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        public static void Write(this ILogUtility logUtility, string message, string category) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, new string[] { category }, DefaultPriority, DefaultEventId, DefaultSeverity, null, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category and priority.
        /// </summary>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        public static void Write(this ILogUtility logUtility, string message, string category, int priority) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, new string[] { category }, priority, DefaultEventId, DefaultSeverity, null, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority and event id.
        /// </summary>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        public static void Write(this ILogUtility logUtility, string message, string category, int priority, int eventId) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, new string[] { category }, priority, eventId, DefaultSeverity, null, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event id and severity.
        /// </summary>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log entry severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        public static void Write(this ILogUtility logUtility, string message, string category, int priority, int eventId, TraceEventType severity) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, new string[] { category }, priority, eventId, severity, null, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event id and severity.
        /// </summary>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log entry severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="activityId">The qualified name of activity.</param>
        public static void Write(this ILogUtility logUtility, string message, string category, int priority, int eventId, TraceEventType severity, Guid activityId) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, new string[] { category }, priority, eventId, severity, null, activityId);
        }
        #endregion

        #region Write with Properties

        /// <summary>
        /// Write a new log entry and a dictionary of extended properties.
        /// </summary>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public static void Write(this ILogUtility logUtility, string message, IDictionary<string, object> properties) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, EmptyCategoriesList, DefaultPriority, DefaultEventId, DefaultSeverity, properties, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry to a specific category with a dictionary 
        /// of extended properties.
        /// </summary>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public static void Write(this ILogUtility logUtility, string message, string category, IDictionary<string, object> properties) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, new string[] { category }, DefaultPriority, DefaultEventId, DefaultSeverity, properties, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry to with a specific category, priority and a dictionary 
        /// of extended properties.
        /// </summary>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public static void Write(this ILogUtility logUtility, string message, string category, int priority, IDictionary<string, object> properties) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, new string[] { category }, priority, DefaultEventId, DefaultSeverity, properties, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public static void Write(this ILogUtility logUtility, string message, string category, int priority, int eventId, TraceEventType severity, IDictionary<string, object> properties) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, new string[] { category }, priority, eventId, severity, properties, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="categories">Category names used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        public static void Write(this ILogUtility logUtility, string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, IDictionary<string, object> properties) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, categories, priority, eventId, severity, properties, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="categories">Category names used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        public static void Write(this ILogUtility logUtility, string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, IDictionary<string, object> properties, Guid activityId) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, categories, priority, eventId, severity, properties, activityId);
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
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="exception">Exception to log.</param>
        public static void Write(this ILogUtility logUtility, string message, Exception exception) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, null, DefaultPriority, DefaultEventId, DefaultExceptionSeverity, exception, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="exception">Exception to log.</param>
        public static void Write(this ILogUtility logUtility, string message, string category, Exception exception) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, category, DefaultPriority, DefaultEventId, DefaultExceptionSeverity, exception, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="exception">Exception to log.</param>
        public static void Write(this ILogUtility logUtility, string message, string category, int priority, Exception exception) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, category, priority, DefaultEventId, DefaultExceptionSeverity, exception, Guid.Empty);
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="exception">Exception to log.</param>
        public static void Write(this ILogUtility logUtility, string message, string category, int priority, int eventId, TraceEventType severity, Exception exception) {
            if (logUtility == null) {
                throw new ArgumentNullException("logUtility");
            }

            logUtility.Write(message, category, priority, eventId, severity, exception, Guid.Empty);
        }

        #endregion
    }
}

// ----------------------------------------------------------------------------
// <copyright file="ILogWriter.cs" company="ABC Software Ltd">
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

    /// <summary>
    /// Instance to write log messages.
    /// </summary>
    public interface ILogWriter {
        /// <summary>
        /// Gets a value indicating whether this instance is logging enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is logging enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsLoggingEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is tracing enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tracing enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsTracingEnabled { get; }

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
        /// <param name="severity">Log message severity as a <see cref="T:System.Diagnostics.TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="title">Additional description of the log entry message.</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        /// <param name="relatedActivityId">The qualified name of related activity.</param>
        void Write(
            string message, 
            ICollection<string> categories, 
            int priority, 
            int eventId, 
            TraceEventType severity, 
            string title, 
            IDictionary<string, object> properties,
            Exception exception,
            Guid activityId, 
            Guid? relatedActivityId);

        /// <summary>
        /// Flushes log writer.
        /// </summary>
        void Flush();
    }
}
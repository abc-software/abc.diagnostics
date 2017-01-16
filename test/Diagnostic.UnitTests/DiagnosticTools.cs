// ----------------------------------------------------------------------------
// <copyright file="DiagnosticTools.cs" company="ABC software">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    The source code or its parts to use, reproduce, transfer, copy or
//    keep in an electronic form only from written agreement ABC SOFTWARE.
// </copyright>
// ----------------------------------------------------------------------------

namespace Diagnostic.UnitTests {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Diagnostic;
    using Diagnostic.ExtraInformation;

    /// <summary>
    /// Diagnostic tools.
    /// </summary>
    internal static class DiagnosticTools {
        private static readonly object lockObject = new object();
        private static MyLogUtility logUtil;
        private static ExceptionUtility exceptionUtil;

        /// <summary>
        /// Gets the library urn.
        /// </summary>
        /// <value>The library urn.</value>
        public static string LibraryUrn {
            get {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetCallingAssembly().GetName();
                return string.Format("URN:IVIS:100001:LIB-{0}-v{1}-{2}", assemblyName.Name, assemblyName.Version.Major, assemblyName.Version.Minor);
            }
        }

        /// <summary>
        /// Gets the log utility.
        /// </summary>
        /// <value>The log utility.</value>
        public static MyLogUtility LogUtil {
            get { return logUtil ?? GetLogUtility(); }
        }

        /// <summary>
        /// Gets the exception utility.
        /// </summary>
        /// <value>The exception utility.</value>
        public static ExceptionUtility ExceptionUtil {
            get { return exceptionUtil ?? GetExceptionUtility(); }
        }

        private static MyLogUtility GetLogUtility() {
            lock (lockObject) {
                if (logUtil == null) {
                    logUtil = new MyLogUtility(LibraryUrn);
                    
                    // Enable activity logging
                    LogActivity.UseDiagnosticTrace(logUtil as LogUtility);  
                }
            }

            return logUtil;
        }

        private static ExceptionUtility GetExceptionUtility() {
            lock (lockObject) {
                if (exceptionUtil == null) {
                    exceptionUtil = new ExceptionUtility(LogUtil);
                }
            }

            return exceptionUtil;
        }
    }

    /// <summary>
    /// Custom log utility.
    /// </summary>
    internal class MyLogUtility : LogUtility {
        internal const string AuditCategory = "Audit";
        internal const string DefaultLogCategory = "MockCategoryOne";

        /// <summary>
        /// Initializes a new instance of the <see cref="MyLogUtility"/> class.
        /// </summary>
        /// <param name="sourceName">Name of the source.</param>
        public MyLogUtility(string sourceName)
            : base(sourceName) {
        }

        /// <summary>
        /// Writes the audit.
        /// </summary>
        /// <param name="actionCode">The action code.</param>
        /// <param name="eventId">The event id.</param>
        /// <param name="message">The message.</param>
        public void WriteAudit(string actionCode, int eventId, string message) {
            IDictionary<string, object> properties = new Dictionary<string, object>();

            //// TODO: Add InformationProviders here

            this.WriteCore(message, new string[] { AuditCategory }, -1, eventId, TraceEventType.Information, properties, Guid.Empty); 
        }

        /// <summary>
        /// Write a new log entry with a specific category, priority, event Id, severity
        /// title and dictionary of extended properties.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="categories">Category names used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log message severity as a <see cref="TraceEventType"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="properties">Dictionary of key/value pairs to log.</param>
        /// <param name="activityId">The qualified name of activity.</param>
        /// <example>The following example demonstrates use of the Write method with
        /// a full set of parameters.
        /// <code></code></example>
        protected override void WriteCore(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, IDictionary<string, object> properties, Guid activityId) {
            if (categories.Count == 0) {
                categories.Add(DefaultLogCategory); 
            }
            else if (!categories.Contains(AuditCategory)) {
                this.PopulateDictionary(properties);
            }

            base.WriteCore(message, categories, priority, eventId, severity, properties, activityId);
        }

        /// <summary>
        /// Populates the dictionary.
        /// </summary>
        /// <param name="properties">The properties.</param>
        protected virtual void PopulateDictionary(IDictionary<string, object> properties) {
            if (properties == null) {
                properties = new Dictionary<string, object>(); 
            }

            ManagedSecurityContextInformationProvider provider = new ManagedSecurityContextInformationProvider();
            provider.PopulateDictionary(properties);  
        }
    }
}


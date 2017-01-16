// ----------------------------------------------------------------------------
// <copyright file="MockTraceListeners.cs" company="ABC software">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    The source code or its parts to use, reproduce, transfer, copy or
//    keep in an electronic form only from written agreement ABC SOFTWARE.
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Diagnostic.UnitTests {
    using System;

    /// <summary>
    /// mock trace listener.
    /// </summary>
    public class MockTraceListener : TraceListener {
        #region Fields
        public object tracedData = null;
        public TraceEventType tracedEventType = TraceEventType.Information;
        public string tracedSource = null;
        public bool wasDisposed = false;

        private static readonly List<LogEntry> entries = new List<LogEntry>();
        private static readonly List<MockTraceListener> instances = new List<MockTraceListener>();
        private static readonly object traceRequestMonitor = new object();
        private static readonly List<Guid> transferGuids = new List<Guid>();
        private static int processedTraceRequests = 0;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MockTraceListener"/> class.
        /// </summary>
        public MockTraceListener()
            : this(string.Empty) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockTraceListener"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MockTraceListener(string name) {
            this.Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the traced data.
        /// </summary>
        /// <value>The traced data.</value>
        public object TracedData {
            get { return tracedData; }
        }

        /// <summary>
        /// Gets Entries.
        /// </summary>
        public static List<LogEntry> Entries {
            get { return entries; }
        }

        /// <summary>
        /// Gets Instances.
        /// </summary>
        public static List<MockTraceListener> Instances {
            get { return instances; }
        }

        /// <summary>
        /// Gets LastEntry.
        /// </summary>
        public static LogEntry LastEntry {
            get { return entries.Count > 0 ? entries[entries.Count - 1] : null; }
        }

        /// <summary>
        /// Gets ProcessedTraceRequests.
        /// </summary>
        public static int ProcessedTraceRequests {
            get {
                lock (traceRequestMonitor) {
                    return processedTraceRequests;
                }
            }
        }

        /// <summary>
        /// Gets TransferGuids.
        /// </summary>
        public static List<Guid> TransferGuids {
            get { return transferGuids; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public static void Reset() {
            lock (traceRequestMonitor) {
                entries.Clear();
                instances.Clear();
                transferGuids.Clear();
                processedTraceRequests = 0;
            }
        }

        /// <summary>
        /// Writes trace information, a data object and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">The trace data to emit.</param>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override void TraceData(
            TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data) {
            lock (traceRequestMonitor) {
                tracedData = data;
                tracedSource = source;
                tracedEventType = eventType;
                MockTraceListener.Entries.Add(data as LogEntry);
                MockTraceListener.Instances.Add(this);
                processedTraceRequests++;

                // Debug.Write((data as LogEntry).ActivityId);   
            }
        }


        /// <summary>
        /// Writes trace information, a message, a related activity identity and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">The event cache.</param>
        /// <param name="source">The source.</param>
        /// <param name="id">The id.</param>
        /// <param name="message">The message.</param>
        /// <param name="relatedActivityId">The related activity id.</param>
        public override void TraceTransfer(
            TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId) {
            lock (traceRequestMonitor) {
                MockTraceListener.transferGuids.Add(relatedActivityId);
            }
        }


        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void Write(string message) {
            throw new Exception("The method or operation is not implemented.");
        }


        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Diagnostics.TraceListener"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            wasDisposed = true;
        }

        #endregion
    }
}
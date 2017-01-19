// ----------------------------------------------------------------------------
// <copyright file="TraceUtility.cs" company="ABC Software Ltd">
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
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;

    /// <summary>
    /// Represents a performance tracing class to log method entry/exit and duration.    
    /// </summary>
    public class TraceUtility : IDisposable {
        private Stopwatch stopwatch;

        private bool tracingAvailable;
        private bool tracingAvailableInitialized;
        private bool tracerDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceUtility"/> class with the given logical operation name.
        /// </summary>
        /// <remarks>
        /// If an existing activity id is already set, it will be kept. Otherwise, a new activity id will be created.
        /// </remarks>
        /// <param name="operation">The operation for the <see cref="TraceUtility"/></param>
        public TraceUtility(string operation) {
            this.Initialize(operation, null); 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceUtility"/> class with the given logical operation name and activity id.
        /// </summary>
        /// <remarks>
        /// The activity id will override a previous activity id
        /// </remarks>
        /// <param name="operation">The operation for the <see cref="TraceUtility"/></param>
        /// <param name="activityId">The activity id</param>
        public TraceUtility(string operation, Guid activityId) {
            this.Initialize(operation, activityId);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TraceUtility"/> class.
        /// </summary>
        ~TraceUtility() {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is tracing enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tracing enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsTracingEnabled {
            get {
                return LogUtility.Writer.IsTracingEnabled; 
            }
        }

        internal bool IsTracingAvailable {
            get {
                if (!this.tracingAvailableInitialized) {
                    this.tracingAvailable = false;
                    try {
#if NET20 || NET35 
                        this.tracingAvailable = SecurityManager.IsGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
#else
                        this.tracingAvailable = new PermissionSet(PermissionState.None).IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);
#endif
                    }
                    catch (SecurityException) {
                        // Nothing to do
                    }

                    this.tracingAvailableInitialized = true;
                }

                return this.tracingAvailable;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceUtility"/> class with the given logical operation name.
        /// </summary>
        /// <param name="operation">The operation for the <see cref="TraceUtility"/></param>
        /// <returns>The <see cref="TraceUtility"/> instance.</returns>
        public static TraceUtility StartTrace(string operation) {
            return new TraceUtility(operation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceUtility"/> class with the given logical operation name and activity id.
        /// </summary>
        /// <param name="operation">The operation for the <see cref="TraceUtility"/></param>
        /// <param name="activityId">The activity id</param>
        /// <returns>The <see cref="TraceUtility"/> instance.</returns>
        public static TraceUtility StartTrace(string operation, Guid activityId) {
            return new TraceUtility(operation, activityId);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="activityId">The activity id.</param>
        protected void Initialize(string operation, Guid? activityId) {
            if (this.IsTracingAvailable) {
                if (!activityId.HasValue) {
                    if (TraceUtility.GetActivityId().Equals(Guid.Empty)) {
                        TraceUtility.SetActivityId(Guid.NewGuid()); 
                    }
                }
                else {
                    TraceUtility.SetActivityId(activityId.Value); 
                }

                StartLogicalOperation(operation);
                this.WriteTraceStartMessage();
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="TraceUtility"/> and optionally releases
        /// the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/>
        /// to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing && !this.tracerDisposed) {
                if (this.IsTracingAvailable) {
                    try {
                        this.WriteTraceEndMessage();
                    }
                    finally {
                        try {
                            StopLogicalOperation();
                        }
                        catch (SecurityException) {
                        }
                    }
                }

                this.tracerDisposed = true;
            }
        }

        private static decimal GetSecondsElapsed(long milliseconds) {
            decimal result = Convert.ToDecimal(milliseconds) / 1000m;
            return Math.Round(result, 6);
        }

        private static Guid GetActivityId() {
            return LogUtility.ActivityId;
        }

        private static void SetActivityId(Guid activityId) {
            LogUtility.ActivityId = activityId;
        }

        private static void StartLogicalOperation(string operation) {
            Trace.CorrelationManager.StartLogicalOperation(operation);
        }

        private static void StopLogicalOperation() {
            Trace.CorrelationManager.StopLogicalOperation();
        }

        private static object PeekLogicalOperationStack() {
            return Trace.CorrelationManager.LogicalOperationStack.Peek();
        }

        private void WriteTraceStartMessage() {
            if (this.IsTracingEnabled) {
                this.stopwatch = Stopwatch.StartNew();

                long tracingStartTicks = Stopwatch.GetTimestamp();
                string methodName = this.GetExecutingMethodName();
                Guid activityId = TraceUtility.GetActivityId(); 
                string message = string.Format(
                    Abc.Diagnostics.SR.Culture, Abc.Diagnostics.SR.TraceStartMessage, activityId, methodName, tracingStartTicks);

                this.WriteTraceMessage(message, TraceEventType.Start, activityId);
            }
        }

        private void WriteTraceEndMessage() {
            if (this.IsTracingEnabled) {
                long tracingEndTicks = Stopwatch.GetTimestamp();
                decimal secondsElapsed = TraceUtility.GetSecondsElapsed(this.stopwatch.ElapsedMilliseconds);

                string methodName = this.GetExecutingMethodName();
                Guid activityId = GetActivityId(); 
                string message = string.Format(
                    Abc.Diagnostics.SR.Culture, Abc.Diagnostics.SR.TraceEndMessage, activityId, methodName, tracingEndTicks, secondsElapsed);

                this.WriteTraceMessage(message, TraceEventType.Stop, activityId);
            }
        }

        private void WriteTraceMessage(string message, TraceEventType severity, Guid activityId) {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            string category = PeekLogicalOperationStack() as string;

            LogUtility.Write(message, new string[] { category }, LogUtility.DefaultPriority, LogUtility.DefaultEventId, severity, LogUtility.LogSourceName, properties, null, activityId);
        }

        private string GetExecutingMethodName() {
            string result = "Unknown";
            StackTrace trace = new StackTrace(false);

            for (int index = 0; index < trace.FrameCount; ++index) {
                StackFrame frame = trace.GetFrame(index);
                MethodBase method = frame.GetMethod();
                if (method.DeclaringType != this.GetType()) {
                    result = string.Concat(method.DeclaringType.FullName, ".", method.Name);
                    break;
                }
            }

            return result;
        }
    }
}

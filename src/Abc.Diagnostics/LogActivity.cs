// ----------------------------------------------------------------------------
// <copyright file="LogActivity.cs" company="ABC Software Ltd">
//    Copyright © 2017 ABC Software Ltd. All rights reserved.
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

#if !NETSTANDARD1_x
#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic {
#else
namespace Abc.Diagnostics {
#endif
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Log activity class.
    /// </summary>
    public sealed class LogActivity : IDisposable {
        [ThreadStatic]
        private static LogActivity currentActivity;
        
        private TransferActivity localActivity;
        private readonly Guid activityId;
        private readonly LogActivity previousActivity;
        
        private bool disposed;
        private bool autoStop;
        private bool asynchronous;
        private int stopCount;
        private bool autoResume;
        private ActivityState lastState;
        private string name;
        private string type;

        private LogActivity(Guid activityId) {
            this.activityId = activityId;
            this.previousActivity = Current;
        }

        private enum ActivityState {
            Unknown,
            Start,
            Suspend,
            Resume,
            Stop,
        }

        /// <summary>
        /// Gets the current log activity.
        /// </summary>
        /// <value>The current log activity.</value>
        public static LogActivity Current {
            get { return currentActivity; }
            private set { currentActivity = value; }
        }

        /// <summary>
        /// Gets the activity id.
        /// </summary>
        /// <value>The activity id.</value>
        public Guid Id {
            get { return this.activityId; }
        }

        /// <summary>
        /// Gets the previous log activity.
        /// </summary>
        /// <value>The previous log activity.</value>
        internal LogActivity PreviousActivity {
            get { return this.previousActivity; }
        }

#region CreateActivity

        /// <summary>
        /// Creates the log activity.
        /// </summary>
        /// <returns>The <see cref="LogActivity"/>.</returns>
        public static LogActivity CreateActivity() {
            return CreateActivity(Guid.NewGuid(), true);
        }

        /// <summary>
        /// Creates the log activity .
        /// </summary>
        /// <param name="autoStop">if set to <c>true</c> then the activity auto stop.</param>
        /// <returns>The <see cref="LogActivity"/>.</returns>
        public static LogActivity CreateActivity(bool autoStop) {
            return CreateActivity(Guid.NewGuid(), autoStop);
        }

        /// <summary>
        /// Creates the log activity .
        /// </summary>
        /// <param name="activityId">The activity id.</param>
        /// <returns>The <see cref="LogActivity"/>.</returns>
        public static LogActivity CreateActivity(Guid activityId) {
            return CreateActivity(activityId, true);
        }

        /// <summary>
        /// Creates the log activity .
        /// </summary>
        /// <param name="activityId">The activity id.</param>
        /// <param name="autoStop">if set to <c>true</c> then the activity auto stop.</param>
        /// <returns>The <see cref="LogActivity"/>.</returns>
        public static LogActivity CreateActivity(Guid activityId, bool autoStop) {
            LogActivity activity = null;
            if (activityId != Guid.Empty) {
                activity = new LogActivity(activityId);
            }

            if (activity != null) {
                activity.autoStop = autoStop;
                Current = activity;
            }

            return activity;
        }

        /// <summary>
        /// Creates the asynchronous log activity .
        /// </summary>
        /// <returns>The <see cref="LogActivity"/>.</returns>
        public static LogActivity CreateAsyncActivity() {
            LogActivity activity = CreateActivity(true);
            if (activity != null) {
                activity.asynchronous = true;
            }

            return activity;
        }

        /// <summary>
        /// Creates the bounded log activity .
        /// </summary>
        /// <returns>The <see cref="LogActivity"/>.</returns>
        public static LogActivity CreateBoundedActivity() {
            return CreateBoundedActivity(Guid.NewGuid(), false);
        }

        /// <summary>
        /// Creates the bounded log activity .
        /// </summary>
        /// <param name="activityId">The activity id.</param>
        /// <returns>The <see cref="LogActivity"/>.</returns>
        public static LogActivity CreateBoundedActivity(Guid activityId) {
            return CreateBoundedActivity(activityId, false);
        }

        /// <summary>
        /// Creates the bounded log activity .
        /// </summary>
        /// <param name="suspendCurrent">if set to <c>true</c> then suspend current log activity.</param>
        /// <returns>The <see cref="LogActivity"/>.</returns>
        public static LogActivity CreateBoundedActivity(bool suspendCurrent) {
            return CreateBoundedActivity(Guid.NewGuid(), suspendCurrent);
        }

        /// <summary>
        /// Creates the bounded log activity .
        /// </summary>
        /// <param name="activityId">The activity id.</param>
        /// <param name="suspendCurrent">if set to <c>true</c> then suspend current log activity.</param>
        /// <returns>The <see cref="LogActivity"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Must be disposed in the parent class.")]
        public static LogActivity CreateBoundedActivity(Guid activityId, bool suspendCurrent) {
            LogActivity current = Current;
            LogActivity activity = CreateActivity(activityId, true);
            if (activity != null) {
                // activity.activity = TransferActivity.CreateActivity(activity.activityId, true);
                TransferActivity activity2 = TransferActivity.CreateActivity(activity.activityId, true);
                if (activity2 != null) {
                    activity2.SetPreviousActivity(current);
                }

                activity.localActivity = activity2;
                Current = activity;

                if (suspendCurrent) {
                    activity.autoResume = true;
                }
            }

            if (current != null && suspendCurrent) {
                current.Suspend();
            }

            return activity;
        }

        /// <summary>
        /// Creates and start the log activity.
        /// </summary>
        /// <param name="autoStop">if set to <c>true</c> then the activity auto stop.</param>
        /// <param name="activityName">Name of the activity.</param>
        /// <param name="activityType">Type of the activity.</param>
        /// <returns>The <see cref="LogActivity"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Must be used in using.")]
        public static LogActivity CreateActivity(bool autoStop, string activityName, string activityType) {
            LogActivity activity = CreateActivity(autoStop);
            Start(activity, activityName, activityType);
            return activity;
        }
#endregion

        /// <summary>
        /// Starts the specified log activity.
        /// </summary>
        /// <param name="activity">The log activity.</param>
        /// <param name="activityName">Name of the activity.</param>
        /// <param name="activityType">Type of the activity.</param>
        public static void Start(LogActivity activity, string activityName, string activityType) {
            if (activity != null) {
                activity.Start(activityName, activityType);
            }
        }

        /// <summary>
        /// Stops the specified log activity.
        /// </summary>
        /// <param name="activity">The log activity.</param>
        public static void Stop(LogActivity activity) {
            if (activity != null) {
                activity.Stop();
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="activityName">Name of the activity.</param>
        /// <param name="activityType">Type of the activity.</param>
        public void Start(string activityName, string activityType) {
            if (this.lastState == ActivityState.Unknown) {
                this.lastState = ActivityState.Start;
                this.name = activityName;
                this.type = activityType;
                this.TraceMilestone(TraceEventType.Start);
            }
        }

        /// <summary>
        /// Suspends this instance.
        /// </summary>
        public void Suspend() {
            if (this.lastState != ActivityState.Stop) {
                this.lastState = ActivityState.Suspend;
                this.TraceMilestone(TraceEventType.Suspend);
            }
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        public void Resume() {
            if (this.lastState == ActivityState.Suspend) {
                this.lastState = ActivityState.Resume;
                this.TraceMilestone(TraceEventType.Resume);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() {
            int num = 0;
            if (this.asynchronous) {
                num = System.Threading.Interlocked.Increment(ref this.stopCount);
            }

            if ((this.lastState != ActivityState.Stop) && (!this.asynchronous || (this.asynchronous && (num >= 2)))) {
                this.lastState = ActivityState.Stop;
                this.TraceMilestone(TraceEventType.Stop);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            if (!this.disposed) {
                this.disposed = true;
                try {
                    if (this.localActivity != null) {
                        this.localActivity.Dispose();
                    }

                    if (this.autoStop) {
                        this.Stop();
                    }

                    LogActivity current = Current;
                    if (current != null && this.autoResume) {
                        current.Resume();
                    }
                }
                finally {
                    Current = this.previousActivity;
                    GC.SuppressFinalize(this);
                }
            }
        }

        private static void TraceTransfer(Guid newId) {
            Guid currentActivityId = LogUtility.ActivityId;
            if (newId != currentActivityId) {
                LogUtility.Writer.Write(null, new string[] { LogUtility.GeneralCategory, LogUtility.LogActivityCategory }, LogUtility.DefaultPriority, LogUtility.DefaultEventId, TraceEventType.Transfer, LogUtility.LogSourceName, null, null, currentActivityId, newId);
            }
        }

        private void TraceMilestone(TraceEventType severity) {
            IDictionary<string, object> dictionary = null;
            if (!string.IsNullOrEmpty(this.name)) {
                dictionary = new Dictionary<string, object>(2);
                dictionary["ActivityName"] = this.name;
                dictionary["ActivityType"] = this.type;
            }

            LogUtility.Write(SR.ActivityBoundary, new string[] { LogUtility.GeneralCategory, LogUtility.LogActivityCategory }, LogUtility.DefaultPriority, LogUtility.DefaultEventId, severity, LogUtility.LogSourceName, dictionary, null, this.Id);
        }

        /// <summary>
        /// Activity with transfer logging.
        /// </summary>
        private class TransferActivity : Activity {
            private bool addTransfer;
            private LogActivity previousActivity;

            private TransferActivity(Guid activityId, Guid parentId)
                : base(activityId, parentId) {
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "base class call GC.SuppressFinalize")]
            public override void Dispose() {
                try {
                    if (this.addTransfer) {
                        using (Activity.CreateActivity(this.Id)) {
                            LogActivity.TraceTransfer(this.ParentId);
                        }
                    }
                }
                finally {
                    if (this.previousActivity != null) {
                        LogActivity.Current = this.previousActivity;
                    }

                    base.Dispose();
                }
            }

            internal static TransferActivity CreateActivity(Guid activityId, bool addTransfer) {
                if (activityId == Guid.Empty) {
                    return null;
                }
                
                Guid parentId = LogUtility.ActivityId;
                if (activityId == parentId) {
                    return null;
                }
                
                if (addTransfer) {
                    LogActivity.TraceTransfer(activityId);
                }

                return new TransferActivity(activityId, parentId) { addTransfer = addTransfer };
            }

            internal void SetPreviousActivity(LogActivity previous) {
                this.previousActivity = previous;
            }
        }
    }
}
#endif
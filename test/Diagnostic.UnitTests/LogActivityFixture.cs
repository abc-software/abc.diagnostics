using System;
using System.Collections.Generic;
using System.Diagnostics;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using Microsoft.Practices.EnterpriseLibrary.Logging;
#endif 

namespace Diagnostic.UnitTests {
    [TestClass()]
    public class LoagActivityFixture {
        private const string title = "bar";
        private const string message = "testing.... message and category";
        private const string category = "MockCategoryOne";
        private const string activityCategory = "Activity";
        private static string[] categories = new string[] { category };
        private const int priority = 420;
        private const int eventId = 421;
        private const TraceEventType severity = TraceEventType.Information;
        private readonly Guid testActivityId1 = new Guid("1CF75C3C-127F-41c9-97B2-FBEDB64F974A");
        private readonly Guid testActivityId2 = new Guid("5D352811-28DF-4baf-BD18-5997FE1260A3");

        [TestInitialize()]
        public void MyTestInitialize() {
            AppDomain.CurrentDomain.SetData("APPBASE", Environment.CurrentDirectory);
            MockTraceListener.Reset();
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            MockTraceListener.Reset();
        }

        /// <summary>
        /// New bounded ActivityId without activityID create new ActivityId
        ///</summary>
        [TestMethod()]
        public void NewBOundedActivityWithoutAcivityIdCreateNewActivityId() {
            LogUtility.ActivityId = Guid.Empty;

            Assert.AreEqual(Guid.Empty, LogUtility.ActivityId); 
            using (LogActivity.CreateBoundedActivity()) {
                Assert.IsFalse(Guid.Empty == LogUtility.ActivityId);
            }
        }

        /// <summary>
        /// New bounded ActivityId keep existing activityID
        ///</summary>
        [TestMethod()]
        public void NewBoundedActivityKeepExixtingActivityId() {
            LogUtility.ActivityId = testActivityId1;

            Assert.AreEqual(testActivityId1, LogUtility.ActivityId);
            using (LogActivity.CreateBoundedActivity()) {
                Assert.IsFalse(testActivityId1 == LogUtility.ActivityId);
            }

            Assert.AreEqual(testActivityId1, LogUtility.ActivityId);
        }

        /// <summary>
        /// New bounded ActivityId ovveride existing activityID
        ///</summary>
        [TestMethod()]
        public void NewBoundedActivityOvverideExixtingActivityId() {
            LogUtility.ActivityId = testActivityId1;

            Assert.AreEqual(testActivityId1, LogUtility.ActivityId);
            using (LogActivity.CreateBoundedActivity(testActivityId2)) {
                Assert.AreEqual(testActivityId2, LogUtility.ActivityId);
            }

            Assert.AreEqual(testActivityId1, LogUtility.ActivityId);
        }

        /// <summary>
        /// New Activity Create ActivityId
        ///</summary>
        [TestMethod()]
        public void NewActivityCreateActivityId() {
            using (LogActivity activity = LogActivity.CreateActivity()) {
                Assert.IsFalse(Guid.Empty == activity.Id);
            }
        }

        /// <summary>
        /// New Activity Create keep ActivityId
        ///</summary>
        [TestMethod()]
        public void NewActivityKeepActivityId() {
            using (LogActivity activity = LogActivity.CreateActivity(testActivityId1)) {
                Assert.AreEqual(testActivityId1, activity.Id);
            }
        }

        /// <summary>
        /// NestedActivityOverrideExistingActivityId
        ///</summary>
        [TestMethod()]
        public void NestedActivityOverrideExistingActivityId() {
            using (LogActivity activity = LogActivity.CreateActivity(testActivityId1)) {
                Assert.AreEqual(testActivityId1, activity.Id);

                using (LogActivity activity2 = LogActivity.CreateActivity(testActivityId2)) {
                    Assert.AreEqual(testActivityId2, activity2.Id);
                }

                Assert.AreEqual(testActivityId1, activity.Id);
            }
        }

        /// <summary>
        /// A test for Write with fixed ActivityId
        ///</summary>
        [TestMethod()]
        public void NewBoundedActivityWriteWithAcitivityId() {
            LogUtility target = new LogUtility(title);

            using (LogActivity.CreateBoundedActivity(testActivityId1)) {
                target.Write(message, category, priority, eventId, severity);
            }

            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(testActivityId1, MockTraceListener.LastEntry.ActivityId, "ActivityId");
        }

        /// <summary>
        ///A test for Write with fixed ActivityId and RelatedActivityId
        ///</summary>
        [TestMethod()]
        public void NestedBoundedActivityOvverrideActivityId() {
            LogUtility target = new LogUtility(title);

            using (LogActivity.CreateBoundedActivity(testActivityId1)) {
                target.Write(message, category, priority, eventId, severity);
                Assert.AreEqual(1, MockTraceListener.Entries.Count);
                Assert.AreEqual(testActivityId1, MockTraceListener.LastEntry.ActivityId, "ActivityId");

                MockTraceListener.Reset();
                using (LogActivity.CreateBoundedActivity(false)) {
                    target.Write(message, category, priority, eventId, severity);
                    Assert.AreEqual(1, MockTraceListener.Entries.Count);
                    Assert.AreNotEqual(testActivityId1, MockTraceListener.LastEntry.ActivityId, "ActivityId");
                }

            }
        }

        /// <summary>
        /// A test for Write start and stop activity
        ///</summary>
        [TestMethod()]
        public void NewActivityWriteStartStopActivity() {
            LogUtility target = new LogUtility(title);

            LogUtility.ActivityId = testActivityId1;  

            LogActivity logActivity = LogActivity.CreateActivity(false);
            logActivity.Start("ActivityName", "activityType");

            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            AssertLogEntryIsValid(MockTraceListener.LastEntry, activityCategory, TraceEventType.Start, logActivity.Id);
            MockTraceListener.Reset(); 
 
            target.Write(message, category, priority, eventId, severity);

            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            AssertLogEntryIsValid(MockTraceListener.LastEntry, category, severity, testActivityId1);
            MockTraceListener.Reset(); 

            logActivity.Stop();

            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            AssertLogEntryIsValid(MockTraceListener.LastEntry, activityCategory, TraceEventType.Stop, logActivity.Id);
            MockTraceListener.Reset();
        }

        /// <summary>
        /// A test for Write start and stop activity
        ///</summary>
        [TestMethod()]
        public void NewActivityWriteStartStopActivityWithAutostop() {
            LogUtility target = new LogUtility();
            
            LogUtility.ActivityId = testActivityId1;
            Guid activityId = Guid.Empty; 

            using (LogActivity logActivity = LogActivity.CreateActivity(true)) {
                activityId = logActivity.Id;  
                logActivity.Start("ActivityName", "activityType");

                Assert.AreEqual(1, MockTraceListener.Entries.Count);
                AssertLogEntryIsValid(MockTraceListener.LastEntry, activityCategory, TraceEventType.Start, logActivity.Id);
                MockTraceListener.Reset();

                target.Write(message, category, priority, eventId, severity);

                Assert.AreEqual(1, MockTraceListener.Entries.Count);
                AssertLogEntryIsValid(MockTraceListener.LastEntry, category, severity, testActivityId1);
                MockTraceListener.Reset();
            }

            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            AssertLogEntryIsValid(MockTraceListener.LastEntry, activityCategory, TraceEventType.Stop, activityId);
            MockTraceListener.Reset();
        }

        /// <summary>
        ///A test for Write start and stop activity
        ///</summary>
        [TestMethod()]
        public void StartSuspendResumeStopActivity() {
            LogUtility target = new LogUtility();
            
            LogUtility.ActivityId = testActivityId1;   

            // Create and start activity
            LogActivity logActivity = LogActivity.CreateActivity(false, "ActivityName", "activityType");
            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            AssertLogEntryIsValid(MockTraceListener.LastEntry, activityCategory, TraceEventType.Start, logActivity.Id);
            MockTraceListener.Reset(); 

            logActivity.Suspend();
            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            AssertLogEntryIsValid(MockTraceListener.LastEntry, activityCategory, TraceEventType.Suspend, logActivity.Id);
            MockTraceListener.Reset(); 

            target.Write(message, category, priority, eventId, severity);
            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            AssertLogEntryIsValid(MockTraceListener.LastEntry, category, severity, testActivityId1);
            MockTraceListener.Reset(); 

            logActivity.Resume();
            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            AssertLogEntryIsValid(MockTraceListener.LastEntry, activityCategory, TraceEventType.Resume, logActivity.Id);
            MockTraceListener.Reset(); 

            // Stop
            LogActivity.Stop(logActivity);
            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            AssertLogEntryIsValid(MockTraceListener.LastEntry, activityCategory, TraceEventType.Stop, logActivity.Id);
            MockTraceListener.Reset(); 
        }

        /// <summary>
        ///A test for Write auto Suspend & Resume activity
        ///</summary>
        [TestMethod()]
        public void StartAutoSuspendResumeStopActivity() {
            LogUtility target = new LogUtility(); 

            // Create and start activity
            using (LogActivity activity = LogActivity.CreateBoundedActivity(testActivityId1)) {
                LogActivity.Start(activity, null, null);  
                // SuspendCurrent
                using (LogActivity activity2 = LogActivity.CreateBoundedActivity(testActivityId2, true)) {
                    LogActivity.Start(activity2, null, null);
                    target.Write(message, category, priority, eventId, severity);
                }
            }

            Assert.AreEqual(7, MockTraceListener.Entries.Count);

            // 1. Create autostop activity
            Assert.AreEqual(testActivityId1, MockTraceListener.Entries[0].ActivityId, "ActivityId");
            Assert.AreEqual(TraceEventType.Start, MockTraceListener.Entries[0].Severity, "Severity");

            // 2. Susped current activity
            Assert.AreEqual(testActivityId1, MockTraceListener.Entries[1].ActivityId, "ActivityId");
            Assert.AreEqual(TraceEventType.Suspend, MockTraceListener.Entries[1].Severity, "Severity");
             
            // 3. Create new Activity
            Assert.AreEqual(testActivityId2, MockTraceListener.Entries[2].ActivityId, "ActivityId");
            Assert.AreEqual(TraceEventType.Start, MockTraceListener.Entries[2].Severity, "Severity");

            // 4. Write message
            Assert.AreEqual(testActivityId2, MockTraceListener.Entries[3].ActivityId, "ActivityId");
            Assert.AreEqual(severity, MockTraceListener.Entries[3].Severity, "Severity");

            // 5. Stop
            Assert.AreEqual(testActivityId2, MockTraceListener.Entries[4].ActivityId, "ActivityId");
            Assert.AreEqual(TraceEventType.Stop, MockTraceListener.Entries[4].Severity, "Severity");

            // 6. Resume
            Assert.AreEqual(testActivityId1, MockTraceListener.Entries[5].ActivityId, "ActivityId");
            Assert.AreEqual(TraceEventType.Resume, MockTraceListener.Entries[5].Severity, "Severity");

            // 7. Stop
            Assert.AreEqual(testActivityId1, MockTraceListener.Entries[6].ActivityId, "ActivityId");
            Assert.AreEqual(TraceEventType.Stop, MockTraceListener.Entries[6].Severity, "Severity");
        }

        /// <summary>
        ///A test for Write Create async activity
        ///</summary>
        [TestMethod()]
        public void CreateAsyncActivity() {
            // Create and start activity
            LogActivity logActivity = LogActivity.CreateAsyncActivity();
            logActivity.Start("activityName", "activityType"); 
            logActivity.Stop();
            logActivity.Stop(); 

            Assert.AreEqual(2, MockTraceListener.Entries.Count);
            Assert.AreEqual(TraceEventType.Start, MockTraceListener.Entries[0].Severity, "Severity");
            Assert.AreEqual(TraceEventType.Stop, MockTraceListener.Entries[1].Severity, "Severity");
        }

        private void AssertLogEntryIsValid(LogEntry entry,
                                    string expectedCategory,
                                    TraceEventType expectedSeverity,
                                    Guid expectedActivityId) {

            Assert.IsTrue(entry.Categories.Contains(expectedCategory));
            Assert.AreEqual(expectedSeverity, entry.Severity);
            Assert.AreEqual(expectedActivityId, entry.ActivityId);

            Assert.AreEqual(title, entry.Title);
            //Assert.AreEqual(eventId, entry.EventId);
            //Assert.AreEqual(priority, entry.Priority);

        }
    }
}

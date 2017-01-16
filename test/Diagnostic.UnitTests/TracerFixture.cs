using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using System.Configuration;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Storage;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Diagnostic.UnitTests {
    [TestClass]
    public class TracerFixture {
        private readonly Guid referenceGuid = new Guid("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        private readonly Guid overwriteGuid = new Guid("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
        private readonly Guid testActivityId1 = new Guid("1CF75C3C-127F-41c9-97B2-FBEDB64F974A");
        private readonly Guid testActivityId2 = new Guid("5D352811-28DF-4baf-BD18-5997FE1260A3");

        private const string title = "bar"; // Assembly name
        private const string operation = "Operation";
        private const string nestedOperation = "Nested operation";
        //private const string badOperation = "bad operation";
        private const string category = "MockCategoryOne";

        [TestInitialize()]
        public void MyTestInitialize() {
            AppDomain.CurrentDomain.SetData("APPBASE", Environment.CurrentDirectory);
            MockTraceListener.Reset();
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            MockTraceListener.Reset();
        }

        [TestMethod]
        public void UsingTracerWritesEntryAndExitMessages() {
            MockTraceListener.Reset();
            Guid currentActivityId = Guid.Empty;

            using (new TraceUtility(operation)) {
                currentActivityId = Trace.CorrelationManager.ActivityId;
                Assert.AreEqual(1, MockTraceListener.Entries.Count);
                AssertLogEntryIsValid(MockTraceListener.LastEntry, title, operation, currentActivityId, true);
                MockTraceListener.Reset();
            }

            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            AssertLogEntryIsValid(MockTraceListener.LastEntry, title, operation, currentActivityId, false);
        }

        //[TestMethod]
        //public void TraceListenerWritesFullTypeNameAndMethodName() {
        //    string MyTypeAndMethodName = "Diagnostic.UnitTests.TracerFixture.TraceListenerWritesFullTypeNameAndMethodName";

        //    MockTraceListener.Reset();

        //    using (new TraceUtility(operation)) {
        //        Assert.AreEqual(1, MockTraceListener.Entries.Count);
        //        MockTraceListener.Reset();
        //    }

        //    Assert.AreEqual(1, MockTraceListener.Entries.Count);
        //    Assert.IsTrue(MockTraceListener.Entries[0].Message.Contains(MyTypeAndMethodName));
        //}

        [TestMethod]
        public void TracerUpdatesStackAfterEntryAndExit() {
            Assert.AreEqual(0, Trace.CorrelationManager.LogicalOperationStack.Count);

            using (new TraceUtility(operation)) {
                Assert.AreEqual(1, Trace.CorrelationManager.LogicalOperationStack.Count);
                Assert.AreEqual(operation, Trace.CorrelationManager.LogicalOperationStack.Peek());
            }

            Assert.AreEqual(0, Trace.CorrelationManager.LogicalOperationStack.Count);
        }

        [TestMethod]
        public void NestedTracerUpdatesStackAfterEntryAndExit() {
            Assert.AreEqual(0, Trace.CorrelationManager.LogicalOperationStack.Count);

            using (new TraceUtility(operation)) {
                Assert.AreEqual(1, Trace.CorrelationManager.LogicalOperationStack.Count);
                Assert.AreEqual(operation, Trace.CorrelationManager.LogicalOperationStack.Peek());

                using (new TraceUtility(nestedOperation)) {
                    Assert.AreEqual(2, Trace.CorrelationManager.LogicalOperationStack.Count);
                    Assert.AreEqual(nestedOperation, Trace.CorrelationManager.LogicalOperationStack.Peek());
                }

                Assert.AreEqual(1, Trace.CorrelationManager.LogicalOperationStack.Count);
                Assert.AreEqual(operation, Trace.CorrelationManager.LogicalOperationStack.Peek());
            }

            Assert.AreEqual(0, Trace.CorrelationManager.LogicalOperationStack.Count);
        }

        [TestMethod]
        public void NewTracerWithoutActivityWillCreateNewActivityIdIfThereIsNoExistingActivityId() {
            Trace.CorrelationManager.ActivityId = Guid.Empty;

            Assert.AreEqual(Guid.Empty, Trace.CorrelationManager.ActivityId);
            using (new TraceUtility(operation)) {
                Assert.IsFalse(Guid.Empty == Trace.CorrelationManager.ActivityId);
            }
        }

        [TestMethod]
        public void NewTracerWithoutActivityWillKeepExistingActivityId() {
            Trace.CorrelationManager.ActivityId = referenceGuid;

            Assert.AreEqual(referenceGuid, Trace.CorrelationManager.ActivityId);
            using (new TraceUtility(operation)) {
                Assert.AreEqual(referenceGuid, Trace.CorrelationManager.ActivityId);
            }

            Assert.AreEqual(referenceGuid, Trace.CorrelationManager.ActivityId);
        }

        [TestMethod]
        public void NewTracerWithActivityWillOverwriteExistingActivityId() {
            Trace.CorrelationManager.ActivityId = referenceGuid;

            Assert.AreEqual(referenceGuid, Trace.CorrelationManager.ActivityId);
            using (new TraceUtility(operation, overwriteGuid)) {
                Assert.AreEqual(overwriteGuid, Trace.CorrelationManager.ActivityId);
            }
        }

        [TestMethod]
        public void NestedTracerInheritsActivityId() {
            Trace.CorrelationManager.ActivityId = Guid.Empty;

            Assert.AreEqual(Guid.Empty, Trace.CorrelationManager.ActivityId);

            using (new TraceUtility(operation)) {
                Assert.IsFalse(Guid.Empty == Trace.CorrelationManager.ActivityId);
                Guid outerActivityId = Trace.CorrelationManager.ActivityId;

                using (new TraceUtility(nestedOperation)) {
                    Assert.AreEqual(outerActivityId, Trace.CorrelationManager.ActivityId);
                }
            }
        }

        [TestMethod]
        public void NestedTracerOverwritesActivityId() {
            Trace.CorrelationManager.ActivityId = Guid.Empty;

            Assert.AreEqual(Guid.Empty, Trace.CorrelationManager.ActivityId);

            using (new TraceUtility(operation)) {
                Assert.IsFalse(Guid.Empty == Trace.CorrelationManager.ActivityId);
                Guid outerActivityId = Trace.CorrelationManager.ActivityId;

                using (new TraceUtility(nestedOperation, referenceGuid)) {
                    Assert.AreEqual(referenceGuid, Trace.CorrelationManager.ActivityId);
                }

                Assert.AreEqual(referenceGuid, Trace.CorrelationManager.ActivityId);
            }
        }

        [TestMethod]
        public void LoggedMessagesDuringTracerAddsCategoryIds() {
            MockTraceListener.Reset();

            using (new TraceUtility(operation)) {
                Assert.AreEqual(1, MockTraceListener.Entries.Count);
                Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
                Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(operation));
                MockTraceListener.Reset();

                Logger.Write("message", category);
                Assert.AreEqual(1, MockTraceListener.Entries.Count);
                Assert.AreEqual(2, MockTraceListener.LastEntry.Categories.Count);
                Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(operation));
                Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
                MockTraceListener.Reset();
            }
            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(operation));
        }

        void AssertLogEntryIsValid(LogEntry entry,
                                    string expectedTitle,
                                    string expectedCategory,
                                    Guid expectedActivityId,
                                    bool isStartMessage) {
            Assert.AreEqual(expectedTitle, entry.Title);

            Assert.AreEqual(expectedActivityId, entry.ActivityId);

            Assert.AreEqual(LogUtility.DefaultEventId, entry.EventId);
            Assert.AreEqual(LogUtility.DefaultPriority, entry.Priority);
            Assert.IsTrue(entry.Categories.Contains(expectedCategory));

            if (isStartMessage) {
                Assert.AreEqual(TraceEventType.Start, entry.Severity);
                AssertMessageIsValidStartMessage(entry.Message);
            }
            else {
                Assert.AreEqual(TraceEventType.Stop, entry.Severity);
                AssertMessageIsValidEndMessage(entry.Message);
            }
        }

        void AssertMessageIsValidStartMessage(string message) {
            Assert.IsNotNull(message);

            string format = "Start Trace: Activity '{0}' in method '{1}' at {2} ticks";
            string pattern = ConvertFormatToRegex(format);

            Regex re = new Regex(pattern);

            Assert.IsTrue(re.IsMatch(message));

            MatchCollection matches = re.Matches(message);
            foreach (Match match in matches) {
                Assert.IsNotNull(match.Value);
                Assert.IsTrue(match.Value.ToString().Length > 0);
            }
        }

        void AssertMessageIsValidEndMessage(string message) {
            Assert.IsNotNull(message);

            string format = "End Trace: Activity '{0}' in method '{1}' at {2} ticks (elapsed time: {3} seconds)";
            string pattern = ConvertFormatToRegex(format);

            Regex re = new Regex(pattern);

            Assert.IsTrue(re.IsMatch(message));

            MatchCollection matches = re.Matches(message);
            foreach (Match match in matches) {
                Assert.IsNotNull(match.Value);
                Assert.IsTrue(match.Value.ToString().Length > 0);
            }
        }

        string ConvertFormatToRegex(string format) {
            string pattern = format;
            pattern = pattern.Replace("(", @"\(");
            pattern = pattern.Replace(")", @"\)");
            pattern = Regex.Replace(pattern, @"\{[0-9]\}", "(.*?)");
            return pattern;
        }
    }
}

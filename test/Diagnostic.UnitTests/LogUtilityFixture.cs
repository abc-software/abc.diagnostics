using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using System.IO;
#endif 

namespace Diagnostic.UnitTests
{
    /// <summary>
    ///This is a test class for LogUtilityTest and is intended
    ///to contain all LogUtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LogUtilityFixture {
        private const string title = "bar";
        private const string message = "testing.... message and category";
        private const string category = "MockCategoryOne";
        private static string[] categories = new string[] { category };
        private const int priority = 420;
        private const int eventId = 421;
        private const TraceEventType severity = TraceEventType.Information;
        private readonly Guid activityId = new Guid("5D352811-28DF-4baf-BD18-5997FE1260A3");

        public static Dictionary<string, object> GetPropertiesDictionary() {
            Dictionary<string, object> hash = new Dictionary<string, object>();
            hash["key1"] = "value1";
            hash["key2"] = "value2";
            hash["key3"] = "value3";

            return hash;
        }

        public static Exception GetException() {
            return new ArgumentNullException(title, message); 
        }

        [TestInitialize()]
        public void MyTestInitialize() {
            AppDomain.CurrentDomain.SetData("APPBASE", Environment.CurrentDirectory);
            MockTraceListener.Reset();

            LogUtility.Reset();
            LogUtility.SetLogWriter(new TestLogWriterProxy());
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            MockTraceListener.Reset();
        }


        /// <summary>
        ///A test for LoggingEnabled
        ///</summary>
        [TestMethod()]
        public void LoggingEnabledTest() {
            string sourceName = string.Empty;
            LogUtility target = new LogUtility(sourceName);
            bool actual;
            actual = target.LoggingEnabled;
            Assert.AreEqual(true, actual);  
        }

        /// <summary>
        ///A test for ActivityId
        ///</summary>
        [TestMethod()]
        public void ActivityIdTest() {
            Guid expected = new Guid(); // TODO: Initialize to an appropriate value
            Guid actual;
            LogUtility.ActivityId = expected;
            actual = LogUtility.ActivityId;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for LogSourceName
        ///</summary>
        [TestMethod()]
        public void LogSourceName() {
            string actual;
            actual = LogUtility.LogSourceName;
            Assert.AreEqual(title, actual);
        }

        /// <summary>
        ///A test for LogSourceName
        ///</summary>
        [TestMethod()]
        public void Constructor() {
            string actual;
            actual = new LogUtility().SourceName;
            Assert.AreEqual(title, actual);
        }

        /// <summary>
        ///A test for Write
        ///</summary>
        [TestMethod()]
        public void WriteMessageOnlyOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message);

            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(TraceEventType.Verbose, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageAndCategoryOnlyOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, category);

            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(TraceEventType.Verbose, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageCategoryAndPriorityOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, category, priority);

            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(priority, MockTraceListener.LastEntry.Priority, "priority");
            Assert.AreEqual(TraceEventType.Verbose, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageCategoryPriorityEventIdOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, category, priority, eventId);

            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(priority, MockTraceListener.LastEntry.Priority, "priority");
            Assert.AreEqual(eventId, MockTraceListener.LastEntry.EventId, "eventid");
            Assert.AreEqual(TraceEventType.Verbose, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageCategoryPriorityEventIdSeverityOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, category, priority, eventId, severity);

            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(priority, MockTraceListener.LastEntry.Priority, "priority");
            Assert.AreEqual(eventId, MockTraceListener.LastEntry.EventId, "eventid");
            Assert.AreEqual(severity, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageCategoryPriorityEventIdSeverityActivityIdOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, category, priority, eventId, severity, activityId);

            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(priority, MockTraceListener.LastEntry.Priority, "priority");
            Assert.AreEqual(eventId, MockTraceListener.LastEntry.EventId, "eventid");
            Assert.AreEqual(severity, MockTraceListener.LastEntry.Severity, "severity");
            Assert.AreEqual(activityId, MockTraceListener.LastEntry.ActivityId, "activityId");
        }
        [TestMethod]
        public void WriteMessageAndDictionaryOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, GetPropertiesDictionary());

            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(GetPropertiesDictionary().Count,
                            MockTraceListener.LastEntry.ExtendedProperties.Count, "hash count");

            Assert.AreEqual(GetPropertiesDictionary()["key1"],
                            MockTraceListener.LastEntry.ExtendedProperties["key1"], "hash count");
            Assert.AreEqual(TraceEventType.Verbose, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageCategoryAndDictionaryOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, category, GetPropertiesDictionary());

            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(GetPropertiesDictionary().Count,
                            MockTraceListener.LastEntry.ExtendedProperties.Count, "hash count");
            Assert.AreEqual(TraceEventType.Verbose, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageCategoryPriorityAndDictionaryOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, category, priority, GetPropertiesDictionary());

            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(priority, MockTraceListener.LastEntry.Priority, "priority");
            Assert.AreEqual(GetPropertiesDictionary().Count,
                            MockTraceListener.LastEntry.ExtendedProperties.Count, "hash count");
            Assert.AreEqual(TraceEventType.Verbose, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageCategoryPriorityEventIdSeverityAndDictionaryOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, category, priority, eventId, severity, GetPropertiesDictionary());

            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(priority, MockTraceListener.LastEntry.Priority, "priority");
            Assert.AreEqual(eventId, MockTraceListener.LastEntry.EventId, "eventid");
            Assert.AreEqual(severity, MockTraceListener.LastEntry.Severity, "severity");
            Assert.AreEqual(GetPropertiesDictionary().Count,
                            MockTraceListener.LastEntry.ExtendedProperties.Count, "hash count");
        }

        [TestMethod]
        public void WriteMessageCategoriesPriorityEventIdSeverityAndDictionaryOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, categories, priority, eventId, severity, GetPropertiesDictionary());

            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(priority, MockTraceListener.LastEntry.Priority, "priority");
            Assert.AreEqual(eventId, MockTraceListener.LastEntry.EventId, "eventid");
            Assert.AreEqual(severity, MockTraceListener.LastEntry.Severity, "severity");
            Assert.AreEqual(GetPropertiesDictionary().Count,
                            MockTraceListener.LastEntry.ExtendedProperties.Count, "hash count");
        }

        [TestMethod]
        public void WriteMessageAndExceptionOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, GetException());

            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");

            Assert.AreEqual(typeof(ArgumentNullException).AssemblyQualifiedName,
                            ExceptionUtilityFixture.GetExceptionQualifiedName(MockTraceListener.LastEntry), "hash count");
            Assert.AreEqual(TraceEventType.Error, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageAndNullExceptionOverload() {
            LogUtility target = new LogUtility(title);
            Exception exception = null;
            target.Write(message, exception);

            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");

            Assert.AreEqual(0, MockTraceListener.LastEntry.ExtendedProperties.Count, "no extended properties");
            Assert.AreEqual(TraceEventType.Error, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageCategoryAndExceptionOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, category, GetException());

            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");

            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(typeof(ArgumentNullException).AssemblyQualifiedName,
                            ExceptionUtilityFixture.GetExceptionQualifiedName(MockTraceListener.LastEntry), "hash count");
            Assert.AreEqual(TraceEventType.Error, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageCategoryAndNullExceptionOverload() {
            LogUtility target = new LogUtility(title);
            Exception exception = null;
            target.Write(message, category, exception);

            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");

            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(0, MockTraceListener.LastEntry.ExtendedProperties.Count, "no extended properties");
            Assert.AreEqual(TraceEventType.Error, MockTraceListener.LastEntry.Severity, "severity");
        }

        [TestMethod]
        public void WriteMessageCategoryPriorityAndExceptionOverload() {
            LogUtility target = new LogUtility(title);
            target.Write(message, category, priority, GetException());

            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");

            Assert.AreEqual(1, MockTraceListener.LastEntry.Categories.Count);
            Assert.IsTrue(MockTraceListener.LastEntry.Categories.Contains(category));
            Assert.AreEqual(priority, MockTraceListener.LastEntry.Priority, "priority");
            Assert.AreEqual(typeof(ArgumentNullException).AssemblyQualifiedName,
                            ExceptionUtilityFixture.GetExceptionQualifiedName(MockTraceListener.LastEntry), "hash count");
            Assert.AreEqual(TraceEventType.Error, MockTraceListener.LastEntry.Severity, "severity");
        } 

    }
}

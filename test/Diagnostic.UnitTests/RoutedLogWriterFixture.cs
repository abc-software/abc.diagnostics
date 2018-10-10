#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

using Diagnostic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diagnostic.UnitTests;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Diagnostic.UnitTests {
    [TestClass()]
    public class RoutedLogWriterFixture {
        private const string title = "bar";
        private const string message = "testing.... message and category";
        private const string category = "MockCategoryOne";
        private static string[] categories = new string[] { category };
        private const int priority = 420;
        private const int eventId = 421;
        private const TraceEventType severity = TraceEventType.Information;
        private readonly Guid activityId = new Guid("5D352811-28DF-4baf-BD18-5997FE1260A3");

        [TestInitialize()]
        public void MyTestInitialize() {
            AppDomain.CurrentDomain.SetData("APPBASE", Environment.CurrentDirectory);
            RouterLogWriterMock0.Reset();
            RouterLogWriterMock1.Reset();
            RouterLogWriterMock2.Reset();
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            RouterLogWriterMock0.Reset();
            RouterLogWriterMock1.Reset();
            RouterLogWriterMock2.Reset();
        }

        [TestMethod()]
        public void TestIsLoggingEnable() {
            {
                RoutedLogWriter logWriter = new RoutedLogWriter();
                Assert.IsFalse(logWriter.IsLoggingEnabled);
            }

            {
                RoutedLogWriter logWriter = new RoutedLogWriter();
                var filters = new Configuration.FilterElementCollection() {
                    new Configuration.FilterElement() {
                        Categories = new System.Configuration.CommaDelimitedStringCollection() { "*" },
                        TypeName = "Diagnostic.UnitTests.RouterLogWriterMock0, Diagnostic.UnitTests"
                    }
                };

                logWriter.SetFilters(filters);
                Assert.IsTrue(logWriter.IsLoggingEnabled);
            }
        }

        [TestMethod()]
        public void TestIsTracingEnabled() {
            {
                RoutedLogWriter logWriter = new RoutedLogWriter();
                Assert.IsFalse(logWriter.IsTracingEnabled);
            }

            {
                RoutedLogWriter logWriter = new RoutedLogWriter();
                var filters = new Configuration.FilterElementCollection() {
                    new Configuration.FilterElement() {
                        Categories = new System.Configuration.CommaDelimitedStringCollection() { "*" },
                        TypeName = "Diagnostic.UnitTests.RouterLogWriterMock0, Diagnostic.UnitTests"
                    }
                };

                logWriter.SetFilters(filters);
                Assert.IsTrue(logWriter.IsTracingEnabled);
            }
        }

        [TestMethod()]
        public void TestWrite() {
            RoutedLogWriter logWriter = new RoutedLogWriter();
            var filters = new Configuration.FilterElementCollection() {
                    new Configuration.FilterElement() {
                        Categories = new System.Configuration.CommaDelimitedStringCollection() { "MockCategoryOne" },
                        TypeName = "Diagnostic.UnitTests.RouterLogWriterMock0, Diagnostic.UnitTests"
                    },
                    new Configuration.FilterElement() {
                        Categories = new System.Configuration.CommaDelimitedStringCollection() { "*" },
                        TypeName = "Diagnostic.UnitTests.RouterLogWriterMock1, Diagnostic.UnitTests"
                    },
                    new Configuration.FilterElement() {
                        Categories = new System.Configuration.CommaDelimitedStringCollection() { "B" },
                        TypeName = "Diagnostic.UnitTests.RouterLogWriterMock2, Diagnostic.UnitTests"
                    }
                };

            logWriter.SetFilters(filters);

            logWriter.Write("bar0", categories, priority, eventId, severity, title, null, null, activityId, null);
            logWriter.Write("bar1", new string[] { "A" }, priority, eventId, severity, title, null, null, activityId, null);
            logWriter.Write("bar2", new string[] { "B", "C" }, priority, eventId, severity, title, null, null, activityId, null);
            logWriter.Write("bar3", (string)null, priority, eventId, severity, title, null, null, activityId, null);

            Assert.AreEqual(1, RouterLogWriterMock0.Entries.Count);
            Assert.AreEqual(1, RouterLogWriterMock0.Entries[0].Categories.Count);
            Assert.AreEqual("MockCategoryOne", RouterLogWriterMock0.Entries[0].Categories.First());

            Assert.AreEqual(3, RouterLogWriterMock1.Entries.Count);
            Assert.AreEqual(1, RouterLogWriterMock1.Entries[0].Categories.Count);
            Assert.AreEqual("A", RouterLogWriterMock1.Entries[0].Categories.First());
            Assert.AreEqual(1, RouterLogWriterMock1.Entries[1].Categories.Count);
            Assert.AreEqual("C", RouterLogWriterMock1.Entries[1].Categories.First());
            Assert.AreEqual(1, RouterLogWriterMock1.Entries[2].Categories.Count);
            Assert.AreEqual(null, RouterLogWriterMock1.Entries[2].Categories.First());

            Assert.AreEqual(1, RouterLogWriterMock2.Entries.Count);
            Assert.AreEqual(1, RouterLogWriterMock2.Entries[0].Categories.Count);
            Assert.AreEqual("B", RouterLogWriterMock2.Entries[0].Categories.First());
        }


        [TestMethod()]
        public void TestWrite2() {
            RoutedLogWriter logWriter = new RoutedLogWriter();
            var filters = new Configuration.FilterElementCollection() {
                    new Configuration.FilterElement() {
                        Categories = new System.Configuration.CommaDelimitedStringCollection() { "MockCategoryOne" },
                        TypeName = "Diagnostic.UnitTests.RouterLogWriterMock0, Diagnostic.UnitTests"
                    },
                    new Configuration.FilterElement() {
                        Categories = new System.Configuration.CommaDelimitedStringCollection() { "B", "*" },
                        TypeName = "Diagnostic.UnitTests.RouterLogWriterMock1, Diagnostic.UnitTests"
                    },
                    new Configuration.FilterElement() {
                        Categories = new System.Configuration.CommaDelimitedStringCollection() { "MockCategoryOne", "B" },
                        TypeName = "Diagnostic.UnitTests.RouterLogWriterMock2, Diagnostic.UnitTests"
                    }
                };

            logWriter.SetFilters(filters);

            logWriter.Write("bar0", categories, priority, eventId, severity, title, null, null, activityId, null);
            logWriter.Write("bar1", new string[] { "A" }, priority, eventId, severity, title, null, null, activityId, null);
            logWriter.Write("bar2", new string[] { "B", "C" }, priority, eventId, severity, title, null, null, activityId, null);
            logWriter.Write("bar3", (string)null, priority, eventId, severity, title, null, null, activityId, null);

            {
                var e = RouterLogWriterMock0.Entries;
                Assert.AreEqual(1, e.Count);
                Assert.AreEqual(1, e[0].Categories.Count);
                Assert.AreEqual("MockCategoryOne", e[0].Categories.First());
            }

            {
                var e = RouterLogWriterMock1.Entries;
                Assert.AreEqual(4, e.Count);
                Assert.AreEqual("A", e[0].Categories.First());
                Assert.AreEqual("B", e[1].Categories.First());
                Assert.AreEqual("C", e[2].Categories.First());
                Assert.AreEqual(null, e[3].Categories.First());
            }

            {
                var e = RouterLogWriterMock2.Entries;
                Assert.AreEqual(2, e.Count);
                Assert.AreEqual("MockCategoryOne", e[0].Categories.First());
                Assert.AreEqual("B", e[1].Categories.First());
            }
        }
    }

    public class RouterLogWriterMock0 : ILogWriter {
        static List<LogEntry> entries = new List<LogEntry>();

        public bool IsLoggingEnabled => true;
        public bool IsTracingEnabled => true;

        public void Flush() {
            // do nothing
        }

        public void Write(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties, Exception exception, Guid activityId, Guid? relatedActivityId) {
            LogEntry log = new LogEntry();
            log.Message = message;
            log.Categories = categories;
            log.Priority = priority;
            log.EventId = eventId;
            log.Severity = severity;
            log.Title = title;
            log.ExtendedProperties = properties;
            log.ActivityId = activityId;
            log.RelatedActivityId = relatedActivityId;

            entries.Add(log);
        }

        public static IList<LogEntry> Entries => entries;

        public static void Reset() {
            entries.Clear();
        }
    }

    public class RouterLogWriterMock1 : ILogWriter {
        static List<LogEntry> entries = new List<LogEntry>();

        public bool IsLoggingEnabled => true;
        public bool IsTracingEnabled => true;

        public void Flush() {
            // do nothing
        }

        public void Write(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties, Exception exception, Guid activityId, Guid? relatedActivityId) {
            LogEntry log = new LogEntry();
            log.Message = message;
            log.Categories = categories;
            log.Priority = priority;
            log.EventId = eventId;
            log.Severity = severity;
            log.Title = title;
            log.ExtendedProperties = properties;
            log.ActivityId = activityId;
            log.RelatedActivityId = relatedActivityId;

            entries.Add(log);
        }

        public static IList<LogEntry> Entries => entries;

        public static void Reset() {
            entries.Clear();
        }
    }

    public class RouterLogWriterMock2 : ILogWriter {
        static List<LogEntry> entries = new List<LogEntry>();

        public bool IsLoggingEnabled => true;
        public bool IsTracingEnabled => true;

        public void Flush() {
            // do nothing
        }

        public void Write(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties, Exception exception, Guid activityId, Guid? relatedActivityId) {
            LogEntry log = new LogEntry();
            log.Message = message;
            log.Categories = categories;
            log.Priority = priority;
            log.EventId = eventId;
            log.Severity = severity;
            log.Title = title;
            log.ExtendedProperties = properties;
            log.ActivityId = activityId;
            log.RelatedActivityId = relatedActivityId;

            entries.Add(log);
        }

        public static IList<LogEntry> Entries => entries;

        public static void Reset() {
            entries.Clear();
        }
    }
}
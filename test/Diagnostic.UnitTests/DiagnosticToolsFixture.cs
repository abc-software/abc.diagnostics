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
using System.IO;
#endif 

namespace Diagnostic.UnitTests {
    [TestClass()]
    public class DiagnosticToolsFixture {
        const string title = "bar";
        const string message = "testing.... message and category";
        const string category = "MockCategoryOne";
        static string[] categories = new string[] { category };
        const int priority = 420;
        const int eventId = 421;
        const TraceEventType severity = TraceEventType.Information;

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
        ///A test for Write with ActivityId
        ///</summary>
        [TestMethod()]
        public void WriteMessageActivityIdOverload() {
            DiagnosticTools.LogUtil.Write(message, category, priority, eventId, severity);

            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(message, MockTraceListener.LastEntry.Message, "message");
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "title");
        }
    }
}

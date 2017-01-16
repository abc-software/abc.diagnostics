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
#endif 

using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Xml;
using System.IO;
using System.Xml.XPath;

namespace Diagnostic.UnitTests {
    /// <summary>
    /// Configuration fixture.
    /// </summary>
    [TestClass()]
    public class DefaultLogWriterFixture {
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
            MockTraceListener.Reset();
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            MockTraceListener.Reset();  
        }

        [TestMethod()]
        public void TestIsLoggingEnable() {
            DefaultLogWriter logWriter = new DefaultLogWriter();
            Assert.IsTrue(logWriter.IsLoggingEnabled);
        }

        [TestMethod()]
        public void TestIsTracingEnabled() {
            DefaultLogWriter logWriter = new DefaultLogWriter();
            Assert.IsFalse(logWriter.IsTracingEnabled);

            //var list = new TraceSource("Trace").Listeners;
            //list.Add(new ConsoleTraceListener());
            //Assert.IsTrue(logWriter.IsTracingEnabled);
        }

        [TestMethod()]
        public void TestWrite() {
            DefaultLogWriter logWriter = new DefaultLogWriter();
            logWriter.Write(message, categories, priority, eventId, severity, title, null, null, activityId, null);

            Assert.IsTrue(MockTraceListener.Instances[0].TracedData is XPathNavigator);

            XPathNavigator nav = (XPathNavigator)MockTraceListener.Instances[0].TracedData;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);
            nsmgr.AddNamespace("x", "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord");

            Assert.AreEqual(message, nav.SelectSingleNode("x:TraceRecord/x:Description", nsmgr).Value, "message");
            Assert.AreEqual(priority.ToString(), nav.SelectSingleNode("x:TraceRecord/x:Priority", nsmgr).Value, "priority");
            Assert.AreEqual(severity.ToString(), nav.SelectSingleNode("x:TraceRecord/@Severity", nsmgr).Value, "severity");
            Assert.AreEqual(title, nav.SelectSingleNode("x:TraceRecord/x:Source", nsmgr).Value, "sourcename");
        }

        [TestMethod()]
        public void TestWriteWithProperties() {
            DefaultLogWriter logWriter = new DefaultLogWriter();
            IDictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("key", "value");
            properties.Add("key2", "value2");

            logWriter.Write(message, categories, priority, eventId, severity, title, properties, null, activityId, null);

            Assert.IsTrue(MockTraceListener.Instances[0].TracedData is XPathNavigator);

            XPathNavigator nav = (XPathNavigator)MockTraceListener.Instances[0].TracedData;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);
            nsmgr.AddNamespace("x", "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord");
            nsmgr.AddNamespace("d", "http://schemas.microsoft.com/2006/08/ServiceModel/DictionaryTraceRecord");

            Assert.AreEqual("value", nav.SelectSingleNode("x:TraceRecord/d:ExtendedData/d:key", nsmgr).Value, "properties");
            Assert.AreEqual("value2", nav.SelectSingleNode("x:TraceRecord/d:ExtendedData/d:key2", nsmgr).Value, "properties");
        }

        [TestMethod()]
        public void TestWriteWithException() {
            DefaultLogWriter logWriter = new DefaultLogWriter();
            Exception exception = new ArgumentNullException();
            exception.Data.Add("Exkey", "ExData");  

            logWriter.Write(message, categories, priority, eventId, severity, title, null, exception, activityId, null);

            XPathNavigator nav = (XPathNavigator)MockTraceListener.Instances[0].TracedData;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);
            nsmgr.AddNamespace("x", "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord");

            Assert.AreEqual(exception.GetType().AssemblyQualifiedName, nav.SelectSingleNode("x:TraceRecord/x:Exception/x:ExceptionType", nsmgr).Value, "properties");
        }

        [TestMethod()]
        public void TestWriteTransfer() {
            DefaultLogWriter logWriter = new DefaultLogWriter();
            Guid realatedActivityId = new Guid("{7846111E-1EC8-4664-BECE-BA5572BCE3F2}");

            logWriter.Write(message, categories, priority, eventId, TraceEventType.Transfer, title, null, null, activityId, realatedActivityId);

            Assert.AreEqual(MockTraceListener.TransferGuids.Count, 1);
            Assert.AreEqual(MockTraceListener.TransferGuids[0], realatedActivityId);
        }

        [TestMethod()]
        public void TestWriteWithXmlValues() {
            DefaultLogWriter logWriter = new DefaultLogWriter();
            IDictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add(AddLTGT("key"), AddLTGT("value"));

            logWriter.Write(AddLTGT(message), categories, priority, eventId, severity, AddLTGT(title), properties, null, activityId, null);

            Assert.IsTrue(MockTraceListener.Instances[0].TracedData is XPathNavigator);
        }


        private string AddLTGT(string s) {
            return "<" + s + ">";
        }
    }
}

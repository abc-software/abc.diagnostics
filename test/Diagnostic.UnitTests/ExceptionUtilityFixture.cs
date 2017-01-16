using Diagnostic;
using System;
using System.Diagnostics;
using System.Xml.XPath;
using System.Xml;
using Microsoft.Practices.EnterpriseLibrary.Logging;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Diagnostic.UnitTests
{
    /// <summary>
    ///This is a test class for ExceptionUtilityTest and is intended
    ///to contain all ExceptionUtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExceptionUtilityFixture {
        private const string title = "bar";
        private static Exception exception = new ArgumentNullException();

        [TestInitialize()]
        public void MyTestInitialize() {
            AppDomain.CurrentDomain.SetData("APPBASE", Environment.CurrentDirectory);
            MockTraceListener.Reset();
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            ExceptionUtility.ClearActivityId(); 
            MockTraceListener.Reset();
        }

        /// <summary>
        ///A test for IsFatal
        ///</summary>
        [TestMethod()]
        public void IsFatalTest() {
            bool actual;

            actual = ExceptionUtility.IsFatal(null);
            Assert.AreEqual(false, actual);

            actual = ExceptionUtility.IsFatal(new ArgumentException());
            Assert.AreEqual(false, actual);

            actual = ExceptionUtility.IsFatal(new FatalException());
            Assert.AreEqual(true, actual);

            actual = ExceptionUtility.IsFatal(new OutOfMemoryException());
            Assert.AreEqual(true, actual);

            actual = ExceptionUtility.IsFatal(new AccessViolationException());
            Assert.AreEqual(true, actual);

            actual = ExceptionUtility.IsFatal(new System.Runtime.InteropServices.SEHException());
            Assert.AreEqual(true, actual);

            actual = ExceptionUtility.IsFatal(new TypeInitializationException("", new ArgumentException()));
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for IsInfrastructureException
        ///</summary>
        [TestMethod()]
        public void IsInfrastructureExceptionTest() {
            bool actual;

            actual = ExceptionUtility.IsInfrastructure(null);
            Assert.AreEqual(false, actual);

            actual = ExceptionUtility.IsInfrastructure(new AppDomainUnloadedException());
            Assert.AreEqual(true, actual);
        }

        [TestMethod()]
        public void ThrowHelperTest() {
            Guid activityId = Guid.NewGuid();
            ExceptionUtility.UseActivityId(activityId);

            TraceEventType serverity = TraceEventType.Verbose;
            ExceptionUtility target = new ExceptionUtility();

            Exception actual = target.ThrowHelper(exception, serverity);
            Assert.AreEqual(exception, actual);
            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(activityId, MockTraceListener.LastEntry.ActivityId, "activityId");
            Assert.AreEqual(serverity, MockTraceListener.LastEntry.Severity);

            Assert.AreEqual(exception.GetType().AssemblyQualifiedName,
                            GetExceptionQualifiedName(MockTraceListener.LastEntry), "hash count");
        }

        /// <summary>
        ///A test for ThrowHelperError
        ///</summary>
        [TestMethod()]
        public void ThrowHelperErrorTest() {
            Guid activityId = Guid.NewGuid();
            ExceptionUtility.UseActivityId(activityId);

            ExceptionUtility target = new ExceptionUtility();

            Exception actual = target.ThrowHelperError(exception);
            Assert.AreEqual(exception, actual);
            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(activityId, MockTraceListener.LastEntry.ActivityId, "activityId");
            Assert.AreEqual(TraceEventType.Error, MockTraceListener.LastEntry.Severity);

            Assert.AreEqual(exception.GetType().AssemblyQualifiedName,
                            GetExceptionQualifiedName(MockTraceListener.LastEntry), "hash count");
        }

        /// <summary>
        ///A test for ThrowHelperWarning
        ///</summary>
        [TestMethod()]
        public void ThrowHelperWarningTest() {
            Guid activityId = Guid.NewGuid();
            ExceptionUtility.UseActivityId(activityId);

            ExceptionUtility target = new ExceptionUtility();

            Exception actual = target.ThrowHelperWarning(exception);
            Assert.AreEqual(exception, actual);
            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(activityId, MockTraceListener.LastEntry.ActivityId, "activityId");
            Assert.AreEqual(TraceEventType.Warning, MockTraceListener.LastEntry.Severity);

            Assert.AreEqual(exception.GetType().AssemblyQualifiedName,
                            GetExceptionQualifiedName(MockTraceListener.LastEntry), "hash count");
        }

        /// <summary>
        ///A test for ThrowHelperCritical
        ///</summary>
        [TestMethod()]
        public void ThrowHelperCriticalTest() {
            Guid activityId = Guid.NewGuid();
            ExceptionUtility.UseActivityId(activityId);

            ExceptionUtility target = new ExceptionUtility();

            Exception actual = target.ThrowHelperCritical(exception);
            Assert.AreEqual(exception, actual);
            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(activityId, MockTraceListener.LastEntry.ActivityId, "activityId");
            Assert.AreEqual(TraceEventType.Critical, MockTraceListener.LastEntry.Severity);

            Assert.AreEqual(exception.GetType().AssemblyQualifiedName,
                            GetExceptionQualifiedName(MockTraceListener.LastEntry), "hash count");
        }

        /// <summary>
        ///A test for ThrowHelperFatal
        ///</summary>
        [TestMethod()]
        public void ThrowHelperFatalTest() {
            Guid activityId = Guid.NewGuid();
            ExceptionUtility.UseActivityId(activityId);

            ExceptionUtility target = new ExceptionUtility();

            Exception actual = target.ThrowHelperFatal("", exception);
            Assert.IsTrue(actual is FatalException);
            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(activityId, MockTraceListener.LastEntry.ActivityId, "activityId");
            Assert.AreEqual(TraceEventType.Error, MockTraceListener.LastEntry.Severity);

            Assert.AreEqual(typeof(FatalException).AssemblyQualifiedName,
                            GetExceptionQualifiedName(MockTraceListener.LastEntry), "hash count");
        }

        /// <summary>
        ///A test for ThrowHelperArgumentNull
        ///</summary>
        [TestMethod()]
        public void ThrowHelperArgumentNullTest() {
            ExceptionUtility target = new ExceptionUtility();
            Exception actual = target.ThrowHelperArgumentNull("param");

            Assert.IsTrue(actual is ArgumentNullException);
        }

        /// <summary>
        ///A test for ThrowHelperArgumentNull
        ///</summary>
        [TestMethod()]
        public void ThrowHelperArgumentNullTest1() {
            ExceptionUtility target = new ExceptionUtility(); 
            Exception actual = target.ThrowHelperArgumentNull("param" , "message");

            Assert.IsTrue(actual is ArgumentNullException);
        }

        /// <summary>
        ///A test for ThrowHelperArgument
        ///</summary>
        [TestMethod()]
        public void ThrowHelperArgumentTest() {
            ExceptionUtility target = new ExceptionUtility();
            Exception actual = target.ThrowHelperArgument("param", "message");

            Assert.IsTrue(actual is ArgumentException);
        }

        /// <summary>
        ///A test for ThrowHelperArgument
        ///</summary>
        [TestMethod()]
        public void ThrowHelperArgumentTest1() {
            ExceptionUtility target = new ExceptionUtility();
            Exception actual = target.ThrowHelperArgument("message");

            Assert.IsTrue(actual is ArgumentException);
        }

        /// <summary>
        ///A test for TraceHandledException
        ///</summary>
        [TestMethod()]
        public void TraceHandledExceptionTest() {
            Guid activityId = Guid.NewGuid();
            ExceptionUtility.UseActivityId(activityId);
            TraceEventType serverity = TraceEventType.Information;  

            ExceptionUtility target = new ExceptionUtility();

            target.TraceHandledException(exception, serverity);
            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
            Assert.AreEqual(activityId, MockTraceListener.LastEntry.ActivityId, "activityId");
            Assert.AreEqual(serverity, MockTraceListener.LastEntry.Severity);

            Assert.AreEqual(exception.GetType().AssemblyQualifiedName,
                            GetExceptionQualifiedName(MockTraceListener.LastEntry), "hash count");
        }

        [TestMethod()]
        public void WriteExceptionAsDictionary() {
            ConfigurationFixture.ChangeConfigAttribute("type", typeof(EntrLib40LogWriter).AssemblyQualifiedName);
            ConfigurationFixture.ChangeConfigAttribute("initializeData", "exceptionAsDictionary");
            // HACK: reset writer value
            System.Reflection.FieldInfo field = typeof(LogUtility).GetField(
                "writer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field.SetValue(null, null);
            
            try {
                Guid activityId = Guid.NewGuid();
                ExceptionUtility.UseActivityId(activityId);

                TraceEventType serverity = TraceEventType.Verbose;
                ExceptionUtility target = new ExceptionUtility();

                Exception actual = target.ThrowHelper(exception, serverity);
                Assert.AreEqual(exception, actual);
                Assert.IsNotNull(MockTraceListener.LastEntry);
//                Assert.AreEqual(title, MockTraceListener.LastEntry.Title, "sourcename");
                Assert.AreEqual(activityId, MockTraceListener.LastEntry.ActivityId, "activityId");
                Assert.AreEqual(serverity, MockTraceListener.LastEntry.Severity);

                Assert.AreEqual(
                    exception.GetType().AssemblyQualifiedName,
                    MockTraceListener.LastEntry.ExtendedProperties["ExceptionType"],
                    "hash count");
            }
            finally {
                ConfigurationFixture.ChangeConfigAttribute("initializeData", string.Empty);
                ConfigurationFixture.ChangeConfigAttribute("type", typeof(TestLogWriterProxy).AssemblyQualifiedName);

                // HACK: reset writer value
                field.SetValue(null, null); 
            }
        }

        [TestMethod()]
        public void ActivityIdTest() {
            Guid savedActivityId = Guid.NewGuid();  
            Trace.CorrelationManager.ActivityId = savedActivityId;

            LogUtility log = new LogUtility(title);
            ExceptionUtility target = new ExceptionUtility();

            //Swith on activityId
            Guid activityId = Guid.NewGuid();
            ExceptionUtility.UseActivityId(activityId);

            target.TraceHandledException(exception, TraceEventType.Information);
            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(activityId, MockTraceListener.LastEntry.ActivityId, "activityId");

            // restored activityId
            MockTraceListener.Reset();
            log.Write("message", "General"); 
            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(savedActivityId, MockTraceListener.LastEntry.ActivityId, "activityId");

            // swith off ActivityId
            MockTraceListener.Reset();
            ExceptionUtility.ClearActivityId(); 
            target.TraceHandledException(exception, TraceEventType.Information);
            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(savedActivityId, MockTraceListener.LastEntry.ActivityId, "activityId");

            // restored activityId
            MockTraceListener.Reset();
            log.Write("message", "General");
            Assert.IsNotNull(MockTraceListener.LastEntry);
            Assert.AreEqual(savedActivityId, MockTraceListener.LastEntry.ActivityId, "activityId");
        }

        internal static string GetExceptionQualifiedName(LogEntry logEntry) {
            string qualifiedType = null;

            XmlLogEntry xmlLogEntry = logEntry as XmlLogEntry;
            if (xmlLogEntry != null && xmlLogEntry.Xml != null) {
                XPathNavigator nav = xmlLogEntry.Xml;
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);
                nsmgr.AddNamespace("x", "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord");

                nav = nav.SelectSingleNode("x:TraceRecord/x:Exception/x:ExceptionType", nsmgr);
                if (nav != null) {
                    qualifiedType = nav.Value; 
                }
            }

            return qualifiedType;
        }
    }
}

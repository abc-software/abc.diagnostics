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
using System.Configuration;

namespace Diagnostic.UnitTests {
    /// <summary>
    /// Configuration fixture.
    /// </summary>
    [TestClass()]
    public class ConfigurationFixture {
        private static string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

        [TestInitialize()]
        public void MyTestInitialize() {
            ChangeConfigAttribute("type", string.Empty);
            ChangeConfigAttribute("initializeData", string.Empty);
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            ChangeConfigAttribute("type", typeof(TestLogWriterProxy).AssemblyQualifiedName);
            ChangeConfigAttribute("initializeData", string.Empty);
        }

        [TestMethod()]
        public void GetDefaultConfiguration() {
            Configuration.DiagnosticSettings settings = Configuration.DiagnosticSettings.Current;
            Assert.AreEqual(settings.TypeName.Length, 0);
            Assert.AreEqual(settings.InitData.Length, 0);
            
            object writer = settings.CreateLogWriter();
            Assert.IsTrue(typeof(ILogWriter).IsAssignableFrom(writer.GetType()));
        }

        [TestMethod()]
        public void GetTypedConfiguration() {
            string typeName = typeof(TestLogWriterProxy).AssemblyQualifiedName;
            ChangeConfigAttribute("type", typeName); 
            Assert.AreEqual(typeName, Configuration.DiagnosticSettings.Current.TypeName);

            Configuration.DiagnosticSettings settings = Configuration.DiagnosticSettings.Current;
            object writer = settings.CreateLogWriter();
            Assert.AreEqual(writer.GetType(), typeof(TestLogWriterProxy));
        }

        /// <summary>
        /// Not ILogWriter
        /// </summary>
        [TestMethod()]
        public void GetTypedConfigurationNotLogger() {
            string typeName = typeof(TraceListener).AssemblyQualifiedName;
            ChangeConfigAttribute("type", typeName);
            Assert.AreEqual(typeName, Configuration.DiagnosticSettings.Current.TypeName);

            Configuration.DiagnosticSettings settings = Configuration.DiagnosticSettings.Current;
            Assert.Throws<ConfigurationErrorsException>(() => settings.CreateLogWriter());
        }

        /// <summary>
        /// No founded type
        /// </summary>
        [TestMethod()]
        public void GetTypedConfigurationInvalidType() {
            string typeName = "MyType, MyLogWriter";
            ChangeConfigAttribute("type", typeName);

            Configuration.DiagnosticSettings settings = Configuration.DiagnosticSettings.Current;
            Assert.Throws<ConfigurationErrorsException>(() => settings.CreateLogWriter());
        }

        /// <summary>
        /// Is ILogWriter no public constructor
        /// </summary>
        [TestMethod()]
        public void GetTypedConfigurationNoDefaultConstructor() {
            string typeName = typeof(NoPublicConstructor).AssemblyQualifiedName;
            ChangeConfigAttribute("type", typeName);

            Configuration.DiagnosticSettings settings = Configuration.DiagnosticSettings.Current;
            Assert.Throws<ConfigurationErrorsException>(() => settings.CreateLogWriter());
        }

        [TestMethod()]
        public void GetInitializeConfiguration() {
            string typeName = typeof(TestLogWriterProxy).AssemblyQualifiedName;
            ChangeConfigAttribute("type", typeName);
            Assert.AreEqual(typeName, Configuration.DiagnosticSettings.Current.TypeName);
            
            string initializeData = "INIT";
            ChangeConfigAttribute("initializeData", initializeData);
            Assert.AreEqual(initializeData, Configuration.DiagnosticSettings.Current.InitData);

            Configuration.DiagnosticSettings settings = Configuration.DiagnosticSettings.Current;
            object writer = settings.CreateLogWriter();
            Assert.AreEqual(writer.GetType(), typeof(TestLogWriterProxy));
            Assert.AreEqual((writer as TestLogWriterProxy).InitData, initializeData);
            Assert.AreEqual((writer as TestLogWriterProxy).SourceName, "bar");
        }

        [TestMethod()]
        public void GetInitializeConfigurationNoConstructor() {
            string typeName = typeof(NoPublicConstructor).AssemblyQualifiedName;
            ChangeConfigAttribute("type", typeName);

            string initializeData = "INIT";
            ChangeConfigAttribute("initializeData", initializeData);
            Assert.AreEqual(initializeData, Configuration.DiagnosticSettings.Current.InitData);

            Configuration.DiagnosticSettings settings = Configuration.DiagnosticSettings.Current;
            Assert.Throws<ConfigurationErrorsException>(() => settings.CreateLogWriter());
        }

        /// <summary>
        ///A test for Write with BadListener
        ///</summary>
        //[TestMethod()]
        //[ExpectedException(typeof(ConfigurationErrorsException))]
        //public void WriteToBadListener() {
        //    string typeName = typeof(DefaultLogWriter).AssemblyQualifiedName;
        //    ChangeConfigAttribute("type", typeName);

        //    DiagnosticTools.LogUtil.Write("message", "BadListener", -1, 1, TraceEventType.Verbose);
        //}

        internal static void ChangeConfigAttribute(string attributeName, string attributeValue) {
            // Get the configuration file.
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);
            XmlNodeList nodeList = doc.GetElementsByTagName(Configuration.DiagnosticSettings.DiagnosticSettingsSectionName);
            XmlElement element = (nodeList[0] as XmlElement);
            if (element.GetAttribute(attributeName) != attributeValue) {
                element.SetAttribute(attributeName, attributeValue);
                doc.Save(configFile);

                Configuration.DiagnosticSettings.Reload();
            }
        }

        private class NoPublicConstructor : ILogWriter {
            public NoPublicConstructor(TraceListener listener) {
            }

            public bool IsLoggingEnabled {
                get { throw new NotImplementedException(); }
            }

            public bool IsTracingEnabled {
                get { throw new NotImplementedException(); }
            }

            public void Write(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties, Exception exception, Guid activityId, Guid? relatedActivityId) {
                throw new NotImplementedException();
            }

            public void Flush() {
                throw new NotImplementedException();
            }
        }
    }
}

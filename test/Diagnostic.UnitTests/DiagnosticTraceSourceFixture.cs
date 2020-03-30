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
    public class DiagnosticTraceSourceFixture {
        
        [TestInitialize()]
        public void MyTestInitialize() {
            AppDomain.CurrentDomain.SetData("APPBASE", Environment.CurrentDirectory);
        }

        [TestCleanup()]
        public void MyTestCleanup() {
        }

        [TestMethod()]
        public void WriteMessageActivityIdOverload() {
            var traceSource = new DiagnosticTraceSource("DignosticTraceSource");

            Assert.AreEqual(2048, traceSource.MaxContentLogSize);
            Assert.AreEqual(true, traceSource.UseProtocolOnlySettings());
            Assert.AreEqual(true, traceSource.PropagateActivity);
        }
    }
}

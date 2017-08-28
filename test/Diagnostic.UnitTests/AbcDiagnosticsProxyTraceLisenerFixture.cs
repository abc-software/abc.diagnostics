using Microsoft.Practices.EnterpriseLibrary.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Diagnostic.UnitTests {
    [TestFixture]
    public class AbcDiagnosticsProxyTraceListenerFixture {
        public const string Xml = @"<data attr=""MyValue""/>";
        private AbcDiagnosticsProxyTraceListener proxy;
        ILogWriter writer;

        [SetUp]
        public void SetUp() {
            proxy = new AbcDiagnosticsProxyTraceListener();
            writer = LogUtility.Writer;
            MockTraceListener.Reset();

            LogUtility.Reset();
            LogUtility.SetLogWriter(new EntrLib40LogWriter());
        }

        [TearDown]
        public void Teardown() {
            proxy.Dispose();

            Logger.Reset();

            LogUtility.Reset();
            LogUtility.SetLogWriter(writer);
        }

        [Test]
        public void ProxyXPathNavigatorData() {
            XPathNavigator navigator = new XPathDocument(new StringReader(Xml)).CreateNavigator();
            proxy.TraceData(new TraceEventCache(), "System.ServiceModel", TraceEventType.Error, 1, navigator);

            Assert.IsTrue(MockTraceListener.LastEntry is XmlLogEntry);

            XmlLogEntry lastEntry = MockTraceListener.LastEntry as XmlLogEntry;
            Assert.AreEqual(lastEntry.Xml, navigator);
        }

        [Test]
        public void ProxyStringData() {
            string data = "someData";
            proxy.TraceData(new TraceEventCache(), "System.ServiceModel", TraceEventType.Error, 1, data);

            LogEntry lastEntry = MockTraceListener.LastEntry;
            Assert.IsNotNull(lastEntry);
            Assert.AreEqual(lastEntry.Message, data);
        }

        [Test]
        public void ProxyLogEntryData() {
            var entry = new Dictionary<string, object>() { { "XPathDocument", new XPathDocument(new StringReader(Xml)).CreateNavigator() } };

            int eventId = 1;
            proxy.TraceData(new TraceEventCache(), "System.ServiceModel", TraceEventType.Error, eventId, entry);

            LogEntry lastEntry = MockTraceListener.LastEntry;
            Assert.IsNotNull(lastEntry);
            // CommonUtil.AssertLogEntries(lastEntry, entry);
        }

        [Test]
        public void CanGetInstanceFromConfigurationObject() {
            TraceSource traceSource = new TraceSource("abcproxy");
            AbcDiagnosticsProxyTraceListener listener = traceSource.Listeners["abcproxy"] as AbcDiagnosticsProxyTraceListener;

            Assert.IsNotNull(listener);
            Assert.AreEqual(0, listener.CategoriesXPathQueries.Count);
            Assert.AreEqual(0, listener.NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local).Count);
        }

        [Test]
        public void CanGetInstanceWithSinglePathAndSingleNamespaceFromConfigurationObject() {
            TraceSource traceSource = new TraceSource("abcproxy");
            AbcDiagnosticsProxyTraceListener listener = traceSource.Listeners["abcproxywithxpath"] as AbcDiagnosticsProxyTraceListener;

            Assert.IsNotNull(listener);
            Assert.AreEqual(1, listener.CategoriesXPathQueries.Count);
            Assert.AreEqual("//MessageLogTraceRecord/@Source", listener.CategoriesXPathQueries[0]);
            Assert.AreEqual(1, listener.NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local).Count);
            Assert.AreEqual("urn:test", listener.NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local)["pre"]);
        }

        [Test]
        public void CanGetInstanceWithMultiplePathsFromConfigurationObject() {
            TraceSource traceSource = new TraceSource("abcproxy");
            AbcDiagnosticsProxyTraceListener listener = traceSource.Listeners["abcproxywithmultiplexpaths"] as AbcDiagnosticsProxyTraceListener;

            Assert.IsNotNull(listener);
            Assert.AreEqual(2, listener.CategoriesXPathQueries.Count);
            Assert.AreEqual("//MessageLogTraceRecord/@Source", listener.CategoriesXPathQueries[0]);
            Assert.AreEqual("//MessageLogTraceRecord/@Source2", listener.CategoriesXPathQueries[1]);
            Assert.AreEqual(2, listener.NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local).Count);
            Assert.AreEqual("urn:test", listener.NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local)["pre"]);
            Assert.AreEqual("urn:test2", listener.NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local)["pre2"]);
        }

        [Test]
        public void SplittingEmptyXPathsStringReturnsEmptyList() {
            string xpathsStrings = "";

            IList<string> xpaths = AbcDiagnosticsProxyTraceListener.SplitXPathQueriesString(xpathsStrings);

            Assert.AreEqual(0, xpaths.Count);
        }

        [Test]
        public void SplittingSinglePathXPathsStringReturnsSingleElementList() {
            string xpathsStrings = "single path";

            IList<string> xpaths = AbcDiagnosticsProxyTraceListener.SplitXPathQueriesString(xpathsStrings);

            Assert.AreEqual(1, xpaths.Count);
            Assert.AreEqual("single path", xpaths[0]);
        }

        [Test]
        public void SplittingEscapedSemicolonReturnsSingleElementList() {
            string xpathsStrings = @"single\;path";

            IList<string> xpaths = AbcDiagnosticsProxyTraceListener.SplitXPathQueriesString(xpathsStrings);

            Assert.AreEqual(1, xpaths.Count);
            Assert.AreEqual("single;path", xpaths[0]);
        }

        [Test]
        public void SplittingWithSingleSemicolonReturnsTwoElementsList() {
            string xpathsStrings = @"multiple;paths";

            IList<string> xpaths = AbcDiagnosticsProxyTraceListener.SplitXPathQueriesString(xpathsStrings);

            Assert.AreEqual(2, xpaths.Count);
            Assert.AreEqual("multiple", xpaths[0]);
            Assert.AreEqual("paths", xpaths[1]);
        }

        [Test]
        public void SplittingWithTwoSemicolonsReturnsThreeElementsList() {
            string xpathsStrings = @"three;multiple;paths";

            IList<string> xpaths = AbcDiagnosticsProxyTraceListener.SplitXPathQueriesString(xpathsStrings);

            Assert.AreEqual(3, xpaths.Count);
            Assert.AreEqual("three", xpaths[0]);
            Assert.AreEqual("multiple", xpaths[1]);
            Assert.AreEqual("paths", xpaths[2]);
        }

        [Test]
        public void SplittingWithTrailingSemiColonIsOk() {
            string xpathsStrings = @"three;multiple;paths;";

            IList<string> xpaths = AbcDiagnosticsProxyTraceListener.SplitXPathQueriesString(xpathsStrings);

            Assert.AreEqual(3, xpaths.Count);
            Assert.AreEqual("three", xpaths[0]);
            Assert.AreEqual("multiple", xpaths[1]);
            Assert.AreEqual("paths", xpaths[2]);
        }

        [Test]
        public void SplittingMixedEscapedAndNonEscapedSemicolonsIsOk() {
            string xpathsStrings = @"thr\;ee;\;multiple;paths\;;";

            IList<string> xpaths = AbcDiagnosticsProxyTraceListener.SplitXPathQueriesString(xpathsStrings);

            Assert.AreEqual(3, xpaths.Count);
            Assert.AreEqual("thr;ee", xpaths[0]);
            Assert.AreEqual(";multiple", xpaths[1]);
            Assert.AreEqual("paths;", xpaths[2]);
        }

        [Test]
        public void ExtractingNamespacesWithEmptyStringReturnsEmptyDictionary() {
            string namespacesString = @"";

            IDictionary<string, string> namespaces = AbcDiagnosticsProxyTraceListener.SplitNamespacesString(namespacesString);

            Assert.AreEqual(0, namespaces.Count);
        }

        [Test]
        public void ExtractingNamespacesWithSingleEntryReturnsSingleEntryDictionary() {
            string namespacesString = @"xmlns:pre='urn:test'";

            IDictionary<string, string> namespaces = AbcDiagnosticsProxyTraceListener.SplitNamespacesString(namespacesString);

            Assert.AreEqual(1, namespaces.Count);
            Assert.AreEqual("urn:test", namespaces["pre"]);
        }

        [Test]
        public void ExtractingNamespacesWithSingleEntryWithNoPrefixReturnsEmptyDictionary() {
            string namespacesString = @"xmlns='urn:test'";

            IDictionary<string, string> namespaces = AbcDiagnosticsProxyTraceListener.SplitNamespacesString(namespacesString);

            Assert.AreEqual(0, namespaces.Count);
        }

        [Test]
        public void ExtractingNamespacesWithMultipleEntriesWithNoPrefixReturnsMultiEntryDictionaryAndIgnoresNoPrefix() {
            string namespacesString = @"xmlns='urn:test' xmlns:='urn:test' xmlns:pre2='urn:test2'    xmlns:pre1='http://microsoft.com'";

            IDictionary<string, string> namespaces = AbcDiagnosticsProxyTraceListener.SplitNamespacesString(namespacesString);

            Assert.AreEqual(2, namespaces.Count);
            Assert.AreEqual("urn:test2", namespaces["pre2"]);
            Assert.AreEqual("http://microsoft.com", namespaces["pre1"]);
        }

        [Test]
        public void ExtractingNamespacesWithMultipleEntriesWithWrongSyntaxIgnoresWrongEntries() {
            string namespacesString = @"xmlns'urn:test' xmlns:pre2='urn:test2'    xmlns:pre1'http://microsoft.com'";

            IDictionary<string, string> namespaces = AbcDiagnosticsProxyTraceListener.SplitNamespacesString(namespacesString);

            Assert.AreEqual(1, namespaces.Count);
            Assert.AreEqual("urn:test2", namespaces["pre2"]);
        }

        string xmlPayloadString = @"<?xml version='1.0'?>
<E2ETraceEvent xmlns='http://schemas.microsoft.com/2004/06/E2ETraceEvent'>
    <System xmlns='http://schemas.microsoft.com/2004/06/windows/eventlog/system'>
        <EventID>0</EventID>
        <Type>3</Type>
        <SubType Name='Information'>0</SubType>
        <Level>8</Level>
        <TimeCreated SystemTime='2006-10-18T02:58:03.8287806Z'/>
        <Source Name='System.ServiceModel.MessageLogging'/>
        <Correlation ActivityID='{de8d38a9-dcb2-4a18-97c7-e026fbe610b0}'/>
        <Execution ProcessName='CalculatorService' ProcessID='592' ThreadID='3'/>
        <Channel/>
        <Computer>HKAWANO-VISTA</Computer>
    </System>
    <ApplicationData>
        <TraceData>
            <DataItem>
                <MessageLogTraceRecord Time='2006-10-18T11:58:03.8287806+09:00' Source='TransportSend' Type='System.ServiceModel.Channels.BodyWriterMessage' xmlns='http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace'>
                    <s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/' xmlns:a='http://www.w3.org/2005/08/addressing'>
                        <s:Header>
                            <a:Action s:mustUnderstand='1'>http://RMCalculator/20061017/RMCalculator/MultiplyResponse</a:Action>
                            <ActivityId CorrelationId='52ab5a81-e8ee-4f15-aecb-6bc5104c2c10' xmlns='http://schemas.microsoft.com/2004/09/ServiceModel/Diagnostics'>de8d38a9-dcb2-4a18-97c7-e026fbe610b0</ActivityId>
                            <a:RelatesTo>urn:uuid:2778c51a-9941-4c19-b423-6c76b2374bdd</a:RelatesTo>
                        </s:Header>
                        <s:Body>
                            <MultiplyResponse xmlns='http://RMCalculator/20061017/'>
                                <MultiplyResult>10</MultiplyResult>
                            </MultiplyResponse>
                        </s:Body>
                    </s:Envelope>
                </MessageLogTraceRecord>
            </DataItem>
        </TraceData>
    </ApplicationData>
</E2ETraceEvent>";

        [Test]
        public void ShouldFilterTrace() {
            proxy.Filter = new EventTypeFilter(SourceLevels.Off);

            proxy.TraceData(new TraceEventCache(), "System.ServiceModel", TraceEventType.Error, 1, "Message");

            Assert.AreEqual(0, MockTraceListener.ProcessedTraceRequests);
        }

        [Test]
        public void ShouldTraceUsingFilter() {
            proxy.Filter = new EventTypeFilter(SourceLevels.Error);

            proxy.TraceData(new TraceEventCache(), "System.ServiceModel", TraceEventType.Critical, 1, "Message");

            Assert.AreEqual(1, MockTraceListener.ProcessedTraceRequests);
        }

        [Test]
        public void TracingXPathNavigatorWithoutConfiguredCategoriesQueryCreatesLogEntryWithoutCategories() {
            XPathNavigator navigator = new XPathDocument(new StringReader(xmlPayloadString)).CreateNavigator();
            proxy.TraceData(new TraceEventCache(), "System.ServiceModel", TraceEventType.Error, 1, navigator);

            Assert.IsTrue(MockTraceListener.LastEntry is XmlLogEntry);

            XmlLogEntry lastEntry = MockTraceListener.LastEntry as XmlLogEntry;
            Assert.AreSame(lastEntry.Xml, navigator);
            Assert.AreEqual(1, lastEntry.Categories.Count);
            Assert.IsTrue(lastEntry.Categories.Contains("System.ServiceModel"));
        }

        [Test]
        public void TracingXPathNavigatorWithConfiguredCategoriesQueryForNonExistingValueCreatesLogEntryWithoutCategories() {
            XPathNavigator navigator = new XPathDocument(new StringReader(xmlPayloadString)).CreateNavigator();
            proxy.Attributes.Add("categoriesXPathQueries", @"//NonExistingNode/@NonExistingAttribute");
            proxy.TraceData(new TraceEventCache(), "System.ServiceModel", TraceEventType.Error, 1, navigator);

            Assert.IsTrue(MockTraceListener.LastEntry is XmlLogEntry);

            XmlLogEntry lastEntry = MockTraceListener.LastEntry as XmlLogEntry;
            Assert.AreSame(lastEntry.Xml, navigator);
            Assert.AreEqual(1, lastEntry.Categories.Count);
            Assert.IsTrue(lastEntry.Categories.Contains("System.ServiceModel"));
        }

        [Test]
        public void TracingXPathNavigatorWithConfiguredCategoriesQueryForExistingValueCreatesLogEntryWithExtraCategories() {
            XPathNavigator navigator = new XPathDocument(new StringReader(xmlPayloadString)).CreateNavigator();
            proxy.Attributes.Add("categoriesXPathQueries", @"//mt:MessageLogTraceRecord/@Source");
            proxy.Attributes.Add("namespaces", @"xmlns:mt='http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace'");
            proxy.TraceData(new TraceEventCache(), "System.ServiceModel", TraceEventType.Error, 1, navigator);

            Assert.IsTrue(MockTraceListener.LastEntry is XmlLogEntry);

            XmlLogEntry lastEntry = MockTraceListener.LastEntry as XmlLogEntry;
            Assert.AreSame(lastEntry.Xml, navigator);
            Assert.AreEqual(2, lastEntry.Categories.Count);
            Assert.IsTrue(lastEntry.Categories.Contains("System.ServiceModel"));
            Assert.IsTrue(lastEntry.Categories.Contains(@"TransportSend"));
        }

        [Test]
        public void TracingXPathNavigatorWithMultipleConfiguredCategoriesQueryForExistingValueCreatesLogEntryWithExtraCategories() {
            XPathNavigator navigator = new XPathDocument(new StringReader(xmlPayloadString)).CreateNavigator();
            proxy.Attributes.Add("categoriesXPathQueries", @"//mt:MessageLogTraceRecord/@Source;//a:Action/text()");
            proxy.Attributes.Add("namespaces", @"xmlns:mt='http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace'
                                                    xmlns:a='http://www.w3.org/2005/08/addressing'");
            proxy.TraceData(new TraceEventCache(), "System.ServiceModel", TraceEventType.Error, 1, navigator);

            Assert.IsTrue(MockTraceListener.LastEntry is XmlLogEntry);

            XmlLogEntry lastEntry = MockTraceListener.LastEntry as XmlLogEntry;
            Assert.AreSame(lastEntry.Xml, navigator);
            Assert.AreEqual(3, lastEntry.Categories.Count);
            Assert.IsTrue(lastEntry.Categories.Contains("System.ServiceModel"));
            Assert.IsTrue(lastEntry.Categories.Contains(@"TransportSend"));
            Assert.IsTrue(lastEntry.Categories.Contains(@"http://RMCalculator/20061017/RMCalculator/MultiplyResponse"));
        }

        [Test]
        public void WriteLineGoesThroughLoggingBlock() {
            proxy.WriteLine("test");

            LogEntry lastEntry = MockTraceListener.LastEntry;
            Assert.IsNotNull(lastEntry);
            Assert.AreEqual("test", lastEntry.Message);
        }

        [Test]
        public void TraceTransferGoesThroughLoggingBlock() {
            Guid relatedActivityGuid = Guid.NewGuid();

            Assert.AreEqual(0, MockTraceListener.TransferGuids.Count);

            proxy.TraceTransfer(new TraceEventCache(), string.Empty, 1000, "message", relatedActivityGuid);

            Assert.AreEqual(1, MockTraceListener.TransferGuids.Count);
            Assert.AreEqual(relatedActivityGuid, MockTraceListener.TransferGuids[0]);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.ObjectBuilder2;

namespace Diagnostic.UnitTests {
    /*
    public class TestLogWriterCustomFactory : ICustomFactory {
        #region Methods
        public object CreateObject(
            IBuilderContext context,
            string name,
            IConfigurationSource configurationSource,
            ConfigurationReflectionCache reflectionCache) {
            TraceListener listener = new MockTraceListener();

            LogSource traceSource = new LogSource("mock", new List<TraceListener>() { listener }, SourceLevels.All, true);
            List<LogSource> traceSources = new List<LogSource>() { traceSource };
            List<ILogFilter> filters = new List<ILogFilter>() { new LogEnabledFilter("filter", true) };

            return new TestLogWriter(filters, traceSources, traceSource, "MockCategoryOne");
        }

        #endregion
    }

    /// <summary>
    /// Factory to create <see cref="MyLogWriter"/> instances.
    /// </summary>
    public class TestLogWriterFactory {
        #region Fields
        private readonly IConfigurationSource configurationSource;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLogWriterFactory"/> class. 
        /// </summary>
        public TestLogWriterFactory()
            : this(ConfigurationSourceFactory.Create()) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLogWriterFactory"/> class. 
        /// </summary>
        /// <param name="configurationSource">
        /// The source for configuration information.
        /// </param>
        public TestLogWriterFactory(IConfigurationSource configurationSource) {
            this.configurationSource = configurationSource;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of <see cref="LogWriter"/> based on the configuration in the <see cref="IConfigurationSource"/> 
        /// instance of the factory.
        /// </summary>
        /// <returns>
        /// The created <see cref="LogWriter"/> object.
        /// </returns>
        public TestLogWriter Create() {
            return EnterpriseLibraryFactory.BuildUp<TestLogWriter>(configurationSource);
        }

        #endregion
    }

    [CustomFactory(typeof(TestLogWriterCustomFactory))]
    public class TestLogWriter : LogWriter {
        #region Constructors
        public TestLogWriter(
            ICollection<ILogFilter> filters, 
            ICollection<LogSource> traceSources, 
            LogSource errorsTraceSource, 
            string defaultCategory)
            : base(filters, traceSources, errorsTraceSource, defaultCategory) {
        }

        #endregion
    }
    */
    public class TestLogWriterProxy : ILogWriter, ILogSource {
        private LogWriter writer;
        private string initData;

        public TestLogWriterProxy()
            : this(null) {
        }

        public TestLogWriterProxy(string initializeData) {
            this.initData = initializeData;
            //writer = new TestLogWriterFactory().Create();
            writer = new LogWriterFactory().Create();
        }

        public string InitData {
            get { return initData; }
        }

        public bool IsLoggingEnabled {
            get { return writer.IsLoggingEnabled(); }
        }

        public bool IsTracingEnabled {
            get { return writer.IsTracingEnabled(); }
        }

        public string SourceName {
            get { return "bar"; }
        }

        public void Write(string message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties, Exception exception, Guid activityId, Guid? relatedActivityId) {
            XmlLogEntry log = new XmlLogEntry();
            log.Message = message;
            log.Categories = categories;
            log.Priority = priority;
            log.EventId = eventId;
            log.Severity = severity;
            log.Title = title;
            log.ExtendedProperties = properties;
            log.ActivityId = activityId;
            log.RelatedActivityId = relatedActivityId;
            log.Xml = DefaultLogWriter.BuildTraceRecord(message, priority, severity, title, properties, exception); 

            writer.Write(log);            
        }

        public void Flush() {
            writer.FlushContextItems();
        }
    } 
}
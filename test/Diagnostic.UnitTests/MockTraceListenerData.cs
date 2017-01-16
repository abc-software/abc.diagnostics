// ----------------------------------------------------------------------------
// <copyright file="MockTraceListenerData.cs" company="ABC software">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    The source code or its parts to use, reproduce, transfer, copy or
//    keep in an electronic form only from written agreement ABC SOFTWARE.
// </copyright>
// ----------------------------------------------------------------------------

namespace Diagnostic.UnitTests {
    using System.Diagnostics;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
    using Microsoft.Practices.ObjectBuilder2;

    [Assembler(typeof(MockTraceListenerAssembler))]
    public class MockTraceListenerData : TraceListenerData {
        public MockTraceListenerData() {
        }

        public MockTraceListenerData(string name)
            : base(name, typeof(MockTraceListener), TraceOptions.None, SourceLevels.All) {
        }
    }

    public class MockTraceListenerAssembler : IAssembler<TraceListener, TraceListenerData> {
        public TraceListener Assemble(IBuilderContext context,
                                      TraceListenerData objectConfiguration,
                                      IConfigurationSource configurationSource,
                                      ConfigurationReflectionCache reflectionCache) {
            return new MockTraceListener();
        }
    }
}
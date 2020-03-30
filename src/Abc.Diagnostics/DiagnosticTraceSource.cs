// ----------------------------------------------------------------------------
// <copyright file="DiagnosticTraceSource.cs" company="ABC Software Ltd">
//    Copyright © 2020 ABC Software Ltd. All rights reserved.
//
//    This library is free software; you can redistribute it and/or.
//    modify it under the terms of the GNU Lesser General Public
//    License  as published by the Free Software Foundation, either
//    version 3 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with the library. If not, see http://www.gnu.org/licenses/.
// </copyright>
// ----------------------------------------------------------------------------

#if !NETSTANDARD1_x
#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic {
#else
namespace Abc.Diagnostics {
#endif
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// The diagnostics trace source.
    /// </summary>
    /// <seealso cref="System.Diagnostics.TraceSource" />
    public class DiagnosticTraceSource : TraceSource {
        private const string AttributeNamePropagateActivity = "propagateActivity";
        private const string AttributeNameMaxSize = "maxdatasize";
        private const string AttributeNameTraceMode = "tracemode";
        private const string AttributeValueProtocolOnly = "protocolonly";
        private const int DefaultMaxContentLogSize = 1024;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticTraceSource"/> class, using the specified name for the source..
        /// </summary>
        /// <param name="name">The name of the source (typically, the name of the application)</param>
        public DiagnosticTraceSource(string name)
            : base(name) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticTraceSource"/> class, using the specified name for the source and the default source level at which tracing is to occur.
        /// </summary>
        /// <param name="name">The name of the source, typically the name of the application.</param>
        /// <param name="defaultLevel">A bitwise combination of the enumeration values that specifies the default source level at which to trace.</param>
        public DiagnosticTraceSource(string name, SourceLevels defaultLevel)
            : base(name, defaultLevel) {
        }

        /// <summary>
        /// Gets or sets a value indicates whether the propagate attribute is set to <c>true</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if propagate activity attribute is set to <c>true</c>; otherwise, <c>false</c>.
        /// </value>
        public bool PropagateActivity {
            get {
                bool result = false;
                if (this.Attributes.ContainsKey(AttributeNamePropagateActivity)) {
                    string value = this.Attributes[AttributeNamePropagateActivity];
                    if (!string.IsNullOrEmpty(value) && !bool.TryParse(value, out result)) {
                        result = false;
                    }
                }

                return result;
            }

            set {
                this.Attributes[AttributeNamePropagateActivity] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the maximum size of the content log.
        /// </summary>
        /// <value>
        /// The maximum size of the content log.
        /// </value>
        public int MaxContentLogSize {
            get {
                int result = DefaultMaxContentLogSize;
                if (this.Attributes.ContainsKey(AttributeNameMaxSize)) {
                    string value = this.Attributes[AttributeNameMaxSize];
                    if (!string.IsNullOrEmpty(value) && !int.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result)) {
                        result = DefaultMaxContentLogSize;
                    }
                }

                return result;
            }

            set {
                this.Attributes[AttributeNameMaxSize] = value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
            }
        }

        /// <summary>
        /// Gets a value indicates whether the trace mode attribute is set to <c>protocolonly</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if propagate trace mode attribute is set to <c>protocolonly</c>; otherwise, <c>false</c>.
        /// </value>
        public bool UseProtocolOnlySettings() {
            bool result = false;
            if (this.Attributes.ContainsKey(AttributeNameTraceMode) && this.Attributes[AttributeNameTraceMode] == AttributeValueProtocolOnly) {
                result = true;
            }

            return result;
        }

        /// <inheritdoc/>
        protected override string[] GetSupportedAttributes() {
            return new string[] { AttributeNamePropagateActivity, AttributeNameMaxSize, AttributeNameTraceMode };
        }
    }
}
#endif
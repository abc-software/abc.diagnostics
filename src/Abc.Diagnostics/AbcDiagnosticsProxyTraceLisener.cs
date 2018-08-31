// ----------------------------------------------------------------------------
// <copyright file="DefaultLogWriter.cs" company="ABC Software Ltd">
//    Copyright © 2015 ABC Software Ltd. All rights reserved.
//
//    This library is free software; you can redistribute it and/or
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

#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic {
#else
namespace Abc.Diagnostics {
#endif
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.XPath;

    /// <summary>
    /// Proxy listener for writing the log using the <see cref="LogUtility" /> class.
    /// </summary>
    public class AbcDiagnosticsProxyTraceListener : TraceListener {
        private const string TraceEventCacheKey = "TraceEventCache";
        private const string XPathNavigatorKey = "XPathNavigator";

#if !NETSTANDARD1_x
        private static readonly string[] SupportedAttributes = new string[] { "categoriesXPathQueries", "namespaces" };

        private static readonly Regex NamespaceRegex
            = new Regex(
                        @"
                        xmlns                       # the 'xmlns' prefix
                            :(?<prefix>[^'""]+?)    # followed by a mandatory prefix name
                        =                           # the '=' sign
                            (?<quote>[""'])         # a start quote
                                (?<uri>.*?)         # the uri
                            \<quote>                # the matching end quote
                        ", RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

        private static readonly Regex XPathRegex
            = new Regex(
                        @"
                        (?<path>.*?)            # but non greedy path
                        (                       # until either
                            $                   # the string ends
                                |               # or
                            (?<!\\);            # a non escaped ; is found
                        )
                        ", RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

        private IList<string> categoriesXPathQueries;
        private XmlNamespaceManager xmlNamespaceManager;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="AbcDiagnosticsProxyTraceListener"/> class.
        /// </summary>
        public AbcDiagnosticsProxyTraceListener() {
        }

        /// <inheritdoc/>
        public override bool IsThreadSafe {
            get {
                return true;
            }
        }

#if !NETSTANDARD1_x
        /// <summary>
        /// Gets the xpath queries for the categories.
        /// </summary>
        /// <value>
        /// The xpath queries for the categories.
        /// </value>
        public IList<string> CategoriesXPathQueries {
            get {
                if (this.categoriesXPathQueries == null) {
                    var str = this.Attributes["categoriesXPathQueries"];
                    this.categoriesXPathQueries = string.IsNullOrEmpty(str) ? new List<string>(0) : SplitXPathQueriesString(str);
                }

                return this.categoriesXPathQueries;
            }
        }

        /// <summary>
        /// Gets the XmlNamespaceManager for the listenter.
        /// </summary>
        /// <value>
        /// The XmlNamespaceManager for the listenter.
        /// </value>
        public XmlNamespaceManager NamespaceManager {
            get {
                if (this.xmlNamespaceManager == null) {
                    var manager = new XmlNamespaceManager(new NameTable());
                    var str = this.Attributes["namespaces"];
                    if (!string.IsNullOrEmpty(str)) {
                        foreach (KeyValuePair<string, string> pair in SplitNamespacesString(str)) {
                            manager.AddNamespace(pair.Key, pair.Value);
                        }
                    }

                    this.xmlNamespaceManager = manager;
                }

                return this.xmlNamespaceManager;
            }
        }
#endif

        /// <inheritdoc/>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data) {
            if (this.Filter == null || this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null)) {
                var properties = new Dictionary<string, object>();
                properties.Add(TraceEventCacheKey, eventCache);

#if NETSTANDARD1_x
                var activityId = Guid.Empty;
#else
                var activityId = LogUtility.ActivityId;
#endif

                var navigator = data as XPathNavigator;
                if (navigator != null) {
                    var category = new List<string>() { source };
#if !NETSTANDARD1_x
                    foreach (string str in this.CategoriesXPathQueries) {
                        foreach (object obj2 in navigator.Select(str, this.NamespaceManager)) {
                            category.Add(((XPathNavigator)obj2).Value);
                        }
                    }
#endif

                    properties.Add(XPathNavigatorKey, navigator);
                    LogUtility.Writer.Write(data.ToString(), category, LogUtility.DefaultPriority, id, eventType, LogUtility.LogSourceName, properties, null, activityId, null);
                }
                else {
                    LogUtility.Writer.Write(data.ToString(), string.IsNullOrEmpty(source) ? new string[0] : new string[] { source }, LogUtility.DefaultPriority, id, eventType, LogUtility.LogSourceName, properties, null, activityId, null);
                }
            }
        }

        /// <inheritdoc/>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message) {
            this.TraceData(eventCache, source, eventType, id, message);
        }

        /// <inheritdoc/>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args) {
            var message = args != null ? string.Format(CultureInfo.InvariantCulture, format, args) : format;
            this.TraceEvent(eventCache, source, eventType, id, message);
        }

        /// <inheritdoc/>
        public override void Write(string message) {
            this.WriteLine(message);
        }

        /// <inheritdoc/>
        public override void WriteLine(string message) {
            this.TraceData(new TraceEventCache(), string.Empty, TraceEventType.Information, 0, message);
        }

#if !NETSTANDARD1_x
        /// <inheritdoc/>
        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId) {
            var properties = new Dictionary<string, object>();
            properties.Add(TraceEventCacheKey, eventCache);

            LogUtility.Writer.Write(string.IsNullOrEmpty(message) ? string.Empty : message, string.IsNullOrEmpty(source) ? new string[0] : new string[] { source }, LogUtility.DefaultPriority, id, TraceEventType.Transfer, LogUtility.LogSourceName, properties, null, LogUtility.ActivityId, relatedActivityId);
        }

        /// <summary>
        /// Splits a namespace string.
        /// </summary>
        /// <param name="namespacesString">The string to split.</param>
        /// <returns>The string split into keys and values.</returns>
        internal static IDictionary<string, string> SplitNamespacesString(string namespacesString) {
            if (namespacesString == null) {
                throw new ArgumentNullException("namespacesString");
            }

            var dictionary = new Dictionary<string, string>();
            namespacesString = namespacesString.Trim();
            if (namespacesString.Length > 0) {
                for (Match match = NamespaceRegex.Match(namespacesString); match.Success; match = match.NextMatch()) {
                    dictionary[match.Groups["prefix"].Value] = match.Groups["uri"].Value;
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Splits the XPathQuery strings.
        /// </summary>
        /// <param name="xpathsStrings">The XPath queries.</param>
        /// <returns>A list of xpaths.</returns>
        internal static IList<string> SplitXPathQueriesString(string xpathsStrings) {
            if (xpathsStrings == null) {
                throw new ArgumentNullException("xpathsStrings");
            }

            var list = new List<string>();
            xpathsStrings = xpathsStrings.Trim();
            if (xpathsStrings.Length > 0) {
                for (Match match = XPathRegex.Match(xpathsStrings); match.Success; match = match.NextMatch()) {
                    string item = match.Groups["path"].Value.Replace(@"\;", ";");
                    if (item.Length > 0) {
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        /// <inheritdoc/>
        protected override string[] GetSupportedAttributes() {
            return SupportedAttributes;
        }
#endif
    }
}
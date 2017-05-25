// ----------------------------------------------------------------------------
// <copyright file="DebugInformationProvider.cs" company="ABC Software Ltd">
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
namespace Diagnostic.ExtraInformation {
#else
namespace Abc.Diagnostics.ExtraInformation {
#endif
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Security;
    using System.Text;

    /// <summary>
    /// Provides useful diagnostic information from the debug subsystem.
    /// </summary>
    public class DebugInformationProvider : IExtraInformationProvider {
        #region Fields
        private readonly IStackTraceUtility debugUtils;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInformationProvider"/> class.
        /// </summary>
        public DebugInformationProvider()
            : this(new StackTraceUtility()) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInformationProvider"/> class.
        /// </summary>
        /// <param name="utility">
        /// Alternative <see cref="IStackTraceUtility"/> to use.
        /// </param>
        public DebugInformationProvider(IStackTraceUtility utility) {
            this.debugUtils = utility;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Populates an <see cref="T:System.Collections.Generic.IDictionary`2"/> with helpful diagnostic information.
        /// </summary>
        /// <param name="dictionary">
        /// Dictionary used to populate the <see cref="T:Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.DebugInformationProvider"></see>
        /// </param>
        public void PopulateDictionary(IDictionary<string, object> dictionary) {
            if (dictionary == null) {
                throw new ArgumentNullException("dictionary"); 
            }

            string stackTrace;

            try {
                stackTrace = this.debugUtils.GetStackTraceWithSourceInfo(new StackTrace(true));
            }
            catch (SecurityException) {
                stackTrace = string.Format(
                    SR.Culture, SR.ExtraInformation_PropertyError, SR.ExtraInformation_StackTraceSecurityException);
            }
            catch (Exception ex) {
                if (ExceptionUtility.IsFatal(ex)) {
                    throw;
                }

                stackTrace = string.Format(
                    SR.Culture, SR.ExtraInformation_PropertyError, SR.ExtraInformation_StackTraceException);
            }

            dictionary.Add(SR.ExtraInformation_StackTrace, stackTrace);
        }

        #endregion

        #region Nested type: StackTraceUtility
        /// <summary>
        /// Represent StackTrace Utility.
        /// </summary>
        private class StackTraceUtility : IStackTraceUtility {
            #region Methods
            /// <summary>
            /// Returns a text representation of the stack trace with source information if available.
            /// </summary>
            /// <param name="stackTrace">The source to represent textually.</param>
            /// <returns>The textual representation of the stack.</returns>
            public string GetStackTraceWithSourceInfo(StackTrace stackTrace) {
                if (stackTrace == null) {
                    throw new ArgumentNullException("stackTrace");
                }

                string aatString = SR.ExtraInformation_SchemaHelperAtString;
                string unknownTypeString = SR.ExtraInformation_SchemaHelperUnknownType;
                string newLine = Environment.NewLine;

                StringBuilder stringBuilder = new StringBuilder(255);

                for (int i = 0; i < stackTrace.FrameCount; i++) {
                    StackFrame stackFrame = stackTrace.GetFrame(i);

                    stringBuilder.Append(aatString);

                    MethodBase method = stackFrame.GetMethod();
                    Type t = method.DeclaringType;
                    if (t != null) {
                        string nameSpace = t.Namespace;
                        if (nameSpace != null) {
                            stringBuilder.Append(nameSpace);
                            if (stringBuilder.Length > 0) {
                                stringBuilder.Append(".");
                            }
                        }

                        stringBuilder.Append(t.Name);
                        stringBuilder.Append(".");
                    }

                    stringBuilder.Append(method.Name);
                    stringBuilder.Append("(");

                    ParameterInfo[] arrParams = method.GetParameters();
                    for (int j = 0; j < arrParams.Length; j++) {
                        string typeName = unknownTypeString;
                        if (arrParams[j].ParameterType != null) {
                            typeName = arrParams[j].ParameterType.Name;
                        }

                        stringBuilder.Append((j != 0 ? ", " : string.Empty) + typeName + " " + arrParams[j].Name);
                    }

                    stringBuilder.Append(")");

                    if (stackFrame.GetILOffset() != -1) {
                        // It's possible we have a debug version of an executable but no PDB.  In
                        // this case, the file name will be null.
                        string fileName = stackFrame.GetFileName();

                        if (fileName != null) {
                            stringBuilder.Append(
                                string.Format(
                                    SR.Culture,
                                    SR.ExtraInformation_SchemaHelperLine, 
                                    fileName, 
                                    stackFrame.GetFileLineNumber()));
                        }
                    }

                    if (i != stackTrace.FrameCount - 1) {
                        stringBuilder.Append(newLine);
                    }
                }

                return stringBuilder.ToString();
            }

            #endregion
        }
        #endregion
    }
}
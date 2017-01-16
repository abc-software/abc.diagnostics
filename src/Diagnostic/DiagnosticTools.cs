// ----------------------------------------------------------------------------
// <copyright file="DiagnosticTools.cs" company="ABC Software Ltd">
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
    using System.Reflection;

    /// <summary>
    /// Diagnostic Tools
    /// </summary>
    public static class DiagnosticTools {
        private static readonly object LockObject = new object();
        private static LogUtility logUtil;
        private static ExceptionUtility exceptionUtil;

        /// <summary>
        /// Gets the log utility.
        /// </summary>
        /// <value>The log utility.</value>
        public static LogUtility LogUtil {
            get { return logUtil ?? GetLogUtility(); }
        }

        /// <summary>
        /// Gets the exception utility.
        /// </summary>
        /// <value>The exception utility.</value>
        public static ExceptionUtility ExceptionUtil {
            get { return exceptionUtil ?? GetExceptionUtility(); }
        }

        private static LogUtility GetLogUtility() {
            lock (LockObject) {
                if (logUtil == null) {
                    logUtil = new LogUtility();
                }
            }

            return logUtil;
        }

        private static ExceptionUtility GetExceptionUtility() {
            lock (LockObject) {
                if (exceptionUtil == null) {
                    exceptionUtil = new ExceptionUtility();
                }
            }

            return exceptionUtil;
        }
    }
}

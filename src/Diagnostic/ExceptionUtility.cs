// ----------------------------------------------------------------------------
// <copyright file="ExceptionUtility.cs" company="ABC Software Ltd">
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
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represent exception utility
    /// </summary>
    public class ExceptionUtility : IExceptionUtility {
        #region Fields
        [ThreadStatic]
        private static Guid activityId;
        [ThreadStatic]
        private static bool useStaticActivityId;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionUtility"/> class.
        /// </summary>
        public ExceptionUtility() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionUtility"/> class with specified diagnostic trace.
        /// </summary>
        /// <param name="diagnosticTrace">The diagnostic trace.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "diagnosticTrace", Justification = "For backward compatibility")]
        [Obsolete("Deprecated")]
        public ExceptionUtility(LogUtility diagnosticTrace) {
        }

        #endregion Constructors

        #region Methods
        /// <summary>
        /// Clears the activity id.
        /// </summary>
        public static void ClearActivityId() {
            useStaticActivityId = false;
            activityId = Guid.Empty;
        }

        /// <summary>
        /// Uses the specified activity id to trace log.
        /// </summary>
        /// <param name="activityId">The activity id.</param>
        public static void UseActivityId(Guid activityId) {
            ExceptionUtility.activityId = activityId;
            useStaticActivityId = true;
        }

        /// <summary>
        /// Determines whether the specified exception is fatal.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        ///     <c>true</c> if the specified exception is fatal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFatal(Exception exception) {
            while (exception != null) {
#if NETSTANDARD
                if (exception is FatalException || 
                    exception is OutOfMemoryException || 
                    exception is SEHException) {
                    return true;
                }
#else
                if ((exception is FatalException || 
                    (exception is OutOfMemoryException && !(exception is InsufficientMemoryException))) || 
                    ((exception is System.Threading.ThreadAbortException || exception is AccessViolationException) || 
                    exception is SEHException)) {
                    return true;
                }
#endif

                if (!(exception is TypeInitializationException) && !(exception is TargetInvocationException)) {
                    break;
                }

                exception = exception.InnerException;
            }

            return false;
        }

#if !NETSTANDARD
        /// <summary>
        /// Determines whether the specified exceptions infrastructure exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        ///     <c>true</c> if the specified exception is infrastructure exception; otherwise, <c>false</c>.
        /// </returns>
        [System.Runtime.ConstrainedExecution.ReliabilityContract(System.Runtime.ConstrainedExecution.Consistency.WillNotCorruptState, System.Runtime.ConstrainedExecution.Cer.Success)]
        public static bool IsInfrastructure(Exception exception) {
            if (exception == null) {
                return false;
            }

            return (exception is System.Threading.ThreadAbortException) || (exception is AppDomainUnloadedException);
        }
#endif

        /// <summary>
        /// Throws the helper.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>The thrown exception.</returns>
        public Exception ThrowHelper(Exception exception, TraceEventType eventType) {
            LogUtility.Write(SR.ThrowingException, new string[] { LogUtility.GeneralCategory }, LogUtility.DefaultPriority, LogUtility.DefaultEventId, eventType, LogUtility.LogSourceName, null, exception, useStaticActivityId ? activityId : Guid.Empty);
            return exception;
        }

        /// <summary>
        /// Traces the handled exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="eventType">Type of the event.</param>
        public void TraceHandledException(Exception exception, TraceEventType eventType) {
            LogUtility.Write(SR.TraceHandledException, new string[] { LogUtility.GeneralCategory }, LogUtility.DefaultPriority, LogUtility.DefaultEventId, eventType, LogUtility.LogSourceName, null, exception, useStaticActivityId ? activityId : Guid.Empty);
        }

#endregion Methods
    }
}
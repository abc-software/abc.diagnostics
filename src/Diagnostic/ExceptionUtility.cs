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
    public class ExceptionUtility {
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
            // DiagnosticTools.LogUtil.Write(SR.ThrowingException, LogUtility.GeneralCategory, LogUtility.DefaultPriority, LogUtility.DefaultEventId, eventType, exception, useStaticActivityId ? activityId : Guid.Empty); 
            LogUtility.Write(Abc.Diagnostics.SR.ThrowingException, new string[] { LogUtility.GeneralCategory }, LogUtility.DefaultPriority, LogUtility.DefaultEventId, eventType, LogUtility.LogSourceName, null, exception, useStaticActivityId ? activityId : Guid.Empty);
            return exception;
        }

        /// <summary>
        /// Throws the helper warning.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The thrown exception.</returns>
        public Exception ThrowHelperWarning(Exception exception) {
            return this.ThrowHelper(exception, TraceEventType.Warning);
        }

        /// <summary>
        /// Throws the helper critical.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The thrown exception.</returns>
        public Exception ThrowHelperCritical(Exception exception) {
            return this.ThrowHelper(exception, TraceEventType.Critical);
        }

        /// <summary>
        /// Throws the helper error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The thrown exception.</returns>
        public Exception ThrowHelperError(Exception exception) {
            return this.ThrowHelper(exception, TraceEventType.Error);
        }

        /// <summary>
        /// Throw <see cref="T:System.ArgumentException"></see> with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <returns>The <see cref="T:System.ArgumentException"></see>.</returns>
        public ArgumentException ThrowHelperArgument(string message) {
            return (ArgumentException)this.ThrowHelperError(new ArgumentException(message));
        }

        /// <summary>
        /// Throw <see cref="T:System.ArgumentException"></see> with a specified error message and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <returns>The <see cref="T:System.ArgumentException"></see>.</returns>
        public ArgumentException ThrowHelperArgument(string message, string parameterName) {
            return (ArgumentException)this.ThrowHelperError(new ArgumentException(message, parameterName));
        }

        /// <summary>
        /// Throw <see cref="T:System.ArgumentNullException"></see> with a specified error message.
        /// </summary>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <returns>The <see cref="T:System.ArgumentNullException"></see>.</returns>
        public ArgumentNullException ThrowHelperArgumentNull(string parameterName) {
            return (ArgumentNullException)this.ThrowHelperError(new ArgumentNullException(parameterName));
        }

        /// <summary>
        /// Throw <see cref="T:System.ArgumentNullException"></see> with a specified error message and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <returns>The <see cref="T:System.ArgumentNullException"></see>.</returns>
        public ArgumentNullException ThrowHelperArgumentNull(string parameterName, string message) {
            return (ArgumentNullException)this.ThrowHelperError(new ArgumentNullException(parameterName, message));
        }

        /// <summary>
        /// Throw <see cref="FatalException"></see> with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        /// <returns>The <see cref="FatalException"></see>.</returns>
        public Exception ThrowHelperFatal(string message, Exception innerException) {
            return this.ThrowHelperError(new FatalException(message, innerException));
        }

        /// <summary>
        /// Traces the handled exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="eventType">Type of the event.</param>
        public void TraceHandledException(Exception exception, TraceEventType eventType) {
            // DiagnosticTools.LogUtil.Write(SR.TraceHandledException, LogUtility.GeneralCategory, LogUtility.DefaultPriority, LogUtility.DefaultEventId, eventType, exception, useStaticActivityId ? activityId : Guid.Empty); 
            LogUtility.Write(Abc.Diagnostics.SR.TraceHandledException, new string[] { LogUtility.GeneralCategory }, LogUtility.DefaultPriority, LogUtility.DefaultEventId, eventType, LogUtility.LogSourceName, null, exception, useStaticActivityId ? activityId : Guid.Empty);
        }

#endregion Methods
    }
}
#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic {
#else
namespace Abc.Diagnostics {
#endif
    using System;
    using System.Diagnostics;

    /// <summary>
    /// <see cref="IExceptionUtiltyExtension"/> extension.
    /// </summary>
    public static class IExceptionUtiltyExtension {
        /// <summary>
        /// Throws the helper warning.
        /// </summary>
        /// <param name="exceptionUtility">The exception utility.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>The thrown exception.</returns>
        /// <exception cref="System.ArgumentNullException">exceptionUtility</exception>
        public static Exception ThrowHelperWarning(this IExceptionUtility exceptionUtility, Exception exception) {
            if (exceptionUtility == null) {
                throw new ArgumentNullException("exceptionUtility");
            }

            return exceptionUtility.ThrowHelper(exception, TraceEventType.Warning);
        }

        /// <summary>
        /// Throws the helper critical.
        /// </summary>
        /// <param name="exceptionUtility">The exception utility.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>The thrown exception.</returns>
        public static Exception ThrowHelperCritical(this IExceptionUtility exceptionUtility, Exception exception) {
            if (exceptionUtility == null) {
                throw new ArgumentNullException("exceptionUtility");
            }

            return exceptionUtility.ThrowHelper(exception, TraceEventType.Critical);
        }

        /// <summary>
        /// Throws the helper error.
        /// </summary>
        /// <param name="exceptionUtility">The exception utility.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>The thrown exception.</returns>
        public static Exception ThrowHelperError(this IExceptionUtility exceptionUtility, Exception exception) {
            if (exceptionUtility == null) {
                throw new ArgumentNullException("exceptionUtility");
            }

            return exceptionUtility.ThrowHelper(exception, TraceEventType.Error);
        }

        /// <summary>
        /// Throw <see cref="T:System.ArgumentException"></see> with a specified error message.
        /// </summary>
        /// <param name="exceptionUtility">The exception utility.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <returns>The <see cref="T:System.ArgumentException"></see>.</returns>
        public static ArgumentException ThrowHelperArgument(this IExceptionUtility exceptionUtility, string message) {
            if (exceptionUtility == null) {
                throw new ArgumentNullException("exceptionUtility");
            }

            return (ArgumentException)exceptionUtility.ThrowHelperError(new ArgumentException(message));
        }

        /// <summary>
        /// Throw <see cref="T:System.ArgumentException"></see> with a specified error message and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="exceptionUtility">The exception utility.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <returns>The <see cref="T:System.ArgumentException"></see>.</returns>
        public static ArgumentException ThrowHelperArgument(this IExceptionUtility exceptionUtility, string message, string parameterName) {
            if (exceptionUtility == null) {
                throw new ArgumentNullException("exceptionUtility");
            }

            return (ArgumentException)exceptionUtility.ThrowHelperError(new ArgumentException(message, parameterName));
        }

        /// <summary>
        /// Throw <see cref="T:System.ArgumentNullException"></see> with a specified error message.
        /// </summary>
        /// <param name="exceptionUtility">The exception utility.</param>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <returns>The <see cref="T:System.ArgumentNullException"></see>.</returns>
        public static ArgumentNullException ThrowHelperArgumentNull(this IExceptionUtility exceptionUtility, string parameterName) {
            if (exceptionUtility == null) {
                throw new ArgumentNullException("exceptionUtility");
            }

            return (ArgumentNullException)exceptionUtility.ThrowHelperError(new ArgumentNullException(parameterName));
        }

        /// <summary>
        /// Throw <see cref="T:System.ArgumentNullException"></see> with a specified error message and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="exceptionUtility">The exception utility.</param>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <returns>The <see cref="T:System.ArgumentNullException"></see>.</returns>
        public static ArgumentNullException ThrowHelperArgumentNull(this IExceptionUtility exceptionUtility, string parameterName, string message) {
            if (exceptionUtility == null) {
                throw new ArgumentNullException("exceptionUtility");
            }

            return (ArgumentNullException)exceptionUtility.ThrowHelperError(new ArgumentNullException(parameterName, message));
        }

        /// <summary>
        /// Throw <see cref="FatalException"></see> with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="exceptionUtility">The exception utility.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        /// <returns>The <see cref="FatalException"></see>.</returns>
        public static Exception ThrowHelperFatal(this IExceptionUtility exceptionUtility, string message, Exception innerException) {
            if (exceptionUtility == null) {
                throw new ArgumentNullException("exceptionUtility");
            }

            return exceptionUtility.ThrowHelperError(new FatalException(message, innerException));
        }
    }
}

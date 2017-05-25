#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic {
#else
namespace Abc.Diagnostics {
#endif
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The exception utility interface
    /// </summary>
    public interface IExceptionUtility {
        /// <summary>
        /// Throws the helper.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>The thrown exception.</returns>
        Exception ThrowHelper(Exception exception, TraceEventType eventType);
    }
}

// -----------------------------------------------------------------------
// <copyright file="LogUtilityExtension.cs" company="ABC software">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace DiagnosicLab {
    using Diagnostic;

    /// <summary>
    /// Log Utility Extension.
    /// </summary>
    public static class LogUtilityExtension {
        private const string Category = "Log";

        /// <summary>
        /// Writes the specified log utility.
        /// </summary>
        /// <param name="logUtility">The log utility.</param>
        /// <param name="message">The message.</param>
        /// <param name="priority">The priority.</param>
        public static void Write(this LogUtility logUtility, string message, int priority) {
            logUtility.Write(message, Category, priority); 
        }
    }
}

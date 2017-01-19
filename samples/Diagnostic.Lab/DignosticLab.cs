using System;
using System.Diagnostics;
using System.Windows.Forms;
using Diagnostic;
using DiagnosicLab.Properties;

namespace DiagnosicLab {
    /// <summary>
    /// Diagnostic samples
    /// </summary>
    public partial class DignosticLab : Form {
        private const string Category = "Log";

        /// <summary>
        /// Initializes a new instance of the <see cref="DignosticLab"/> class.
        /// </summary>
        public DignosticLab() {
            InitializeComponent();
        }

        /// <summary>
        /// Simple write log (By default "Trace" Category)
        /// </summary>
        private void button1_Click(object sender, EventArgs e) {
            DiagnosticTools.LogUtil.Write(Resources.SimpleMessage);
        }

        /// <summary>
        /// Write log with parameters 
        /// </summary>
        private void button2_Click(object sender, EventArgs e) {
            DiagnosticTools.LogUtil.Write(Resources.SimpleMessageWithParameters, Category, -1, 100, TraceEventType.Information);
            DiagnosticTools.LogUtil.Write(Resources.ExceptionToLog, Category, -1, 100, TraceEventType.Error, new ArgumentNullException("sender"));

            // Extension Function.
            DiagnosticTools.LogUtil.Write(Resources.Message, -1);
        }

        /// <summary>
        /// Log Exception (By default "General" Category)
        /// </summary>
        private void button3_Click(object sender, EventArgs e) {
            try {
                DiagnosticTools.ExceptionUtil.ThrowHelperArgumentNull("sender");
            }
            catch (ArgumentNullException) {
                // nothing do
            }
        }

        /// <summary>
        /// Use simple method execution trasing (By default "Trace" Category)
        /// </summary>
        private void button4_Click(object sender, EventArgs e) {
            // Create traser ("ButtonClick" Category)
            using (new TraceUtility("ButtonClick")) {
                // Log Message
                DiagnosticTools.LogUtil.Write(Resources.Button4_Clicked, "ButtonClick");

                Execute();
            }
        }

        private void Execute() {
            // Create traser ("Execute" Category)
            using (TraceUtility.StartTrace("Execute")) {
                System.Threading.Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Use activity monitoring (By default "General" Category)
        /// </summary>
        private void button5_Click(object sender, EventArgs e) {
            const string MyActivity = "MyActivity";

            // Create and start activity
            using (LogActivity my = LogActivity.CreateActivity()) {
                my.Start("My Activity", MyActivity);

                // Log Message with activityId
                DiagnosticTools.LogUtil.Write(Resources.MyActivityMessage, Category, -1, 100, TraceEventType.Information, my.Id);

                // Generate exception with activityId
                ExceptionUtility.UseActivityId(my.Id);
                try {
                    DiagnosticTools.ExceptionUtil.ThrowHelperArgumentNull("param");
                }
                catch (ArgumentNullException) {
                }

                ExceptionUtility.ClearActivityId();
            }
        }

        /// <summary>
        /// Use bounded activity monitoring (By default "Activity" Category)
        /// </summary>
        private void button6_Click(object sender, EventArgs e) {
            const string RootActivity = "RootActivity";
            const string NestedActivity = "NestedActivity";

            // Create and start boundary activity
            LogActivity la = LogActivity.CreateBoundedActivity();
            try {
                la.Start("Root activity", RootActivity);

                // Log Message with activityId
                DiagnosticTools.LogUtil.Write(Resources.RootMessage);

                // Create and start boundary activity
                using (LogActivity ba = LogActivity.CreateBoundedActivity(true)) {
                    ba.Start("Nested activity", NestedActivity);

                    // Log Message with activityId
                    DiagnosticTools.LogUtil.Write(Resources.NestedMessage);

                    // Generate exception with activityId
                    try {
                        DiagnosticTools.ExceptionUtil.ThrowHelperArgumentNull("sender");
                    }
                    catch (ArgumentNullException) {
                    }
                }

            }
            finally {
                // Stop and Dispose boundary activity
                la.Dispose();
            }
        }
    }
}

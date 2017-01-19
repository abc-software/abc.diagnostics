using System;
using System.Diagnostics;
using System.Windows.Forms;
using Diagnostic;

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
            DiagnosticTools.LogUtil.Write("Simple Message");
        }

        /// <summary>
        /// Write log with parameters 
        /// </summary>
        private void button2_Click(object sender, EventArgs e) {
            DiagnosticTools.LogUtil.Write("Simple Message with Parameters", Category, -1, 100, TraceEventType.Information);
            DiagnosticTools.LogUtil.Write("Exception to Log", Category, -1, 100, TraceEventType.Error, new ArgumentNullException("sender"));

            // Extension Function.
            DiagnosticTools.LogUtil.Write("message", -1);
        }

        /// <summary>
        /// Log Exception (By default "General" Category)
        /// </summary>
        private void button3_Click(object sender, EventArgs e) {
            try {
                DiagnosticTools.ExceptionUtil.ThrowHelperArgumentNull("sender");
            }
            catch (ArgumentNullException) {
            }
        }

        /// <summary>
        /// Use simple method execution trasing (By default "Trace" Category)
        /// </summary>
        private void button4_Click(object sender, EventArgs e) {
            // Create traser ("ButtonClick" Category)
            using (new TraceUtility("ButtonClick")) {
                // Log Message
                DiagnosticTools.LogUtil.Write("Button4", "ButtonClick");

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
            // Create and start activity
            using (LogActivity my = LogActivity.CreateActivity()) {
                my.Start("My Activity", "MyActivity");

                // Log Message with activityId
                DiagnosticTools.LogUtil.Write("My Activity Message", Category, -1, 100, TraceEventType.Information, my.Id);

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
            // Create and start boundary activity
            LogActivity la = LogActivity.CreateBoundedActivity();
            try {
                la.Start("Root activity", "RootActivity");

                // Log Message with activityId
                DiagnosticTools.LogUtil.Write("Root Message");

                // Create and start boundary activity
                using (LogActivity ba = LogActivity.CreateBoundedActivity(true)) {
                    ba.Start("Nested activity", "Nested Activity");

                    // Log Message with activityId
                    DiagnosticTools.LogUtil.Write("NestedMessage");

                    // Generate exception with activityId
                    try {
                        DiagnosticTools.ExceptionUtil.ThrowHelperArgumentNull("param");
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

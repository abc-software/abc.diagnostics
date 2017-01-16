using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Diagnostic;

namespace WcfClient
{
    public partial class Form1 : Form
    {      
        LogUtility logWriter = new LogUtility("WCFClient");

        public Form1()
        {
            InitializeComponent();
        }

        private void SyncGetDataButton_Click(object sender, EventArgs e)
        {
            int i;
            string rez;
            LogActivity la = LogActivity.CreateBoundedActivity();
            la.Start("SyncCall", "FromClient");
            using (ServiceRef.Service1Client c = new ServiceRef.Service1Client())
            {
                if (!int.TryParse(InputTextBox.Text, out i))
                {
                    OutputTextBox.Text = "Ievadīt skaitli!";
                }
                else
                {
                    logWriter.Write("Entered: " + i.ToString(), "Activity");
                    rez = c.GetData(i);
                    logWriter.Write("Response: " + rez, "Activity");
                    OutputTextBox.Text = rez;
                }
            }
            la.Stop();
        }

        private void AsyncGetDataButton_Click(object sender, EventArgs e)
        {
            int i;
             
            LogActivity l = LogActivity.CreateAsyncActivity();
            using (LogActivity la = LogActivity.CreateBoundedActivity(l.Id))
            {
                la.Start("AsyncCall", "FromClient");

                if (!int.TryParse(InputTextBox.Text, out i))
                {
                    OutputTextBox.Text = "Ievadīt skaitli!";
                }
                else
                {
                    logWriter.Write("Async Entered: " + i.ToString(), "Activity");
                    ServiceRef.Service1Client c = new ServiceRef.Service1Client();

                    l.Suspend();
                    c.GetDataAsync(Convert.ToInt32(this.InputTextBox.Text), l);                 
                    c.GetDataCompleted += new EventHandler<ServiceRef.GetDataCompletedEventArgs>(c_GetDataCompleted);                    
                }
            }
        }

        private void c_GetDataCompleted(object sender, ServiceRef.GetDataCompletedEventArgs e)
        {
            LogActivity l = e.UserState as LogActivity;
            l.Resume(); 
            this.OutputTextBox.Text = e.Result;  
            logWriter.Write("Async Response: " + e.Result, "Activity", -1, -1, System.Diagnostics.TraceEventType.Verbose, l.Id);
            l.Stop();  
        }
    }
}

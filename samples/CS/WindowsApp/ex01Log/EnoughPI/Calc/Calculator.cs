using System;
using System.Configuration;
using System.Windows.Forms;
using System.Text;
using EnoughPI.Logging;

using Diagnostic;

// TODO: Use Diagnostic Library 

namespace EnoughPI.Calc
{
    public class Calculator
    {
        LogUtility logwriter = new LogUtility("EnoughPI");
        

        public event CalculatedEventHandler Calculated;
        public event CalculatingEventHandler Calculating;
        public event CalculatorExceptionEventHandler CalculatorException;
        
        private delegate string CalculateDelegate(int digits);
        private CalculateDelegate dlg;

        public IAsyncResult BeginCalculate(int digits, AsyncCallback callback) 
        {
            dlg = new CalculateDelegate(this.Calculate);
            return dlg.BeginInvoke(digits, callback, this);
        }
        
        public IAsyncResult BeginCalculate(int digits) 
        {
            dlg = new CalculateDelegate(this.Calculate);
            AsyncCallback callback = new AsyncCallback(this.CalculateCallback);
            return dlg.BeginInvoke(digits, callback, dlg);
        }

        private void CalculateCallback(IAsyncResult ar)
        {
            CalculateDelegate dlg = (CalculateDelegate) ar.AsyncState;
            dlg.EndInvoke(ar);
        }

        public string EndCalculate(IAsyncResult ar)
        {
            return dlg.EndInvoke(ar);
        }

        public string Calculate(int digits)
        {
            StringBuilder pi = new StringBuilder("3", digits + 2);
            string result = null;
 
            try
            {
                if (digits > 0)
                {
                    // TODO: Add Tracing around the calculation
                    
                    pi.Append(".");
                    for (int i = 0; i < digits; i += 9) 
                    {
                        CalculatingEventArgs args;
                        args = new CalculatingEventArgs(pi.ToString(), i + 1);
                        OnCalculating(args);
 
                        // Break out if cancelled
                        if (args.Cancel == true) break;
 
                        // Calculate next 9 digits
                        int nineDigits = NineDigitsOfPi.StartingAt(i+1);
                        int digitCount = Math.Min(digits - i, 9);
                        string ds = string.Format("{0:D9}", nineDigits);
                        pi.Append(ds.Substring(0, digitCount));
                    }
                }
 
                result = pi.ToString();
 
                // Tell the world I've finished!
                OnCalculated(new CalculatedEventArgs(result));
            }
            catch (Exception ex)
            {
                // Tell the world I've crashed!
                OnCalculatorException(new CalculatorExceptionEventArgs(ex));
            }
 
            return result;
        }

        protected void OnCalculated(CalculatedEventArgs args)
        {
            // TODO: Log final result                 
            string message = string.Format("Rezultâts: Pi = {0}, precizitâte = {1}", args.Pi, args.Digits);
            logwriter.Write(message, Category.General, Priority.Normal, 1, System.Diagnostics.TraceEventType.Information);
            
            if (Calculated != null)
                Calculated(this, args);
        }

        protected void OnCalculating(CalculatingEventArgs args)
        {
            // TODO: Log progress
            string message = string.Format("Process: Precizitâte = {0}", args.Pi);
            logwriter.Write(message, Category.Progress, Priority.Normal, 2, System.Diagnostics.TraceEventType.Information);
 
            if (Calculating != null)
                Calculating(this, args);
 
            if (args.Cancel == true)
            {
                // TODO: Log cancellation
                logwriter.Write("Lietotâjs pârtrauca aprçíinu", Category.General, Priority.Normal, 3, System.Diagnostics.TraceEventType.Information);
            }
        }

        protected void OnCalculatorException(CalculatorExceptionEventArgs args)
        {
           
            // TODO: Log exception
            logwriter.Write("Izòçmums", Category.General, Priority.High, 4, System.Diagnostics.TraceEventType.Error, args.Exception);


            if (CalculatorException != null)
                CalculatorException(this, args);
        }    
    }
}

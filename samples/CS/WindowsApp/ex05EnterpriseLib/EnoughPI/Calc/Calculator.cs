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
        LogUtility logWriter = new LogUtility("Calculator");
        ExceptionUtility exceptionWriter = new ExceptionUtility(new LogUtility("Calculator"));

        public event CalculatedEventHandler Calculated;
        public event CalculatingEventHandler Calculating;
        public event CalculatorExceptionEventHandler CalculatorException;
        
        private delegate string CalculateDelegate(int digits);
        private CalculateDelegate dlg;
        private LogActivity la;

        public IAsyncResult BeginCalculate(int digits, AsyncCallback callback) 
        {
            dlg = new CalculateDelegate(this.Calculate);
            return dlg.BeginInvoke(digits, callback, this);
        }

        public IAsyncResult BeginCalculate(int digits)
        {
            la = LogActivity.CreateBoundedActivity();
            la.Start("BeginCalculate", "type");
            dlg = new CalculateDelegate(this.Calculate);
            AsyncCallback callback = new AsyncCallback(this.CalculateCallback);
            return dlg.BeginInvoke(digits, callback, new object[] { dlg, la });            
        }


        private void CalculateCallback(IAsyncResult ar)
        {
            CalculateDelegate dlg = (CalculateDelegate)(ar.AsyncState as object[])[0];
            dlg.EndInvoke(ar);
            LogActivity la = (LogActivity)(ar.AsyncState as object[])[1];
            la.Dispose();
        }


        public string EndCalculate(IAsyncResult ar)
        {
            return dlg.EndInvoke(ar);
        }

        public string Calculate(int digits)
        {
            LogActivity.UseDiagnosticTrace(new LogUtility("EnoughPI"));

            StringBuilder pi = new StringBuilder("3", digits + 2);
            string result = null;
 
            try
            {
                if (digits > 0)
                {
                    // TODO: Add Tracing around the calculation

                    using (TraceUtility.StartTrace(Category.Trace))
                    {
                        pi.Append(".");
                        for (int i = 0; i < digits; i += 9)
                        {
                            CalculatingEventArgs args;
                            args = new CalculatingEventArgs(pi.ToString(), i + 1);
                            OnCalculating(args);

                            // Break out if cancelled
                            if (args.Cancel == true) break;

                            // Calculate next 9 digits
                            int nineDigits = NineDigitsOfPi.StartingAt(i + 1);
                            int digitCount = Math.Min(digits - i, 9);
                            string ds = string.Format("{0:D9}", nineDigits);
                            pi.Append(ds.Substring(0, digitCount));
                        }
                    }
                }

                int x = 0;
                int y = 5 / x;

 
                result = pi.ToString();
 
                // Tell the world I've finished!
                OnCalculated(new CalculatedEventArgs(result));
            }
            catch (Exception ex)
            {
                // Tell the world I've crashed!
                exceptionWriter.ThrowHelperError(ex);
                OnCalculatorException(new CalculatorExceptionEventArgs(ex));
            }
 
            return result;
        }




        protected void OnCalculated(CalculatedEventArgs args)
        {
            // TODO: Log final result
            using (var la = LogActivity.CreateBoundedActivity(false))
            {
                la.Start("Calculated", "type");

                System.Collections.Generic.Dictionary<string, object> p = new System.Collections.Generic.Dictionary<string, object>();
                var x = new ExtraInfoProvider(args.Pi, args.Digits);
                x.PopulateDictionary(p);                

                //logWriter.Write("Rezûltâts: " + args.Pi.ToString());
                logWriter.Write("Result", p);

                if (Calculated != null)
                    Calculated(this, args);
            }
        }

        protected void OnCalculating(CalculatingEventArgs args)
        {
            // TODO: Log progress
            System.Collections.Generic.Dictionary<string, object> p = new System.Collections.Generic.Dictionary<string, object>();
            var x = new ExtraInfoProvider(args.Pi, args.Pi.Length - 2);
            x.PopulateDictionary(p);
            logWriter.Write("Calculating", p);

            if (Calculating != null)
                Calculating(this, args);

            if (args.Cancel == true)
            {
                // TODO: Log cancellation
                logWriter.Write("Calncelled by user!");
            }
        }

        protected void OnCalculatorException(CalculatorExceptionEventArgs args)
        {
            LogActivity.CreateBoundedActivity(false).Start("CalculatorException", "type");

            // TODO: Log exception
            
            if (CalculatorException != null)
                CalculatorException(this, args);
            LogActivity.Current.Dispose();
        }    
    }
}

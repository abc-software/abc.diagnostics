using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WcfClient
{
    public partial class Form1 : Form
    {    
        public Form1()
        {
            InitializeComponent();
        }

        private void SyncGetDataButton_Click(object sender, EventArgs e)
        {
            int i;
            string rez;          
          
            using (ServiceRef.Service1Client c = new ServiceRef.Service1Client())
            {
                if (!int.TryParse(InputTextBox.Text, out i))
                {
                    OutputTextBox.Text = "Ievadīt skaitli!";
                }
                else
                {                  
                    rez = c.GetData(i);                  
                    OutputTextBox.Text = rez;
                }
            }          
        }

        private void AsyncGetDataButton_Click(object sender, EventArgs e)
        {
            int i;

            if (!int.TryParse(InputTextBox.Text, out i))
            {
                OutputTextBox.Text = "Ievadīt skaitli!";
            }
            else
            {
                ServiceRef.Service1Client c = new ServiceRef.Service1Client();
                c.GetDataAsync(Convert.ToInt32(this.InputTextBox.Text));
                c.GetDataCompleted += new EventHandler<ServiceRef.GetDataCompletedEventArgs>(c_GetDataCompleted);
            }
        }

        private void c_GetDataCompleted(object sender, ServiceRef.GetDataCompletedEventArgs e)
        {           
            this.OutputTextBox.Text = e.Result;             
        }
    }
}

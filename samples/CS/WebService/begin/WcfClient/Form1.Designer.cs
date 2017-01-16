namespace WcfClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SyncGetDataButton = new System.Windows.Forms.Button();
            this.InputTextBox = new System.Windows.Forms.TextBox();
            this.OutputTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AsyncGetDataButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SyncGetDataButton
            // 
            this.SyncGetDataButton.Location = new System.Drawing.Point(36, 68);
            this.SyncGetDataButton.Name = "SyncGetDataButton";
            this.SyncGetDataButton.Size = new System.Drawing.Size(104, 23);
            this.SyncGetDataButton.TabIndex = 0;
            this.SyncGetDataButton.Text = "Sync GetData";
            this.SyncGetDataButton.UseVisualStyleBackColor = true;
            this.SyncGetDataButton.Click += new System.EventHandler(this.SyncGetDataButton_Click);
            // 
            // InputTextBox
            // 
            this.InputTextBox.Location = new System.Drawing.Point(112, 23);
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.Size = new System.Drawing.Size(138, 20);
            this.InputTextBox.TabIndex = 1;
            // 
            // OutputTextBox
            // 
            this.OutputTextBox.Enabled = false;
            this.OutputTextBox.Location = new System.Drawing.Point(36, 113);
            this.OutputTextBox.Name = "OutputTextBox";
            this.OutputTextBox.Size = new System.Drawing.Size(214, 20);
            this.OutputTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Enter number:";
            // 
            // AsyncGetDataButton
            // 
            this.AsyncGetDataButton.Location = new System.Drawing.Point(146, 68);
            this.AsyncGetDataButton.Name = "AsyncGetDataButton";
            this.AsyncGetDataButton.Size = new System.Drawing.Size(104, 23);
            this.AsyncGetDataButton.TabIndex = 4;
            this.AsyncGetDataButton.Text = "Async GetData";
            this.AsyncGetDataButton.UseVisualStyleBackColor = true;
            this.AsyncGetDataButton.Click += new System.EventHandler(this.AsyncGetDataButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 154);
            this.Controls.Add(this.AsyncGetDataButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OutputTextBox);
            this.Controls.Add(this.InputTextBox);
            this.Controls.Add(this.SyncGetDataButton);
            this.Name = "Form1";
            this.Text = "Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SyncGetDataButton;
        private System.Windows.Forms.TextBox InputTextBox;
        private System.Windows.Forms.TextBox OutputTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button AsyncGetDataButton;
    }
}


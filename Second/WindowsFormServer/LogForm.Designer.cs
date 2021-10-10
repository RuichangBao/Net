
namespace WindowsFormServer
{
    partial class LogForm
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
            this.labLog = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labLog
            // 
            this.labLog.AutoSize = true;
            this.labLog.Location = new System.Drawing.Point(110, 57);
            this.labLog.Name = "labLog";
            this.labLog.Size = new System.Drawing.Size(696, 17);
            this.labLog.TabIndex = 0;
            this.labLog.Text = "fdddddddddffff444444444444444444444444444444444444444444444444fffffffffffffffffff" +
    "ffffffffffffffffffffffffffdddddddddd";
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.labLog);
            this.Name = "LogForm";
            this.Text = "日志";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label labLog;
    }
}
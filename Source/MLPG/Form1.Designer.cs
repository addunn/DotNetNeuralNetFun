namespace MLPG
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
            this.components = new System.ComponentModel.Container();
            this.mainTimer = new System.Windows.Forms.Timer(this.components);
            this.commandWatcher = new System.Windows.Forms.Timer(this.components);
            this.projectDetector = new System.Windows.Forms.Timer(this.components);
            this.nodeTrainingRefresh = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // mainTimer
            // 
            this.mainTimer.Enabled = true;
            this.mainTimer.Interval = 10;
            this.mainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
            // 
            // commandWatcher
            // 
            this.commandWatcher.Enabled = true;
            this.commandWatcher.Interval = 20;
            this.commandWatcher.Tick += new System.EventHandler(this.CommandWatcher_Tick);
            // 
            // projectDetector
            // 
            this.projectDetector.Enabled = true;
            this.projectDetector.Interval = 20;
            this.projectDetector.Tick += new System.EventHandler(this.ProjectDetector_Tick);
            // 
            // nodeTrainingRefresh
            // 
            this.nodeTrainingRefresh.Enabled = true;
            this.nodeTrainingRefresh.Interval = 5;
            this.nodeTrainingRefresh.Tick += new System.EventHandler(this.NodeTrainingRefresh_Tick);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1304, 631);
            this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ML Playground";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeBegin += new System.EventHandler(this.Form1_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer mainTimer;
        private System.Windows.Forms.Timer commandWatcher;
        private System.Windows.Forms.Timer projectDetector;
        private System.Windows.Forms.Timer nodeTrainingRefresh;
    }
}


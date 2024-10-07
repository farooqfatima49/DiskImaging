namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox comboBoxDrives;
        private System.Windows.Forms.Button btnStartImaging;
       
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
            this.comboBoxDrives = new System.Windows.Forms.ComboBox();
            this.btnStartImaging = new System.Windows.Forms.Button();         
            this.SuspendLayout();
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            this.comboBoxDrives.FormattingEnabled = true;
            this.comboBoxDrives.Location = new System.Drawing.Point(12, 12);
            this.comboBoxDrives.Name = "comboBoxDrives";
            this.comboBoxDrives.Size = new System.Drawing.Size(260, 21);
            this.comboBoxDrives.TabIndex = 0;

            // 
            // btnStartImaging
            // 
            this.btnStartImaging.Location = new System.Drawing.Point(12, 39);
            this.btnStartImaging.Name = "btnStartImaging";
            this.btnStartImaging.Size = new System.Drawing.Size(260, 23);
            this.btnStartImaging.TabIndex = 1;
            this.btnStartImaging.Text = "Start Imaging";
            this.btnStartImaging.UseVisualStyleBackColor = true;
            this.btnStartImaging.Click += new System.EventHandler(this.btnStartImaging_Click);              

            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(284, 101); // Adjusted the height to fit the new button            
            this.Controls.Add(this.btnStartImaging);
            this.Controls.Add(this.comboBoxDrives);
            this.Name = "Form1";
            this.Text = "Disk Imager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
        }

        #endregion
    }
}


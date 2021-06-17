namespace bunifu
{
    partial class Setnotice
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.noctice1 = new bunifu.Noctice();
            this.SuspendLayout();
            // 
            // noctice1
            // 
            this.noctice1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.noctice1.Dock = System.Windows.Forms.DockStyle.Top;
            this.noctice1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noctice1.Location = new System.Drawing.Point(0, 0);
            this.noctice1.Name = "noctice1";
            this.noctice1.Size = new System.Drawing.Size(871, 93);
            this.noctice1.TabIndex = 0;
            this.noctice1.Visible = false;
            this.noctice1.Load += new System.EventHandler(this.noctice1_Load);
            // 
            // Setnotice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.noctice1);
            this.Name = "Setnotice";
            this.Size = new System.Drawing.Size(871, 584);
            this.ResumeLayout(false);

        }

        #endregion

        private Noctice noctice1;
    }
}

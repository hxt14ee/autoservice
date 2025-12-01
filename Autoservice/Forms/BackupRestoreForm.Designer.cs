namespace Autoservice.Forms
{
    partial class BackupRestoreForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 350);
            this.Name = "BackupRestoreForm";
            this.Text = "Резервная копия";
            this.Load += new System.EventHandler(this.BackupRestoreForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BackupRestoreForm_FormClosing);
            this.ResumeLayout(false);
        }
    }
}

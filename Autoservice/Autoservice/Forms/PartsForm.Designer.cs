
namespace Autoservice.Forms
{
    partial class PartsForm
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
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Name = "PartsForm";
            this.Text = "Запчасти";
            this.Load += new System.EventHandler(this.PartsForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PartsForm_FormClosing);
            this.ResumeLayout(false);
        }
    }
}

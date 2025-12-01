
namespace Autoservice.Forms
{
    partial class CarsForm
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
            this.ClientSize = new System.Drawing.Size(1000, 500);
            this.Name = "CarsForm";
            this.Text = "Автомобили";
            this.Load += new System.EventHandler(this.CarsForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CarsForm_FormClosing);
            this.ResumeLayout(false);
        }
    }
}

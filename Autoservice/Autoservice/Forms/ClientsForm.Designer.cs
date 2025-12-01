namespace Autoservice.Forms
{
    partial class ClientsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Name = "ClientsForm";
            this.Text = "Управление клиентами";
            this.Load += new System.EventHandler(this.ClientsForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientsForm_FormClosing);
            this.ResumeLayout(false);
        }
    }
}

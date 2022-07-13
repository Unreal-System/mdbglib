namespace ASDumpAnalyzer
{
    partial class WizForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizForm));
            this.wizardControl1 = new WizardBase.WizardControl();
            this.intermediateStep1 = new WizardBase.IntermediateStep();
            this.intermediateStep2 = new WizardBase.IntermediateStep();
            this.SuspendLayout();
            // 
            // wizardControl1
            // 
            this.wizardControl1.BackButtonEnabled = true;
            this.wizardControl1.BackButtonVisible = true;
            this.wizardControl1.CancelButtonEnabled = true;
            this.wizardControl1.CancelButtonVisible = true;
            this.wizardControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardControl1.EulaButtonEnabled = false;
            this.wizardControl1.EulaButtonText = "eula";
            this.wizardControl1.EulaButtonVisible = false;
            this.wizardControl1.HelpButtonEnabled = false;
            this.wizardControl1.HelpButtonVisible = false;
            this.wizardControl1.Location = new System.Drawing.Point(0, 0);
            this.wizardControl1.Name = "wizardControl1";
            this.wizardControl1.NextButtonEnabled = true;
            this.wizardControl1.NextButtonVisible = true;
            this.wizardControl1.Size = new System.Drawing.Size(554, 575);
            this.wizardControl1.WizardSteps.AddRange(new WizardBase.WizardStep[] {
            this.intermediateStep1,
            this.intermediateStep2});
            this.wizardControl1.NextButtonClick += new WizardBase.GenericCancelEventHandler<WizardBase.WizardControl>(this.wizardControl1_NextButtonClick);
            this.wizardControl1.CancelButtonClick += new System.EventHandler(this.wizardControl1_CancelButtonClick);
            // 
            // intermediateStep1
            // 
            this.intermediateStep1.BindingImage = ((System.Drawing.Image)(resources.GetObject("intermediateStep1.BindingImage")));
            this.intermediateStep1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.intermediateStep1.Name = "intermediateStep1";
            this.intermediateStep1.Subtitle = resources.GetString("intermediateStep1.Subtitle");
            this.intermediateStep1.Title = "Specify Dump and Symbols";
            // 
            // intermediateStep2
            // 
            this.intermediateStep2.BindingImage = ((System.Drawing.Image)(resources.GetObject("intermediateStep2.BindingImage")));
            this.intermediateStep2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.intermediateStep2.Name = "intermediateStep2";
            // 
            // WizForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 575);
            this.Controls.Add(this.wizardControl1);
            this.Name = "WizForm";
            this.Text = "WizForm";
            this.ResumeLayout(false);

        }

        #endregion

        private WizardBase.WizardControl wizardControl1;
        private WizardBase.IntermediateStep intermediateStep1;
        private WizardBase.IntermediateStep intermediateStep2;
    }
}
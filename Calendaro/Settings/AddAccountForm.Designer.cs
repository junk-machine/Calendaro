namespace Calendaro.Settings
{
    partial class AddAccountForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddAccountForm));
            this.lblCalendarServiceType = new System.Windows.Forms.Label();
            this.comboCalendarServiceType = new System.Windows.Forms.ComboBox();
            this.lblAccountId = new System.Windows.Forms.Label();
            this.txtAccountId = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCalendarServiceType
            // 
            resources.ApplyResources(this.lblCalendarServiceType, "lblCalendarServiceType");
            this.lblCalendarServiceType.Name = "lblCalendarServiceType";
            // 
            // comboCalendarServiceType
            // 
            resources.ApplyResources(this.comboCalendarServiceType, "comboCalendarServiceType");
            this.comboCalendarServiceType.DisplayMember = "DisplayName";
            this.comboCalendarServiceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCalendarServiceType.FormattingEnabled = true;
            this.comboCalendarServiceType.Name = "comboCalendarServiceType";
            this.comboCalendarServiceType.ValueMember = "Value";
            this.comboCalendarServiceType.SelectedIndexChanged += new System.EventHandler(this.ValidateInput);
            // 
            // lblAccountId
            // 
            resources.ApplyResources(this.lblAccountId, "lblAccountId");
            this.lblAccountId.Name = "lblAccountId";
            // 
            // txtAccountId
            // 
            resources.ApplyResources(this.txtAccountId, "txtAccountId");
            this.txtAccountId.Name = "txtAccountId";
            this.txtAccountId.TextChanged += new System.EventHandler(this.ValidateInput);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // AddAccountForm
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtAccountId);
            this.Controls.Add(this.lblAccountId);
            this.Controls.Add(this.comboCalendarServiceType);
            this.Controls.Add(this.lblCalendarServiceType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::Calendaro.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddAccountForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lblCalendarServiceType;
        private ComboBox comboCalendarServiceType;
        private Label lblAccountId;
        private TextBox txtAccountId;
        private Button btnCancel;
        private Button btnOk;
    }
}
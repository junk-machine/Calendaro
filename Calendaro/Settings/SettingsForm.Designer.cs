namespace Calendaro.Settings
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.treeCalendars = new UI.ExplicitExpandTreeView();
            this.imagesCalendarsTree = new System.Windows.Forms.ImageList(this.components);
            this.btnAdd = new System.Windows.Forms.Button();
            this.tabsSettings = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.tabCalendars = new System.Windows.Forms.TabPage();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.menuAccount = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miEditAccount = new System.Windows.Forms.ToolStripMenuItem();
            this.miRemoveAccount = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCalendar = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miChangeCalendarColor = new System.Windows.Forms.ToolStripMenuItem();
            this.miRemoveCalendar = new System.Windows.Forms.ToolStripMenuItem();
            this.colorPicker = new System.Windows.Forms.ColorDialog();
            this.tabsSettings.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabCalendars.SuspendLayout();
            this.menuAccount.SuspendLayout();
            this.menuCalendar.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeCalendars
            // 
            resources.ApplyResources(this.treeCalendars, "treeCalendars");
            this.treeCalendars.ImageList = this.imagesCalendarsTree;
            this.treeCalendars.Name = "treeCalendars";
            this.treeCalendars.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.HandleCalendarsTreeNodeMouseClick);
            this.treeCalendars.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.HandleCalendarsTreeNodeMouseDoubleClick);
            // 
            // imagesCalendarsTree
            // 
            this.imagesCalendarsTree.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.imagesCalendarsTree, "imagesCalendarsTree");
            this.imagesCalendarsTree.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.AddAccount);
            // 
            // tabsSettings
            // 
            resources.ApplyResources(this.tabsSettings, "tabsSettings");
            this.tabsSettings.Controls.Add(this.tabGeneral);
            this.tabsSettings.Controls.Add(this.tabCalendars);
            this.tabsSettings.Name = "tabsSettings";
            this.tabsSettings.SelectedIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.chkAutoStart);
            resources.ApplyResources(this.tabGeneral, "tabGeneral");
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // chkAutoStart
            // 
            resources.ApplyResources(this.chkAutoStart, "chkAutoStart");
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // tabCalendars
            // 
            this.tabCalendars.Controls.Add(this.treeCalendars);
            this.tabCalendars.Controls.Add(this.btnAdd);
            resources.ApplyResources(this.tabCalendars, "tabCalendars");
            this.tabCalendars.Name = "tabCalendars";
            this.tabCalendars.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // menuAccount
            // 
            this.menuAccount.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuAccount.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miEditAccount,
            this.miRemoveAccount});
            this.menuAccount.Name = "menuAccount";
            resources.ApplyResources(this.menuAccount, "menuAccount");
            // 
            // miEditAccount
            // 
            this.miEditAccount.Name = "miEditAccount";
            resources.ApplyResources(this.miEditAccount, "miEditAccount");
            this.miEditAccount.Click += new System.EventHandler(this.EditSelectedAccount);
            // 
            // miRemoveAccount
            // 
            this.miRemoveAccount.Name = "miRemoveAccount";
            resources.ApplyResources(this.miRemoveAccount, "miRemoveAccount");
            this.miRemoveAccount.Click += new System.EventHandler(this.RemoveSelectedAccount);
            // 
            // menuCalendar
            // 
            this.menuCalendar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuCalendar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miChangeCalendarColor,
            this.miRemoveCalendar});
            this.menuCalendar.Name = "menuCalendar";
            resources.ApplyResources(this.menuCalendar, "menuCalendar");
            // 
            // miChangeCalendarColor
            // 
            this.miChangeCalendarColor.Name = "miChangeCalendarColor";
            resources.ApplyResources(this.miChangeCalendarColor, "miChangeCalendarColor");
            this.miChangeCalendarColor.Click += new System.EventHandler(this.ChangeCalendarColor);
            // 
            // miRemoveCalendar
            // 
            this.miRemoveCalendar.Name = "miRemoveCalendar";
            resources.ApplyResources(this.miRemoveCalendar, "miRemoveCalendar");
            this.miRemoveCalendar.Click += new System.EventHandler(this.RemoveSelectedCalendar);
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tabsSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::Calendaro.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.tabsSettings.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabCalendars.ResumeLayout(false);
            this.menuAccount.ResumeLayout(false);
            this.menuCalendar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private UI.ExplicitExpandTreeView treeCalendars;
        private Button btnAdd;
        private TabControl tabsSettings;
        private TabPage tabGeneral;
        private CheckBox chkAutoStart;
        private TabPage tabCalendars;
        private Button btnOk;
        private Button btnCancel;
        private ContextMenuStrip menuAccount;
        private ToolStripMenuItem miEditAccount;
        private ToolStripMenuItem miRemoveAccount;
        private ContextMenuStrip menuCalendar;
        private ToolStripMenuItem miRemoveCalendar;
        private ImageList imagesCalendarsTree;
        private ToolStripMenuItem miChangeCalendarColor;
        private ColorDialog colorPicker;
    }
}
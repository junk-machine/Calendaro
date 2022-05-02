namespace Calendaro
{
    partial class SyncStatusForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SyncStatusForm));
            this.treeCalendars = new Calendaro.UI.ExplicitExpandTreeView();
            this.imagesCalendarsTree = new System.Windows.Forms.ImageList(this.components);
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeCalendars
            // 
            resources.ApplyResources(this.treeCalendars, "treeCalendars");
            this.treeCalendars.ImageList = this.imagesCalendarsTree;
            this.treeCalendars.Name = "treeCalendars";
            this.treeCalendars.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.HandleCalendarsTreeNodeMouseDoubleClick);
            // 
            // imagesCalendarsTree
            // 
            this.imagesCalendarsTree.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.imagesCalendarsTree, "imagesCalendarsTree");
            this.imagesCalendarsTree.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // SyncStatusForm
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOk;
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.treeCalendars);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::Calendaro.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.Name = "SyncStatusForm";
            this.ResumeLayout(false);

        }

        #endregion

        private UI.ExplicitExpandTreeView treeCalendars;
        private Button btnOk;
        private ImageList imagesCalendarsTree;
    }
}
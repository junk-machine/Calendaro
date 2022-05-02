namespace Calendaro
{
    partial class NotificationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotificationForm));
            this.pictureEvent = new System.Windows.Forms.PictureBox();
            this.panelEvents = new System.Windows.Forms.Panel();
            this.btnJoinOnline = new System.Windows.Forms.Button();
            this.btnDismiss = new System.Windows.Forms.Button();
            this.listEvents = new System.Windows.Forms.ListView();
            this.columnEventName = new System.Windows.Forms.ColumnHeader();
            this.columnTimeLeft = new System.Windows.Forms.ColumnHeader();
            this.imagesCalendars = new System.Windows.Forms.ImageList(this.components);
            this.labelEventTime = new System.Windows.Forms.Label();
            this.labelEventName = new System.Windows.Forms.Label();
            this.labelSnooze = new System.Windows.Forms.Label();
            this.comboSnoozeInterval = new System.Windows.Forms.ComboBox();
            this.btnSnooze = new System.Windows.Forms.Button();
            this.btnDismissAll = new System.Windows.Forms.Button();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miSyncStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorExit = new System.Windows.Forms.ToolStripSeparator();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEvent)).BeginInit();
            this.panelEvents.SuspendLayout();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureEvent
            // 
            this.pictureEvent.BackgroundImage = global::Calendaro.Properties.Resources.calendar;
            resources.ApplyResources(this.pictureEvent, "pictureEvent");
            this.pictureEvent.Name = "pictureEvent";
            this.pictureEvent.TabStop = false;
            // 
            // panelEvents
            // 
            resources.ApplyResources(this.panelEvents, "panelEvents");
            this.panelEvents.BackColor = System.Drawing.SystemColors.Window;
            this.panelEvents.Controls.Add(this.btnJoinOnline);
            this.panelEvents.Controls.Add(this.btnDismiss);
            this.panelEvents.Controls.Add(this.listEvents);
            this.panelEvents.Controls.Add(this.labelEventTime);
            this.panelEvents.Controls.Add(this.labelEventName);
            this.panelEvents.Controls.Add(this.pictureEvent);
            this.panelEvents.Name = "panelEvents";
            // 
            // btnJoinOnline
            // 
            resources.ApplyResources(this.btnJoinOnline, "btnJoinOnline");
            this.btnJoinOnline.Name = "btnJoinOnline";
            this.btnJoinOnline.UseVisualStyleBackColor = true;
            this.btnJoinOnline.Click += new System.EventHandler(this.JoinOnlineConference);
            // 
            // btnDismiss
            // 
            resources.ApplyResources(this.btnDismiss, "btnDismiss");
            this.btnDismiss.Name = "btnDismiss";
            this.btnDismiss.UseVisualStyleBackColor = true;
            this.btnDismiss.Click += new System.EventHandler(this.DismissSelectedEvent);
            // 
            // listEvents
            // 
            resources.ApplyResources(this.listEvents, "listEvents");
            this.listEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnEventName,
            this.columnTimeLeft});
            this.listEvents.FullRowSelect = true;
            this.listEvents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listEvents.MultiSelect = false;
            this.listEvents.Name = "listEvents";
            this.listEvents.ShowGroups = false;
            this.listEvents.ShowItemToolTips = true;
            this.listEvents.SmallImageList = this.imagesCalendars;
            this.listEvents.UseCompatibleStateImageBehavior = false;
            this.listEvents.View = System.Windows.Forms.View.Details;
            this.listEvents.ItemActivate += new System.EventHandler(this.OpenEventDescription);
            this.listEvents.SelectedIndexChanged += new System.EventHandler(this.UpdateControlsState);
            // 
            // columnEventName
            // 
            resources.ApplyResources(this.columnEventName, "columnEventName");
            // 
            // columnTimeLeft
            // 
            resources.ApplyResources(this.columnTimeLeft, "columnTimeLeft");
            // 
            // imagesCalendars
            // 
            this.imagesCalendars.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.imagesCalendars, "imagesCalendars");
            this.imagesCalendars.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // labelEventTime
            // 
            resources.ApplyResources(this.labelEventTime, "labelEventTime");
            this.labelEventTime.AutoEllipsis = true;
            this.labelEventTime.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelEventTime.Name = "labelEventTime";
            // 
            // labelEventName
            // 
            resources.ApplyResources(this.labelEventName, "labelEventName");
            this.labelEventName.Name = "labelEventName";
            // 
            // labelSnooze
            // 
            resources.ApplyResources(this.labelSnooze, "labelSnooze");
            this.labelSnooze.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelSnooze.Name = "labelSnooze";
            // 
            // comboSnoozeInterval
            // 
            resources.ApplyResources(this.comboSnoozeInterval, "comboSnoozeInterval");
            this.comboSnoozeInterval.DisplayMember = "DisplayName";
            this.comboSnoozeInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSnoozeInterval.FormattingEnabled = true;
            this.comboSnoozeInterval.Name = "comboSnoozeInterval";
            this.comboSnoozeInterval.ValueMember = "Value";
            // 
            // btnSnooze
            // 
            resources.ApplyResources(this.btnSnooze, "btnSnooze");
            this.btnSnooze.Name = "btnSnooze";
            this.btnSnooze.UseVisualStyleBackColor = true;
            this.btnSnooze.Click += new System.EventHandler(this.SnoozeSelectedEvent);
            // 
            // btnDismissAll
            // 
            resources.ApplyResources(this.btnDismissAll, "btnDismissAll");
            this.btnDismissAll.Name = "btnDismissAll";
            this.btnDismissAll.UseVisualStyleBackColor = true;
            this.btnDismissAll.Click += new System.EventHandler(this.DismissAllEvents);
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.menuMain;
            this.trayIcon.Icon = global::Calendaro.Properties.Resources.icon;
            resources.ApplyResources(this.trayIcon, "trayIcon");
            this.trayIcon.DoubleClick += new System.EventHandler(this.OpenSyncStatus);
            // 
            // menuMain
            // 
            this.menuMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSettings,
            this.miSyncStatus,
            this.separatorExit,
            this.miExit});
            this.menuMain.Name = "menuMain";
            resources.ApplyResources(this.menuMain, "menuMain");
            // 
            // miSettings
            // 
            this.miSettings.Image = global::Calendaro.Properties.Resources.settings;
            this.miSettings.Name = "miSettings";
            resources.ApplyResources(this.miSettings, "miSettings");
            this.miSettings.Click += new System.EventHandler(this.OpenSettings);
            // 
            // miSyncStatus
            // 
            this.miSyncStatus.Name = "miSyncStatus";
            resources.ApplyResources(this.miSyncStatus, "miSyncStatus");
            this.miSyncStatus.Click += new System.EventHandler(this.OpenSyncStatus);
            // 
            // separatorExit
            // 
            this.separatorExit.Name = "separatorExit";
            resources.ApplyResources(this.separatorExit, "separatorExit");
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            resources.ApplyResources(this.miExit, "miExit");
            this.miExit.Click += new System.EventHandler(this.ExitApplication);
            // 
            // NotificationForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnDismissAll;
            this.Controls.Add(this.btnDismissAll);
            this.Controls.Add(this.btnSnooze);
            this.Controls.Add(this.comboSnoozeInterval);
            this.Controls.Add(this.labelSnooze);
            this.Controls.Add(this.panelEvents);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::Calendaro.Properties.Resources.reminder_icon;
            this.MaximizeBox = false;
            this.Name = "NotificationForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HandleFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEvent)).EndInit();
            this.panelEvents.ResumeLayout(false);
            this.menuMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox pictureEvent;
        private Panel panelEvents;
        private Label labelEventName;
        private Label labelEventTime;
        private ListView listEvents;
        private ColumnHeader columnEventName;
        private ColumnHeader columnTimeLeft;
        private Button btnDismiss;
        private Label labelSnooze;
        private ComboBox comboSnoozeInterval;
        private Button btnSnooze;
        private Button btnDismissAll;
        private NotifyIcon trayIcon;
        private ContextMenuStrip menuMain;
        private ToolStripMenuItem miSettings;
        private ToolStripSeparator separatorExit;
        private ToolStripMenuItem miExit;
        private ImageList imagesCalendars;
        private Button btnJoinOnline;
        private ToolStripMenuItem miSyncStatus;
    }
}
using Calendaro.EventsTracking;
using Calendaro.Properties;
using Calendaro.Settings;

namespace Calendaro
{
    /// <summary>
    /// Form that display synchronization status for configured calendars.
    /// </summary>
    internal partial class SyncStatusForm : Form
    {
        /// <summary>
        /// A key for the successful synchronization state icon.
        /// </summary>
        private const string SuccessIconKey = nameof(Resources.success);

        /// <summary>
        /// A key for the failed synchronization state icon.
        /// </summary>
        private const string FailureIconKey = nameof(Resources.failure);

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncStatusForm"/> class
        /// with the provided calendar service accounts configuration and synchronization errors.
        /// </summary>
        /// <param name="calendarAccounts">Collection of configured calendar service accounts.</param>
        /// <param name="syncErrors">Collection of calendar synchronization errors.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public SyncStatusForm(
            IList<CalendarAccountConfiguration> calendarAccounts,
            List<CalendarSyncException> syncErrors)
        {
            InitializeComponent();

            PopulateCalendarsTree(calendarAccounts, syncErrors);
        }

        /// <summary>
        /// Populates the tree control that displays the synchronization status of the configured calendars.
        /// </summary>
        /// <param name="calendarAccounts">Collection of configured calendar service accounts.</param>
        /// <param name="syncErrors">Collection of calendar synchronization errors.</param>
        private void PopulateCalendarsTree(
            IList<CalendarAccountConfiguration> calendarAccounts,
            List<CalendarSyncException> syncErrors)
        {
            treeCalendars.BeginUpdate();
            UpdateCalendarsTreeImageList();

            foreach (var calendarAccount in calendarAccounts)
            {
                var accountNode =
                    new TreeNode(calendarAccount.AccountId)
                    {
                        Tag = calendarAccount,
                        ImageIndex = (int)calendarAccount.CalendarServiceType,
                        SelectedImageIndex = (int)calendarAccount.CalendarServiceType,
                    };

                treeCalendars.Nodes.Add(accountNode);

                foreach (var calendar in calendarAccount.Calendars)
                {
                    var syncError =
                        syncErrors.FirstOrDefault(error =>
                            error.Account.AccountId == calendarAccount.AccountId
                            && error.Calendar.Id == calendar.Id);

                    accountNode.Nodes.Add(
                        new TreeNode(calendar.Name)
                        {
                            Tag = syncError,
                            ImageKey = syncError is null ? SuccessIconKey : FailureIconKey,
                            SelectedImageKey = syncError is null ? SuccessIconKey : FailureIconKey,
                        });
                }
            }

            treeCalendars.ExpandAll();
            treeCalendars.EndUpdate();
        }

        /// <summary>
        /// Populates an <see cref="ImageList"/> component that is used by the calendars tree control.
        /// </summary>
        private void UpdateCalendarsTreeImageList()
        {
            imagesCalendarsTree.Images.Clear();

            // Add icons for supported calendar services to the list
            // Zero index will be used by any unknown calendar service
            imagesCalendarsTree.Images.Add(Resources.calendar);
            imagesCalendarsTree.Images.Add(Resources.google);

            // Add success/failure icons to use for individual calendars
            imagesCalendarsTree.Images.Add(SuccessIconKey, Resources.success);
            imagesCalendarsTree.Images.Add(FailureIconKey, Resources.failure);
        }

        /// <summary>
        /// Displays the detailed calendar synchronization status in a separate dialog.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleCalendarsTreeNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // For the failed calendars - display synchronization error message
                if (e.Node.Tag is CalendarSyncException syncError)
                {
                    MessageBox.Show(this, syncError.Message, Resources.SyncFailedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

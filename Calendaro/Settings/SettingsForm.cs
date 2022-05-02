using Calendaro.Abstractions;
using Calendaro.CalendarServices;
using Calendaro.Properties;
using Calendaro.UI;
using Calendaro.Utilities;

namespace Calendaro.Settings
{
    /// <summary>
    /// Application settings form.
    /// </summary>
    internal partial class SettingsForm : Form
    {
        /// <summary>
        /// Factory used to create clients to access different calendar services.
        /// </summary>
        private readonly ICalendarServicesFactory calendarServicesFactory;

        /// <summary>
        /// Provider that populates image list with calendar icons.
        /// </summary>
        private readonly ICalendarIconsProvider calendarIconsProvider;

        /// <summary>
        /// Gets the configured application settings.
        /// </summary>
        public CalendaroSettings Settings { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsForm"/> class
        /// with the provided application settigs and calendar services factory.
        /// </summary>
        /// <param name="settings">Current application settings.</param>
        /// <param name="calendarServicesFactory">Factory used to create clients to access different calendar services.</param>
        /// <param name="calendarIconsProvider">Provider that populates image list with calendar icons.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public SettingsForm(
            CalendaroSettings settings,
            ICalendarServicesFactory calendarServicesFactory,
            ICalendarIconsProvider calendarIconsProvider)
        {
            Settings =
                settings ?? throw new ArgumentNullException(nameof(settings));

            this.calendarServicesFactory =
                calendarServicesFactory ?? throw new ArgumentNullException(nameof(calendarServicesFactory));

            this.calendarIconsProvider =
                calendarIconsProvider ?? throw new ArgumentNullException(nameof(calendarIconsProvider));

            InitializeComponent();

            PopulateControls();
        }

        /// <summary>
        /// Populates UI controls based on the current settings.
        /// </summary>
        private void PopulateControls()
        {
            // Populate settings on 'General' tab
            chkAutoStart.Checked = Settings.AutoStart;
            chkAutoStart.CheckedChanged += UpdateAutoStart;

            // Populate 'Calendars' tab
            UpdateCalendarsTree();
        }

        /// <summary>
        /// Populates calendars tree control from the current <see cref="Settings"/>.
        /// </summary>
        private void UpdateCalendarsTree()
        {
            UpdateCalendarsTreeImageList();

            treeCalendars.BeginUpdate();
            treeCalendars.Nodes.Clear();
            treeCalendars.Nodes.AddRange(
                new CalendarsTreeAdapter()
                    .AsTreeNodes(Settings.AccountsConfiguration));
            treeCalendars.ExpandAll();
            treeCalendars.EndUpdate();
        }

        /// <summary>
        /// Updates auto-start property in the current settings based on the checkbox control state.
        /// </summary>
        /// <param name="sender">Sender of the event/</param>
        /// <param name="e">Event arguments.</param>
        private void UpdateAutoStart(object? sender, EventArgs e)
        {
            Settings.AutoStart = chkAutoStart.Checked;
        }

        /// <summary>
        /// Starts the workflow to add new calendars account.
        /// </summary>
        /// <param name="sender">Sender of the event/</param>
        /// <param name="e">Event arguments.</param>
        private void AddAccount(object sender, EventArgs e)
        {
            // First display the form that collects account information from the user
            CalendarServiceType calendarServiceType;
            string accountId;

            using (var addAccountForm = new AddAccountForm())
            {
                if (addAccountForm.ShowDialog(this) != DialogResult.OK)
                {
                    // User cancelled the action
                    return;
                }

                calendarServiceType = addAccountForm.CalendarServiceType;
                accountId = addAccountForm.AccountId;
            }

            // Then display the form that allows the user to select specific calendars in the account
            EditAccount(calendarServiceType, accountId);
        }

        /// <summary>
        /// Displays context menu when a tree node is right-clicked in the calendars tree.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleCalendarsTreeNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Ensure node is selected, as by default right-click will not keep the selection
                treeCalendars.SelectedNode = e.Node;

                if (e.Node.Tag is CalendarAccountConfiguration)
                {
                    menuAccount.Show(treeCalendars, e.Location);
                }
                else if (e.Node.Tag is CalendarInfo)
                {
                    menuCalendar.Show(treeCalendars, e.Location);
                }
            }
        }

        /// <summary>
        /// Executes default action for the selected calendar node in the tree.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleCalendarsTreeNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Node.Tag is CalendarAccountConfiguration selectedAccount)
                {
                    EditAccount(selectedAccount.CalendarServiceType, selectedAccount.AccountId);
                }
                else
                {
                    ChangeCalendarColor(sender, e);
                }
            }
        }

        /// <summary>
        /// Modifies an account that is currently selected in the calendars tree control.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void EditSelectedAccount(object sender, EventArgs e)
        {
            if (treeCalendars.SelectedNode.Tag is CalendarAccountConfiguration selectedAccount)
            {
                EditAccount(selectedAccount.CalendarServiceType, selectedAccount.AccountId);
            }
        }

        /// <summary>
        /// Modifies calendars for the calendar service account.
        /// If account does not exist, then it will be created and added to the settings.
        /// </summary>
        /// <param name="calendarServiceType">Type of the calendar service.</param>
        /// <param name="accountId">Identifier of the calendar service account.</param>
        private void EditAccount(CalendarServiceType calendarServiceType, string accountId)
        {
            var existingAccountConfiguration =
                Settings.AccountsConfiguration
                    .FirstOrDefault(account =>
                        account.CalendarServiceType == calendarServiceType
                        && string.Equals(account.AccountId, accountId, StringComparison.OrdinalIgnoreCase));

            IReadOnlyList<CalendarInfo> selectedCalendars;

            using (var calendarsSelectionForm =
                new CalendarsSelectionForm(
                    calendarServicesFactory.Create(calendarServiceType, accountId),
                    existingAccountConfiguration?.Calendars.Select(c => c.Id).ToHashSet()))
            {
                if (calendarsSelectionForm.ShowDialog(this) != DialogResult.OK)
                {
                    // User cancelled the action
                    return;
                }

                selectedCalendars = calendarsSelectionForm.SelectedCalendars;
            }

            // Only add accounts if user selected at least one calendar
            if (selectedCalendars.Count > 0)
            {
                if (existingAccountConfiguration is null)
                {
                    // If it was a new account, then we persist it in settings
                    Settings.AccountsConfiguration.Add(
                        existingAccountConfiguration =
                            new CalendarAccountConfiguration(calendarServiceType, accountId));
                }

                existingAccountConfiguration.Calendars.Clear();
                existingAccountConfiguration.Calendars.AddRange(selectedCalendars);
            }
            else if (existingAccountConfiguration is not null)
            {
                // If no calendars were selected, but account already exists,
                // then we delete it, as we do not allow empty accounts
                Settings.AccountsConfiguration.Remove(existingAccountConfiguration);
            }
            else
            {
                // No modifications are necessary, exit here so that calendars tree is not refreshed
                return;
            }

            UpdateCalendarsTree();
        }

        /// <summary>
        /// Removes an account that is currently selected in the calendars tree control
        /// from the configuration.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void RemoveSelectedAccount(object sender, EventArgs e)
        {
            if (treeCalendars.SelectedNode.Tag is CalendarAccountConfiguration selectedAccount)
            {
                Settings.AccountsConfiguration.Remove(selectedAccount);
                treeCalendars.Nodes.Remove(treeCalendars.SelectedNode);
            }
        }

        /// <summary>
        /// Prompts the user for the new color for the calendar.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ChangeCalendarColor(object sender, EventArgs e)
        {
            if (treeCalendars.SelectedNode.Tag is CalendarInfo selectedCalendar
                && colorPicker.ShowDialog(this) == DialogResult.OK)
            {
                selectedCalendar.Color = colorPicker.Color;
                UpdateCalendarsTree();
            }
        }

        /// <summary>
        /// Removes a calendar that is currently selected in the calendars tree control
        /// from the configuration.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void RemoveSelectedCalendar(object sender, EventArgs e)
        {
            if (treeCalendars.SelectedNode.Tag is CalendarInfo selectedCalendar
                && treeCalendars.SelectedNode.Parent.Tag is CalendarAccountConfiguration parentAccount)
            {
                parentAccount.Calendars.Remove(selectedCalendar);

                var parentNode = treeCalendars.SelectedNode.Parent;
                parentNode.Nodes.Remove(treeCalendars.SelectedNode);

                if (parentNode.Nodes.Count <= 0)
                {
                    // If there are no more calendars left under the account, then remove it as well
                    treeCalendars.SelectedNode = parentNode;
                    RemoveSelectedAccount(sender, e);
                }
            }
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

            // Add calendar icons into the list
            calendarIconsProvider.PopulateImageList(
                Settings.AccountsConfiguration,
                imagesCalendarsTree);
        }
    }
}

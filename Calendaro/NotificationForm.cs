using Calendaro.CalendarServices;
using Calendaro.EventsTracking;
using Calendaro.Properties;
using Calendaro.Settings;
using Calendaro.UI;
using Calendaro.Utilities;
using System.Diagnostics;
using System.Media;

namespace Calendaro
{
    /// <summary>
    /// Form that displays upcoming events.
    /// </summary>
    /// <remarks>
    /// This is also a main form that maintains snoozed events and owns icon
    /// in the notification area (system tray).
    /// </remarks>
    internal partial class NotificationForm : AsyncFormBase
    {
        /// <summary>
        /// Provider used to save and restore application settings.
        /// </summary>
        private readonly ICalendaroSettingsProvider settingsProvider;

        /// <summary>
        /// Factory used to create clients for different calendar services.
        /// </summary>
        private readonly ICalendarServicesFactory calendarServicesFactory;

        /// <summary>
        /// Manager that synchronizes events with remote calendars.
        /// </summary>
        private readonly EventsManager eventsManager;

        /// <summary>
        /// Provider that populates image list with calendar icons.
        /// </summary>
        private readonly ICalendarIconsProvider calendarIconsProvider;

        /// <summary>
        /// Application settings that are currently in use.
        /// </summary>
        private readonly AsyncLazy<CalendaroSettings> activeSettings;

        /// <summary>
        /// Sound player to play reminder notification sound when form is shown.
        /// </summary>
        private readonly SoundPlayer notificationSound;

        /// <summary>
        /// Task that refreshes events periodically.
        /// </summary>
        private readonly Task refreshEventsTask;

        /// <summary>
        /// List of synchronization errors that occured during the last sync.
        /// </summary>
        private readonly List<CalendarSyncException> lastSyncErrors = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationForm"/> class
        /// with the provided settings provider and calendar services factory.
        /// </summary>
        /// <param name="settingsProvider">Provider used to save and restore application settings.</param>
        /// <param name="calendarServicesFactory">Factory used to create clients for different calendar services.</param>
        /// <param name="eventsManager">Manager that synchronizes events with remote calendars.</param>
        /// <param name="calendarIconsProvider">Provider that populates image list with calendar icons.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public NotificationForm(
            ICalendaroSettingsProvider settingsProvider,
            ICalendarServicesFactory calendarServicesFactory,
            EventsManager eventsManager,
            ICalendarIconsProvider calendarIconsProvider)
        {
            this.settingsProvider =
                settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));

            this.calendarServicesFactory =
                calendarServicesFactory ?? throw new ArgumentNullException(nameof(calendarServicesFactory));

            this.eventsManager =
                eventsManager ?? throw new ArgumentNullException(nameof(eventsManager));

            this.calendarIconsProvider =
                calendarIconsProvider ?? throw new ArgumentNullException(nameof(calendarIconsProvider));

            activeSettings = new AsyncLazy<CalendaroSettings>(settingsProvider.LoadSettingsAsync);

            InitializeComponent();

            // Set width of the event name column, considering that vertical scroll bar might be shown
            columnEventName.Width =
                listEvents.ClientSize.Width - columnTimeLeft.Width - SystemInformation.VerticalScrollBarWidth;

            notificationSound = new SoundPlayer(Resources.notification_sound);
            notificationSound.Load();

            // Start refreshing the events
            refreshEventsTask = RefreshEvents(FormClosedToken);
        }

        /// <summary>
        /// Sets the form to the specified visible state, except on the very first call.
        /// </summary>
        /// <remarks>
        /// This is our main form and it will be created at the application startup and
        /// passed to the default <see cref="ApplicationContext"/>. In order to start
        /// the application with the form hidden, we override this method and do not show
        /// the form on the first call. All subsequent calls will perform as they should.
        /// </remarks>
        /// <param name="value">true to make the form visible, ottherwise false.</param>
        protected override void SetVisibleCore(bool value)
        {
            if (value && !IsHandleCreated)
            {
                // First call just creates the handle
                CreateHandle();
                return;
            }

            base.SetVisibleCore(value);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    components?.Dispose();
                    refreshEventsTask.Dispose();
                }
                catch
                {
                    // Dispose is best effort only
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Synchronizes calendar events and displays notification form, if needed.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents an asynchronous event refresh operation.</returns>
        private async Task RefreshEvents(CancellationToken cancellation)
        {
            try
            {
                while (!cancellation.IsCancellationRequested)
                {
                    var activeSettingsSnapshot =
                        await activeSettings.GetValue(cancellation);

                    // First we want to check our cached events and see, if user needs to be notified right away.
                    // Syncing events can take some time, and we don't want the user to wait for sync to complete
                    // before getting notifications out for events that were synced in the past and are starting now.
                    // The downside is that new events will only be shown the next minute.
                    // Ensure calendars icons are initialized
                    if (imagesCalendars.Images.Count <= 0)
                    {
                        calendarIconsProvider.PopulateImageList(
                            activeSettingsSnapshot.AccountsConfiguration,
                            imagesCalendars);
                    }

                    // Get active events and update the list
                    PopulateEventsList(eventsManager.GetActiveEvents());
                    var lastRefreshMinute = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMinute;

                    if (listEvents.Items.Count <= 0)
                    {
                        // No upcoming events, hide the notification form
                        if (Visible)
                        {
                            Hide();
                        }
                    }
                    else if (!Visible)
                    {
                        // Form is not currently visible, but there are active events - show the form
                        notificationSound.Play();
                        NativeMethods.ShowInactiveTopmost(this);
                        NativeMethods.FlashWindowEx(this);
                    }

                    // Synchronize events with remote calendars
                    try
                    {
                        await eventsManager.SynchronizeEventsAsync(
                            activeSettingsSnapshot.AccountsConfiguration,
                            TimeSpan.FromHours(activeSettingsSnapshot.LookaheadHours),
                            cancellation);

                        trayIcon.Icon = Resources.icon;
                        trayIcon.Text =
                            string.Format(Resources.SuccessfulSynchronizationTooltip, DateTime.Now);

                        lastSyncErrors.Clear();
                    }
                    catch (Exception syncError)
                    {
                        trayIcon.Icon = Resources.icon_warning;

                        // Store all calendar synchronization errors
                        if (syncError is AggregateException syncErrors)
                        {
                            lastSyncErrors.AddRange(
                                syncErrors.InnerExceptions.OfType<CalendarSyncException>());
                        }
                    }

                    // Snap refresh to the next minute mark.
                    // We actually want to go over the minute mark to ensure the events are shown on time, otherwise we may
                    // wake up few seconds before the minute mark and not show anything. This will cause a delay of 1 minute,
                    // which might be cruicial for events that are starting now.
                    var isInitialDelay = true;
                    do
                    {
                        // We don't want to refresh more often than 5 seconds, so the first time we sleep for at least 5 seconds.
                        // On the subsequent delays we start crawling to the minute mark (if not past it yet) in 1 second increments.
                        await Task.Delay(
                            (int)Math.Max(
                                (TimeSpan.TicksPerMinute - (DateTime.UtcNow.Ticks % TimeSpan.TicksPerMinute))
                                    / TimeSpan.TicksPerMillisecond + 500, // Add 500ms to account for small time skew
                                isInitialDelay ? 5000L : 1000L),
                            cancellation);

                        isInitialDelay = false;
                    }
                    while (DateTime.UtcNow.Ticks / TimeSpan.TicksPerMinute <= lastRefreshMinute);
                }
            }
            catch (TaskCanceledException)
            {
                // We will gracefully shutdown
            }
            catch (Exception error)
            {
                // Something is utterly broken - bail out
                MessageBox.Show(this, error.Message, Resources.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(10);
            }
        }

        /// <summary>
        /// Populates events in the list control based on the provided <paramref name="events"/> collection.
        /// </summary>
        /// <param name="events">Collection of active events.</param>
        private void PopulateEventsList(IReadOnlyList<TrackedEvent> events)
        {
            // Store previously selected event to restore the selection after update
            var selectedEvent = GetFirstSelectedEvent();
            var utcNow = DateTime.UtcNow;

            listEvents.BeginUpdate();
            listEvents.Items.Clear();

            foreach (var trackedEvent in events)
            {
                var eventTitle =
                    string.IsNullOrEmpty(trackedEvent.Event.Title)
                        ? Resources.DefaultEventTitle
                        : trackedEvent.Event.Title;

                listEvents.Items.Add(
                    new ListViewItem(
                        new[]
                        {
                            eventTitle,
                            (trackedEvent.Event.StartTimeUtc - utcNow).AsRemainingTimeString(),
                        })
                    {
                        Tag = trackedEvent,
                        Selected = trackedEvent == selectedEvent,
                        ImageKey = trackedEvent.Calendar.Id,
                        ToolTipText = eventTitle,
                    });
            }

            if (listEvents.Items.Count > 0 && listEvents.SelectedItems.Count <= 0)
            {
                // If no original items are present, we select first item by default
                listEvents.SelectedIndices.Add(0);
            }
            
            listEvents.EndUpdate();

            UpdateControlsState(listEvents, EventArgs.Empty);
        }

        /// <summary>
        /// Updates controls with the information about selected event.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void UpdateControlsState(object sender, EventArgs e)
        {
            var selectedEvent = GetFirstSelectedEvent()?.Event;

            if (btnSnooze.Enabled = btnDismiss.Enabled =
                    selectedEvent != null)
            {
                btnJoinOnline.Visible =
                    !string.IsNullOrEmpty(selectedEvent!.ConferenceUri);

                labelEventName.Text = selectedEvent.Title;
                labelEventTime.Text =
                    string.Format("{0:t} {0:D}", selectedEvent.StartTimeUtc.ToLocalTime());

                // Update available snoozing intervals
                comboSnoozeInterval.BeginUpdate();
                comboSnoozeInterval.DataSource =
                    SnoozingHandler.Instance.SuggestIntervals(selectedEvent);
                comboSnoozeInterval.EndUpdate();
            }
            else
            {
                labelEventName.Text = Resources.DefaultEventTitle;
                labelEventTime.Text = string.Empty;
                btnJoinOnline.Visible = false;
            }
        }

        /// <summary>
        /// Opens an URI that contains full event description.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event argument.</param>
        private void OpenEventDescription(object sender, EventArgs e)
        {
            var selectedEvent = GetFirstSelectedEvent();

            if (selectedEvent != null && !string.IsNullOrEmpty(selectedEvent.Event.EventUri))
            {
                Process.Start(
                    new ProcessStartInfo(selectedEvent.Event.EventUri) { UseShellExecute = true });
            }
        }

        /// <summary>
        /// Updates next notification time for the event based on the selected snoozing option.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void SnoozeSelectedEvent(object sender, EventArgs e)
        {
            var selectedEventIndex =
                listEvents.SelectedIndices.Count > 0 ? listEvents.SelectedIndices[0] : -1;

            if (selectedEventIndex >= 0)
            {
                var selectedEvent = (TrackedEvent)listEvents.Items[selectedEventIndex].Tag;

                selectedEvent.RemindAtUtc =
                    (DateTime)comboSnoozeInterval.SelectedValue;

                if (selectedEvent.RemindAtUtc > DateTime.UtcNow)
                {
                    // It might happen that new reminder time has already, if user had the form
                    // open for prolonged time and refresh failed for some reason.
                    // To handle this gracefully, we only hide the event if it should not be reminded yet.
                    HideEvent(selectedEventIndex);
                }
            }
        }

        /// <summary>
        /// Joins the conference call for the selected event.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void JoinOnlineConference(object sender, EventArgs e)
        {
            var selectedEvent = GetFirstSelectedEvent();

            if (selectedEvent != null && !string.IsNullOrEmpty(selectedEvent.Event.ConferenceUri))
            {
                Process.Start(
                    new ProcessStartInfo(selectedEvent.Event.ConferenceUri) { UseShellExecute = true });
            }
        }

        /// <summary>
        /// Dismisses selected event and removes it from the list.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DismissSelectedEvent(object sender, EventArgs e)
        {
            var selectedEventIndex =
                listEvents.SelectedIndices.Count > 0 ? listEvents.SelectedIndices[0] : -1;

            if (selectedEventIndex >= 0)
            {
                ((TrackedEvent)listEvents.Items[selectedEventIndex].Tag).RemindAtUtc = null;
                HideEvent(selectedEventIndex);
            }
        }

        /// <summary>
        /// Dismisses selected event and removes it from the list.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DismissAllEvents(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    this,
                    Resources.DismissAllPrompt,
                    btnDismissAll.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            foreach (ListViewItem eventItem in listEvents.Items)
            {
                ((TrackedEvent)eventItem.Tag).RemindAtUtc = null;
            }

            listEvents.BeginUpdate();
            listEvents.Items.Clear();
            listEvents.EndUpdate();

            Hide();
        }

        /// <summary>
        /// Removes event from the list and moves selection to the next event,
        /// if removed event was selected.
        /// </summary>
        /// <param name="eventIndex">Index of the event to remove.</param>
        private void HideEvent(int eventIndex)
        {
            listEvents.Items.RemoveAt(eventIndex);

            if (listEvents.Items.Count <= 0)
            {
                // Hide the form, if there are no active events left
                Hide();
            }
            else
            {
                if (listEvents.SelectedIndices.Count <= 0)
                {
                    // If no events are selected after removal, then we adjust the selection to the adjacent event
                    listEvents.SelectedIndices.Add(
                        eventIndex < listEvents.Items.Count ? eventIndex : eventIndex - 1);
                }

                // Return focus to the events list, in case somebody is using arrows to navigate up and down
                listEvents.Focus();
            }
        }

        /// <summary>
        /// Gets the first selected event in the events list.
        /// </summary>
        /// <returns>First selected event or null, if nothing is selected.</returns>
        private TrackedEvent? GetFirstSelectedEvent() =>
            listEvents.SelectedItems.Count > 0
                ? (TrackedEvent)listEvents.SelectedItems[0].Tag
                : null;

        /// <summary>
        /// Opens the settings dialog.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void OpenSettings(object sender, EventArgs e)
        {
            using var settingsForm =
                new SettingsForm(
                    await activeSettings.GetValue(FormClosedToken),
                    calendarServicesFactory,
                    calendarIconsProvider);

            if (settingsForm.ShowDialog(this) == DialogResult.OK)
            {
                // Save new settings and update event notifications
                await settingsProvider
                    .SaveSettingsAsync(settingsForm.Settings, FormClosedToken);

                activeSettings.SetValue(settingsForm.Settings);

                // Hide all current notifications and wait for the next refresh task to happen,
                // as there might be new or removed calendars, etc.
                Hide();
                imagesCalendars.Images.Clear();
            }
        }

        /// <summary>
        /// Opens the calendars synchronization status dialog.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void OpenSyncStatus(object sender, EventArgs e)
        {
            using var syncStatusForm =
                new SyncStatusForm(
                    (await activeSettings.GetValue(FormClosedToken)).AccountsConfiguration,
                    lastSyncErrors);
            syncStatusForm.ShowDialog(this);
        }

        /// <summary>
        /// Checks whether user is closing the form and prompts for events dismissal.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleFormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Do not close the form, we will hide it instead to keep the refresh timer running
                e.Cancel = true;

                if (listEvents.Items.Count > 0)
                {
                    DismissAllEvents(btnDismissAll, EventArgs.Empty);
                }

                if (listEvents.Items.Count <= 0)
                {
                    // Only close the form, if there are no active events left
                    Hide();
                }
            }
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ExitApplication(object sender, EventArgs e)
        {
            FormClosing -= HandleFormClosing;
            Close();
        }
    }
}

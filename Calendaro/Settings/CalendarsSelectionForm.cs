using Calendaro.Abstractions;
using Calendaro.UI;
using System.Data;

namespace Calendaro.Settings
{
    /// <summary>
    /// Form that allows the user to select which calendars within one account should be synchronized.
    /// </summary>
    internal partial class CalendarsSelectionForm : AsyncFormBase
    {
        /// <summary>
        /// Calendar service client used to retrieve available calendars.
        /// </summary>
        private readonly ICalendarService calendarService;

        /// <summary>
        /// Gets the collection of selected calendars.
        /// </summary>
        public IReadOnlyList<CalendarInfo> SelectedCalendars =>
            listCalendars.CheckedItems
                .OfType<ListControlItem<CalendarInfo>>()
                .Select(item => item.Value)
                .ToArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarsSelectionForm"/> class
        /// with the provided calendar service.
        /// </summary>
        /// <param name="calendarService">Calendar service client used to retrieve available calendars.</param>
        /// <param name="preselectedCalendarIds">Identifiers of the calendars that should be pre-selected.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public CalendarsSelectionForm(
            ICalendarService calendarService,
            IReadOnlySet<string>? preselectedCalendarIds)
        {
            this.calendarService =
                calendarService ?? throw new ArgumentNullException(nameof(calendarService));

            InitializeComponent();

            // Load list of calendars
            _ = PopulateControlsAsync(preselectedCalendarIds, FormClosedToken);
        }

        /// <summary>
        /// Retrieves calendars form the calendar service and populates the list control.
        /// </summary>
        /// <param name="preselectedCalendarIds">Identifiers of the calendars that should be pre-selected.</param>
        /// <param name="cancellation">Cancellation token to stop the loading.</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous controls initialization operation.</returns>
        private async Task PopulateControlsAsync(
            IReadOnlySet<string>? preselectedCalendarIds,
            CancellationToken cancellation)
        {
            foreach (var calendar
                in await calendarService.ListCalendarsAsync(cancellation))
            {
                var newItemIndex = 
                    listCalendars.Items.Add(
                        new ListControlItem<CalendarInfo>(calendar.Name, calendar));

                if (preselectedCalendarIds != null && preselectedCalendarIds.Contains(calendar.Id))
                {
                    listCalendars.SetItemChecked(newItemIndex, true);
                }
            }

            // Enable calendars list, once it was initialized and
            // perform initial form validation
            listCalendars.Enabled = true;
            ValidateCheckedItems(this, new ItemCheckEventArgs(0, CheckState.Unchecked, CheckState.Unchecked));
        }

        /// <summary>
        /// Validates that there are selected calendars and updates buttons state.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ValidateCheckedItems(object sender, ItemCheckEventArgs e) =>
            // The event is raised before the checked state is actually updated,
            // so we check arguments to see if current item was checked and if not,
            // then we validate other items.
            btnOk.Enabled =
                e.NewValue == CheckState.Checked
                || listCalendars.CheckedItems.Count > 0;
    }
}

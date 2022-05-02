using Calendaro.Abstractions;

namespace Calendaro.Settings
{
    /// <summary>
    /// Adapts calendars configuration from application settings to the UI tree control.
    /// </summary>
    internal sealed class CalendarsTreeAdapter
    {
        /// <summary>
        /// Creates root node for the UI tree control that contains all account nodes
        /// with their calendars based on provided configuration.
        /// </summary>
        /// <param name="calendarAccounts">Collection of configured calendar accounts.</param>
        /// <returns>Root tree node to bind to UI tree control.</returns>
        public TreeNode[] AsTreeNodes(ICollection<CalendarAccountConfiguration> calendarAccounts)
        {
            var accountNodes = new TreeNode[calendarAccounts.Count];

            // TODO: We would like to accept IReadOnlyList<T> and use regular `for` loop,
            //       but IList<T> doesn't implement IReadOnlyList<T>.
            //       https://github.com/dotnet/runtime/issues/31001
            var accountIndex = 0;
            foreach (var account in calendarAccounts)
            {
                accountNodes[accountIndex++] =
                    CreateCalendarAccountTreeNode(account);
            }

            return accountNodes;
        }

        /// <summary>
        /// Creates UI tree node based on the provided calendar account configuration.
        /// </summary>
        /// <param name="calendarAccount">Calendar account configuration.</param>
        /// <returns>An UI tree node that represents provided calendar account.</returns>
        private TreeNode CreateCalendarAccountTreeNode(CalendarAccountConfiguration calendarAccount)
        {
            var accountNode =
                new TreeNode(calendarAccount.AccountId)
                {
                    Tag = calendarAccount,
                    ImageIndex = (int)calendarAccount.CalendarServiceType,
                    SelectedImageIndex = (int)calendarAccount.CalendarServiceType,
                };

            foreach (var calendar in calendarAccount.Calendars)
            {
                accountNode.Nodes.Add(
                    CreateCalendarTreeNode(calendar));
            }

            return accountNode;
        }

        /// <summary>
        /// Creates a UI tree node based on the provided calendar configuratiion.
        /// </summary>
        /// <param name="calendar">Calendar configuration.</param>
        /// <returns>An UI tree node that represents provided calendar.</returns>
        private TreeNode CreateCalendarTreeNode(CalendarInfo calendar) =>
            new(calendar.Name)
            {
                Tag = calendar,
                ImageKey = calendar.Id,
                SelectedImageKey = calendar.Id,
            };
    }
}

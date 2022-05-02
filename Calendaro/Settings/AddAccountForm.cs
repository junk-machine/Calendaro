using Calendaro.UI;

namespace Calendaro.Settings
{
    /// <summary>
    /// Form that collects information about new calendar service account.
    /// </summary>
    internal partial class AddAccountForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddAccountForm"/> class.
        /// </summary>
        public AddAccountForm()
        {
            InitializeComponent();
            PopulateControls();
        }

        /// <summary>
        /// Gets the selected calendar service type.
        /// </summary>
        public CalendarServiceType CalendarServiceType =>
            (CalendarServiceType)comboCalendarServiceType.SelectedValue;
        
        /// <summary>
        /// Gets the specified account identifier.
        /// </summary>
        public string AccountId =>
            txtAccountId.Text;

        /// <summary>
        /// Populates UI controls with the initial data.
        /// </summary>
        private void PopulateControls()
        {
            // Populate combo box with supported calendar services
            comboCalendarServiceType.DataSource =
                Enum.GetValues<CalendarServiceType>()
                    .Where(type => type != CalendarServiceType.Unknown)
                    .Select(type => new ListControlItem<CalendarServiceType>(type.ToString(), type))
                    .ToArray();

            comboCalendarServiceType.SelectedIndex = 0;
        }

        /// <summary>
        /// Validates all input controls and updates buttons state.
        /// </summary>
        private void ValidateInput(object sender, EventArgs e)
        {
            btnOk.Enabled =
                comboCalendarServiceType.SelectedIndex >= 0
                && !string.IsNullOrEmpty(txtAccountId.Text);
        }
    }
}

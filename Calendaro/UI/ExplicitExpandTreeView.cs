namespace Calendaro.UI
{
    /// <summary>
    /// Tree view control that does not expand the nodes on double-click.
    /// Instead nodes can only be expanded by explicitly clicking the plus/minus icon.
    /// </summary>
    internal sealed class ExplicitExpandTreeView : TreeView
    {
        /// <inheritdoc/>
        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == 515)
            {
                // Ignore WM_LBUTTONDBLCLK.
                // The `NodeMouseDoubleClick` event is called when handliong WM_LBUTTONUP, so this
                // doesn't impact any functionality other than expand/collapse on double-click.
            }
            else
                base.DefWndProc(ref m);
        }
    }
}

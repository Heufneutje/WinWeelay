namespace WinWeelay.Core
{
    /// <summary>
    /// Interface implemented by all option editor dialogs.
    /// </summary>
    public interface IOptionWindow
    {
        /// <summary>
        /// Show the option editor dialog.
        /// </summary>
        /// <returns>The result of the editor dialog.</returns>
        bool? ShowDialog();
    }
}

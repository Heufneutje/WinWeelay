using System;

namespace WinWeelay.Core
{
    /// <summary>
    /// Interface for buffer list control (list or tree).
    /// </summary>
    public interface IBufferDockView
    {
        /// <summary>
        /// Event for when the selected buffer changes.
        /// </summary>
        event EventHandler SelectionChanged;

        /// <summary>
        /// Get the buffer that is currently selected.
        /// </summary>
        /// <returns>The active buffer.</returns>
        RelayBuffer GetSelectedBuffer();
    }
}

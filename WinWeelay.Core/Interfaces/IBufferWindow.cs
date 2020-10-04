namespace WinWeelay.Core
{
    /// <summary>
    /// Interface implemented by all buffer windows.
    /// </summary>
    public interface IBufferWindow
    {
        /// <summary>
        /// Close a given buffer.
        /// </summary>
        /// <param name="buffer">The buffer to close.</param>
        void CloseBuffer(RelayBuffer buffer);
    }
}

namespace WinWeelay.Core
{
    /// <summary>
    /// Delegate for events fired when a message is received.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments for the event.</param>
    public delegate void RelayMessageReceivedHandler(object sender, RelayMessageEventArgs args);

    /// <summary>
    /// Delegate for events fired when an error occurs.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments for the event.</param>
    public delegate void RelayErrorHandler(object sender, RelayErrorEventArgs args);

    /// <summary>
    /// Delegate for events fired when the connection is lost.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments for the event.</param>
    public delegate void ConnectionLostHandler(object sender, ConnectionLostEventArgs args);

    /// <summary>
    /// Delegate for events fired when a message is added to a buffer.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments for the event.</param>
    public delegate void MessageAddedHandler(object sender, RelayBufferMessageEventArgs args);

    /// <summary>
    /// Delegate for events fired when multiple messages are added to a buffer.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments for the event.</param>
    public delegate void MessageBatchAddedHandler(object sender, RelayBufferMessageBatchEventsArgs args);

    /// <summary>
    /// Delegate for events fired when a received message triggers a highlight.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments for the event.</param>
    public delegate void HighlightHandler(object sender, HighlightEventArgs args);
}

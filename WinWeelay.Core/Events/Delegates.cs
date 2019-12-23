namespace WinWeelay.Core
{
    public delegate void RelayMessageReceivedHandler(object sender, RelayMessageEventArgs args);
    public delegate void RelayErrorHandler(object sender, RelayErrorEventArgs args);
    public delegate void ConnectionLostHandler(object sender, ConnectionLostEventArgs args);
    public delegate void MessageAddedHandler(object sender, RelayBufferMessageEventArgs args);
    public delegate void MessageBatchAddedHandler(object sender, RelayBufferMessageBatchEventsArgs args);
    public delegate void HighlightHandler(object sender, HighlightEventArgs args);
}

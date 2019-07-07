namespace WinWeelay.Core
{
    public delegate void ConnectionLostHandler(object sender, ConnectionLostEventArgs args);
    public delegate void MessageAddedHandler(object sender, RelayBufferMessageEventArgs args);
    public delegate void HighlightHandler(object sender, HighlightEventArgs args);
}

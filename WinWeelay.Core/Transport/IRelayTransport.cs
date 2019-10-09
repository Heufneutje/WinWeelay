using System.Threading.Tasks;
using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    public interface IRelayTransport
    {
        event RelayMessageReceivedHandler RelayMessageReceived;
        event RelayErrorHandler ErrorReceived;

        bool IsConnected { get; }

        Task Connect(RelayConfiguration configuration);

        void Disconnect();

        void Write(byte[] data);
    }
}

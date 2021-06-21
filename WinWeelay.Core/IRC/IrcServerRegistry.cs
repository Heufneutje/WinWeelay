using System.Collections.Generic;

namespace WinWeelay.Core
{
    /// <summary>
    /// Registry for the IRC server properties on buffers.
    /// </summary>
    public class IrcServerRegistry
    {
        private Dictionary<string, IrcServer> _servers;

        /// <summary>
        /// Instantiate the registry.
        /// </summary>
        public IrcServerRegistry()
        {
            _servers = new Dictionary<string, IrcServer>();
        }

        /// <summary>
        /// Add a new IRC server to the registry.
        /// </summary>
        /// <param name="infoListItems">Infolist response with IRC server properties.</param>
        public void RegisterIrcServer(Dictionary<string, WeechatRelayObject> infoListItems)
        {
            string bufferPointer = infoListItems["buffer"].AsPointer();
            IrcServer handler = new IrcServer();
            if (_servers.ContainsKey(bufferPointer))
                handler = _servers[bufferPointer];
            else
                _servers.Add(bufferPointer, handler);

            handler.ParseInfoList(infoListItems);
        }

        /// <summary>
        /// Remove all servers from the registry.
        /// </summary>
        public void Clear()
        {
            _servers.Clear();
        }

        /// <summary>
        /// Check whether a server buffer pointer exists in the registry.
        /// </summary>
        /// <param name="bufferPointer">The server buffer pointer to check.</param>
        /// <returns>Whether a server buffer pointer exists in the registry.</returns>
        public bool HasIrcServer(string bufferPointer) => _servers.ContainsKey(bufferPointer);

        /// <summary>
        /// Get a server from the registry with a given buffer pointer.
        /// </summary>
        /// <param name="bufferPointer">The pointer for the server to get.</param>
        /// <returns>IRC server properties.</returns>
        public IrcServer this[string bufferPointer] => _servers[bufferPointer];
    }
}

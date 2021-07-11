using System;
using System.Collections.Generic;
using System.Linq;

namespace WinWeelay.Core
{
    /// <summary>
    /// A messaged received in a WeeChat buffer.
    /// </summary>
    public class RelayBufferMessage
    {
        /// <summary>
        /// Internal ID for the message.
        /// </summary>
        public string LinePointer { get; private set; }

        /// <summary>
        /// Internal ID for the buffer that the message was received in.
        /// </summary>
        public string BufferPointer { get; private set; }

        /// <summary>
        /// The date and time the message was sent on.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Whether this message triggers a highlight.
        /// </summary>
        public bool IsHighlighted { get; private set; }

        /// <summary>
        /// A list of additional message tags.
        /// </summary>
        public string[] Tags { get; private set; }

        /// <summary>
        /// Role prefix for the user that sent the message.
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// The content of the message.
        /// </summary>
        public string MessageBody { get; private set; }

        /// <summary>
        /// The nickname of the user that sent the message.
        /// </summary>
        public string Nick { get; private set; }

        /// <summary>
        /// The type of the message (privmsg or notice).
        /// </summary>
        public string MessageType { get; private set; }

        /// <summary>
        /// Whether a notification for this message has been displayed when it triggers a highlight.
        /// </summary>
        /// <seealso cref="IsHighlighted"/>
        public bool IsNotified { get; set; }

        /// <summary>
        /// Create a new message from a received Hdata structure.
        /// </summary>
        /// <param name="entry">Received Hdata structure with message details.</param>
        /// <param name="showNotifications">Whether a notification should be shown if this message triggers a highlight (set to false for backlog messages).</param>
        /// <param name="linePointerIndex">Index where this message's internal ID is located.</param>
        public RelayBufferMessage(WeechatHdataEntry entry, bool showNotifications, int linePointerIndex)
        {
            LinePointer = entry.GetPointer(linePointerIndex);
            BufferPointer = entry["buffer"].AsPointer();
            Date = entry["date"].AsTime();
            IsHighlighted = Convert.ToBoolean(entry["highlight"].AsBoolean());
            Tags = entry["tags_array"].AsArray().ToStringArray();
            Prefix = entry["prefix"].AsString();
            MessageBody = entry["message"].AsString();

            string[] tags = entry["tags_array"].AsArray().ToStringArray();
            string nickTag = tags.FirstOrDefault(x => x.StartsWith("nick_"));
            if (nickTag != null)
                Nick = nickTag.Substring(5);

            string messageTypeTag = tags.LastOrDefault(x => x.StartsWith("irc_"));
            if (messageTypeTag != null)
                MessageType = messageTypeTag.Substring(4);

            if (!showNotifications)
                IsNotified = true;
        }

        /// <summary>
        /// Override to check if line and buffer pointers match. When this is the case the message is the same.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if messages have the same internal ID in the same buffer.</returns>
        public override bool Equals(object obj)
        {
            return obj is RelayBufferMessage message &&
                   LinePointer == message.LinePointer &&
                   BufferPointer == message.BufferPointer;
        }

        /// <summary>
        /// Override to check if line and buffer pointers match. When this is the case the message is the same.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            int hashCode = 1910936884;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LinePointer);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BufferPointer);
            return hashCode;
        }

        /// <summary>
        /// Override to check if line and buffer pointers match. When this is the case the message is the same.
        /// </summary>
        /// <param name="left">First message.</param>
        /// <param name="right">Second message.</param>
        /// <returns>True if messages have the same internal ID in the same buffer.</returns>
        public static bool operator ==(RelayBufferMessage left, RelayBufferMessage right)
        {
            return EqualityComparer<RelayBufferMessage>.Default.Equals(left, right);
        }

        /// <summary>
        /// Override to check if line and buffer pointers match. When this is the case the message is the same.
        /// </summary>
        /// <param name="left">First message.</param>
        /// <param name="right">Second message.</param>
        /// <returns>False if messages have the same internal ID in the same buffer.</returns>
        public static bool operator !=(RelayBufferMessage left, RelayBufferMessage right)
        {
            return !(left == right);
        }
    }
}

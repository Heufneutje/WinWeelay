using System.Collections.Generic;

namespace WinWeelay.Core
{
    /// <summary>
    /// A nickname for a user in a buffer.
    /// </summary>
    public class RelayNicklistEntry
    {
        /// <summary>
        /// Whether the entry is a group (unused?).
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// The nickname of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The color of the user in chat.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// The prefix on the user in chat.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// The color for the user's prefix.
        /// </summary>
        public string PrefixColor { get; set; }

        /// <summary>
        /// Used to sort higher rank users above lower ranks.
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        /// The buffer the nicklist entry is for.
        /// </summary>
        public RelayBuffer Buffer { get; set; }

        /// <summary>
        /// Create a new nicklist entry.
        /// </summary>
        /// <param name="entry">The Hdata structure which contains the details of the nicklist entry.</param>
        /// <param name="buffer">The buffer the nicklist entry is for.</param>
        public RelayNicklistEntry(WeechatHdataEntry entry, RelayBuffer buffer)
        {
            Buffer = buffer;

            IsGroup = entry["group"].AsBoolean();
            Name = entry["name"].AsString();
            Color = entry["color"].AsString();
            Prefix = entry["prefix"].AsString();
            PrefixColor = entry["prefix_color"].AsString();

            SortIndex = Buffer.IrcServer.GetStatusSortIndex(Prefix);
        }

        /// <summary>
        /// Update the nicklist entry with the values from a received Hdata structure.
        /// </summary>
        /// <param name="entry">The Hdata structure which contains the details of the nicklist entry.</param>
        public void Update(RelayNicklistEntry entry)
        {
            IsGroup = entry.IsGroup;
            Name = entry.Name;
            Color = entry.Color;
            Prefix = entry.Prefix;
            PrefixColor = entry.PrefixColor;

            SortIndex = Buffer.IrcServer.GetStatusSortIndex(Prefix);
        }

        /// <summary>
        /// ToString override to display a user with their prefix.
        /// </summary>
        /// <returns>A user with their prefix.</returns>
        public override string ToString()
        {
            return $"{Prefix}{Name}";
        }

        /// <summary>
        /// Override to check if nickname and buffer match. When this is the case the nicklist entry is the same.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if nickname and buffer match.</returns>
        public override bool Equals(object obj)
        {
            return obj is RelayNicklistEntry entry &&
                   Name == entry.Name &&
                   EqualityComparer<RelayBuffer>.Default.Equals(Buffer, entry.Buffer);
        }

        /// <summary>
        /// Override to check if nickname and buffer match. When this is the case the nicklist entry is the same.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            int hashCode = 1972309649;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<RelayBuffer>.Default.GetHashCode(Buffer);
            return hashCode;
        }

        /// <summary>
        /// Override to check if nickname and buffer match. When this is the case the nicklist entry is the same.
        /// </summary>
        /// <param name="left">First nicklist entry.</param>
        /// <param name="right">Second nicklist entry.</param>
        /// <returns>True if nickname and buffer match.</returns>
        public static bool operator ==(RelayNicklistEntry left, RelayNicklistEntry right)
        {
            return EqualityComparer<RelayNicklistEntry>.Default.Equals(left, right);
        }

        /// <summary>
        /// Override to check if nickname and buffer match. When this is the case the nicklist entry is the same.
        /// </summary>
        /// <param name="left">First nicklist entry.</param>
        /// <param name="right">Second nicklist entry.</param>
        /// <returns>False if nickname and buffer match.</returns>
        public static bool operator !=(RelayNicklistEntry left, RelayNicklistEntry right)
        {
            return !(left == right);
        }
    }
}

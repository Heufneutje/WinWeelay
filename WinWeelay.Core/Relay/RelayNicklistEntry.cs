using System.Collections.Generic;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class RelayNicklistEntry
    {
        public bool IsGroup { get; set; }
        public bool IsVisible { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Prefix { get; set; }
        public string PrefixColor { get; set; }
        public int SortIndex { get; set; }
        public RelayBuffer Buffer { get; set; }

        public string ListDisplay => ToString();

        public RelayNicklistEntry(WeechatHdataEntry entry, RelayBuffer buffer)
        {
            Buffer = buffer;

            IsGroup = entry["group"].AsBoolean();
            IsVisible = entry["visible"].AsBoolean();
            Level = entry["level"].AsInt();
            Name = entry["name"].AsString();
            Color = entry["color"].AsString();
            Prefix = entry["prefix"].AsString();
            PrefixColor = entry["prefix_color"].AsString();

            SortIndex = PrefixHelper.GetSortIndex(Prefix);
        }

        public void Update(RelayNicklistEntry entry)
        {
            IsGroup = entry.IsGroup;
            IsVisible = entry.IsVisible;
            Level = entry.Level;
            Name = entry.Name;
            Color = entry.Color;
            Prefix = entry.Prefix;
            PrefixColor = entry.PrefixColor;

            SortIndex = PrefixHelper.GetSortIndex(Prefix);
        }

        public override string ToString()
        {
            return $"{Prefix}{Name}";
        }

        public override bool Equals(object obj)
        {
            return obj is RelayNicklistEntry entry &&
                   Name == entry.Name &&
                   EqualityComparer<RelayBuffer>.Default.Equals(Buffer, entry.Buffer);
        }

        public override int GetHashCode()
        {
            int hashCode = 1972309649;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<RelayBuffer>.Default.GetHashCode(Buffer);
            return hashCode;
        }

        public static bool operator ==(RelayNicklistEntry left, RelayNicklistEntry right)
        {
            return EqualityComparer<RelayNicklistEntry>.Default.Equals(left, right);
        }

        public static bool operator !=(RelayNicklistEntry left, RelayNicklistEntry right)
        {
            return !(left == right);
        }
    }
}

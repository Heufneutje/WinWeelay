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

        public string ListDisplay
        {
            get
            {
                return ToString();
            }
        }

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
    }
}

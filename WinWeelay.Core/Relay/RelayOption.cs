namespace WinWeelay.Core
{
    public class RelayOption
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public RelayOption(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}

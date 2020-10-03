using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WinWeelay.Core
{
    [Serializable]
    public class RelayOption
    {
        public string Name { get; set; }
        public string Value { get; set; }

        [JsonIgnore]
        public string Description { get; set; }

        [JsonIgnore]
        public List<string> PossibleValues { get; set; }

        [JsonIgnore]
        public int MinValue { get; set; }

        [JsonIgnore]
        public int MaxValue { get; set; }

        [JsonIgnore]
        public bool IsNullValueAllowed { get; set; }

        [JsonIgnore]
        public bool ValueIsNull { get; set; }

        [JsonIgnore]
        public bool DefaultValueIsNull { get; set; }

        [JsonIgnore]
        public string DefaultValue { get; set; }

        [JsonIgnore]
        public string OptionType { get; set; }

        [JsonIgnore]
        public string DisplayValue { get; set; }

        [JsonIgnore]
        public string ParentValue { get; set; }

        [JsonIgnore]
        public bool IsModified => Value != DefaultValue;

        [JsonIgnore]
        public string PossibleValuesString => PossibleValues == null ? null : string.Join(", ", PossibleValues);

        [JsonIgnore]
        public string DescriptionBoxValue => ValueIsNull ? Value : DisplayValue;

        [JsonIgnore]
        public bool IsPassword { get; set; }

        public RelayOption() { }

        public RelayOption(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public RelayOption(Dictionary<string, WeechatRelayObject> item)
        {
            Name = item["full_name"].AsString();
            Description = item["description"].AsString();
            MinValue = item["min"].AsInt();
            MaxValue = item["max"].AsInt();
            IsNullValueAllowed = item["null_value_allowed"].AsBoolean();
            ValueIsNull = item["value_is_null"].AsBoolean();
            DefaultValueIsNull = item["default_value_is_null"].AsBoolean();           
            OptionType = item["type"].AsString();

            string possibleValues = item["string_values"].AsString();
            if (possibleValues != null)
                PossibleValues = possibleValues.Split('|').ToList();

            string lower = Name.ToLower();
            IsPassword = OptionType == "string" && lower.EndsWith("password") && !lower.EndsWith("hide_password");

            if (item.ContainsKey("parent_value"))
            {
                ParentValue = item["parent_value"].AsString();

                if (IsPassword)
                    ParentValue = "".PadLeft(ParentValue.Length, '*');
                if (OptionType == "string")
                    ParentValue = AddStringQuotes(ParentValue);
            }

            if (ValueIsNull)
            {
                DisplayValue = Value = "(null)";
                if (ParentValue != null)
                    DisplayValue = $"{DisplayValue} -> {ParentValue}";
            }
            else if (item.ContainsKey("value"))
            {
                Value = item["value"].AsString();
                if (IsPassword)
                {
                    DisplayValue = "".PadLeft(Value.Length, '*');
                    IsPassword = true;
                }
                else
                {
                    DisplayValue = Value;
                }

                if (OptionType == "string")
                {
                    Value = AddStringQuotes(Value);
                    DisplayValue = AddStringQuotes(DisplayValue);
                }
            }

            if (DefaultValueIsNull)
            {
                DefaultValue = "(null)";
            }
            else if (item.ContainsKey("default_value"))
            {
                DefaultValue = item["default_value"].AsString();
                if (OptionType == "string")
                    DefaultValue = AddStringQuotes(DefaultValue);
            }
        }

        private string AddStringQuotes(string value)
        {
            return $"\"{value}\"";
        }
    }
}

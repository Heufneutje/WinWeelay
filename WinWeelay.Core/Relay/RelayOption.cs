using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WinWeelay.Core
{
    /// <summary>
    /// A WeeChat option.
    /// </summary>
    [Serializable]
    public class RelayOption
    {
        /// <summary>
        /// The full name of the option.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current value of the option.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// A detailed description of what the option does.
        /// </summary>
        [JsonIgnore]
        public string Description { get; set; }

        /// <summary>
        /// Possible selectable values if this options allows for it.
        /// </summary>
        [JsonIgnore]
        public List<string> PossibleValues { get; set; }

        /// <summary>
        /// Mimimum value for integer options.
        /// </summary>
        [JsonIgnore]
        public int MinValue { get; set; }

        /// <summary>
        /// Maximum value for integer options.
        /// </summary>
        [JsonIgnore]
        public int MaxValue { get; set; }

        /// <summary>
        /// Whether this option can be set to null.
        /// </summary>
        [JsonIgnore]
        public bool IsNullValueAllowed { get; set; }

        /// <summary>
        /// Whether the current value is null.
        /// </summary>
        [JsonIgnore]
        public bool ValueIsNull { get; set; }

        /// <summary>
        /// Whether the default value is null.
        /// </summary>
        [JsonIgnore]
        public bool DefaultValueIsNull { get; set; }

        /// <summary>
        /// The default value for this option.
        /// </summary>
        [JsonIgnore]
        public string DefaultValue { get; set; }

        /// <summary>
        /// The data type of the option (string, integer, color or boolean).
        /// </summary>
        [JsonIgnore]
        public string OptionType { get; set; }

        /// <summary>
        /// The value to show in the options editor window.
        /// </summary>
        [JsonIgnore]
        public string DisplayValue { get; set; }

        /// <summary>
        /// The current value of this option's parent option.
        /// </summary>
        [JsonIgnore]
        public string ParentValue { get; set; }

        /// <summary>
        /// Whether the current value is different from the default value.
        /// </summary>
        [JsonIgnore]
        public bool IsModified => Value != DefaultValue;

        /// <summary>
        /// Concatenated string of the possible values for display in the options UI.
        /// </summary>
        [JsonIgnore]
        public string PossibleValuesString => PossibleValues == null ? null : string.Join(", ", PossibleValues);

        /// <summary>
        /// Display value for the options UI.
        /// </summary>
        [JsonIgnore]
        public string DescriptionBoxValue => ValueIsNull ? Value : DisplayValue;

        /// <summary>
        /// Whether this option contains a password.
        /// </summary>
        [JsonIgnore]
        public bool IsPassword { get; set; }

        /// <summary>
        /// Empty constructor for designer.
        /// </summary>
        public RelayOption() { }

        /// <summary>
        /// Create a new option from a new name and value.
        /// </summary>
        /// <param name="name">The full name of the option.</param>
        /// <param name="value">The current value of the option.</param>
        public RelayOption(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Create a new option from a received data structure.
        /// </summary>
        /// <param name="item">The data structure.</param>
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

        /// <summary>
        /// Override to check if the name of this option matches another option's name in which case they are the same option.
        /// </summary>
        /// <param name="obj">The option to check.</param>
        /// <returns>True if options have the same name.</returns>
        public override bool Equals(object obj)
        {
            return obj is RelayOption option && Name == option.Name;
        }

        /// <summary>
        /// Override to check if the name of this option matches another option's name in which case they are the same option.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        /// <summary>
        /// Override to check if the name of this option matches another option's name in which case they are the same option.
        /// </summary>
        /// <param name="left">First option.</param>
        /// <param name="right">Second option.</param>
        /// <returns>True if options have the same name.</returns>
        public static bool operator ==(RelayOption left, RelayOption right)
        {
            return EqualityComparer<RelayOption>.Default.Equals(left, right);
        }

        /// <summary>
        /// Override to check if the name of this option matches another option's name in which case they are the same option.
        /// </summary>
        /// <param name="left">First option.</param>
        /// <param name="right">Second option.</param>
        /// <returns>False if options have the same name.</returns>
        public static bool operator !=(RelayOption left, RelayOption right)
        {
            return !(left == right);
        }
    }
}

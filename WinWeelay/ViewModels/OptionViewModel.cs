using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    public class OptionViewModel : NotifyPropertyChangedBase
    {
        public RelayOption Option { get; set; }
        public bool SetToNull { get; set; }
        public string EditValue { get; set; }
        public string ValueToSave { get; set; }

        public OptionViewModel()
        {
        }

        public OptionViewModel(RelayOption option)
        {
            Option = option;

            switch (option.OptionType)
            {
                case "string":
                    EditValue = option.ValueIsNull ? string.Empty : option.Value.Trim('\"');
                    break;
                case "integer":
                    if (option.PossibleValuesString == null)
                        EditValue = (option.MinValue > 0 ? option.MinValue : 0).ToString();
                    else
                        goto default;
                    break;
                default:
                    EditValue = option.Value;
                    break;
            }
        }

        public void Commit()
        {
            switch (Option.OptionType)
            {
                case "string":
                    ValueToSave = $"\"{EditValue}\"";
                    break;
                default:
                    ValueToSave = EditValue;
                    break;
            }
        }

        public void NotifySetToNullChanged()
        {
            NotifyPropertyChanged(nameof(SetToNull));
        }
    }
}

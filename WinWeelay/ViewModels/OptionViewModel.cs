using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    public class OptionViewModel : NotifyPropertyChangedBase
    {
        public RelayOption Option { get; set; }
        public bool SetToNull { get; set; }
        public string EditValue { get; set; }

        public OptionViewModel()
        {
        }

        public OptionViewModel(RelayOption option)
        {
            Option = option;

            switch (option.OptionType)
            {
                case "string":
                    EditValue = option.Value.Trim('\"');
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
                    Option.Value = $"\"{EditValue}\"";
                    break;
                default:
                    Option.Value = EditValue;
                    break;
            }
        }

        public void NotifySetToNullChanged()
        {
            NotifyPropertyChanged(nameof(SetToNull));
        }
    }
}

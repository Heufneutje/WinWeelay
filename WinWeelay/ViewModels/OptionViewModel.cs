using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// View model for the WeeChat option editor dialogs.
    /// </summary>
    public class OptionViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// The option to edit.
        /// </summary>
        public RelayOption Option { get; set; }

        /// <summary>
        /// Set the current option to null when hitting OK on the dialog.
        /// </summary>
        public bool SetToNull { get; set; }
        
        /// <summary>
        /// The current value of the editor input element.
        /// </summary>
        public string EditValue { get; set; }

        /// <summary>
        /// The value to send back to the WeeChat host after editing finishes.
        /// </summary>
        public string ValueToSave { get; set; }

        /// <summary>
        /// Empty constructor for the designer.
        /// </summary>
        public OptionViewModel()
        {
        }

        /// <summary>
        /// Create a new view model to edit an option.
        /// </summary>
        /// <param name="option">The option to edit.</param>
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

        /// <summary>
        /// Update the value to send back to the host based on the value of the editor input element.
        /// </summary>
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

        /// <summary>
        /// Update the state of the editor input element when setting to null has been checked.
        /// </summary>
        public void NotifySetToNullChanged()
        {
            NotifyPropertyChanged(nameof(SetToNull));
        }
    }
}

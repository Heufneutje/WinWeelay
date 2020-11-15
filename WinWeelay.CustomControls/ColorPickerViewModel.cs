using System.Collections.Generic;

namespace WinWeelay.CustomControls
{
    /// <summary>
    /// View model for the IRC color picker dropdown.
    /// </summary>
    public class ColorPickerViewModel
    {
        /// <summary>
        /// List of all available colors in the dropdown.
        /// </summary>
        public List<IrcColor> Colors { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColorPickerViewModel()
        {
            Colors = IrcColor.GetColors();
        }
    }
}

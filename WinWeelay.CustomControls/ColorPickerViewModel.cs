using System.Collections.Generic;

namespace WinWeelay.CustomControls
{
    public class ColorPickerViewModel
    {
        public List<IrcColor> Colors { get; set; }

        public ColorPickerViewModel()
        {
            Colors = IrcColor.GetColors();
        }
    }
}

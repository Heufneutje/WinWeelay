using System;
using WinWeelay.Core;

namespace WinWeelay
{
    public interface IBufferControl
    {
        event EventHandler SelectionChanged;

        RelayBuffer GetSelectedItem();

        void SelectItem(RelayBuffer buffer);

        void ClearSelection();
    }
}

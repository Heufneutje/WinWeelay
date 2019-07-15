using System;
using WinWeelay.Core;

namespace WinWeelay
{
    public interface IBufferControl
    {
        event EventHandler SelectionChanged;

        RelayBuffer GetSelectedBuffer();

        void SelectBuffer(RelayBuffer buffer);

        void ClearSelection();
    }
}

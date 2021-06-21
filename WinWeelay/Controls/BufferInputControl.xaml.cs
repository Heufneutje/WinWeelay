using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WinWeelay
{
    /// <summary>
    /// Input control.
    /// </summary>
    public partial class BufferInputControl : UserControl
    {
        /// <summary>
        /// The view model for the input control.
        /// </summary>
        public BufferInputViewModel ViewModel => (BufferInputViewModel)DataContext;
       
        /// <summary>
        /// Instantiate the input control.
        /// </summary>
        public BufferInputControl()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(_editorRichTextBox, new DataObjectPastingEventHandler(OnPaste));
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            string clipboardText = e.DataObject.GetData(typeof(string)) as string;
            if (clipboardText == null)
            {
                e.CancelCommand();
                return;
            }

            clipboardText = clipboardText.Replace("\r", string.Empty).Replace("\n", string.Empty);
            DataObject dataObject = new DataObject();
            dataObject.SetText(clipboardText);
            e.DataObject = dataObject;
        }

        /// <summary>
        /// Update the font settings.
        /// </summary>
        /// <param name="fontSize">The font size to set.</param>
        /// <param name="fontFamily">The font family to set.</param>
        /// <param name="defaultColor">The default text color based on the current theme.</param>
        public void UpdateFont(double fontSize, FontFamily fontFamily, Color defaultColor)
        {
            if (ViewModel.RelayConfiguration.IsFormattingToolbarVisible)
            {
                _editorRichTextBox.Document.UpdateFont(fontSize, fontFamily, false);
                _editorRichTextBox.Foreground = new SolidColorBrush(defaultColor);
            }
            else
            {
                _editorTextBox.FontSize = fontSize;
                _editorTextBox.FontFamily = fontFamily;
                _editorTextBox.Foreground = new SolidColorBrush(defaultColor);
            }
        }

        /// <summary>
        /// Focus the editor textbox that is currently being used.
        /// </summary>
        public void FocusEditor()
        {
            if (ViewModel.RelayConfiguration.IsFormattingToolbarVisible)
                _editorRichTextBox?.Focus();
            else
                _editorTextBox?.Focus();
        }

        /// <summary>
        /// Register the correct input box with the spell checker after the preferred input box has been changed.
        /// </summary>
        public void RefreshSpellCheckSubscription()
        {
            if (ViewModel.RelayConfiguration.IsFormattingToolbarVisible)
            {
                ViewModel.UnsubscribeSpellingManager(_editorTextBox);
                ViewModel.SubscribeSpellingManager(_editorRichTextBox);
            }
            else
            {
                ViewModel.UnsubscribeSpellingManager(_editorRichTextBox);
                ViewModel.SubscribeSpellingManager(_editorTextBox);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            if (ViewModel.RelayConfiguration.IsFormattingToolbarVisible)
                ViewModel.SubscribeSpellingManager(_editorRichTextBox);
            else
                ViewModel.SubscribeSpellingManager(_editorTextBox);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.RelayConfiguration.IsFormattingToolbarVisible)
                ViewModel.UnsubscribeSpellingManager(_editorRichTextBox);
            else
                ViewModel.UnsubscribeSpellingManager(_editorTextBox);
        }

        private void _editorRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_toolBarTray.Visibility == Visibility.Collapsed)
                return;

            object value = _editorRichTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            boldToggleButton.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(FontWeights.Bold);
            value = _editorRichTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            italicToggleButton.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(FontStyles.Italic);
            value = _editorRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            underlineToggleButton.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(TextDecorations.Underline);

            value = _editorRichTextBox.Selection.GetPropertyValue(TextElement.ForegroundProperty);
            if (value != null && value is SolidColorBrush textBrush)
                _foregroundIrcColorPicker.SetSelectedColor(textBrush.Color);
            else
                _foregroundIrcColorPicker.SetSelectedIndex(0);

            value = _editorRichTextBox.Selection.GetPropertyValue(TextElement.BackgroundProperty);
            if (value != null && value is SolidColorBrush backBrush)
                _backgroundIrcColorPicker.SetSelectedColor(backBrush.Color);
            else
                _backgroundIrcColorPicker.SetSelectedIndex(0);
        }

        private void _editorRichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    e.Handled = true;
                    ViewModel.HandleNickCompletion(_editorRichTextBox, _editorRichTextBox.GetPlainText());
                    break;
                case Key.Enter:
                    if (!_editorRichTextBox.IsEmpty())
                        ViewModel.SendMessage(_editorRichTextBox);
                    e.Handled = true;
                    break;
            }
        }

        private void _editorRichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    ViewModel.SetPreviousHistoryEntry(_editorRichTextBox);
                    break;
                case Key.Down:
                    ViewModel.SetNextHistoryEntry(_editorRichTextBox);
                    break;
            }
        }

        private void _editorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    e.Handled = true;
                    ViewModel.HandleNickCompletion(_editorTextBox, _editorTextBox.Text);
                    break;
                case Key.Enter:
                    if (!string.IsNullOrEmpty(_editorTextBox.Text))
                        ViewModel.SendMessage(_editorTextBox);
                    e.Handled = true;
                    break;
            }
        }

        private void _editorTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    ViewModel.SetPreviousHistoryEntry(_editorTextBox);
                    break;
                case Key.Down:
                    ViewModel.SetNextHistoryEntry(_editorTextBox);
                    break;
            }
        }

        private void _editorRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.ResetNickCompletion();
        }

        private void _backgroundIrcColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            _editorRichTextBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, _backgroundIrcColorPicker.SelectedColor.ColorBrush);
        }

        private void _foregroundIrcColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            if (_foregroundIrcColorPicker.SelectedColor.ColorIndex == 99)
                _editorRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(ViewModel.DefaultColor));
            else
                _editorRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, _foregroundIrcColorPicker.SelectedColor.ColorBrush);
        }
    }
}

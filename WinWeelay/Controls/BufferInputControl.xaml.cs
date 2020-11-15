using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WinWeelay.Configuration;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for BufferInputControl.xaml
    /// </summary>
    public partial class BufferInputControl : UserControl
    {
       
        private bool _modifiedPaste;

        public BufferInputViewModel ViewModel => (BufferInputViewModel)DataContext;
       
        public BufferInputControl()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(_editorRichTextBox, new DataObjectPastingEventHandler(OnPaste));
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (_modifiedPaste == false)
            {
                _modifiedPaste = true;
                string clipboard = e.DataObject.GetData(typeof(string)) as string;
                e.CancelCommand();

                string result = clipboard.Replace("\r", string.Empty).Replace("\n", string.Empty);
                Clipboard.SetData(DataFormats.Text, result);
                ApplicationCommands.Paste.Execute(result, _editorRichTextBox);
            }
            else
                _modifiedPaste = false;
        }

        public void UpdateFont(double fontSize, FontFamily fontFamily, Color defaultColor)
        {
            _editorRichTextBox.Document.UpdateFont(fontSize, fontFamily, false);
            _editorRichTextBox.Foreground = new SolidColorBrush(defaultColor);
        }

        public void FocusEditor()
        {
            _editorRichTextBox.Focus();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
                ViewModel.SubscribeSpellingManager(_editorRichTextBox);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.UnsubscribeSpellingManager(_editorRichTextBox);
        }

        private void editorRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
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
                foregroundIrcColorPicker.SetSelectedColor(textBrush.Color);
            else
                foregroundIrcColorPicker.SetSelectedIndex(0);

            value = _editorRichTextBox.Selection.GetPropertyValue(TextElement.BackgroundProperty);
            if (value != null && value is SolidColorBrush backBrush)
                backgroundIrcColorPicker.SetSelectedColor(backBrush.Color);
            else
                backgroundIrcColorPicker.SetSelectedIndex(0);
        }

        private void _editorRichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    e.Handled = true;
                    ViewModel.HandleNickCompletion(_editorRichTextBox);
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

        private void _editorRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.ResetNickCompletion();
        }

        private void backgroundIrcColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            _editorRichTextBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, backgroundIrcColorPicker.SelectedColor.ColorBrush);
        }

        private void foregroundIrcColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            if (foregroundIrcColorPicker.SelectedColor.ColorIndex == 99)
                _editorRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(ViewModel.DefaultColor));
            else
                _editorRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, foregroundIrcColorPicker.SelectedColor.ColorBrush);
        }
    }
}

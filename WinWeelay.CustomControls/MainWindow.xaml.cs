using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RichEditTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void editorRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object value = editorRichTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            boldToggleButton.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(FontWeights.Bold);
            value = editorRichTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            italicToggleButton.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(FontStyles.Italic);
            value = editorRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            underlineToggleButton.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(TextDecorations.Underline);

            value = editorRichTextBox.Selection.GetPropertyValue(TextElement.ForegroundProperty);
            if (value != null && value is SolidColorBrush textBrush)
                foregroundIrcColorPicker.SetSelectedColor(textBrush.Color);

            value = editorRichTextBox.Selection.GetPropertyValue(TextElement.BackgroundProperty);
            if (value != null && value is SolidColorBrush backBrush)
                backgroundIrcColorPicker.SetSelectedColor(backBrush.Color);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var test = editorRichTextBox.Document;
        }

        private void backgroundIrcColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            editorRichTextBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, backgroundIrcColorPicker.SelectedColor.ColorBrush);
        }

        private void foregroundIrcColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            editorRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, foregroundIrcColorPicker.SelectedColor.ColorBrush);
        }
    }
}

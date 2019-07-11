using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MWindowLib;

namespace WinWeelay
{
    public partial class ThemedMessageBoxWindow : MetroWindow
    {
        public string Caption
        {
            get
            {
                return Title;
            }
            set
            {
                Title = value;
            }
        }

        public string Message
        {
            get
            {
                return _messageTextBlock.Text;
            }
            set
            {
                _messageTextBlock.Text = value;
            }
        }

        public string OkButtonText
        {
            get
            {
                return _okLabel.Content.ToString();
            }
            set
            {
                _okLabel.Content = value;
            }
        }

        public string YesButtonText
        {
            get
            {
                return _yesLabel.Content.ToString();
            }
            set
            {
                _yesLabel.Content = value;
            }
        }

        public string NoButtonText
        {
            get
            {
                return _noLabel.Content.ToString();
            }
            set
            {
                _noLabel.Content = value;
            }
        }

        public MessageBoxResult Result { get; set; }

        public ThemedMessageBoxWindow(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            InitializeComponent();

            Message = message;
            Caption = caption;
            _messageBoxImage.Visibility = Visibility.Collapsed;

            DisplayButtons(button);
            DisplayImage(image);
        }

        private void DisplayButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.YesNo:
                    _yesButton.Visibility = Visibility.Visible;
                    _yesButton.Focus();
                    _noButton.Visibility = Visibility.Visible;
                    _okButton.Visibility = Visibility.Collapsed;
                    break;
                default:
                    _okButton.Visibility = Visibility.Visible;
                    _okButton.Focus();
                    _yesButton.Visibility = Visibility.Collapsed;
                    _noButton.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void DisplayImage(MessageBoxImage image)
        {
            Icon icon;
            switch (image)
            {
                case MessageBoxImage.Exclamation:
                    icon = SystemIcons.Exclamation;
                    break;
                case MessageBoxImage.Error:
                    icon = SystemIcons.Hand;
                    break;
                case MessageBoxImage.Information:
                    icon = SystemIcons.Information;
                    break;
                case MessageBoxImage.Question:
                    icon = SystemIcons.Question;
                    break;
                default:
                    icon = SystemIcons.Information;
                    break;
            }

            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            _messageBoxImage.Source = imageSource;
            _messageBoxImage.Visibility = Visibility.Visible;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage image, Window owner)
        {
            ThemedMessageBoxWindow window = new ThemedMessageBoxWindow(message, caption, button, image) { Owner = owner };
            window.ShowDialog();
            return window.Result;
        }
    }
}
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MWindowLib;

namespace WinWeelay
{
    /// <summary>
    /// A message box which uses the current theme.
    /// </summary>
    public partial class ThemedMessageBoxWindow : MetroWindow
    {
        /// <summary>
        /// The title of the dialog.
        /// </summary>
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

        /// <summary>
        /// The contents of the message.
        /// </summary>
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

        /// <summary>
        /// The text on the OK button.
        /// </summary>
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

        /// <summary>
        /// The text on the Yes button.
        /// </summary>
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

        /// <summary>
        /// The text on the No button.
        /// </summary>
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

        /// <summary>
        /// The dialog result.
        /// </summary>
        public MessageBoxResult Result { get; set; }

        /// <summary>
        /// Displays a message box that has a message, title bar caption, button, and icon and that returns a result.
        /// </summary>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A System.Windows.MessageBoxButton value that specifies which button or buttons to display.</param>
        /// <param name="image">A System.Windows.MessageBoxImage value that specifies the icon to display.</param>
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

        /// <summary>
        /// Displays a message box that has a message, title bar caption, button, and icon and that returns a result.
        /// </summary>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A System.Windows.MessageBoxButton value that specifies which button or buttons to display.</param>
        /// <param name="image">A System.Windows.MessageBoxImage value that specifies the icon to display.</param>
        /// <param name="owner">The parent window.</param>
        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage image, Window owner)
        {
            ThemedMessageBoxWindow window = new ThemedMessageBoxWindow(message, caption, button, image) { Owner = owner };
            window.ShowDialog();
            return window.Result;
        }
    }
}

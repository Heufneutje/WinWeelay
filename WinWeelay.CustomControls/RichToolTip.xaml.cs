using System.Windows;
using System.Windows.Controls;

namespace WinWeelay.CustomControls
{
    /// <summary>
    /// Tooltip with a formatted title and description.
    /// </summary>
    public partial class RichToolTip : UserControl
    {
        /// <summary>
        /// Property that holds the title for the tooltip message.
        /// </summary>
        public readonly static DependencyProperty TitleProperty;

        /// <summary>
        /// Property that holds the tooltip message.
        /// </summary>
        public readonly static DependencyProperty DescriptionProperty;

        /// <summary>
        /// The title for the tooltip message.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// The tooltip message.
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        static RichToolTip()
        {
            TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(RichToolTip), new UIPropertyMetadata());
            DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(RichToolTip), new UIPropertyMetadata());
        }

        /// <summary>
        /// Create a new instance of the tooltip.
        /// </summary>
        public RichToolTip()
        {
            InitializeComponent();
        }
    }
}

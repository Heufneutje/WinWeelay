using System.Windows;
using System.Windows.Controls;

namespace WinWeelay.CustomControls
{
    /// <summary>
    /// Custom control to display the properties of an option.
    /// </summary>
    public partial class OptionView : UserControl
    {
        /// <summary>
        /// Property that holds the title for the option.
        /// </summary>
        public readonly static DependencyProperty TitleProperty;

        /// <summary>
        /// Property that holds the decription of the option.
        /// </summary>
        public readonly static DependencyProperty DescriptionProperty;

        /// <summary>
        /// The title for the option.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// The decription of the option.
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public OptionView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a new instance of the option view.
        /// </summary>
        static OptionView()
        {
            TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(OptionView), new UIPropertyMetadata());
            DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(OptionView), new UIPropertyMetadata());
        }
    }
}

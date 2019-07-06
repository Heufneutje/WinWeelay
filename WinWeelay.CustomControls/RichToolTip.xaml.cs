using System.Windows;
using System.Windows.Controls;

namespace WinWeelay.CustomControls
{
    /// <summary>
    /// Interaction logic for RichToolTip.xaml
    /// </summary>
    public partial class RichToolTip : UserControl
    {
        public readonly static DependencyProperty TitleProperty;
        public readonly static DependencyProperty DescriptionProperty;

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        static RichToolTip()
        {
            TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(RichToolTip), new UIPropertyMetadata());
            DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(RichToolTip), new UIPropertyMetadata());
        }

        public RichToolTip()
        {
            InitializeComponent();
        }
    }
}

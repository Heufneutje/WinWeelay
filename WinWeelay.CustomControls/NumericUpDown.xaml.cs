using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WinWeelay.CustomControls
{
    public partial class NumericUpDown : UserControl
    {
        private RepeatButton _upButton;
        private RepeatButton _downButton;

        public readonly static DependencyProperty MaximumProperty;
        public readonly static DependencyProperty MinimumProperty;
        public readonly static DependencyProperty ValueProperty;
        public readonly static DependencyProperty StepProperty;

        public event EventHandler ValueChanged;

        public int MaxValue
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public int MinValue
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set
            {
                SetCurrentValue(ValueProperty, value);
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int StepValue
        {
            get { return (int)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
            MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(int.MaxValue));
            MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(int.MinValue));
            StepProperty = DependencyProperty.Register("StepValue", typeof(int), typeof(NumericUpDown), new FrameworkPropertyMetadata(1));
            ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(NumericUpDown), new FrameworkPropertyMetadata(0));
        }

        public NumericUpDown()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _upButton = Template.FindName("PART_UpButton", this) as RepeatButton;
            _downButton = Template.FindName("PART_DownButton", this) as RepeatButton;
            _upButton.Click += NumericUpDownButtonUp_Click;
            _downButton.Click += NumericUpDownTextBox_Click;
        }

        private void NumericUpDownButtonUp_Click(object sender, RoutedEventArgs e)
        {
            StepUp();
        }

        private void NumericUpDownTextBox_Click(object sender, RoutedEventArgs e)
        {
            StepDown();
        }

        private void NumericUpDownTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    StepUp();
                    break;
                case Key.Down:
                    StepDown();
                    break;
            }
        }

        private void NumericUpDownTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int number = 0;
            if (_numericUpDownTextBox.Text != "")
                if (!int.TryParse(_numericUpDownTextBox.Text, out number))
                    number = Value;

            if (number > MaxValue)
                number = MaxValue;
            if (number < MinValue)
                number = MinValue;

            Value = number;
            _numericUpDownTextBox.Text = number.ToString();
            _numericUpDownTextBox.SelectionStart = _numericUpDownTextBox.Text.Length;
        }

        private void NumericUpDownTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                StepUp();
            else if (e.Delta < 0)
                StepDown();
        }

        private void StepUp()
        {
            if (Value + StepValue < MaxValue)
                Value += StepValue;
            else
                Value = MaxValue;
        }

        private void StepDown()
        {
            if (Value - StepValue > MinValue)
                Value -= StepValue;
            else
                Value = MinValue;
        }
    }
}

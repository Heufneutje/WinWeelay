using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WinWeelay.CustomControls
{
    /// <summary>
    /// Control that implements a spin box element.
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        private RepeatButton _upButton;
        private RepeatButton _downButton;

        /// <summary>
        /// Property that holds the maximum numerical value.
        /// </summary>
        public readonly static DependencyProperty MaximumProperty;

        /// <summary>
        /// Property that holds the minimum numerical value.
        /// </summary>
        public readonly static DependencyProperty MinimumProperty;

        /// <summary>
        /// Property that holds the current value.
        /// </summary>
        public readonly static DependencyProperty ValueProperty;

        /// <summary>
        /// Property that holds the number by which to increment/decrement the value.
        /// </summary>
        public readonly static DependencyProperty StepProperty;

        /// <summary>
        /// Event to fire when the current value changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// The maximum numerical value.
        /// </summary>
        public int MaxValue
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        /// <summary>
        /// The minimum numerical value.
        /// </summary>
        public int MinValue
        {
            get => (int)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        /// <summary>
        /// The current value.
        /// </summary>
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set
            {
                SetCurrentValue(ValueProperty, value);
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The number by which to increment/decrement the value.
        /// </summary>
        public int StepValue
        {
            get => (int)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
            MaximumProperty = DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(int.MaxValue));
            MinimumProperty = DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(int.MinValue));
            StepProperty = DependencyProperty.Register(nameof(StepValue), typeof(int), typeof(NumericUpDown), new FrameworkPropertyMetadata(1));
            ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumericUpDown), new FrameworkPropertyMetadata(0));
        }

        /// <summary>
        /// Create a new instance of the control.
        /// </summary>
        public NumericUpDown()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize buttons and event handlers.
        /// </summary>
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

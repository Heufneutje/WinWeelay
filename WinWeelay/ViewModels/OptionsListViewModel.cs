using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    public class OptionsListViewModel : NotifyPropertyChangedBase
    {
        private RelayConnection _connection;
        public OptionsListWindow Owner { get; set; }
        public List<RelayOption> Options { get; set; }
        public RelayOption SelectedOption { get; set; }
        public string SearchFilter { get; set; }
        public bool IsOptionSelected => SelectedOption != null;
        public bool HasParentValue => SelectedOption?.ParentValue != null;
        public bool HasPossibleValues => SelectedOption?.PossibleValues?.Any() == true;
        public bool IsModified => SelectedOption?.IsModified == true;

        public DelegateCommand SearchCommand { get; private set; }
        public DelegateCommand EditCommand { get; private set; }
        public DelegateCommand ResetCommand { get; private set; }

        public OptionsListViewModel() : this(null)
        {
        }

        public OptionsListViewModel(RelayConnection connection)
        {
            _connection = connection;

            if (connection != null)
                _connection.OptionsParsed += Connection_OptionsParsed;

            Options = new List<RelayOption>();
            SearchCommand = new DelegateCommand(Search);
            EditCommand = new DelegateCommand(EditOption, CanEditOption);
            ResetCommand = new DelegateCommand(ResetOption, CanResetOption);
        }

        private void EditOption(object parameter)
        {
            IOptionWindow window = null;
            OptionViewModel viewModel = new OptionViewModel(SelectedOption);

            switch (SelectedOption.OptionType)
            {
                case "string":
                case "color":
                    if (SelectedOption.IsPassword)
                        window = new OptionPasswordWindow(viewModel) { Owner = Owner };
                    else
                        window = new OptionStringWindow(viewModel) { Owner = Owner };
                    break;
                case "boolean":
                    window = new OptionBooleanWindow(viewModel) { Owner = Owner };
                    break;
                case "integer":
                    if (SelectedOption.PossibleValuesString == null)
                        window = new OptionIntegerWindow(viewModel) { Owner = Owner };
                    else
                        window = new OptionComboBoxWindow(viewModel) { Owner = Owner };
                    break;
            }

            if (window == null)
            {
                ThemedMessageBoxWindow.Show("An editor dialog for this option type has not been implemented yet. Please report on the issue tracker.", "Not implemented", MessageBoxButton.OK, MessageBoxImage.Error, Owner);
            }
            else if (window.ShowDialog() == true)
            {
                _connection.OutputHandler.SetOption(SelectedOption.Name, viewModel.SetToNull ? "null" : viewModel.Option.Value);
                Search(null);
            }
        }

        private void ResetOption(object parameter)
        {
            _connection.OutputHandler.SetOption(SelectedOption.Name, SelectedOption.DefaultValueIsNull ? "null" : SelectedOption.DefaultValue);
            Search(null);
        }

        private bool CanEditOption(object parameter)
        {
            return IsOptionSelected;
        }

        private bool CanResetOption(object parameter)
        {
            return IsModified;
        }

        public void Search(object parameter)
        {
            _connection.OutputHandler.RequestOptions(SearchFilter);
        }

        public void OnSelectedOptionChanged()
        {
            NotifyPropertyChanged(nameof(SelectedOption));
            NotifyPropertyChanged(nameof(IsOptionSelected));
            NotifyPropertyChanged(nameof(IsModified));
            NotifyPropertyChanged(nameof(HasParentValue));
            NotifyPropertyChanged(nameof(HasPossibleValues));
            EditCommand.OnCanExecuteChanged();
            ResetCommand.OnCanExecuteChanged();
        }

        private void Connection_OptionsParsed(object sender, EventArgs e)
        {
            Options = _connection.OptionParser.GetParsedOptions();
            NotifyPropertyChanged(nameof(Options));
        }
    }
}

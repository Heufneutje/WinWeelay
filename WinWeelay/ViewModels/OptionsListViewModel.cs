using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    public class OptionsListViewModel : NotifyPropertyChangedBase
    {
        private RelayConnection _connection;
        private List<RelayOption> _options;
        private bool _isRefreshing;

        public OptionsListWindow Owner { get; set; }
        public bool IsChangingScroll { get; set; }
        public bool IsFullyLoaded { get; private set; }

        public ObservableCollection<RelayOption> LoadedOptions { get; set; }
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

            _options = new List<RelayOption>();
            LoadedOptions = new ObservableCollection<RelayOption>();
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
                _connection.OutputHandler.SetOption(SelectedOption.Name, viewModel.SetToNull ? "null" : viewModel.ValueToSave);
                Refresh();
            }
        }

        private void ResetOption(object parameter)
        {
            _connection.OutputHandler.SetOption(SelectedOption.Name, SelectedOption.DefaultValueIsNull ? "null" : SelectedOption.DefaultValue);
            Refresh();
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

        private void Refresh()
        {
            _isRefreshing = true;
            Search(null);
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
            _options = _connection.OptionParser.GetParsedOptions();

            if (_isRefreshing)
            {
                bool hasChanges = false;
                _isRefreshing = false;
                List<RelayOption> optionsToRemove = new List<RelayOption>();
                for (int i = 0; i < LoadedOptions.Count; i++)
                {
                    RelayOption option = LoadedOptions[i];
                    if (!_options.Contains(option))
                    {
                        optionsToRemove.Add(option);
                        hasChanges = true;
                    }
                    else
                    {
                        RelayOption newOption = _options.First(x => x.Name == option.Name);
                        if (newOption.Value != option.Value || newOption.ParentValue != option.ParentValue)
                        {
                            LoadedOptions.RemoveAt(i);
                            LoadedOptions.Insert(i, newOption);
                            hasChanges = true;
                        }
                    }
                }

                foreach (RelayOption option in optionsToRemove)
                    LoadedOptions.Remove(option);

                if (hasChanges)
                    NotifyPropertyChanged(nameof(LoadedOptions));
            }
            else
            {
                LoadedOptions.Clear();
                IsFullyLoaded = false;
                Owner.ResetScroll();
                LoadOptions();
            }
        }

        public void LoadOptions()
        {
            int stepSize = Owner.GetStepSize();
            int start = LoadedOptions.Count;

            for (int i = start; i < start + stepSize; i++)
            {
                if (i < _options.Count)
                {
                    RelayOption option = _options[i];
                    if (!LoadedOptions.Contains(option))
                        LoadedOptions.Add(option);
                }
                else
                {
                    IsFullyLoaded = true;
                    break;
                }
            }

            IsChangingScroll = false;
            NotifyPropertyChanged(nameof(LoadedOptions));

            if (LoadedOptions.Count != 0 && Owner.GetPossibleNumberOfVisibleItems() > LoadedOptions.Count && !IsFullyLoaded)
                LoadOptions();
        }
    }
}

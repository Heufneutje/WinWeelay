using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// View model for the WeeChat options editor window.
    /// </summary>
    public class OptionsListViewModel : NotifyPropertyChangedBase
    {
        private readonly OptionsListWindow _window;
        private List<RelayOption> _options;
        private bool _isRefreshing;

        /// <summary>
        /// Connection to the WeeChat host.
        /// </summary>
        public RelayConnection Connection;

        /// <summary>
        /// The optins editor window.
        /// </summary>
        public OptionsListWindow Owner { get; set; }

        /// <summary>
        /// Whether the scroll position has caused more messages to be loaded.
        /// </summary>
        public bool IsChangingScroll { get; set; }

        /// <summary>
        /// Whether all options have been loaded into the view based on the current search.
        /// </summary>
        public bool IsFullyLoaded { get; private set; }

        /// <summary>
        /// All options which have been loaded into the view.
        /// </summary>
        public ObservableCollection<RelayOption> LoadedOptions { get; set; }

        /// <summary>
        /// The option which is currently selected.
        /// </summary>
        public RelayOption SelectedOption { get; set; }

        /// <summary>
        /// The current search string.
        /// </summary>
        public string SearchFilter { get; set; }

        /// <summary>
        /// Whether an option is currently selected.
        /// </summary>
        public bool IsOptionSelected => SelectedOption != null;

        /// <summary>
        /// Whether the currently selected option has a description.
        /// </summary>
        public bool HasDescription => !string.IsNullOrEmpty(SelectedOption?.Description);

        /// <summary>
        /// Whether the currently selected option has a parent value.
        /// </summary>
        public bool HasParentValue => SelectedOption?.ParentValue != null;

        /// <summary>
        /// Whether the currently selected option has a list of values to choose from.
        /// </summary>
        public bool HasPossibleValues => SelectedOption?.PossibleValues?.Any() == true;

        /// <summary>
        /// Whether the currently selected option's value differs from its default value.
        /// </summary>
        public bool IsModified => SelectedOption?.IsModified == true;

        /// <summary>
        /// Whether to show the option list's detail view.
        /// </summary>
        public bool IsOptionsDetailViewVisible { get; set; }

        /// <summary>
        /// The description of the detail view that contains the currently selected option's name if an option is selected.
        /// </summary>
        public string SelectedOptionName => SelectedOption?.Name == null ? "No option selected" : SelectedOption.Name;

        /// <summary>
        /// Command to execute the defined search.
        /// </summary>
        public DelegateCommand SearchCommand { get; private set; }

        /// <summary>
        /// Command to edit the currently selected option.
        /// </summary>
        public DelegateCommand EditCommand { get; private set; }

        /// <summary>
        /// Command to reset the currently selected option to its default value.
        /// </summary>
        public DelegateCommand ResetCommand { get; private set; }

        /// <summary>
        /// Command to close the option list window.
        /// </summary>
        public DelegateCommand CloseCommand { get; private set; }

        /// <summary>
        /// Empty constructor for the designer.
        /// </summary>
        public OptionsListViewModel() : this(null, null)
        {
        }

        /// <summary>
        /// Create a new view model for the options window.
        /// </summary>
        /// <param name="connection">Connection to the WeeChat host.</param>
        /// <param name="window">The option list window.</param>
        public OptionsListViewModel(RelayConnection connection, OptionsListWindow window)
        {
            Connection = connection;
            _window = window;

            if (connection != null)
            {
                Connection.OptionsParsed += Connection_OptionsParsed;
                IsOptionsDetailViewVisible = Connection.Configuration.IsOptionsDetailViewVisible;
            }

            _options = new List<RelayOption>();
            LoadedOptions = new ObservableCollection<RelayOption>();
            SearchCommand = new DelegateCommand(Search);
            EditCommand = new DelegateCommand(EditOption, CanEditOption);
            ResetCommand = new DelegateCommand(ResetOption, CanResetOption);
            CloseCommand = new DelegateCommand(Exit);
        }

        private void EditOption(object parameter)
        {
            IOptionWindow window = null;
            OptionViewModel viewModel = new(SelectedOption);

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
                case "enum":
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
                Connection.OutputHandler.SetOption(SelectedOption.Name, viewModel.SetToNull ? "null" : viewModel.ValueToSave);
                Refresh();
            }
        }

        private void ResetOption(object parameter)
        {
            Connection.OutputHandler.SetOption(SelectedOption.Name, SelectedOption.DefaultValueIsNull ? "null" : SelectedOption.DefaultValue);
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

        private void Exit(object parameter)
        {
            _window.Close();
        }

        /// <summary>
        /// Command handler to execute the defined search.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        public void Search(object parameter)
        {
            Connection.OutputHandler.RequestOptions(SearchFilter);
        }

        private void Refresh()
        {
            _isRefreshing = true;
            Search(null);
        }

        /// <summary>
        /// Update the state of all commands which interact with the currently selected option.
        /// </summary>
        public void OnSelectedOptionChanged()
        {
            NotifyPropertyChanged(nameof(SelectedOption));
            NotifyPropertyChanged(nameof(IsOptionSelected));
            NotifyPropertyChanged(nameof(IsModified));
            NotifyPropertyChanged(nameof(HasDescription));
            NotifyPropertyChanged(nameof(HasParentValue));
            NotifyPropertyChanged(nameof(HasPossibleValues));
            NotifyPropertyChanged(nameof(SelectedOptionName));
            EditCommand.OnCanExecuteChanged();
            ResetCommand.OnCanExecuteChanged();
        }

        /// <summary>
        /// Update the state of the option detail view.
        /// </summary>
        public void UpdateViewSettings()
        {
            Connection.Configuration.IsOptionsDetailViewVisible = IsOptionsDetailViewVisible;
            NotifyPropertyChanged(nameof(IsOptionsDetailViewVisible));
        }

        private void Connection_OptionsParsed(object sender, EventArgs e)
        {
            _options = Connection.OptionParser.GetParsedOptions();

            if (_isRefreshing)
            {
                bool hasChanges = false;
                _isRefreshing = false;
                List<RelayOption> optionsToRemove = new();
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

        /// <summary>
        /// Load more options from the search result into view after it has been scrolled down to the bottom.
        /// </summary>
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

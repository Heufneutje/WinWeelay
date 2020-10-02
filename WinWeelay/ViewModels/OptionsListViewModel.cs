using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    public class OptionsListViewModel : NotifyPropertyChangedBase
    {
        private RelayConnection _connection;
        public List<RelayOption> Options { get; set; }
        public RelayOption SelectedOption { get; set; }
        public string SearchFilter { get; set; }
        public bool IsOptionSelected => SelectedOption != null;
        public bool HasParentValue => SelectedOption?.ParentValue != null;
        public bool HasPossibleValues => SelectedOption?.PossibleValues?.Any() == true;
        public bool IsModified => SelectedOption?.IsModified == true;

        public DelegateCommand SearchCommand { get; private set; }

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
        }

        private void Connection_OptionsParsed(object sender, EventArgs e)
        {
            Options = _connection.OptionParser.GetParsedOptions();
            NotifyPropertyChanged(nameof(Options));
        }
    }
}

using SignalDocumentBaseManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDocumentBaseManager.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand DocumentExplorerViewCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public DocumentExplorerViewModel DocumentExplorerVM { get; set; }

        public SettingsViewModel SettingsVM { get; set; }

        public MainViewModel()
        {
            DocumentExplorerVM = new DocumentExplorerViewModel();
            SettingsVM = new SettingsViewModel();

            CurrentView = SettingsVM;

            DocumentExplorerViewCommand = new RelayCommand(o =>
            {
                CurrentView = DocumentExplorerVM;
            });

            SettingsViewCommand = new RelayCommand(o =>
            {
                CurrentView = SettingsVM;
            });
        }


    }
}

using Newtonsoft.Json;
using SignalDocumentBaseManager.Classes;
using SignalDocumentBaseManager.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SignalDocumentBaseManager.MVVM.ViewModel
{
    internal class DocumentExplorerViewModel : ViewModelBase
    {
        string searchFilter = "None";
        List<DocumentFile> _documents = new List<DocumentFile>();
        List<User> _users = new List<User>();

        ObservableCollection<string> _searchByColumOptions = new ObservableCollection<string>() { "Нет", "Тип", "Название", "Номер", "Дата выхода", "Дата ввода в действи", "Ключевые слова" };

        public List<string> DocumentTypes = new List<string>() { "None", "ГОСТ", "РД", "Указ", "СТО", "МИ", "РИ", "Приказ", "Уведомление", "Постановление правительства" };

        User currentUser = null;

        public ObservableCollection<string> SearchByColumOptions { get => _searchByColumOptions; }

        public List<DocumentFile> Documents { get => _documents;}

        private List<DocumentFile> _outputCollection;
        public List<DocumentFile> OutputCollection
        {
            get { return _outputCollection; }
            set
            {
                _outputCollection = value;
                RaisePropertyChanged(nameof(OutputCollection));
            }
        }


        private string _searchOption;
        public string SearchOption
        {
            get { return _searchOption; }
            set
            {
                //mrthod that saves value for later
                _searchOption = value;
            }
        }

        private string _inputText;
        public string InputText
        {
            get { return _inputText; }
            set
            {
                _inputText = value;
            }
        }

        public ICommand ButtonCommand { get; set; }

        internal DocumentExplorerViewModel()
        {
            ButtonCommand = new RelayCommand(o => Search());

            LoadJson();

            OutputCollection = Documents;
        }

        private void LoadJson()
        {
            using (StreamReader r = new StreamReader("../../../Files/DocumentsJsonToParse.txt"))
            {
                string json = r.ReadToEnd();
                _documents = JsonConvert.DeserializeObject<List<DocumentFile>>(json);
            }

            using (StreamReader r = new StreamReader("../../../Files/UsersJsonToParse.txt"))
            {
                string json = r.ReadToEnd();
                _users = JsonConvert.DeserializeObject<List<User>>(json);
            }
        }

        private void Search()
        {
            var input = InputText.ToLower();
            List<DocumentFile> searchResult = new List<DocumentFile>();

            switch (searchFilter)
            {
                case "Тип":
                    foreach (var document in _documents)
                    {
                        if (document.Type.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;

                case "Название":
                    foreach (var document in _documents)
                    {
                        if (document.Name.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;

                case "Номер":
                    foreach (var document in _documents)
                    {
                        if (document.Number.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;

                case "Дата выхода":
                    foreach (var document in _documents)
                    {
                        if (document.ReleaseDate.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;

                case "Дата ввода в действие":
                    foreach (var document in _documents)
                    {
                        if (document.EntryDate.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;

                case "Ключевые слова":
                    foreach (var document in _documents)
                    {
                        if (document.KeyWords.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;
                default:

                    searchResult = _documents.Where(document =>
                    document.Id.ToString().ToLower().Contains(input) ||
                    document.Type.ToLower().Contains(input) ||
                    document.Name.ToLower().Contains(input) ||
                    document.Number.ToLower().Contains(input) ||
                    document.ReleaseDate.ToLower().Contains(input) ||
                    document.EntryDate.ToLower().Contains(input) ||
                    document.KeyWords.ToLower().Contains(input)).ToList();

                    break;
            }

            OutputCollection = searchResult;
           
        }

    }
}

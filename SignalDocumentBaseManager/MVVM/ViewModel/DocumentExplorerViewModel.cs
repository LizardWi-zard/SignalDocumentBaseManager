using Newtonsoft.Json;
using SignalDocumentBaseManager.Classes;
using SignalDocumentBaseManager.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static SignalDocumentBaseManager.Classes.DocumentSorter;

namespace SignalDocumentBaseManager.MVVM.ViewModel
{
    internal class DocumentExplorerViewModel : ViewModelBase
    {
        string searchFilter = "None";
        List<DocumentFile> _documents = new List<DocumentFile>();
        List<User> _users = new List<User>();

        ObservableCollection<string> _searchByColumOptions = new ObservableCollection<string>() { "Нет", "Тип", "Название", "Номер", "Дата выхода", "Дата ввода в действие", "Ключевые слова" };

        public List<string> DocumentTypes = new List<string>() { "None", "ГОСТ", "РД", "Указ", "СТО", "МИ", "РИ", "Приказ", "Уведомление", "Постановление правительства" };

        User currentUser = null;

        public ObservableCollection<string> SearchByColumOptions { get => _searchByColumOptions; }

        public List<DocumentFile> Documents { get => _documents; }

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
                Search();
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
        public ICommand ApplyDataCommand { get; set; }
        public ICommand ApplyLoginDataCommand { get; set; }
        public ICommand CreateAccountCommand {  get; set; }

        public ICommand SortByTypeCommand { get; set; } //
        public ICommand SortByNameCommand { get; set; }
        public ICommand SortByNumberCommand { get; set; }
        public ICommand SortByReleaseDateCommand { get; set; } //
        public ICommand SortByEntryDateCommand { get; set; } //

        internal DocumentExplorerViewModel()
        {
            ButtonCommand = new RelayCommand(o => Search());
            ApplyDataCommand = new RelayCommand(o => ApplyData());
            ApplyLoginDataCommand = new RelayCommand(o => SetCurrentUser());
            CreateAccountCommand = new RelayCommand(o => CreateAccount());

            SortByTypeCommand = new RelayCommand(o => InitiateSort(SortType.ByType));
            SortByNameCommand = new RelayCommand(o => InitiateSort(SortType.ByName));
            SortByNumberCommand = new RelayCommand(o => InitiateSort(SortType.ByNumber));
            SortByReleaseDateCommand = new RelayCommand(o => InitiateSort(SortType.ByRelease));
            SortByEntryDateCommand = new RelayCommand(o => InitiateSort(SortType.ByEntry));

            LoadJson();

            OutputCollection = CheckForAccess(Documents);
        }

        private List<DocumentFile> CheckForAccess(List<DocumentFile> documents)
        {
            Random r = new Random();
            int user_level_access = currentUser != null ? currentUser.AccessLevel : 3;

            var lst = documents;

            switch (user_level_access)
            {
                case 2:
                    lst = documents.Where(x => (x.AccessLevel == 2 || x.AccessLevel == 3)).ToList();
                    break;
                case 3:
                    lst = documents.Where(x => (x.AccessLevel == 3)).ToList();
                    break;
                default:
                    lst = documents;
                    break;
            }

            return lst;
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

            switch (SearchOption)
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

            OutputCollection = CheckForAccess(searchResult);
        }

        public string NewDocumentType { get; set; }
        public string NewDocumentName { get; set; }
        public string NewDocumentNumber { get; set; }
        public DateTime NewDocumentReleaseDate { get; set; }
        public DateTime NewDocumentEntryDate { get; set; }
        public string NewDocumentKeyWords { get; set; }

        private void ApplyData()
        {
            DocumentFile newDocument = new DocumentFile();

            newDocument.Id = OutputCollection.Count() + 1;
            newDocument.Type = NewDocumentType;
            newDocument.Name = NewDocumentName;
            newDocument.Number = NewDocumentNumber;
            newDocument.ReleaseDate = NewDocumentReleaseDate.ToString();
            newDocument.EntryDate = NewDocumentEntryDate.ToString();
            newDocument.KeyWords = NewDocumentKeyWords;

            ValidateDataAndSend(newDocument);
        }

        private void ValidateDataAndSend(DocumentFile newDocument)
        {
            // if 1930 < date < DateTime.Now
            // if Name.Length > 0 || name != null
            // if number != null

            DateTime date = DateTime.Now;

            bool isName = newDocument.Name.All(symbol => Char.IsLetter(symbol) || Char.IsPunctuation(symbol) ||
                                                         Char.IsSeparator(symbol)) && newDocument.Name.Length > 0;
            bool isNumber = newDocument.Number.All(symbol => (symbol >= '0' && symbol <= '9') || (symbol == '.') ||
                                                             (symbol == '/') || (symbol == '-')) && newDocument.Number.Length > 0;

            bool isReleaseDateLowerThanEntryDate;

            if (!String.IsNullOrEmpty(newDocument.EntryDate) && !String.IsNullOrEmpty(newDocument.ReleaseDate))
            {
                isReleaseDateLowerThanEntryDate = DateTime.Parse(newDocument.EntryDate) > DateTime.Parse(newDocument.ReleaseDate);
            }
            else
            {
                isReleaseDateLowerThanEntryDate = false;
            }

            if (isName && isNumber && isReleaseDateLowerThanEntryDate)
            {
                try
                {
                    DataBaseInteracter.PostDocumentsAsync(newDocument);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The exception happend: " + ex.ToString());
                }
                finally
                {
                    MessageBox.Show("Method done");
                }
            }
            else
            {
                MessageBox.Show("Некоторые поля не были заполнены или содержат недопустимые символы. Исправьте ошибки и попробуйте еще раз.");
                // DocumentDataInput.Visibility = Visibility.Visible;
            }
        }


        public string LoginBoxInput { get; set; }
        public string PasswordBoxInput { get; set; }

        private void SetCurrentUser()
        {
            var login = LoginBoxInput;
            var password = PasswordBoxInput;

            try
            {
                currentUser = (User)_users.FirstOrDefault(x => x.Login == login && x.Password == password) ?? null;

                if (currentUser == null)
                {
                    throw new NullReferenceException(); //UserNotFoundException
                }

                MessageBox.Show("Вход выполнен! \n Добро пожаловать, " + currentUser.Login);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Пользователь не найден\nПопробуйте снова");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex);
            }

            OutputCollection = CheckForAccess(Documents);
        }

        public string LoginCreationBoxInput { get; set; }
        public string PasswordCreationBoxInput { get; set; }
        public string PasswordConfirmBoxInput { get; set; }

        private void CreateAccount()
        {
            // This method starts user account creation
            var newLogin = LoginCreationBoxInput;
            var newPassword = PasswordCreationBoxInput;
            var newPasswordComfirmation = PasswordConfirmBoxInput;

            var stat1 = !String.IsNullOrEmpty(newPassword);
            var stat2 = !String.IsNullOrEmpty(newPasswordComfirmation);
            var stat3 = newPassword == newPasswordComfirmation;
            try
            {
                if (stat1 && stat2 && stat3)
                {
                    User newUser = new User(newLogin, newPassword);
                    currentUser = newUser;
                   _users.Add(newUser);
                }

                MessageBox.Show("Аккаунт создан! \nДобро пожаловать, " + currentUser.Login);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("Ошибка в создании пользовател\n" + ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка! \nОшибка: " + ex);
            }
        }

        void InitiateSort(SortType type)
        {
            OutputCollection = DocumentSorter.Sort(OutputCollection, type); ;
        }
    }
}

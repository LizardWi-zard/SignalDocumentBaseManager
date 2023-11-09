using Newtonsoft.Json;
using SignalDocumentBaseManager.Classes;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SignalDocumentBaseManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string searchFilter = "None";
        List<Document> documents = new List<Document>();
        List<User> users = new List<User>();
        public List<string> DocumentTypes = new List<string>() { "ГОСТ", "РД", "Указ", "СТО", "МИ", "РИ", "Приказ", "Уведомление", "Постановление правительства" };

        User currentUser = null;


        enum AccessLEvel
        {
            Admin = 1,
            Worker = 2,
            Guest = 3
        }

        public MainWindow()
        {
            InitializeComponent();

            LoadJson();

            TypeFilter_ComboBox.ItemsSource = DocumentTypes;
            TypeFilter_ComboBox.Items.Refresh();

            DocumentType_Combobox.ItemsSource = DocumentTypes;
            DocumentType_Combobox.Items.Refresh();

            // documents = GetDocumentsAsync().Result.ToList();

            RefreshData();
        }

        public void LoadJson()
        {
            using (StreamReader r = new StreamReader("../../../Files/DocumentsJsonToParse.txt"))
            {
                string json = r.ReadToEnd();
                documents = JsonConvert.DeserializeObject<List<Document>>(json);
            }

            using (StreamReader r = new StreamReader("../../../Files/UsersJsonToParse.txt"))
            {
                string json = r.ReadToEnd();
                users = JsonConvert.DeserializeObject<List<User>>(json);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            //documents = GetDocumentsAsync().Result.ToList();


           // documents = JsonConvert.DeserializeObject<List<Document>>(data);

            var input = searchBox.Text.ToLower();

            var searchResult = SearchAtDataBase(input, documents);

            searchResult = CheckForAccess(searchResult);

            DocumentsListBox.ItemsSource = searchResult;
            DocumentsListBox.Items.Refresh();
        }
             
        private List<Document> SearchAtDataBase(string input, List<Document> documents)
        {
            List<Document> searchResult = new List<Document>();

            switch (searchFilter)
            {
                case "Тип":
                    foreach (var document in documents)
                    {
                        if (document.Type.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;

                case "Название":
                    foreach (var document in documents)
                    {
                        if (document.Name.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;

                case "Номер":
                    foreach (var document in documents)
                    {
                        if (document.Number.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;

                case "Дата выхода":
                    foreach (var document in documents)
                    {
                        if (document.ReleaseDate.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;

                case "Дата ввода в действие":
                    foreach (var document in documents)
                    {
                        if (document.EntryDate.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;

                case "Ключевые слова":
                    foreach (var document in documents)
                    {
                        if (document.KeyWords.ToLower().Contains(input))
                        {
                            searchResult.Add(document);
                        }
                    }
                    break;
                default:
                    
                    searchResult = documents.Where(document =>
                    document.Id.ToString().ToLower().Contains(input) ||
                    document.Type.ToLower().Contains(input) ||
                    document.Name.ToLower().Contains(input) ||
                    document.Number.ToLower().Contains(input) ||
                    document.ReleaseDate.ToLower().Contains(input) ||
                    document.EntryDate.ToLower().Contains(input) ||
                    document.KeyWords.ToLower().Contains(input)).ToList();

                    break;
            }

            return searchResult;
        }

        private void AddDocument_Click(object sender, RoutedEventArgs e)
        {
            DocumentDataInput.Visibility = Visibility.Visible;
        }

        private void Filter_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchFilter = Filter_ComboBox.SelectedValue.ToString();

            documents = GetDocumentsAsync().Result.ToList();

            var input = searchBox.Text.ToLower();

            SearchAtDataBase(input, documents);
        }

        public class Document
        {
            public int Id { get; set; }

            public string Type { get; set; }

            public string Name { get; set; }

            public string Number { get; set; }

            public string ReleaseDate { get; set; }

            public string EntryDate { get; set; }

            public string KeyWords { get; set; }

            public int AccessLevel { get; set; }
        }

        private void ApplyData_Click(object sender, RoutedEventArgs e)
        {
            DocumentDataInput.Visibility = Visibility.Collapsed;

            Document newDocument = new Document();

            newDocument.Id = documents.Count() + 1;
            newDocument.Type = DocumentType_Combobox.SelectedValue.ToString();
            newDocument.Name = DocumentName_textbox.Text;
            newDocument.Number = DocumentNumber_textbox.Text;
            newDocument.ReleaseDate = DocumentReleaseDate_textbox.Text;
            newDocument.EntryDate = DocumentEntryDate_textbox.Text;
            newDocument.KeyWords = DocumentKeyWords_textbox.Text;

            ValidateDataAndSend(newDocument);
        }

        private void ValidateDataAndSend(Document newDocument)
        {
            // if 1930 < date < DateTime.Now
            // if Name.Length > 0 || name != null
            // if number != null

            DateTime date = DateTime.Now;

            bool isName = newDocument.Name.All(symbol => Char.IsLetter(symbol) || Char.IsPunctuation(symbol) || 
                                                         Char.IsSeparator(symbol)) && newDocument.Name.Length > 0;
            bool isNumber = newDocument.Number.All(symbol => (symbol >= '0' && symbol <= '9') ||(symbol == '.') || 
                                                             (symbol == '/') || (symbol == '-')) && newDocument.Number.Length > 0;
            bool isReleaseDate = DateTime.TryParse(newDocument.ReleaseDate, out date) && newDocument.ReleaseDate.Length > 0;
            bool isEntryDate = DateTime.TryParse(newDocument.EntryDate, out date) && newDocument.EntryDate.Length > 0;

            if (isName && isNumber && isReleaseDate && isEntryDate)
            {
                try
                {
                    PostDocumentsAsync(newDocument);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The exception happend: " + ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Некоторые поля не были заполнены или содержат недопустимые символы. Исправьте ошибки и попробуйте еще раз.");
                DocumentDataInput.Visibility = Visibility.Visible;
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            DocumentDataInput.Visibility = Visibility.Collapsed;
        }

        async Task<Document[]> GetDocumentsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync("https://localhost:7231/Documents").ConfigureAwait(false);

                return await result.Content.ReadFromJsonAsync<Document[]>();
            }
        }

        async Task PostDocumentsAsync(Document documentToPost)
        {
            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(documentToPost);

                string param = $"json={json}";

                var content = new StringContent(param, Encoding.UTF8, "application/json");

                string ct = content.ToString();

                try
                {
                    var result = await client.PostAsync("https://localhost:7231/Documents?" + param, content);

                    result.EnsureSuccessStatusCode();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Exception heppend: " + ex);
                }
            }
        }

        private void DocumentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        List<Document> CheckForAccess(List<Document> documents)
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

        private void ShowAutorizationWindow_Click(object sender, RoutedEventArgs e)
        {
            PasswordEnter_textBox.Text = "";
            LoginEnter_textBox.Text = "";

            AutorizationWindow.Visibility = Visibility.Visible;
        }

        private void ShowRegistrationWindow_Click(object sender, RoutedEventArgs e)
        {
            PasswordCreationConfirm_textBox.Text = "";
            PasswordCreation_textBox.Text = "";
            LoginCreation_textBox.Text = "";

            RegistrationWindow.Visibility = Visibility.Visible;

            AutorizationWindow.Visibility = Visibility.Collapsed;
        }

        private void CloseAutorizationWindow_Click(object sender, RoutedEventArgs e)
        {
            AutorizationWindow.Visibility = Visibility.Collapsed;
        }

        private void ReturnToAutorizationWindow_Click(object sender, RoutedEventArgs e)
        {
            PasswordEnter_textBox.Text = "";
            LoginEnter_textBox.Text = "";

            RegistrationWindow.Visibility = Visibility.Collapsed;

            AutorizationWindow.Visibility = Visibility.Visible;
        }

        private void SetCurrentUser_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginEnter_textBox.Text;
            var password = PasswordEnter_textBox.Text;

            try
            {
                currentUser = (User)users.FirstOrDefault(x => x.Login == login && x.Password == password) ?? null;

                if(currentUser == null)
                {
                    throw new NullReferenceException(); //UserNotFoundException
                }

                MessageBox.Show("Вход выполнен! \n Добро пожаловать, " + currentUser.Login);

                AutorizationWindow.Visibility = Visibility.Collapsed;

                RefreshData();
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Пользователь не найден\nПопробуйте снова");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex);
            }
        }

        void RefreshData()
        {
            var accessableDocuments = CheckForAccess(documents);

            DocumentsListBox.ItemsSource = accessableDocuments;
            DocumentsListBox.Items.Refresh();
        }

        void RefreshData(List<Document> lst)
        {
            var accessableDocuments = CheckForAccess(lst);

            DocumentsListBox.ItemsSource = accessableDocuments;
            DocumentsListBox.Items.Refresh();
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            // This method starts user account creation
            string newLogin = LoginCreation_textBox.Text;
            string newPassword = PasswordCreation_textBox.Text;
            string newPasswordComfirmation = PasswordCreationConfirm_textBox.Text;

            try
            {
                if (String.IsNullOrEmpty(newPassword) && String.IsNullOrEmpty(newPasswordComfirmation) && newPassword == newPasswordComfirmation)
                {
                    User newUser = new User(newLogin, newPassword);
                    currentUser = newUser;
                    users.Add(newUser);
                }

                PasswordCreationConfirm_textBox.Text = "";
                PasswordCreation_textBox.Text = "";
                LoginCreation_textBox.Text = "";

                RegistrationWindow.Visibility = Visibility.Collapsed;

                AutorizationWindow.Visibility = Visibility.Collapsed;

                MessageBox.Show("Аккаунт создан! \nДобро пожаловать, " + currentUser.Login);
            }
            catch(ArgumentException ex)
            {
                MessageBox.Show("Ошибка в создании пользовател\n" + ex);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка! \nОшибка: " + ex);
            }
        }

        private void RemoveFilters_Click(object sender, RoutedEventArgs e)
        {
            DateFromFilter_DatePicker.SelectedDate = null;
            DateBeforeFilter_DatePicker.SelectedDate = null;

            //TypeFilter_ComboBox.SelectedItem = DocumentTypes[0];
        }

        private void TypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = TypeFilter_ComboBox.SelectedValue.ToString().ToLower();

           // documents = JsonConvert.DeserializeObject<List<Document>>(data);

            var input = searchBox.Text.ToLower();

            var searchResult = SearchAtDataBase(input, documents);

            searchResult = searchResult.Where(x => x.Type.ToLower().Contains(selectedItem)).ToList();

            RefreshData(searchResult);
        }

        private void DateFromFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var date = DateFromFilter_DatePicker.SelectedDate;

           // documents = JsonConvert.DeserializeObject<List<Document>>(data);

            var input = searchBox.Text.ToLower();

            var searchResult = documents;

            searchResult = documents.Where(x => DateTime.Parse(x.EntryDate) > date).ToList();

            RefreshData(searchResult);

        }

        private void DateBeforeFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var date = DateFromFilter_DatePicker.SelectedDate;

           // documents = JsonConvert.DeserializeObject<List<Document>>(data);

            var input = searchBox.Text.ToLower();

            var searchResult = documents;

            searchResult = documents.Where(x => DateTime.Parse(x.EntryDate) < date).ToList();

            RefreshData(searchResult);
        }

        private void SortById_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SortByType_Click(object sender, RoutedEventArgs e)
        {
            //bool sortingInAscendingOrder

            int docsCount = documents.Count;
            for (int indexOfLastDocumentInSortedPart = 0; indexOfLastDocumentInSortedPart < docsCount - 1; indexOfLastDocumentInSortedPart++)// indexOfLastDocumentInSortedPart is index of last document, which is in sorted part of documents
            {
                for (int indexOfCurrentDocument = 0; indexOfCurrentDocument < docsCount - indexOfLastDocumentInSortedPart - 1; indexOfCurrentDocument++)
                {
                    int indexOfNextDocument = indexOfCurrentDocument + 1;
                    if (documents[indexOfCurrentDocument].Type.CompareTo(documents[indexOfNextDocument].Type) > 0)
                    {
                        var docCopy = documents[indexOfCurrentDocument];                        
                        documents[indexOfCurrentDocument] = documents[indexOfNextDocument];
                        documents[indexOfNextDocument] = docCopy;
                    }
                }
            }
            DocumentsListBox.Items.Refresh();
        }

        private void SortByName_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SortByNumber_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SortByReleaseDate_Click(object sender, RoutedEventArgs e)
        {
            int docsCount = documents.Count;
            for (int indexOfLastDocumentInSortedPart = 0; indexOfLastDocumentInSortedPart < docsCount - 1; indexOfLastDocumentInSortedPart++)// indexOfLastDocumentInSortedPart is index of last document, which is in sorted part of documents
            {
                for (int indexOfCurrentDocument = 0; indexOfCurrentDocument < docsCount - indexOfLastDocumentInSortedPart - 1; indexOfCurrentDocument++)
                {
                    int indexOfNextDocument = indexOfCurrentDocument + 1;
                    DateTime currentReleaseDate = DateTime.Parse(documents[indexOfCurrentDocument].ReleaseDate);
                    DateTime nextReleaseDate = DateTime.Parse(documents[indexOfNextDocument].ReleaseDate);
                    if (currentReleaseDate.CompareTo(nextReleaseDate) > 0)
                    {
                        var docCopy = documents[indexOfCurrentDocument];
                        documents[indexOfCurrentDocument] = documents[indexOfNextDocument];
                        documents[indexOfNextDocument] = docCopy;
                    }
                }
            }
            DocumentsListBox.Items.Refresh();
        }

        private void SortByEntryDate_Click(object sender, RoutedEventArgs e)
        {
            int docsCount = documents.Count;
            for (int indexOfLastDocumentInSortedPart = 0; indexOfLastDocumentInSortedPart < docsCount - 1; indexOfLastDocumentInSortedPart++)// indexOfLastDocumentInSortedPart is index of last document, which is in sorted part of documents
            {
                for (int indexOfCurrentDocument = 0; indexOfCurrentDocument < docsCount - indexOfLastDocumentInSortedPart - 1; indexOfCurrentDocument++)
                {
                    int indexOfNextDocument = indexOfCurrentDocument + 1;
                    DateTime currentEntryDate = DateTime.Parse(documents[indexOfCurrentDocument].EntryDate);
                    DateTime nextEntryDate = DateTime.Parse(documents[indexOfNextDocument].EntryDate);
                    if (currentEntryDate.CompareTo(nextEntryDate) > 0)
                    {
                        var docCopy = documents[indexOfCurrentDocument];
                        documents[indexOfCurrentDocument] = documents[indexOfNextDocument];
                        documents[indexOfNextDocument] = docCopy;
                    }
                }
            }
            DocumentsListBox.Items.Refresh();
        }
    }
}

﻿using Newtonsoft.Json;
using SignalDocumentBaseManager.Classes;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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

        string data = "[{\"ID\":1,\"Type\":\"ГОСТ (гос. стандарт)\",\"Name\":\"Информационные технологии. Комплекс стандартов на автоматизированные системы управления\",\"Number\":\"34.602-2020\",\"ReleaseDate\":\"2020-12-22\",\"EntryDate\":\"2022-01-01\",\"KeyWords\":\"технологии\",\"AccessLevel\":1}, " +
                       "{\"ID\":2,\"Type\":\"РД (рук документ)\",\"Name\":\"Котлы паровые и водогрейные, трубопроводы пара и горячей воды.\",\"Number\":\"2730.940.103-92\",\"ReleaseDate\":\"1992-12-25\",\"EntryDate\":\"1993-01-01\",\"KeyWords\":\"Котлы\",\"AccessLevel\":3}," +
                       "{\"ID\":3,\"Type\":\"Указ (президента)\",\"Name\":\"О призыве в октябре — декабре 2023 г. граждан Российской Федерации на военную службу\",\"Number\":\"375\",\"ReleaseDate\":\"2023-09-29\",\"EntryDate\":\"2023-10-01\",\"KeyWords\":\"Указ\",\"AccessLevel\":2}, " +
                       "{\"ID\":4,\"Type\":\"Постановление правительства\",\"Name\":\"О внесении изменений в Правила холодного водоснабжения и водоотведения\",\"Number\":\"1670\",\"ReleaseDate\":\"2023-10-10\",\"EntryDate\":\"2023-11-01\",\"KeyWords\":\"водоотведения водоснабжения\",\"AccessLevel\":1}," +
                       "{\"ID\":5,\"Type\":\"СТО (стандарт организации)\",\"Name\":\"Проведения аттестации испытательной лаборатории\",\"Number\":\"7.5.18-2020\",\"ReleaseDate\":\"2023-05-23\",\"EntryDate\":\"2023-07-01\",\"KeyWords\":\"аттестации лаборатории\",\"AccessLevel\":1}," +
                       "{\"ID\":6,\"Type\":\"МИ (металогическая инструкция)\",\"Name\":\"Ведение электронной документации\",\"Number\":\"8.12-2018\",\"ReleaseDate\":\"2018-08-16\",\"EntryDate\":\"2019-01-01\",\"KeyWords\":\"металогическая инструкция\",\"AccessLevel\":2}," +
                       "{\"ID\":7,\"Type\":\"РИ (Рабочая инструкция)\",\"Name\":\"Обработка входящих писем\",\"Number\":\"5.45-2016\",\"ReleaseDate\":\"2016-07-24\",\"EntryDate\":\"2016-08-01\",\"KeyWords\":\"Обработка\",\"AccessLevel\":2}, " +
                       "{\"ID\":8,\"Type\":\"Приказ (директора предприятия)\",\"Name\":\"О поощрении работников годовой премией\",\"Number\":\"475\",\"ReleaseDate\":\"2022-12-24\",\"EntryDate\":\"2023-01-01\",\"KeyWords\":\"Приказ премия\",\"AccessLevel\":2}, " +
                       "{\"ID\":9,\"Type\":\"Уведомление (подразделений предприятия)\",\"Name\":\"О проведение годового собрания акционеров\",\"Number\":\"336\",\"ReleaseDate\":\"2023-03-03\",\"EntryDate\":\"2023-04-01\",\"KeyWords\":\"Уведомление \",\"AccessLevel\":3}]";
       
        string localUsers = "[{\"Login\":\"111\",\"Password\":\"111\",\"AccessLevel\":1}," +
                             "{\"Login\":\"222\",\"Password\":\"222\",\"AccessLevel\":2}," +
                             "{\"Login\":\"333\",\"Password\":\"333\",\"AccessLevel\":3}," +
                             "{\"Login\":\"1_Gosha_1\",\"Password\":\"nd13ah8\",\"AccessLevel\":2}," +
                             "{\"Login\":\"VituaUtkin\",\"Password\":\"be1ur34g\",\"AccessLevel\":3}]";

        User currentUser = null;

        public List<string> DocumentTypes = new List<string>() { "ГОСТ", "Указ", "ОФВ", "ДТ", "АПБ", "ЖУКС", "ЫЫс"};

        enum AccessLEvel
        {
            Admin = 1,
            Worker = 2,
            Guest = 3
        }

        public MainWindow()
        {
            InitializeComponent();

            documents = JsonConvert.DeserializeObject<List<Document>>(data);
            users = JsonConvert.DeserializeObject<List<User>>(localUsers);

            TypeFilter_ComboBox.ItemsSource = DocumentTypes;
            TypeFilter_ComboBox.Items.Refresh();

            DocumentType_Combobox.ItemsSource = DocumentTypes;
            DocumentType_Combobox.Items.Refresh();

            // documents = GetDocumentsAsync().Result.ToList();

            RefreshData();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            //documents = GetDocumentsAsync().Result.ToList();


            documents = JsonConvert.DeserializeObject<List<Document>>(data);

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
                    MessageBox.Show("The exceprion happend: " + ex.ToString());
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

        }

        private void TypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = TypeFilter_ComboBox.SelectedValue.ToString().ToLower();

            documents = JsonConvert.DeserializeObject<List<Document>>(data);

            var input = searchBox.Text.ToLower();

            var searchResult = SearchAtDataBase(input, documents);

            searchResult = searchResult.Where(x => x.Type.ToLower().Contains(selectedItem)).ToList();

            RefreshData(searchResult);
        }

        private void DateFromFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var date = DateFromFilter_DatePicker.SelectedDate;

            documents = JsonConvert.DeserializeObject<List<Document>>(data);

            var input = searchBox.Text.ToLower();

            var searchResult = documents;

            searchResult = documents.Where(x => DateTime.Parse(x.EntryDate) > date).ToList();

            RefreshData(searchResult);

        }

        private void DateBeforeFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var date = DateFromFilter_DatePicker.SelectedDate;

            documents = JsonConvert.DeserializeObject<List<Document>>(data);

            var input = searchBox.Text.ToLower();

            var searchResult = documents;

            searchResult = documents.Where(x => DateTime.Parse(x.EntryDate) < date).ToList();

            RefreshData(searchResult);
        }
    }
}

using Newtonsoft.Json;
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

        public MainWindow()
        {
            InitializeComponent();


            documents = GetDocumentsAsync().Result.ToList();
            DocumentsListBox.ItemsSource = documents;
            DocumentsListBox.Items.Refresh();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            documents = GetDocumentsAsync().Result.ToList();

            var input = searchBox.Text.ToLower();

            SearchAtDataBase(input, documents);
        }
             
        private void SearchAtDataBase(string input, List<Document> documents)
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
            DocumentsListBox.ItemsSource = searchResult;
            DocumentsListBox.Items.Refresh();
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
        }

        private void ApplyData_Click(object sender, RoutedEventArgs e)
        {
            DocumentDataInput.Visibility = Visibility.Collapsed;

            Document newDocument = new Document();

            newDocument.Id = documents.Count() + 1;
            newDocument.Type = DocumentType_textbox.Text;
            newDocument.Name = DocumentName_textbox.Text;
            newDocument.Number = DocumentNumber_textbox.Text;
            newDocument.ReleaseDate = DocumentReleaseDate_textbox.Text;
            newDocument.EntryDate = DocumentEntryDate_textbox.Text;
            newDocument.KeyWords = DocumentKeyWords_textbox.Text;

            PostDocumentsAsync(newDocument);
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
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        string searchFilter = "None"; //TODO: использовать в реализации алгоритма поиска с фильтром

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            string data = "[{\"ID\":1,\"Type\":\"ГОСТ (гос. стандарт)\",\"Name\":\"Информационные технологии. Комплекс стандартов на автоматизированные системы управления\",\"Number\":\"34.602-2020\",\"ReleaseDate\":\"2020-12-22\",\"EntryDate\":\"2022-01-01\",\"KeyWords\":\"технологии\"}, {\"ID\":2,\"Type\":\"РД (рук документ)\",\"Name\":\"Котлы паровые и водогрейные, трубопроводы пара и горячей воды.\",\"Number\":\"2730.940.103-92\",\"ReleaseDate\":\"1992-12-25\",\"EntryDate\":\"1993-01-01\",\"KeyWords\":\"Котлы\"}, {\"ID\":3,\"Type\":\"Указ (президента)\",\"Name\":\"О призыве в октябре — декабре 2023 г. граждан Российской Федерации на военную службу\",\"Number\":\"375\",\"ReleaseDate\":\"2023-09-29\",\"EntryDate\":\"2023-10-01\",\"KeyWords\":\"Указ\"}, {\"ID\":4,\"Type\":\"Постановление правительства\",\"Name\":\"О внесении изменений в Правила холодного водоснабжения и водоотведения\",\"Number\":\"1670\",\"ReleaseDate\":\"2023-10-10\",\"EntryDate\":\"2023-11-01\",\"KeyWords\":\"водоотведения водоснабжения\"}, {\"ID\":5,\"Type\":\"СТО (стандарт организации)\",\"Name\":\"Проведения аттестации испытательной лаборатории\",\"Number\":\"7.5.18-2020\",\"ReleaseDate\":\"2023-05-23\",\"EntryDate\":\"2023-07-01\",\"KeyWords\":\"аттестации лаборатории\"}, {\"ID\":6,\"Type\":\"МИ (металогическая инструкция)\",\"Name\":\"Ведение электронной документации\",\"Number\":\"8.12-2018\",\"ReleaseDate\":\"2018-08-16\",\"EntryDate\":\"2019-01-01\",\"KeyWords\":\"металогическая инструкция\"}]";

            List<Document> documents = new List<Document>();

            documents = JsonConvert.DeserializeObject<Document[]>(data).ToList();

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
                    document.KeyWords.ToLower().Contains(input));
            }
            DocumentsListBox.ItemsSource = searchResult;
            DocumentsListBox.Items.Refresh();
        }

        private void Filter_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchFilter = Filter_ComboBox.SelectedValue.ToString();

            string data = "[{\"ID\":1,\"Type\":\"ГОСТ (гос. стандарт)\",\"Name\":\"Информационные технологии. Комплекс стандартов на автоматизированные системы управления\",\"Number\":\"34.602-2020\",\"ReleaseDate\":\"2020-12-22\",\"EntryDate\":\"2022-01-01\",\"KeyWords\":\"технологии\"}, {\"ID\":2,\"Type\":\"РД (рук документ)\",\"Name\":\"Котлы паровые и водогрейные, трубопроводы пара и горячей воды.\",\"Number\":\"2730.940.103-92\",\"ReleaseDate\":\"1992-12-25\",\"EntryDate\":\"1993-01-01\",\"KeyWords\":\"Котлы\"}, {\"ID\":3,\"Type\":\"Указ (президента)\",\"Name\":\"О призыве в октябре — декабре 2023 г. граждан Российской Федерации на военную службу\",\"Number\":\"375\",\"ReleaseDate\":\"2023-09-29\",\"EntryDate\":\"2023-10-01\",\"KeyWords\":\"Указ\"}, {\"ID\":4,\"Type\":\"Постановление правительства\",\"Name\":\"О внесении изменений в Правила холодного водоснабжения и водоотведения\",\"Number\":\"1670\",\"ReleaseDate\":\"2023-10-10\",\"EntryDate\":\"2023-11-01\",\"KeyWords\":\"водоотведения водоснабжения\"}, {\"ID\":5,\"Type\":\"СТО (стандарт организации)\",\"Name\":\"Проведения аттестации испытательной лаборатории\",\"Number\":\"7.5.18-2020\",\"ReleaseDate\":\"2023-05-23\",\"EntryDate\":\"2023-07-01\",\"KeyWords\":\"аттестации лаборатории\"}, {\"ID\":6,\"Type\":\"МИ (металогическая инструкция)\",\"Name\":\"Ведение электронной документации\",\"Number\":\"8.12-2018\",\"ReleaseDate\":\"2018-08-16\",\"EntryDate\":\"2019-01-01\",\"KeyWords\":\"металогическая инструкция\"}]";

            List<Document> documents = new List<Document>();

            documents = JsonConvert.DeserializeObject<Document[]>(data).ToList();

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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Xml.Linq;

namespace SignalDocumentBaseManager.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для DocumentExplorerView.xaml
    /// </summary>
    public partial class DocumentExplorerView : UserControl
    {
        public DocumentExplorerView()
        {
            InitializeComponent();

            TypeFilter_ComboBox.ItemsSource = new List<string>() {"None", "ГОСТ", "РД", "Указ", "СТО", "МИ", "РИ", "Приказ", "Уведомление", "Постановление правительства" };
            DateFromFilter_DatePicker.SelectedDate = null;
            DateBeforeFilter_DatePicker.SelectedDate = null;
        }

        private void AddDocument_Click(object sender, RoutedEventArgs e)
        {
            DocumentDataInput.Visibility = Visibility.Visible;
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            DocumentDataInput.Visibility = Visibility.Collapsed;
        }

        private void ShowAutorizationWindow_Click(object sender, RoutedEventArgs e)
        {
            PasswordEnter_textBox.Text = "";
            LoginEnter_textBox.Text = "";

            AutorizationWindow.Visibility = Visibility.Visible;
        }

        private void CloseAutorizationWindow_Click(object sender, RoutedEventArgs e)
        {
            PasswordEnter_textBox.Text = "";
            LoginEnter_textBox.Text = "";

            AutorizationWindow.Visibility = Visibility.Collapsed;
        }

        private void ShowRegistrationWindow_Click(object sender, RoutedEventArgs e)
        {
            PasswordCreationConfirm_textBox.Text = "";
            PasswordCreation_textBox.Text = "";
            LoginCreation_textBox.Text = "";

            RegistrationWindow.Visibility = Visibility.Visible;

            AutorizationWindow.Visibility = Visibility.Collapsed;
        }

        private void ReturnToAutorizationWindow_Click(object sender, RoutedEventArgs e)
        {
            PasswordEnter_textBox.Text = "";
            LoginEnter_textBox.Text = "";

            RegistrationWindow.Visibility = Visibility.Collapsed;

            AutorizationWindow.Visibility = Visibility.Visible;
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            PasswordCreationConfirm_textBox.Text = "";
            PasswordCreation_textBox.Text = "";
            LoginCreation_textBox.Text = "";

            RegistrationWindow.Visibility = Visibility.Collapsed;

            AutorizationWindow.Visibility = Visibility.Collapsed;

        }

        private void SetCurrentUser_Click(object sender, RoutedEventArgs e)
        {
            AutorizationWindow.Visibility = Visibility.Collapsed;
        }

        private void RefreshList_Click(object sender, RoutedEventArgs e)
        {
            DocumentsListBox.Items.Refresh();
        }
    }
}

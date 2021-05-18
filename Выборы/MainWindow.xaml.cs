using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Выборы.Classes;


namespace Выборы
{    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User user;
        public MainWindow()
        {
            InitializeComponent();
            Controller controller = new Controller();

        }
        
        /// <summary>
        /// Авторизация пользователя на странице авторизации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = AuthLoginTextBox.Text.ToString();
            var password = AuthPasswordBox.Password.ToString();
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните логин и пароль");
                return;
            }

            user = Controller.AuthUser(login, password);
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }
            AuthGrid.Visibility = Visibility.Hidden;
            MainGrid.Visibility = Visibility.Visible;

        }
        /// <summary>
        /// Переход на страницу регистрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromAuthToRegistration_Click(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(RegistrationGrid, AuthGrid);
        }

        private void UserExitMainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            user = null;
            MainGrid_IsVisibleChanged(MainGrid, new DependencyPropertyChangedEventArgs());
        }

        private void FromMainToAuth_Click(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(AuthGrid, MainGrid);
        }

        private void MainGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((Grid)sender).Visibility == Visibility.Visible)
            {

                if (user == null)
                {
                    MenuMyProfileButton.Visibility = Visibility.Hidden;
                    UserEnterMainMenuButton.Visibility = Visibility.Visible;
                    UserExitMainMenuButton.Visibility = Visibility.Hidden;

                    MenuNavigationPanel.Margin = new Thickness(10, 10, UserEnterMainMenuButton.Width + UserEnterMainMenuButton.Margin.Right + 10, 0);
                    
                }
                else
                {
                    
                    MenuMyProfileButton.Visibility = Visibility.Visible;
                    UserEnterMainMenuButton.Visibility = Visibility.Hidden;
                    UserExitMainMenuButton.Visibility = Visibility.Visible;

                    MenuNavigationPanel.Margin = new Thickness(10, 10, UserExitMainMenuButton.Width + UserExitMainMenuButton.Margin.Right + 10, 0);

                    if (user.Role_id == 1)
                    {
                        MenuNavigationPanelCreateElection.Visibility = Visibility.Visible;
                    }
                    
                }
            }
        }

        private void MenuNavigationPanelCreateElection_Click(object sender, RoutedEventArgs e)
        {
            NewsGrid.Visibility = Visibility.Hidden;
            CreateElectionGrid.Visibility = Visibility.Visible;
        }
        private void ChangeGridVisibility(Grid makeVisible, Grid makeHidden)
        {
            makeVisible.Visibility = Visibility.Visible;
            makeHidden.Visibility = Visibility.Hidden;
        }

        private void CreateElectionGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DateStartElectionDataPicker.DisplayDateStart = DateTime.Now;
            DateEndElectionDataPicker.DisplayDateStart = DateTime.Now;
        }

        private void DateStartElectionDataPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {            
            DateEndElectionDataPicker.DisplayDateStart = ((DatePicker)sender).SelectedDate;
            if (DateStartElectionDataPicker.SelectedDate > DateEndElectionDataPicker.SelectedDate)
            {
                DateEndElectionDataPicker.SelectedDate = DateStartElectionDataPicker.SelectedDate;
            }
        }

        private void AddElectionButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ElectionNameTextBox.Text;
            DateTime? start = DateStartElectionDataPicker.SelectedDate;
            DateTime? end = DateEndElectionDataPicker.SelectedDate;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название голосования");
                return;
            }
            if (start == null)
            {
                MessageBox.Show("Введите дату начала голосования");
                return;
            }
            if (end == null)
            {
                MessageBox.Show("Введите дату окончания голосования");
                return;
            }

            if (!Controller.AddElection(name, (DateTime)start, (DateTime)end))
            {
                MessageBox.Show("Не удалось сохранить голосование");
            }
        }

        private void RegistrateButton_Click(object sender, RoutedEventArgs e)
        {
            string login = RegistrationLoginTextBox.Text;
            string wrongLoginMessage = Controller.IsLoginValidate(login);
            if (wrongLoginMessage != "")
            {
                MessageBox.Show(wrongLoginMessage);
                return;
            }
            string password = RegistrationPasswordPasswordBox.Password;
            string wrongPasswordMessage = Controller.IsPasswordValidate(password);
            if (wrongPasswordMessage != "")
            {
                MessageBox.Show(wrongPasswordMessage);
                return;
            }
            
            string password2 = RegistrationPasswordDoublePasswordBox.Password;
            if (string.IsNullOrEmpty(password2))
            {
                MessageBox.Show("Введите повторный пароль");
                return;
            }
            if (password != password2)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }

            string passportSeries = RegistrationSeriesPassportTextBox.Text;
            string passportNumber = RegistrationNumberPassportTextBox.Text;
            string wrongPassportMessage = Controller.IsPassportValidate(passportSeries, passportNumber);
            if (wrongPassportMessage != "")
            {
                MessageBox.Show(wrongPassportMessage);
                return;
            }

            string firstName = RegistrationFirstNameTextBox.Text;
            if (string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("Введите фамилию");
                return;
            }
            string name = RegistrationNameTextBox.Text;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите имя");
                return;
            }
            string lastName = RegistrationLastNameTextBox.Text;
            
            string email = RegistrationEmailTextBox.Text;
            string wrongEmailMessage = Controller.IsEmailValidate(email);
            if (wrongEmailMessage != "")
            {
                MessageBox.Show(wrongEmailMessage);
                return;
            }

            string phone = RegistrationPhoneTextBox.Text;
            string wrongPhoneMessage = Controller.IsPhoneValidate(phone);
            if (wrongEmailMessage != "")
            {
                MessageBox.Show(wrongPhoneMessage);
                return;
            }

            DateTime? bith = RegistrationBirthdayDatePicker.SelectedDate;
            if (bith == null)
            {
                MessageBox.Show("Укажите дату рождения");
                return;
            }

            //если сюда дошло, то все данные заполнены верно
            var result = Controller.AddUser(login, password, passportSeries, passportNumber, firstName, name, lastName, email, phone, (DateTime)bith);
            if (result is string)
            {
                MessageBox.Show(result);
            }
            else if (result is User)
            {
                MessageBox.Show("Регистрация прошла успешно");
                RegistrationGrid.Visibility = Visibility.Hidden;
                MainGrid.Visibility = Visibility.Visible;
            }
        }

        private void RegistrationLoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string message = Controller.IsLoginValidate(RegistrationLoginTextBox.Text);
            if(message != "")
            {
                RegistrationLoginTextBox.BorderBrush = Brushes.Red;
                RegistrationLoginHelperLabel.ToolTip = Properties.Language.RegistrationLabelHelperMessage + ". " + message;
                RegistrationLoginHelperLabel.Background = Brushes.LightPink;
            }
            else
            {
                RegistrationLoginTextBox.BorderBrush = Brushes.Green;
                RegistrationLoginHelperLabel.ToolTip = Properties.Language.RegistrationLabelHelperMessage;
                RegistrationLoginHelperLabel.Background = null;
            }
        }

        private void RegistrationPassportData_TextChanged(object sender, TextChangedEventArgs e)
        {
            string message = Controller.IsPassportValidate(RegistrationSeriesPassportTextBox.Text, RegistrationNumberPassportTextBox.Text);
            if (message != "")
            {

            }
        }

        private void RegistrationGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}

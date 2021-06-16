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
        const int electionsOnOnePage = 5;

        User user;
        Election election;
        List<Grid> lastGrid = new List<Grid>();
        Grid nowMenuGrid;
        List<string> listOptions = new List<string>();
        List<Option> options;
        List<User> listCandidates = new List<User>();
        Election[] listNews;
        int page = 1;
        public MainWindow()
        {
            MakeBasicSettings();
            InitializeComponent();
            
        }
        /// <summary>
        /// Выполняет базовые проверки и настройки при запуске приложения
        /// </summary>
        private void MakeBasicSettings()
        {
            CheckConnection();
        }
        /// <summary>
        /// Проверяет подключение к БД
        /// </summary>
        private void CheckConnection()
        {
            string message = Controller.TryConnectBD();
            if (message != null)
            {
                MessageBox.Show(message);
            }
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
            lastGrid.Add(AuthGrid);
        }
        /// <summary>
        /// Нажатие на кнопку выхода из акканта пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserExitMainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            user = null;
            MainGrid_IsVisibleChanged(MainGrid, new DependencyPropertyChangedEventArgs());
            ChangeGridVisibility(NewsGrid, nowMenuGrid);
        }
        /// <summary>
        /// переход на страницу авторизации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromMainToAuth_Click(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(AuthGrid, MainGrid);
            lastGrid.Add(MainGrid);
        }
        /// <summary>
        /// изменение видимости главной страницы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((Grid)sender).Visibility == Visibility.Visible)
            {
                if (user == null)
                {
                    MenuMyProfileButton.Visibility = Visibility.Collapsed;
                    UserEnterMainMenuButton.Visibility = Visibility.Visible;
                    UserExitMainMenuButton.Visibility = Visibility.Hidden;
                    MenuNavigationPanelAdministration.Visibility = Visibility.Collapsed;
                    MenuNavigationPanelMyElections.Visibility = Visibility.Collapsed;

                    MenuNavigationPanel.Margin = new Thickness(10, 10, UserEnterMainMenuButton.Width + UserEnterMainMenuButton.Margin.Right + 10, 0);
                }
                else
                {
                    MenuMyProfileButton.Visibility = Visibility.Visible;
                    UserEnterMainMenuButton.Visibility = Visibility.Hidden;
                    UserExitMainMenuButton.Visibility = Visibility.Visible;
                    MenuNavigationPanelMyElections.Visibility = Visibility.Visible;

                    MenuNavigationPanel.Margin = new Thickness(10, 10, UserExitMainMenuButton.Width + UserExitMainMenuButton.Margin.Right + 10, 0);

                    if (user.Role_id == 1)
                    {
                        MenuNavigationPanelAdministration.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MenuNavigationPanelAdministration.Visibility = Visibility.Collapsed;
                    }

                }
            }
        }
        /// <summary>
        /// Переход на страницу создания голосования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuNavigationPanelCreateElection_Click(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(CreateElectionGrid, nowMenuGrid);
            nowMenuGrid = CreateElectionGrid;
        }
        /// <summary>
        /// изменить видимости Grid
        /// </summary>
        /// <param name="makeVisible">сделать видимым</param>
        /// <param name="makeHidden">сделать скрытым</param>
        private void ChangeGridVisibility(Grid makeVisible, Grid makeHidden)
        {
            makeHidden.Visibility = Visibility.Collapsed;
            makeVisible.Visibility = Visibility.Visible;
        }

        private void CreateElectionGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (CreateElectionGrid.Visibility == Visibility.Visible)
            {
                nowMenuGrid = CreateElectionGrid;

                DateStartElectionDataPicker.DisplayDateStart = DateTime.Now;
                DateStartElectionDataPicker.SelectedDate = null;
                DateEndElectionDataPicker.DisplayDateStart = DateTime.Now;
                DateEndElectionDataPicker.SelectedDate = null;
                
                DescriptionElectionTextBox.Text = "";
                ElectionNameTextBox.Text = "";
                InterviewRadioButton.IsChecked = true;

                InterviewOptionsComboBox.ItemsSource = null;
                CandidatsChoosedComboBox.ItemsSource = null;
                DescriptionElectionTextBox.Text = "";

                InterviewOptionsCreateGrid.Visibility = Visibility.Visible;
                ElectionOptionsCreateGrid.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// Событие при изменении даты начала голосования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateStartElectionDataPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateEndElectionDataPicker.DisplayDateStart = ((DatePicker)sender).SelectedDate;
            if (DateStartElectionDataPicker.SelectedDate > DateEndElectionDataPicker.SelectedDate)
            {
                DateEndElectionDataPicker.SelectedDate = DateStartElectionDataPicker.SelectedDate;
            }
        }
        /// <summary>
        /// Создать голосование
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddElectionButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ElectionNameTextBox.Text.Trim();
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
            
            if (InterviewRadioButton.IsChecked == true)
            {
                if (listOptions == null || listOptions.Count == 0)
                {
                    MessageBox.Show("Добавьте варинты голосования");
                    return;
                }
                string message = Controller.AddInterview(name, (DateTime)start, (DateTime)end, DescriptionElectionTextBox.Text, listOptions);
                MessageBox.Show(message);
            }
            if (ElectionRadioButton.IsChecked == true)
            {
                if (listCandidates == null || listCandidates.Count == 0)
                {
                    MessageBox.Show("Добавьте кандидатов голосования");
                    return;
                }
                string message = Controller.AddElection(name, (DateTime)start, (DateTime)end, DescriptionElectionTextBox.Text, listCandidates);
                MessageBox.Show(message);
            }
            ChangeGridVisibility(NewsGrid, CreateElectionGrid);
        }
        /// <summary>
        /// Нажата кнопка зарегистрироваться
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrateButton_Click(object sender, RoutedEventArgs e)
        {
            string login = RegistrationLoginTextBox.Text.Trim();
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


            string firstName = RegistrationFirstNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("Введите фамилию");
                return;
            }
            string name = RegistrationNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите имя");
                return;
            }
            string lastName = RegistrationLastNameTextBox.Text.Trim();

            string email = RegistrationEmailTextBox.Text.Trim();
            string wrongEmailMessage = Controller.IsEmailValidate(email);
            if (wrongEmailMessage != "")
            {
                MessageBox.Show(wrongEmailMessage);
                return;
            }

            string phone = RegistrationPhoneTextBox.Text.Trim();
            string wrongPhoneMessage = Controller.IsPhoneValidate(phone);
            if (wrongPhoneMessage != "")
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
            if (result == null)
            {
                MessageBox.Show("Ошибка регистрации");
            }
            else
            {
                MessageBox.Show("Регистрация прошла успешно");
                user = Controller.AuthUser(login, password);
                ChangeGridVisibility(MainGrid, RegistrationGrid);
            }
        }
        /// <summary>
        /// Изменение Логина на странице регистрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrationLoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string message = Controller.IsLoginValidate(RegistrationLoginTextBox.Text);
            if (message != "")
            {
                RegistrationLoginTextBox.BorderBrush = Brushes.Red;
                RegistrationLoginHelperLabel.ToolTip = Properties.Language.RegistrationLoginLabelHelperMessage + ". " + message;
                RegistrationLoginHelperLabel.Background = Brushes.LightPink;
            }
            else
            {
                RegistrationLoginTextBox.BorderBrush = Brushes.Green;
                RegistrationLoginHelperLabel.ToolTip = Properties.Language.RegistrationLoginLabelHelperMessage;
                RegistrationLoginHelperLabel.Background = null;
            }
        }
        /// <summary>
        /// Изменение видимости страницы регистрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrationGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RegistrationLoginTextBox.Text = "";
            RegistrationPasswordPasswordBox.Password = "";
            RegistrationPasswordDoublePasswordBox.Password = "";
            RegistrationSeriesPassportTextBox.Text = "";
            RegistrationNumberPassportTextBox.Text = "";
            RegistrationFirstNameTextBox.Text = "";
            RegistrationNameTextBox.Text = "";
            RegistrationLastNameTextBox.Text = "";
            RegistrationEmailTextBox.Text = "";
            RegistrationPhoneTextBox.Text = "";
            RegistrationBirthdayDatePicker.SelectedDate = null;

            RegistrationLoginTextBox_TextChanged(null, null);
            RegistrationPasswordPasswordBox_PasswordChanged(null, null);
            RegistrationPassportData_TextChanged(null, null);
            RegistrationEmailTextBox_TextChanged(null, null);

        }
        /// <summary>
        /// Изменение паспортных данных на странице регистрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrationPassportData_TextChanged(object sender, TextChangedEventArgs e)
        {
            string message = Controller.IsPassportValidate(RegistrationSeriesPassportTextBox.Text, RegistrationNumberPassportTextBox.Text);
            if (message != "")
            {
                RegistrationSeriesPassportTextBox.BorderBrush = Brushes.Red;
                RegistrationNumberPassportTextBox.BorderBrush = Brushes.Red;
                RegistrationPasportHelperLabel.Background = Brushes.LightPink;
                RegistrationPasportHelperLabel.ToolTip = Properties.Language.RegistrationPassportLabelHelperMessage + ". " + message;
            }
            else
            {
                RegistrationSeriesPassportTextBox.BorderBrush = Brushes.Green;
                RegistrationNumberPassportTextBox.BorderBrush = Brushes.Green;
                RegistrationPasportHelperLabel.Background = null;
            }
        }
        /// <summary>
        /// Изменение пароля на странице регистрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrationPasswordPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string message = Controller.IsPasswordValidate(RegistrationPasswordPasswordBox.Password);
            if (message != "")
            {
                RegistrationPasswordPasswordBox.BorderBrush = Brushes.Red;
                RegistrationPassworgHelperLabel.Background = Brushes.LightPink;
            }
            else
            {
                RegistrationPasswordPasswordBox.BorderBrush = Brushes.Green;
                RegistrationPassworgHelperLabel.Background = null;
            }
        }
        /// <summary>
        /// Изменение почты на странице регистрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrationEmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string message = Controller.IsEmailValidate(RegistrationEmailTextBox.Text);
            if (message != "")
            {
                RegistrationEmailTextBox.BorderBrush = Brushes.Red;
                RegistrationEmailHelperLabel.Background = Brushes.LightPink;
                RegistrationEmailHelperLabel.ToolTip = Properties.Language.RegistrationEmailLabelHelperMessage + ". " + message;
            }
            else
            {
                RegistrationEmailTextBox.BorderBrush = Brushes.Green;
                RegistrationEmailHelperLabel.Background = null;
                RegistrationEmailHelperLabel.ToolTip = Properties.Language.RegistrationEmailLabelHelperMessage + ". " + Properties.Language.FieldIsRequired;
            }
        }
        /// <summary>
        /// Нажата кнопка "Назад"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComeBackButton_Click(object sender, RoutedEventArgs e)
        {
            var last = lastGrid.LastOrDefault();
            var now = (Grid)((Button)sender).Parent;
            if (last != null && now.GetType() == typeof(Grid))
            {
                ChangeGridVisibility(last, now);
                lastGrid.Remove(last);
            }
        }
        /// <summary>
        /// Переход на страницу "Пользователи"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuNavigationPanelUsers_Click(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(UsersGrid, nowMenuGrid);
            nowMenuGrid = UsersGrid;
        }
        /// <summary>
        /// Изменение видимости страницы "Пользователи"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsersGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (UsersGrid.Visibility == Visibility.Visible)
            {
                nowMenuGrid = UsersGrid;

                UsersDataGrid.ItemsSource = Controller.GetAllUsers();
                RolesComboBox.ItemsSource = Controller.GetAllRoles();
            }
        }
        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = UsersDataGrid.SelectedItem;
            if (selectedItem != null)
            {
                if (Controller.DeleteUser((User)selectedItem))
                {
                    MessageBox.Show("Пользователь удален успешно");
                    UsersDataGrid.ItemsSource = null;
                    UsersDataGrid.ItemsSource = UsersDataGrid.ItemsSource = Controller.GetAllUsers();
                }
                else
                {
                    MessageBox.Show("Ошибка удаления пользователя");
                }
                
            }
            else
            {
                MessageBox.Show("Выберите пользователя");
            }
        }
        /// <summary>
        /// Изменение роли пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeUserRoleButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRole = RolesComboBox.SelectedItem;
            var selectedUser = UsersDataGrid.SelectedItem;

            if (selectedRole == null)
            {
                MessageBox.Show("Выберите роль");
                return;
            }
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }
            if (Controller.ChahgeRole((User)selectedUser, (Role)selectedRole))
            {
                MessageBox.Show("Изменение роли прошло успешно");
                UsersDataGrid.ItemsSource = null;
                UsersDataGrid.ItemsSource = UsersDataGrid.ItemsSource = Controller.GetAllUsers();
            }
            else
            {
                MessageBox.Show("Ошибка изменения роли");
            }
        }
        /// <summary>
        /// Добавить опцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddOptionButton_Click(object sender, RoutedEventArgs e)
        {
            var optionName = OptionsTextBox.Text;
            if (string.IsNullOrEmpty(optionName))
            {
                MessageBox.Show("Введите название опции");
                return;
            }
            if (listOptions.Contains(optionName))
            {
                MessageBox.Show("Такой вариант уже добавлен");
                return;
            }
            listOptions.Add(optionName);
            InterviewOptionsComboBox.ItemsSource = null;
            InterviewOptionsComboBox.ItemsSource = listOptions;
            if (listOptions.Count > 0) InterviewOptionsComboBox.SelectedItem = listOptions.Last();
        }
        /// <summary>
        /// Изменение видимости создания опций для опроса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterviewOptionsCreateStackPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OptionsTextBox.Text = "";
            InterviewOptionsComboBox.ItemsSource = null;
            listOptions.Clear();
        }
        /// <summary>
        /// Удалить опцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteOptionButton_Click(object sender, RoutedEventArgs e)
        {
            var item = InterviewOptionsComboBox.SelectedItem;
            if (item == null)
            {
                MessageBox.Show("Выберите, какую опцию удалить");
                return;
            }
            listOptions.Remove((string)item);
            InterviewOptionsComboBox.ItemsSource = null;
            InterviewOptionsComboBox.ItemsSource = listOptions;
            if (listOptions.Count > 0 ) InterviewOptionsComboBox.SelectedItem = listOptions.Last();

        }
        /// <summary>
        /// выбрано создание опроса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterviewRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(InterviewOptionsCreateGrid, ElectionOptionsCreateGrid);
        }
        /// <summary>
        /// выбрано создание выборов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElectionRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(ElectionOptionsCreateGrid, InterviewOptionsCreateGrid);
        }
        /// <summary>
        /// изменение видимости создания опций для выборов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElectionOptionsCreateGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ElectionOptionsCreateGrid.Visibility == Visibility.Visible)
            {
                CandidatsComboBox.ItemsSource = Controller.GetCandidates();
            }
        }
        /// <summary>
        /// добавить кандидата в качестве опции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCandidateOptionToChoosedButton_Click(object sender, RoutedEventArgs e)
        {
            User item = (User)CandidatsComboBox.SelectedItem;
            if (item == null)
            {
                MessageBox.Show("Выберите кандидата");
                return;
            }
            if (listCandidates.Contains(item))
            {
                MessageBox.Show("Этот кандидат уже добавлен");
                return;
            }
            listCandidates.Add(item);
            CandidatsChoosedComboBox.ItemsSource = null;
            CandidatsChoosedComboBox.ItemsSource = listCandidates;
            CandidatsChoosedComboBox.SelectedItem = listCandidates.Last();

            List<User> l = (List<User>)CandidatsComboBox.ItemsSource;
            l.Remove(item);
            CandidatsComboBox.ItemsSource = null;
            CandidatsComboBox.ItemsSource = l;


        }
        /// <summary>
        /// Удалить кандидата из опций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCandidateOptionToChoosedButton_Click(object sender, RoutedEventArgs e)
        {
            User item = (User)CandidatsChoosedComboBox.SelectedItem;
            if (item == null)
            {
                MessageBox.Show("Выберите кандидата");
                return;
            }
            listCandidates.Remove(item);
            CandidatsChoosedComboBox.ItemsSource = null;
            CandidatsChoosedComboBox.ItemsSource = listCandidates;
            if (listCandidates.Count > 0) CandidatsChoosedComboBox.SelectedItem = listCandidates.Last();

            List<User> l = (List<User>)CandidatsComboBox.ItemsSource;
            l.Add(item);
            CandidatsComboBox.ItemsSource = null;
            CandidatsComboBox.ItemsSource = l;
            CandidatsComboBox.SelectedItem = item;
        }
        /// <summary>
        /// изменена видимость страницы "Новости"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewsGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (NewsGrid.Visibility == Visibility.Visible)
            {
                nowMenuGrid = NewsGrid;

                listNews = Controller.GetElections().ToArray();

                CountPagesLabel.Content = (listNews.Length/electionsOnOnePage + 1).ToString();

                LoadPageNews(page);
            }
        }
        private void LoadPageNews(int page)
        {
            if (listNews == null || listNews.Length == 0) return;
            NewsStackPanel.Children.Clear();
            for (int i = (page - 1) * electionsOnOnePage; i < page * electionsOnOnePage; i++)
            {
                if (i < listNews.Length)
                {
                    NewsStackPanel.Children.Add(CreateElectionConteiner(listNews[i]));
                }
                else break;
            }
            NowPageLabel.Content = page.ToString();

            PageForward.Visibility = (page == listNews.Length / electionsOnOnePage + 1) ? Visibility.Hidden : Visibility.Visible;
            PageBack.Visibility = (page == 1) ? Visibility.Hidden : Visibility.Visible;
            
        }
        /// <summary>
        /// Создание шаблона для отображения 1 голосования
        /// </summary>
        /// <param name="election">голосование</param>
        /// <returns>заполненый Border</returns>
        private Border CreateElectionConteiner(Election election)
        {
            Grid grid = new Grid();
            grid.MouseLeftButtonDown += Grid_MouseLeftButtonDown;
            grid.Children.Add(new Label()
            {
                Content = election.Id,
                Visibility = Visibility.Collapsed,
            });
            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
            };
            Label name = new Label()
            {
                Content = election.Name,
                FontSize = 30,
            };
            if (!string.IsNullOrEmpty(election.Description))
            {
                name.ToolTip = new TextBlock()
                {
                    Text = election.Description,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 14,
                };
            }
            Label date = new Label()
            {
                Content = election.DateStart.ToString("d") + " - " + election.DateEnd.ToString("d"),
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            Label content = new Label()
            {
                FontSize = 14,
            };
            if (election.DateStart < DateTime.Now)
            {
                if (election.DateEnd < DateTime.Now)
                {
                    name.Foreground = Brushes.Red;
                    content.Content = "Голосование завершено!";
                }
                else
                {
                    name.Foreground = Brushes.Green;
                    content.Content = "Голосование началось. До конца голосования ";
                    var time = election.DateEnd - DateTime.Now;
                    content.Content += time.Days.ToString() + "д ";
                    content.Content += time.Hours.ToString() + "ч ";
                    content.Content += time.Minutes.ToString() + "м";
                }
            }
            else
            {
                name.Foreground = Brushes.Blue;
                content.Content = "Голосование начнется через ";
                var time = election.DateStart - DateTime.Now;
                content.Content += time.Days.ToString() + "д ";
                content.Content += time.Hours.ToString() + "ч ";
                content.Content += time.Minutes.ToString() + "м";
            }

            StackPanel header = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Children = { name, date },
            };
            grid.Children.Add(stackPanel);
            stackPanel.Children.Add(header);
            stackPanel.Children.Add(content);
            Border border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Margin = new Thickness(10),
                Child = grid
            };
            return border;
        }
        /// <summary>
        /// нажатие на голосование
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = ((Grid)sender).Children[0];
            if (item.GetType().Name == nameof(Label))
            {
                election = Controller.GetElectionById(Int32.Parse(((Label)item).Content.ToString()));
                if (election != null)
                {
                    ChangeGridVisibility(ElectionGrid, nowMenuGrid);
                }
            }            
        }
        /// <summary>
        /// изменение видимости страницы голосования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElectionGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           

            if (ElectionGrid.Visibility == Visibility.Visible)
            {
                nowMenuGrid = ElectionGrid;

                if (election != null)
                {
                    ElectionNameLabel.Content = election.Name;
                    ElectionDateLabel.Content = election.DateStart.ToString("d") + " - " + election.DateEnd.ToString("d");
                    ElectionDescriptionTextBlock.Text = election.Description;
                    OptionsStackPanel.Children.Clear();

                    options = Controller.GetOptions(election);
                    if (options == null)
                    {
                        MessageBox.Show("Голосование не валидно.");
                        return;
                    }
                    int countVoit = 0;
                    foreach (var o in options) countVoit += o.Voites;
                    int? option_id = null;
                    if (user!=null) option_id = Controller.IfUserVoted(user, election);

                    foreach (var option in options)
                    {
                        RadioButton radioButton = new RadioButton() 
                        { 
                            VerticalContentAlignment = VerticalAlignment.Center,
                            FontSize = 20,
                        };
                        radioButton.Content = new StackPanel()
                        {
                            Orientation = Orientation.Horizontal,
                            Children =
                            {
                                new TextBlock(){Text = option.Name},
                                new TextBlock(){Text = " - " + option.Voites},
                                new TextBlock(){Text = " (%)"},
                            }
                        };
                        var countTextBlock = (TextBlock)((StackPanel)radioButton.Content).Children[2];
                        if (countVoit != 0) countTextBlock.Text = countTextBlock.Text.Insert(2, ((Double)option.Voites / (Double)countVoit * 100).ToString("00.##"));
                        else countTextBlock.Text = countTextBlock.Text.Insert(2,"0");

                        if (user!= null && option_id != null && option.Id == option_id)
                        {
                            ((TextBlock)((StackPanel)radioButton.Content).Children[0]).Foreground = Brushes.Green;
                        }
                        OptionsStackPanel.Children.Add(radioButton);
                    }
                    if(election.DateStart > DateTime.UtcNow)
                    {
                        VoteImpossibleLabel.Visibility = Visibility.Visible;
                        VoteButton.Visibility = Visibility.Collapsed;
                        
                        string str = "Голосование начнется через ";
                        var time = election.DateEnd - DateTime.Now;
                        str += time.Days.ToString() + "д ";
                        str += time.Hours.ToString() + "ч ";
                        str += time.Minutes.ToString() + "м";
                        VoteImpossibleLabel.Content = str;
                        VoteImpossibleLabel.Foreground = Brushes.Blue;
                    }
                    else if (election.DateEnd < DateTime.UtcNow)
                    {
                        VoteImpossibleLabel.Visibility = Visibility.Visible;
                        VoteButton.Visibility = Visibility.Collapsed;

                        VoteImpossibleLabel.Content = "Голосование завершено";
                        VoteImpossibleLabel.Foreground = Brushes.Red;
                    }else if (user == null)
                    {
                        VoteImpossibleLabel.Visibility = Visibility.Visible;
                        VoteButton.Visibility = Visibility.Collapsed;

                        VoteImpossibleLabel.Content = "Чтобы проголосовать, необходимо авторизоваться";
                        VoteImpossibleLabel.Foreground = Brushes.Red;
                    }
                    else
                    {
                        if (option_id != null)
                        {
                            VoteImpossibleLabel.Content = "Вы уже проголосовали";
                            VoteImpossibleLabel.Foreground = Brushes.Green;

                            VoteImpossibleLabel.Visibility = Visibility.Visible;
                            VoteButton.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            VoteImpossibleLabel.Visibility = Visibility.Collapsed;
                            VoteButton.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// переход на страницу "Новости"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(NewsGrid, nowMenuGrid);
        }
        /// <summary>
        /// нажата кнопка "Проголосовать"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (user == null)
            {
                MessageBox.Show("Чтобы учавствовать в голосовании, необходимо авторизироваться");
                return;
            }
            if (election == null)
            {
                MessageBox.Show("Ошибка! не выбрано голосование!");
                return;
            }
            string option = null;
            foreach (RadioButton obj in OptionsStackPanel.Children)
            {
                if (obj.IsChecked == true)
                {
                    option = ((TextBlock)((StackPanel)obj.Content).Children[0]).Text;
                    break;
                };
            }
            if (option == null)
            {
                MessageBox.Show("Ошибка! Не удалось найти выбранную опцию");
                return;
            }
            Option opt = options.Find((o) => o.Name == option);
            if (opt == null)
            {
                MessageBox.Show("Ошибка! Не удалось найти выбранную опцию");
                return;
            }
            if (Controller.Vote(user, election, opt))
            {
                MessageBox.Show("Вы проголосовали");
                ElectionGrid_IsVisibleChanged(null, new DependencyPropertyChangedEventArgs());
            }
            else
            {
                MessageBox.Show("Ошибка голосования!");
            }
        }
        /// <summary>
        /// изменение видимости страницы авторизации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AuthLoginTextBox.Text = "";
            AuthPasswordBox.Password = "";
        }
        /// <summary>
        /// Переход на страницу "Мой профиль"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuMyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(MyProfileGrid, nowMenuGrid);
        }
        /// <summary>
        /// изменение видимости страницы "Мой профиль"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyProfileGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (MyProfileGrid.Visibility == Visibility)
            {
                nowMenuGrid = MyProfileGrid;

                NameLabel.Content = user.First_name + " " + user.Name;
                if (user.Last_name != null) NameLabel.Content += " " + user.Last_name;
                string passport = user.Passport;
                PassportLabel.Content = passport.Insert(4, " ");
                BirthdayLabel.Content = user.Birthday.ToString("D");
                EmailLabel.Content = user.Email;
                PhoneLabel.Content = user.Phone ?? "Не указан";
            }
        }
        /// <summary>
        /// Переход на страницу "Мои голосования"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuNavigationPanelMyElections_Click(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(MyElectionsGrid, nowMenuGrid);
        }
        /// <summary>
        /// изменение видимости страницы "Мои голосования"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyElectionsGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(MyElectionsGrid.Visibility == Visibility.Visible)
            {
                nowMenuGrid = MyElectionsGrid;

                MyElectionsStackPanel.Children.Clear();
                var elections = Controller.GetElections(user.Id);
                if (elections!=null && elections.Count != 0)
                {
                    foreach(var el in elections)
                    {
                        MyElectionsStackPanel.Children.Add(CreateElectionConteiner(el));
                    }
                    
                }
            }
        }

        private void PageBack_Click(object sender, RoutedEventArgs e)
        {
            LoadPageNews(--page);
        }

        private void PageForward_Click(object sender, RoutedEventArgs e)
        {
            LoadPageNews(++page);
        }
    }
}

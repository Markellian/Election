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
            UserGrid.Visibility = Visibility.Visible;

        }
        private void UserRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: переход на регистрацию
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Chain chain = new Chain(new Election("Любимый цвет", DateTime.Parse("01/01/2020 00:00:00:000"), DateTime.Parse("01/03/2020 00:00:00:000")));

        }
    }
}

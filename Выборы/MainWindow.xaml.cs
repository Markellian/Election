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
        public MainWindow()
        {
            InitializeComponent();
            using (var db = new ElectionsDataBase())
            {
                Blocks blocks = new Blocks();
                //db.SaveChanges();
            }
            
        }

        private void UserLoginButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Авторизация
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

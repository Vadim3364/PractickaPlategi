using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UchetPlatejei
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        string login;
        string password;

        string list_letters = "qw1er2ty3uu4io5pl6kj7hg8fd9sa0zxcvbnm";
        string capcha_code;

        int capcha_lenght = 10;
        int line_count = 10;

        int errors = 0;

        public MainWindow()
        {
            InitializeComponent();
            password = login = "";
        }

        private void enter_btn_Click(object sender, RoutedEventArgs e)
        {
            password = tbPass.Text.ToString();
            login = tbName.Text.ToString();

            if (!String.IsNullOrEmpty(login) && !String.IsNullOrEmpty(password))
            {
                password = hash(password);

                var users = Instances.db.users.Where(u => u.login == login && u.password == password).ToList();
                if (capcha.Visibility != Visibility.Visible)
                {
                    if (users.Count() > 0)
                    {
                        Main main = new Main(users[0]);
                        main.Show();
                        Session.dateAuth = DateTime.Now;                     
                        Close();
                    }
                    else
                        error_login();
                }
                else if (capcha.Visibility == Visibility.Visible)
                {
                    if (!String.IsNullOrEmpty(capcha_textbox.Text))
                    {
                        if (users.Count() > 0 && capcha_textbox.Text == capcha_code)
                        {
                            Main main = new Main(users[0]);
                            main.Show();
                            Session.dateAuth = DateTime.Now;
                            Close();
                        }
                        else
                            error_login();
                    }
                    else
                    {
                        CreateCapcha();
                        MessageBox.Show("НЕВЕРНАЯ капча");
                    }
                }
            }
        }

        private void exit_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private String hash(string pass)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(pass);
                byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);

                string hashPass = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                return hashPass;
            }
        }



        private void error_login()
        {
            CreateCapcha();
            tries_error.Visibility = Visibility.Visible;

            errors += 1;
            tries_error.Text = "Осталось попыток: " + (3 - errors).ToString();

            if (errors >= 3)
            {
                enter_btn.IsEnabled = false;
                enter_btn.Background = Brushes.LightGray;

                Timer timer = new Timer();
                timer.Interval = 30000;
                timer.Elapsed += (Object source, ElapsedEventArgs e) =>
                {
                    

                    errors = 0;
                    
                    Dispatcher.Invoke((Action)(() =>
                    {
                        tries_error.Text = "Осталось попыток: " + (3 - errors).ToString();
                        enter_btn.Background = Brushes.White;
                        enter_btn.IsEnabled = true;
                    }));
                    timer.Stop();
                };

                timer.Start();
            }

            MessageBox.Show("НЕВЕРНЫЙ логин или пароль");
        }

        private void CreateCapcha()
        {
            capcha.Visibility = Visibility.Visible;
            capcha_textbox.Visibility = Visibility.Visible;

            capcha_textbox.Text = "";
            capcha.Children.Clear();

            CapchaRandomText();
            CapchaAddLines();
        }
        private void CapchaRandomText()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "";
            textBlock.FontSize = 12;

            char[] letter = list_letters.ToCharArray();

            Random r = new Random();

            for (int i = 0; i < capcha_lenght; i++)
                textBlock.Text += letter[r.Next(0, letter.Length)] + " ";

            capcha_code = textBlock.Text;

            capcha.Children.Add(textBlock);
        }
        private void CapchaAddLines()
        {
            Random r = new Random();

            for (int i = 0; i < line_count; i++)
            {
                double width = capcha.Width;
                double height = capcha.Height;

                Line ln = new Line();

                ln.X1 = r.Next(0, int.Parse(width.ToString()));
                ln.X2 = r.Next(0, int.Parse(width.ToString()));

                ln.Y1 = 0;
                ln.Y2 = height;

                ln.Stroke = System.Windows.Media.Brushes.Aquamarine;
                ln.StrokeThickness = 1;
                ln.HorizontalAlignment = HorizontalAlignment.Left;
                ln.VerticalAlignment = VerticalAlignment.Center;

                capcha.Children.Add(ln);
            }

        }
    }
}

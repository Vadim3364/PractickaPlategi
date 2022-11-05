using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Syncfusion.UI.Xaml.Grid.Converter;

namespace UchetPlatejei
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private user user;
        public List<products_users> products;
        products_users selectedProduct;
        public int todayId = 0;
      
        public Main(user user)
        {
            InitializeComponent();
            ordersDataGrid.SearchHelper.AllowFiltering = true;
            this.user = user;

            Session.userId = this.user.id;

            products = Instances.db.products_users.Where(u => u.user_id == this.user.id).ToList();


            categor_combo.ItemsSource = Instances.db.categories.ToList();

            dataPager.Source = products;
            //ordersDataGrid.ItemsSource = products;

            to_date.DisplayDateEnd = from_date.DisplayDateEnd = DateTime.Today;


            //getTodayAnalyz();
        }


        private void ordersDataGrid_SelectionChanging(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangingEventArgs e)
        {
            
        }

        private void select_btn_Click(object sender, RoutedEventArgs e)
        {
            if (ordersDataGrid.SelectedItem != null)
            {
                selectedProduct = ordersDataGrid.SelectedItem as products_users;
                AddWindow addWindow = new AddWindow(user, selectedProduct);
                if (addWindow.ShowDialog() == true)
                {
                    Instances.db.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                    products = Instances.db.products_users.Where(u => u.user_id == user.id).ToList();
                    ordersDataGrid.ItemsSource = products;

                    getTodayAnalyz();
                    
     
                };
            }                      
        }

        private void clear_btn_Click(object sender, RoutedEventArgs e)
        {
            var list = Instances.db.products_users.Where(u => u.user_id == user.id).ToList();
            ordersDataGrid.ItemsSource = list;


            to_date.SelectedDate = from_date.SelectedDate = null;
            categor_combo.SelectedItem = null;
        }

        private void otchet_btn_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExportMode = ExportMode.Text;
            var excelEngine = ordersDataGrid.ExportToExcel(ordersDataGrid.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];
            workBook.SaveAs("Sample.xlsx");
            var document = ordersDataGrid.ExportToPdf();
            document.Save("Sample1.pdf");
            MessageBox.Show("Отчёт сохранён");
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow(user);
            if (addWindow.ShowDialog() == true)
            {
                Instances.db.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                products = Instances.db.products_users.Where(u => u.user_id == user.id).ToList();
                ordersDataGrid.ItemsSource = products;

                getTodayAnalyz();
              
            };


        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<products_users> selectedProduct = ordersDataGrid.SelectedItems.Cast<products_users>().ToList();
            if (selectedProduct.Count > 0)
            {
                AttentionWindow attentionWindow = new AttentionWindow(selectedProduct);
                if (attentionWindow.ShowDialog() == true)
                {
                    Instances.db.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                    products = Instances.db.products_users.Where(u => u.user_id == user.id).ToList();
                    ordersDataGrid.ItemsSource = products;

                    getTodayAnalyz();
                }
            }

        }

        private void categor_combo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            SortData();
        }


        private void from_date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            to_date.DisplayDateStart = from_date.SelectedDate;
            SortData();
        }

        private void to_date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            from_date.DisplayDateEnd = to_date.SelectedDate;
            SortData();
        }

        private void SortData()
        {

            List<products_users> list;


            // category
            if (categor_combo.SelectedItem != null && to_date.SelectedDate == null && from_date.SelectedDate == null)
                list = Instances.db.products_users.Where(u => u.user_id == user.id && categor_combo.SelectedValue.ToString() == u.product.category.category_name).ToList();

            // to_date
            else if (categor_combo.SelectedItem == null && to_date.SelectedDate != null && from_date.SelectedDate == null)
                list = Instances.db.products_users.Where(u => u.user_id == user.id && u.order_date <= to_date.SelectedDate).ToList();

            // from_date
            else if (categor_combo.SelectedItem == null && to_date.SelectedDate == null && from_date.SelectedDate != null)
                list = Instances.db.products_users.Where(u => u.user_id == user.id && u.order_date >= from_date.SelectedDate).ToList();

            // to_date from_date
            else if (categor_combo.SelectedItem == null && to_date.SelectedDate != null && from_date.SelectedDate != null)
                list = Instances.db.products_users.Where(u => u.user_id == user.id && u.order_date >= from_date.SelectedDate && u.order_date <= to_date.SelectedDate).ToList();

            // category to_date
            else if (categor_combo.SelectedItem != null && to_date.SelectedDate != null && from_date.SelectedDate == null)
                list = Instances.db.products_users.Where(u => u.user_id == user.id && categor_combo.SelectedValue.ToString() == u.product.category.category_name
                        && u.order_date <= to_date.SelectedDate).ToList();

            // category from_date
            else if (categor_combo.SelectedItem != null && to_date.SelectedDate == null && from_date.SelectedDate != null)
                list = Instances.db.products_users.Where(u => u.user_id == user.id && categor_combo.SelectedValue.ToString() == u.product.category.category_name
                        && u.order_date >= from_date.SelectedDate).ToList();

            // all
            else if (categor_combo.SelectedItem != null && to_date.SelectedDate != null && from_date.SelectedDate != null)
                list = Instances.db.products_users.Where(u => u.user_id == user.id && categor_combo.SelectedValue.ToString() == u.product.category.category_name
                            && u.order_date >= from_date.SelectedDate && u.order_date <= to_date.SelectedDate).ToList();

            // nobody
            else
                list = Instances.db.products_users.Where(u => u.user_id == user.id).ToList();

            ordersDataGrid.ItemsSource = list;


        }

        private void ordersDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var list = Instances.db.products_users.Select(u => u.id);
        }

        private void ordersDataGrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            //AddWindow addWindow = new AddWindow(user, (products_users)sender);
            //if (addWindow.ShowDialog() == true)
            //{
            //    Instances.db.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
            //    products = Instances.db.products_users.Where(u => u.user_id == user.id).ToList();
            //    ordersDataGrid.ItemsSource = products;
            //};
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {         
            ordersDataGrid.SearchHelper.Search(tbSearch.Text);

        }

        private void Window_Closed(object sender, EventArgs e)
        {         
            string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pathToSave = mydocu + $@"\{user.login}.csv";
            var csv = new StringBuilder();
            csv.AppendLine($"Время авторизации: {Session.dateAuth}");
            csv.AppendLine($"Время выхода: {Session.dateExit=DateTime.Now}");
            csv.AppendLine($"Добавлено: {Session.countAdd}");
            csv.AppendLine($"Изменено: {Session.countUpdate}");
            csv.AppendLine($"Удалено: {Session.countDelete}");
            csv.AppendLine($"Кол-во затронутых записей: {Session.count}");
            File.WriteAllText(pathToSave, csv.ToString());
            MessageBox.Show("Логи сохранены");
        }

        private void analiz_btn_Click(object sender, RoutedEventArgs e)
        {
            OtchetWindow otchetWindow = new OtchetWindow(user);
            otchetWindow.ShowDialog();
        }

        private void getTodayAnalyz()
        {
            var obj = Instances.db.analizs.Select(u => u.user_id == user.id).FirstOrDefault();
            if (obj)
            {
                var today = Instances.db.analizs.Where(u => u.user_id == user.id).ToList();
                if(today.Count != 0)
                    todayId = today[0].id;
               
            }

            if (todayId == 0)
            {
                analiz analiz = new analiz();
                analiz.delete = Session.countDelete;
                analiz.update = Session.countUpdate;
                analiz.create = Session.countAdd;
                analiz.date = DateTime.Now;
                analiz.user_id = user.id;
                Instances.db.analizs.Add(analiz);
            }
            else
            {
                var analiz = Instances.db.analizs.Where(u => u.id == todayId).FirstOrDefault() as analiz;

                analiz.delete += Session.countDelete;
                analiz.update += Session.countUpdate;
                analiz.create += Session.countAdd;

            }

            Instances.db.SaveChanges();
        }
    }

}


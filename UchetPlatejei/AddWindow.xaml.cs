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
using System.Windows.Shapes;

namespace UchetPlatejei
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        List <product> products;
        List<category> categories;
        products_users prod;

        Boolean create;

        user user;
        public AddWindow(user user)
        {
            InitializeComponent();
            products = Instances.db.products.ToList();
            categories = Instances.db.categories.ToList();
            categ.ItemsSource = categories;
            textbox_name.ItemsSource = products;
            this.user = user;

            create = true;
        }

        public AddWindow(user user, products_users prod)
        {
            InitializeComponent();
            products = Instances.db.products.ToList();
            categories = Instances.db.categories.ToList();
            categ.ItemsSource = categories;
            textbox_name.ItemsSource = products;
            this.user = user;

            create = false;

            this.prod = prod;

            categ.SelectedValue = prod.product.category.category_name;
            textbox_name.SelectedValue = prod.product.product_name;
            text_kol.Value = prod.count;
            text_cost.Text = prod.price.ToString();
            btnAdd.Content = "Сохранить";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string choiceCategory = categ.SelectedValue.ToString();
            string choiceProduct = textbox_name.SelectedValue.ToString();

            int count = text_kol.Value.Value;

            string price = text_cost.Text.ToString();

            if ( !choiceCategory.Equals("") && !choiceProduct.Equals("") && !count.Equals(0) && !price.Equals(""))
            {
                int max = Instances.db.products_users.Max(u => u.id);
                max += 1;

                if (create)
                {
                    products_users item = new products_users();
                    item.user_id = user.id;
                    item.product_id = products.Where(y => y.product_name == choiceProduct).Select(u => u.id).Max();
                    item.price = decimal.Parse(price);
                    item.count = count;
                    item.sum = count * decimal.Parse(price);
                    item.order_date = DateTime.Now;
                    item.unique_name = $"{(choiceCategory.ToCharArray())[0].ToString()}-{max}-{DateTime.Now.ToString("dd.MM.yyyy")}";

                    Instances.db.products_users.Add(item);
                    Session.countAdd++;
                    Session.count++;
                }
                else
                {
                    var obj = Instances.db.products_users.Where(u => u.id == prod.id).FirstOrDefault();
                    obj.price = decimal.Parse(price);
                    obj.sum = count * decimal.Parse(price);
                    obj.product_id = products.Where(y => y.product_name == choiceProduct).Select(u => u.id).Max();
                    Session.countUpdate++;
                    Session.count++;
                }
                
               try
                {
                    Instances.db.SaveChanges();
                    MessageBox.Show("Информация сохранена");
                    this.DialogResult = true;
                } catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            } else
            {
                MessageBox.Show("Заполнены не все поля!");
            }
        }

        private void SearchErrors()
        {
            //if()
        }

        private void categ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = categories.Where(c => c.category_name == categ.SelectedValue.ToString()).ToList()[0].id;
            var listProducts = products.Where(p => p.category_id == selectedCategory);
            textbox_name.ItemsSource = listProducts;
        }
    }
}

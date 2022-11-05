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
    /// Логика взаимодействия для AttentionWindow.xaml
    /// </summary>
    public partial class AttentionWindow : Window
    {
        List<products_users> product;
        public AttentionWindow(List<products_users> product)
        {
            InitializeComponent();
            this.product = product;
            if (this.product.Count == 1)
            {
                tbInfo.Text = $"Удалить запись {this.product[0].unique_name} '{this.product[0].product.product_name}' ?";
            } else
            {
                decimal sum = 0;
                foreach (products_users p in this.product)
                {
                    sum += p.sum;
                }
                tbInfo.Text = $"Удалить {this.product.Count} записей на сумму {sum} руб. ?";
            }
            
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (product.Count == 1)
            {
                deleteOnce(product[0]);
            } else
            {
                deleteAll(product);
            }
            MessageBox.Show("Удалено успешно");
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void deleteOnce(products_users products_)
        {
            Instances.db.products_users.Remove(products_);
            Instances.db.SaveChanges();
            Session.countDelete++;
            Session.count++;
        }

        private void deleteAll(List<products_users> products_)
        {
            Instances.db.products_users.RemoveRange(products_);
            Instances.db.SaveChanges();
            Session.countDelete += products_.Count;
        }
    }
}

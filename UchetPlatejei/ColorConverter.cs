using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UchetPlatejei
{
    public class ColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var products = Instances.db.products_users.Where(p => p.user_id == Session.userId);


            var sortList = products.OrderByDescending(p => p.sum).ToList();

            var data = value as products_users;

            //custom condition is checked based on data.

            var result = DependencyProperty.UnsetValue;
            for (int i = 0; i < sortList.Count; i++)
            {
                if (data.id == sortList[i].id && i <= 9)
                    result = new SolidColorBrush(Colors.Red);
                else if (data.id == sortList[i].id && (i >= (sortList.Count - 10) && (i < sortList.Count)))
                    result = new SolidColorBrush(Colors.LightGreen);
            }

            //for (int i = sortList.Count-1; i >= 0; i--)
            //{
            //    if (data.id == sortList[i].id && i >= (sortList.Count-11))
            //        result = new SolidColorBrush(Colors.LightGreen);
            //}


            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

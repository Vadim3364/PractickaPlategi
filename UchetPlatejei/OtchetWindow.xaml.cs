using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UchetPlatejei
{
    /// <summary>
    /// Логика взаимодействия для OtchetWindow.xaml
    /// </summary>
    public partial class OtchetWindow : Window
    {
        user user;
        public OtchetWindow(user user)
        {
            InitializeComponent();
            this.user = user;
            // Вывод данных из таблицы "Анализ" в DataGrid
            dataGrid.ItemsSource = Instances.db.analizs.Where(u => u.user_id == user.id).ToList();
            // Настройка элементов WindowsFormsHost
            chartAnalysAdded.ChartAreas.Add(new ChartArea("Main"));
            chartAnalysDeleted.ChartAreas.Add(new ChartArea("Main"));
            chartAnalysUpdate.ChartAreas.Add(new ChartArea("Main"));
            var currentSeriesAdded = new Series("analys")
            {
                IsValueShownAsLabel = true
            };
            var currentSeriesDeleted = new Series("analys")
            {
                IsValueShownAsLabel = true
            };
            var currentSeriesUpdate = new Series("analys")
            {
                IsValueShownAsLabel = true
            };
            chartAnalysAdded.Series.Add(currentSeriesAdded);
            chartAnalysDeleted.Series.Add(currentSeriesDeleted);
            chartAnalysUpdate.Series.Add(currentSeriesUpdate);
            // Получение данных из таблицы "Анализ" и создание на их основе графиков
            var res = Instances.db.analizs.ToList().Where(p => p.user_id == user.id).ToList();
            for (int i = 0; i < res.Count; i++)
            {
                currentSeriesAdded.Points.AddXY(res[i].date, res[i].create);
                currentSeriesUpdate.Points.AddXY(res[i].date, res[i].update);
                currentSeriesDeleted.Points.AddXY(res[i].date, res[i].delete);
            }
        }
    }
}

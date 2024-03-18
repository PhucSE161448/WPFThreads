using InvokeTimerToControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using System.Windows.Threading;

namespace Assignment03_WPFThreads
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void LoadData()
        {
            StreamReader file = new StreamReader("test.txt");
            string[] columnnames = file.ReadLine().Split(',');
            DataTable dt = new DataTable();
            foreach (string c in columnnames)
            {
                dt.Columns.Add(c);
            }
            string newline;
            while ((newline = file.ReadLine()) != null)
            {
                DataRow dr = dt.NewRow();
                string[] values = newline.Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    dr[i] = values[i];
                }
                dt.Rows.Add(dr);
            }
            file.Close();
            dataGridView1.ItemsSource = dt.DefaultView;

            dataGridView1.Columns[0].Width = new DataGridLength(50);
            dataGridView1.Columns[1].Width = new DataGridLength(70);
            dataGridView1.Columns[2].Width = new DataGridLength(440);

            dataGridView1.CanUserAddRows = false;
        }

       


        private async Task GetAction()
        {
            int action1 = Convert.ToInt32(textBox1.Text);
            int action2 = Convert.ToInt32(textBox2.Text);
            int action3 = Convert.ToInt32(textBox3.Text);

            await Task.Run(async () =>
            {
                int rowi = 0;
                while (rowi < dataGridView1.Items.Count)
                {
                    List<Task> tasks = new List<Task>();
                    for (int i = 0; i < 2; i++)
                    {
                        tasks.Add(DoAction(rowi, action1, action2, action3,dataGridView1));
                        await Task.Delay(300); 
                        rowi++;
                    }
                    await Task.WhenAll(tasks.ToArray());
                }
                MessageBox.Show("Tasks completed!");
            });
        }
        private async Task DoAction(int rowi, int action1, int action2, int action3, DataGrid dgv1)
        {
            await Task.Run(async () =>
            {
                // Select row in the UI thread
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (rowi >= 0 && rowi < dgv1.Items.Count)
                    {
                        dgv1.SelectedItem = dgv1.Items[rowi];
                        dgv1.UpdateLayout();
                    }
                });

                SetTimerTicker.SetTimeCountDownDataGrid(action1, dgv1, rowi, 2, "Thực hiện thao tác 1 trong: ");
                await Task.Delay(action1 * 1000);

                SetTimerTicker.SetTimeCountDownDataGrid(action2, dgv1, rowi, 2, "Thực hiện thao tác 2 trong: ");
                await Task.Delay(action2 * 1000);

                SetTimerTicker.SetTimeCountDownDataGrid(action3, dgv1, rowi, 2, "Thực hiện thao tác 3 trong: ");
                await Task.Delay(action3 * 1000);

                // Deselect row in the UI thread
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    dgv1.SelectedItem = null;
                    dgv1.UpdateLayout();
                });

                await Task.Delay(2000);
            });
        }

        private void dataGridView1_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private async void button1_Click_1(object sender, RoutedEventArgs e)
        {
            await GetAction();
        }
    }
}


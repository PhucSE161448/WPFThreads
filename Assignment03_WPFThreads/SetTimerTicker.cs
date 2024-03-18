using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace InvokeTimerToControls
{
    class SetTimerTicker
    {
        public static void SetTimeCountDownDataGrid(int timers, DataGrid dataGrid, int rowIndex, int cellIndex, string message)
        {
            if (dataGrid.Dispatcher.CheckAccess())
            {
                
                SetTimeCountDownDataGridInternal(timers, dataGrid, rowIndex, cellIndex, message);
            }
            else
            {
                
                dataGrid.Dispatcher.Invoke(new Action<int, DataGrid, int, int, string>(SetTimeCountDownDataGrid), timers, dataGrid, rowIndex, cellIndex, message);
            }
        }

        private static void SetTimeCountDownDataGridInternal(int timers, DataGrid dataGrid, int rowIndex, int cellIndex, string message)
        {
            
            string copyrightUnicode = "2606";
            int value = int.Parse(copyrightUnicode, System.Globalization.NumberStyles.HexNumber);
            string symbol = char.ConvertFromUtf32(value).ToString();

            
            int minuteD = (int)(Math.Floor((double)(timers / 60)));
            int secondD = timers % 60;

            
            CountDownTimer timer1d = new CountDownTimer();
            timer1d.SetTime(minuteD, secondD);
            timer1d.Start();

            
            if (timers > 60)
            {
                timer1d.TimeChanged += () =>
                {
                    UpdateDataGridCell(dataGrid, rowIndex, cellIndex, $"{symbol} {message} {timer1d.TimeLeftStr} phút (giây)");
                };
            }
            else
            {
                timer1d.TimeChanged += () =>
                {
                    UpdateDataGridCell(dataGrid, rowIndex, cellIndex, $"{symbol} {message} {timer1d.TimeLeftStr} giây");
                };
            }
            timer1d.CountDownFinished += () =>
            {
                UpdateDataGridCell(dataGrid, rowIndex, cellIndex, $"{symbol} Kết thúc thời gian.");
            };
            timer1d.StepMs = 1000;
        }

        private static void UpdateDataGridCell(DataGrid dataGrid, int rowIndex, int cellIndex, string value)
        {
            
            if (rowIndex >= 0 && rowIndex < dataGrid.Items.Count && cellIndex >= 0 && cellIndex < dataGrid.Columns.Count)
            {
                var row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);

                if (row != null)
                {
                   
                    var presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    if (presenter != null)
                    {
                        var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(cellIndex);
                        if (cell != null)
                        {
                            cell.Content = value;
                        }
                    }
                }
            }
        }

        private static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

    }
}

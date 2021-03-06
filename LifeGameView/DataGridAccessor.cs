﻿
namespace LifeGameView
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    internal class DataGridAccessor
    {
        private DataGridAccessor() { }
        internal static DataGridAccessor Instance { get; } = new DataGridAccessor();

        internal void SetCellColor(DataGrid dataGrid, int row, int column, Brush brush)
        {
            DispatchAction(() =>
            {
                var cell = GetCell(dataGrid, row, column);
                if (cell == null)
                {
                    Trace.WriteLine($"[ERR] Failed to get cell : [Row{row},Column{column}]");
                    return;
                }
                cell.Background = brush;
            });
        }

        internal void ToggleCellColor(DataGridCell cell)
        {
            if (cell == null)
            {
                Trace.WriteLine("[ERR] The specified cell is null");
                return;
            }

            DispatchAction(() =>
            {
                SolidColorBrush brush = cell.Background as SolidColorBrush;
                if (brush == Brushes.Black)
                    cell.Background = Brushes.White;
                else
                    cell.Background = Brushes.Black;
            });
        }

        private DataGridCell GetCell(DataGrid dataGrid, int i, int j)
        {
            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
            if (row == null)
            {
                Trace.WriteLine("[ERR] Failed to cast to DataGridRow from ItemContainer");
                return null;
            }
            DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
            if (presenter == null)
            {
                Trace.WriteLine("[ERR] Failed to get DataGridCellsPresenter from row");
                return null;
            }

            return presenter.ItemContainerGenerator.ContainerFromIndex(j) as DataGridCell;
        }

        private T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default;
            int visCt = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < visCt; i++)
            {
                Visual v = VisualTreeHelper.GetChild(parent, i) as Visual;
                child = v as T;
                if (child == null) child = GetVisualChild<T>(v);

                if (child != null) break;
            }
            return child;
        }

        private void DispatchAction(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(action);
            }
        }
    }
}

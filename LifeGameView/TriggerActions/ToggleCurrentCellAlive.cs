
namespace LifeGameView.TriggerActions
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using LifeGameViewModel.InteractionRequests;

    internal class ToggleCurrentCellAlive : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            if (parameter is DependencyPropertyChangedEventArgs e
             && e.NewValue is ToggleCurrentCellAliveRequest req)
            {
                var view = Window.GetWindow(AssociatedObject) as MainView;
                if (view == null)
                {
                    Trace.WriteLine("[ERR] Failed to cast associated window as MainView");
                    return;
                }

                var curCellInfo = view.dgSample.CurrentCell;
                var curCell = curCellInfo.Column.GetCellContent(curCellInfo.Item).Parent as DataGridCell;
                if (curCell == null)
                {
                    Trace.WriteLine("[ERR] Failed to get current selected cell.");
                    return;
                }

                req.Row = view.dgSample.Items.IndexOf(curCellInfo.Item);
                req.Column = curCellInfo.Column.DisplayIndex;
                DataGridAccessor.Instance.ToggleCellColor(curCell);
            }
        }
    }
}

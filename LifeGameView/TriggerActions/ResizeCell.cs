using LifeGameViewModel.InteractionRequests;
using System.Diagnostics;

namespace LifeGameView.TriggerActions
{
    using System.Windows;
    using System.Windows.Interactivity;

    internal class ResizeCell : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            if (parameter is DependencyPropertyChangedEventArgs e
             && e.NewValue is ResizeCellRequest req)
            {
                if (req.Rank <= 0)
                {
                    Trace.WriteLine("[ERR] LifeGameTable size is zero.");
                    return;
                }

                var view = Window.GetWindow(AssociatedObject) as MainView;
                if (view == null)
                {
                    Trace.WriteLine("[ERR] Failed to cast associated window as MainView");
                    return;
                }

                double parentHeghit = view.parentGridSample.ActualHeight;
                double parentWidth = view.parentGridSample.ActualWidth;
                double buttonHeight = view.gridSample.ActualHeight;

                view.dgSample.MinRowHeight = 0;
                view.dgSample.RowHeight = (parentHeghit - buttonHeight) / req.Rank;
                foreach (var col in view.dgSample.Columns)
                {
                    col.MinWidth = 0;
                    col.Width = (parentWidth - 2) / req.Rank;
                }
            }
        }
    }
}

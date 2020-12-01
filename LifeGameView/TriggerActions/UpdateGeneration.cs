
namespace LifeGameView.TriggerActions
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Interactivity;
    using LifeGameViewModel.InteractionRequests;

    internal class UpdateGeneration : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            if (parameter is DependencyPropertyChangedEventArgs e
             && e.NewValue is UpdateGenerationRequest req)
            {
                var view = Window.GetWindow(AssociatedObject) as MainView;
                var dataGrid = view?.dgSample;
                if (dataGrid == null)
                {
                    Trace.WriteLine("[ERR] Failed to get DataGrid control from MainView.");
                    return;
                }

                for (int i = 0; i < req.Rank; i++)
                {
                    for (int j = 0; j < req.Rank; j++)
                    {
                        if (req.GenerationInformation[i][j])
                            DataGridAccessor.Instance.SetCellColor(dataGrid, i, j, Brushes.Black);
                        else
                            DataGridAccessor.Instance.SetCellColor(dataGrid, i, j, Brushes.White);
                    }
                }
            }
        }
    }
}

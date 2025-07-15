using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace RequestManagement.WpfClient.ViewModels.Helpers;

public class QuantityUpdateBehavior : Behavior<DataGrid>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.CellEditEnding += DataGrid_CellEditEnding;
    }

    private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.Column.Header.ToString() == "Кол-во" &&
            e.EditAction == DataGridEditAction.Commit)
        {
            if (AssociatedObject.DataContext is LabelPrintListViewModel vm)
            {
                vm.UpdateLabelList();
            }
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.CellEditEnding -= DataGrid_CellEditEnding;
    }
}
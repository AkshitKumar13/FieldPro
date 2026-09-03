using FieldPro.ViewModels;

namespace FieldPro.Views;

public partial class WorkOrderDetailsPage : ContentPage
{
    public WorkOrderDetailsPage(WorkOrderDetailsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
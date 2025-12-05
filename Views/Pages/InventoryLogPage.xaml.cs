using System.Windows;
using System.Windows.Controls;
using gentech_services.ViewsModels;

namespace gentech_services.Views.Pages
{
    public partial class InventoryLogPage : UserControl
    {
        private InventoryLogViewModel viewModel;

        public InventoryLogPage()
        {
            InitializeComponent();
            viewModel = new InventoryLogViewModel();
            DataContext = viewModel;
        }

        private void ClearDateRange_Click(object sender, RoutedEventArgs e)
        {
            viewModel.StartDate = null;
            viewModel.EndDate = null;
            UpdateClearButtonVisibility();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClearButtonVisibility();
        }

        private void UpdateClearButtonVisibility()
        {
            ClearDateRangeButton.Visibility = (viewModel.StartDate.HasValue || viewModel.EndDate.HasValue)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}

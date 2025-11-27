
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YourApp.Controls
{
    public partial class EditUserControl : UserControl
    {
        private static readonly Regex _nonDigit = new Regex(@"\D", RegexOptions.Compiled);

        public EditUserControl()
        {
            InitializeComponent();
            DataContext = new EditUserViewModel
            {
                FullName = "Juan Pablo",
                Username = "Pablo Jab",
                Roles = new System.Collections.ObjectModel.ObservableCollection<string> { "Staff", "Manager", "Admin" },
                SelectedRole = "Staff",
                Pin = "1234"
            };
        }

        private void Pin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _nonDigit.IsMatch(e.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as EditUserViewModel;
            MessageBox.Show($"Saved:\n{vm.FullName}\n{vm.Username}\nRole: {vm.SelectedRole}\nPIN: {vm.Pin}", "Saved");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cancelled", "Cancel");
        }
    }
}

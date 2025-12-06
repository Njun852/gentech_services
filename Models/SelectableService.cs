using ProductServicesManagementSystem.Models;
using System.ComponentModel;

namespace gentech_services.Models
{
    public class SelectableService : INotifyPropertyChanged
    {
        private Service service;
        private bool isSelected;

        public Service Service
        {
            get { return service; }
            set { service = value; OnPropertyChanged(nameof(Service)); }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        // Proxy properties for binding
        public int ServiceID => service?.ServiceID ?? 0;
        public string Name => service?.Name ?? string.Empty;
        public string Description => service?.Description ?? string.Empty;
        public decimal Price => service?.Price ?? 0;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

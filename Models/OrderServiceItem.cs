using gentech_services.MVVM;
using ProductServicesManagementSystem.Models;
using System;
using System.ComponentModel;

namespace gentech_services.Models
{
    public class OrderServiceItem : INotifyPropertyChanged
    {
        private Service service;
        private User technician;
        private string status;
        private ServiceOrder serviceOrder;

        public Service Service
        {
            get { return service; }
            set { service = value; OnPropertyChanged(nameof(Service)); }
        }

        public User Technician
        {
            get { return technician; }
            set { technician = value; OnPropertyChanged(nameof(Technician)); }
        }

        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        public ServiceOrder ServiceOrder
        {
            get { return serviceOrder; }
            set { serviceOrder = value; OnPropertyChanged(nameof(ServiceOrder)); }
        }

        public OrderServiceItem()
        {
            Status = "Pending";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

namespace PUJASM.POS.ViewModels
{
    using System.Collections.ObjectModel;
    using Models;
    using MVVMFramework;

    public class ItemVM : ViewModelBase<Item>
    {
        private Supplier _selectedSupplier;

        public string ID
        {
            get { return Model.ID; }
            set
            {
                Model.ID = value;
                OnPropertyChanged("ID");
            }
        }

        public string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public ItemCategory ItemCategory
        {
            get { return Model.ItemCategory; }
            set
            {
                Model.ItemCategory = value;
                OnPropertyChanged("ItemCategory");
            }
        }

        public decimal PurchasePrice
        {
            get { return Model.PurchasePrice; }
            set
            {
                Model.PurchasePrice = value;
                OnPropertyChanged("PurchasePrice");
            }
        }

        public decimal SalesPrice
        {
            get { return Model.SalesPrice; }
            set
            {
                Model.SalesPrice = value;
                OnPropertyChanged("SalesPrice");
            }
        }

        public Supplier SelectedSupplier
        {
            get { return _selectedSupplier; }
            set { SetProperty(ref _selectedSupplier, value, () => SelectedSupplier); }
        }

        public bool IsActive
        {
            get { return Model.IsActive; }
            set
            {
                Model.IsActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public ObservableCollection<Supplier> Suppliers => Model.Suppliers;

        public ObservableCollection<Promotion> Promotions => Model.Promotions;
         
        public void UpdatePropertiesToUI()
        {
            OnPropertyChanged("ID");
            OnPropertyChanged("Name");
            OnPropertyChanged("ItemCategory");
            OnPropertyChanged("PurchasePrice");
            OnPropertyChanged("SalesPrice");
            OnPropertyChanged("SelectedSupplier");
            OnPropertyChanged("IsActive");
        }
    }
}

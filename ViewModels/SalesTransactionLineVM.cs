namespace PUJASM.POS.ViewModels
{
    using Models;
    using MVVMFramework;

    public class SalesTransactionLineVM : ViewModelBase<SalesTransactionLine>
    {
        public SalesTransaction SalesTransaction => Model.SalesTransaction;

        public Item Item => Model.Item;

        public decimal SalesPrice => Model.SalesPrice;

        public decimal Discount
        {
            get { return Model.Discount; }
            set
            {
                Model.Discount = value;
                OnPropertyChanged("Discount");
                UpdateTotal();
            }
        }

        public int Quantity
        {
            get { return Model.Quantity; }
            set
            {
                Model.Quantity = value;
                OnPropertyChanged("Quantity");
                UpdateTotal();
            }
        }

        public decimal Total
        {
            get { return Model.Total; }
            set
            {
                Model.Total = value;
                OnPropertyChanged("Total");
            }
        }

        private void UpdateTotal()
        {
            Total = Model.Quantity*(Model.SalesPrice - Model.Discount);
        }
    }
}

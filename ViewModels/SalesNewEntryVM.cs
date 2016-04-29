namespace PUJASM.POS.ViewModels
{
    using System.Linq;
    using System.Windows;
    using Models;
    using MVVMFramework;
    using Utilities;

    public class SalesNewEntryVM : ViewModelBase
    {
        private readonly SalesVM _parentVM;

        #region Backing Fields
        private string _newEntryItemID;
        private ItemVM _newEntryItem;
        private decimal _newEntrySalesPrice;
        private int _newEntryRemainingStock;
        #endregion

        public SalesNewEntryVM(SalesVM parentVM)
        {
            _parentVM = parentVM;
        }

        #region Properties
        public string NewEntryItemID
        {
            get { return _newEntryItemID; }
            set
            {
                SetProperty(ref _newEntryItemID, value, () => NewEntryItemID);
                if (_newEntryItemID == null) return;
                SetNewEntryItem();
            }
        }

        private void SetNewEntryItem()
        {
            using (var context = new ERPContext())
            {
                var itemFromDatabase =
                    context.Inventory
                        .Include("Promotions")
                        .SingleOrDefault(item => item.ID.Equals(_newEntryItemID));

                if (itemFromDatabase == null)
                {
                    MessageBox.Show("Failed to find item.", "Invalid Item ID", MessageBoxButton.OK);
                    return;
                }

                NewEntryItem = new ItemVM { Model = itemFromDatabase };
            }
        }

        public ItemVM NewEntryItem
        {
            get { return _newEntryItem; }
            set
            {
                SetProperty(ref _newEntryItem, value, () => NewEntryItem);
                if (_newEntryItem == null) return;
                NewEntrySalesPrice = _newEntryItem.SalesPrice;
                SubmitNewEntry();
            }
        }

        public decimal NewEntrySalesPrice
        {
            get { return _newEntrySalesPrice; }
            set { SetProperty(ref _newEntrySalesPrice, value, () => NewEntrySalesPrice); }
        }

        public int NewEntryRemainingStock
        {
            get { return _newEntryRemainingStock; }
            set { SetProperty(ref _newEntryRemainingStock, value, () => NewEntryRemainingStock); }
        }
        #endregion

        #region Helper Methods
        private void SubmitNewEntry()
        {
            if (!IsNewEntryItemSelected()) return;
            AddNewEntryToTransaction();
            ResetEntryFields();
            _parentVM.UpdateTransactionGrossTotal();
        }

        private bool IsNewEntryItemSelected()
        {
            if (_newEntryItem != null) return true;
            MessageBox.Show("Please select an item.", "Missing Field(s)", MessageBoxButton.OK);
            return false;
        }

        private void AddNewEntryToTransaction()
        {
            var newSalesTransactionLine = MakeNewEntrySalesTransactionLine();
            foreach (var line in _parentVM.DisplayedLines.Where(
                line => line.Item.ID.Equals(_newEntryItem.ID) && 
                line.SalesPrice.Equals(_newEntrySalesPrice)))
            {
                line.Quantity++;
                ApplyValidPromotions();
                return;
            }
            _parentVM.DisplayedLines.Add(newSalesTransactionLine);
            ApplyValidPromotions();
        }

        private void ApplyValidPromotions()
        {
            if (_newEntryItem.Promotions == null) return;
            var activePromotions =
                _newEntryItem.Promotions.Where(
                    promotion => promotion.EndDate >= UtilityMethods.GetCurrentDate().Date);
            foreach (var promotion in activePromotions)
            {
                if (promotion.PromotionQuantity == 0) continue;
                foreach (var line in _parentVM.DisplayedLines)
                {
                    if (!line.Item.ID.Equals(_newEntryItem.ID)) continue;
                    if (line.SalesPrice == 0) continue;
                    if (line.Quantity >= promotion.PromotionQuantity)
                        line.Discount = promotion.Discount;
                }
            }
        }

        private SalesTransactionLineVM MakeNewEntrySalesTransactionLine()
        {
            var salesTransactionLine = new SalesTransactionLine
            {
                SalesTransaction = _parentVM.Model,
                Item = _newEntryItem.Model,
                WarehouseID = 1,
                Quantity = 1,
                SalesPrice = _newEntrySalesPrice,
                Discount = 0,
                Total = _newEntrySalesPrice * 1
            };
            return new SalesTransactionLineVM {Model = salesTransactionLine};
        }

        private void ResetEntryFields()
        {
            NewEntryItemID = null;
            NewEntryItem = null;
            NewEntrySalesPrice = 0;
            NewEntryRemainingStock = 0;
        }
        #endregion
    }
}

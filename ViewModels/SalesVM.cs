namespace PUJASM.POS.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Drawing.Printing;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using Models.Item;
    using Models.Sales;
    using MVVMFramework;
    using Utilities;
    using Utilities.ModelHelpers;
    using Views;
    using Application = System.Windows.Application;
    using MessageBox = System.Windows.MessageBox;

    public class SalesVM : ViewModelBase<SalesTransaction>
    {
        #region Backing Fields
        private string _salesTransactionID;
        private decimal _transactionGrossTotal;
        private decimal _transactionNetTotal;
        private decimal _transactionDiscount;
        private ICommand _deleteLineCommand;
        private SalesTransactionLineVM _selectedLine;
        private ICommand _proceedTransactionCommand;
        private decimal _transactionReceive;
        private decimal _transactionChange;
        private ICommand _confirmTransactionCommand;
        private ICommand _newTransactionCommand;
        #endregion

        public SalesVM()
        {
            Model = new SalesTransaction();
            DisplayedLines = new ObservableCollection<SalesTransactionLineVM>();
            NewEntryVM = new SalesNewEntryVM(this);
            SetTransactionID();
        }

        public SalesNewEntryVM NewEntryVM { get; }

        public ObservableCollection<SalesTransactionLineVM> DisplayedLines { get; }

        #region Properties
        public string SalesTransactionID 
        {
            get { return _salesTransactionID; }
            set { SetProperty(ref _salesTransactionID, value, () => SalesTransactionID); }
        }

        public decimal TransactionGrossTotal 
        {
            get { return _transactionGrossTotal; }
            set
            {
                SetProperty(ref _transactionGrossTotal, value, () => TransactionGrossTotal);
                UpdateTransactionNetTotal();
            }
        }

        public decimal TransactionDiscount
        {
            get { return _transactionDiscount; }
            set
            {
                SetProperty(ref _transactionDiscount, value, () => TransactionDiscount);
                UpdateTransactionNetTotal();
            }
        }

        public decimal TransactionNetTotal
        {
            get { return _transactionNetTotal; }
            set { SetProperty(ref _transactionNetTotal, value, () => TransactionNetTotal); }
        }

        public decimal TransactionReceive
        {
            get { return _transactionReceive; }
            set
            {
                if (value < 0) return;
                SetProperty(ref _transactionReceive, value, () => TransactionReceive);
                TransactionChange = _transactionReceive - _transactionNetTotal < 0 ? 0 : _transactionReceive - _transactionNetTotal;
            }
        }

        public decimal TransactionChange
        {
            get { return _transactionChange; }
            set { SetProperty(ref _transactionChange, value, () => TransactionChange); }
        }

        public SalesTransactionLineVM SelectedLine
        {
            get { return _selectedLine; }
            set { SetProperty(ref _selectedLine, value, () => SelectedLine); }
        }
        #endregion

        #region Commands
        public ICommand DeleteLineCommand
        {
            get
            {
                return _deleteLineCommand ?? (_deleteLineCommand = new RelayCommand(() =>
                {
                    if (_selectedLine != null &&
                        MessageBox.Show("Confirm Deletion?", "Confirmation", MessageBoxButton.YesNo)
                        == MessageBoxResult.No) return;
                    DisplayedLines.Remove(_selectedLine);
                    UpdateTransactionGrossTotal();
                }));
            }
        }

        public ICommand ProceedTransactionCommand
        {
            get
            {
                return _proceedTransactionCommand ?? (_proceedTransactionCommand =
                    new RelayCommand(() =>
                    {
                        if (DisplayedLines.Count == 0 || !AreAllItemsQuantityEnoughInDatabase()) return;
                        ShowConfirmationWindow();
                    }));
            }
        }

        private bool AreAllItemsQuantityEnoughInDatabase()
        {
            foreach (var line in DisplayedLines.Where(line => GetAvailableQuantity(line.Item) <= 0))
            {
                MessageBox.Show($"{line.Item.Name} has not enough quantity in the database.", "Insufficient Quantity",
                    MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        public ICommand NewTransactionCommand => _newTransactionCommand ?? (_newTransactionCommand =
            new RelayCommand(ResetTransaction));

        public ICommand ConfirmTransactionCommand
        {
            get
            {
                return _confirmTransactionCommand ?? (_confirmTransactionCommand =
                    new RelayCommand(() =>
                    {                     
                        if (!IsConfirmationYes() || !IsAmountReceivedSufficient()) return;
                        AssignTransactionPropertiesToModel();
                        PrintReceiptForTransaction();
                        SalesTransactionHelper.SaveTransaction(Model);
                        //AddPointsToCustomer();
                        ResetTransaction();
                        UtilityMethods.CloseForemostWindow();
                    }));
            }
        }
        #endregion

        #region Helper Methods
        private void SetTransactionID()
        {
            var month = UtilityMethods.GetCurrentDate().Month;
            var year = UtilityMethods.GetCurrentDate().Year;
            var leadingIDString = "M" + (long)((year - 2000) * 100 + month) + "-";
            var endingIDString = 0.ToString().PadLeft(6, '0');
            _salesTransactionID = leadingIDString + endingIDString;

            string lastTransactionID = null;
            using (var context = new ERPContext())
            {
                var IDs = from SalesTransaction in context.SalesTransactions
                          where SalesTransaction.SalesTransactionID.Substring(0, 6).Equals(leadingIDString)
                          && string.Compare(SalesTransaction.SalesTransactionID, _salesTransactionID, StringComparison.Ordinal) >= 0
                          orderby SalesTransaction.SalesTransactionID descending
                          select SalesTransaction.SalesTransactionID;
                if (IDs.Count() != 0) lastTransactionID = IDs.First();
            }

            if (lastTransactionID != null)
            {
                var newIDIndex = Convert.ToInt64(lastTransactionID.Substring(6, 6)) + 1;
                endingIDString = newIDIndex.ToString().PadLeft(6, '0');
                _salesTransactionID = leadingIDString + endingIDString;
            }

            Model.SalesTransactionID = _salesTransactionID;
            OnPropertyChanged("SalesTransactionID");
        }

        public void UpdateTransactionGrossTotal()
        {
            var total = DisplayedLines.Sum(line => line.Total);
            TransactionGrossTotal = total;
        }

        private void UpdateTransactionNetTotal()
        {
            TransactionNetTotal = _transactionGrossTotal - _transactionDiscount;
        }

        public int GetAvailableQuantity(Item item)
        {
            using (var context = new ERPContext())
            {
                var stockFromDatabase =
                    context.Stocks.SingleOrDefault(
                        stock => stock.ItemID.Equals(item.ID) && stock.Warehouse.Name.Equals("Supermarket"));
                if (stockFromDatabase == null) return 0;
                return DisplayedLines.Where(line => line.Item.ID.Equals(item.ID))
                    .Aggregate(stockFromDatabase.Pieces, (current, line) => current - line.Quantity);
            }
        }

        public void ResetTransaction()
        {
            Model = new SalesTransaction();
            DisplayedLines.Clear();
            TransactionChange = 0;
            TransactionReceive = 0;
            TransactionGrossTotal = 0;
            TransactionDiscount = 0;
            TransactionNetTotal = 0;
            SetTransactionID();
        }

        private void ShowConfirmationWindow()
        {
            TransactionReceive = 0;
            TransactionChange = 0;
            var editWindow = new SalesConfirmationView(this)
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            editWindow.ShowDialog();
        }

        private bool IsAmountReceivedSufficient()
        {
            if (_transactionReceive >= _transactionNetTotal) return true;
            MessageBox.Show("Not enough cash received.", "Invalid Command", MessageBoxButton.OK);
            return false;
        }

        private static bool IsConfirmationYes()
        {
            return
                MessageBox.Show("Confirm transaction?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes;
        }

        private void AssignTransactionPropertiesToModel()
        {
            AssignTransactionLinesToModel();
            Model.GrossTotal = _transactionGrossTotal;
            Model.Discount = _transactionDiscount;
            Model.Total = _transactionNetTotal;
            SetTransactionID();
        }

        private void AssignTransactionLinesToModel()
        {
            foreach (var line in DisplayedLines)
                Model.SalesTransactionLines.Add(line.Model);
        }
        #endregion

        #region Print Receipt Helper Methods
        public void PrintReceiptForTransaction()
        {
            using (var recordDoc = new PrintDocument {DocumentName = "Customer Receipt"})
            {

                var printer = new ReceiptPrinter(Model);
                recordDoc.PrintPage += printer.PrintReceiptPage;
                recordDoc.PrintController = new StandardPrintController(); // hides status dialog popup

                // Comment if debugging 
                var printerSettings = new PrinterSettings { PrinterName = "EPSON TM-U220 ReceiptE4" };
                recordDoc.PrinterSettings = printerSettings;
                recordDoc.Print();

                // --------------------------------------
                // Uncomment if debugging - shows dialog instead
                //var printPrvDlg = new PrintPreviewDialog
                //{
                //    Document = recordDoc,
                //    Width = 1200,
                //    Height = 800
                //};
                //printPrvDlg.ShowDialog();
                // --------------------------------------
            }
        }
        #endregion
    }
}

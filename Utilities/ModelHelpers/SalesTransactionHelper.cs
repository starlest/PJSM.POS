namespace PUJASM.POS.Utilities.ModelHelpers
{
    using System.Linq;
    using System.Transactions;
    using Models;

    internal static class SalesTransactionHelper
    {
        public static void SaveTransaction(SalesTransaction salesTransaction)
        {
            using (var ts = new TransactionScope())
            {
                var context = new ERPContext();

                salesTransaction.Date = context.Dates.Single(date => date.Name.Equals("Current")).DateTime;
                //Model.User = context.Users.Single()
                AttachLinesToDatabaseContext(context, salesTransaction);
                ReduceSalesTransactionItemsStock(context, salesTransaction);
                context.SalesTransactions.Add(salesTransaction);
                IssueSalesTransactionInvoice(context, salesTransaction);
                context.SaveChanges();
                ts.Complete();
            }
        }

        private static void AttachLinesToDatabaseContext(ERPContext context, SalesTransaction salesTransaction)
        {
            foreach (var line in salesTransaction.SalesTransactionLines)
            {
                line.Item = context.Inventory.Single(item => item.ID.Equals(line.Item.ID));
                line.Warehouse = context.Warehouses.Single(warehouse => warehouse.ID.Equals(line.WarehouseID));
            }
        }

        private static void ReduceSalesTransactionItemsStock(ERPContext context, SalesTransaction salesTransaction)
        {
            foreach (var line in salesTransaction.SalesTransactionLines)
            {
                var stockFromDatabase = context.Stocks.Single(stock => stock.ItemID.Equals(line.Item.ID) &&
                                                                       stock.WarehouseID.Equals(1));
                stockFromDatabase.Pieces -= line.Quantity;
            }
            context.SaveChanges();
        }

        public static void IssueSalesTransactionInvoice(ERPContext context, SalesTransaction salesTransaction)
        {
            RecordSalesRevenueRecognitionLedgerTransactionInDatabaseContext(context, salesTransaction);
            RecordCostOfGoodsSoldLedgerTransactionInDatabaseContext(context, salesTransaction);
        }

        private static void RecordSalesRevenueRecognitionLedgerTransactionInDatabaseContext(ERPContext context,
            SalesTransaction salesTransaction)
        {
            var salesRevenueRecognitionLedgerTransaction = new LedgerTransaction();
            if (
                !LedgerTransactionHelper.AddTransactionToDatabase(context, salesRevenueRecognitionLedgerTransaction,
                    UtilityMethods.GetCurrentDate().Date, salesTransaction.SalesTransactionID, "Sales Revenue")) return;
            context.SaveChanges();
            LedgerTransactionHelper.AddTransactionLineToDatabase(context, salesRevenueRecognitionLedgerTransaction,
                "Cash", "Debit", salesTransaction.Total);
            LedgerTransactionHelper.AddTransactionLineToDatabase(context, salesRevenueRecognitionLedgerTransaction,
                "Sales Revenue", "Credit", salesTransaction.Total);
            context.SaveChanges();
        }

        private static void RecordCostOfGoodsSoldLedgerTransactionInDatabaseContext(ERPContext context,
            SalesTransaction salesTransaction)
        {
            var costOfGoodsSoldAmount = CalculateCOGSAndIncreaseSoldOrReturned(context, salesTransaction);
            var costOfGoodsSoldLedgerTransaction = new LedgerTransaction();
            if (
                !LedgerTransactionHelper.AddTransactionToDatabase(context, costOfGoodsSoldLedgerTransaction,
                    UtilityMethods.GetCurrentDate().Date, salesTransaction.SalesTransactionID, "Cost of Goods Sold"))
                return;
            context.SaveChanges();
            LedgerTransactionHelper.AddTransactionLineToDatabase(context, costOfGoodsSoldLedgerTransaction,
                "Cost of Goods Sold", "Debit", costOfGoodsSoldAmount);
            LedgerTransactionHelper.AddTransactionLineToDatabase(context, costOfGoodsSoldLedgerTransaction, "Inventory",
                "Credit", costOfGoodsSoldAmount);
            context.SaveChanges();
        }

        private static decimal CalculateCOGSAndIncreaseSoldOrReturned(ERPContext context,
            SalesTransaction salesTransaction)
        {
            var costOfGoodsSoldAmount = 0m;
            foreach (var line in salesTransaction.SalesTransactionLines)
            {
                var purchases = context.PurchaseTransactionLines
                    .Include("PurchaseTransaction")
                    .Where(
                        purchaseTransactionLine =>
                            purchaseTransactionLine.ItemID.Equals(line.Item.ID) &&
                            purchaseTransactionLine.SoldOrReturned < purchaseTransactionLine.Quantity)
                    .OrderBy(purchaseTransaction => purchaseTransaction.PurchaseTransactionID)
                    .ThenByDescending(
                        purchaseTransactionLine =>
                            purchaseTransactionLine.Quantity - purchaseTransactionLine.SoldOrReturned)
                    .ThenByDescending(purchaseTransactionLine => purchaseTransactionLine.PurchasePrice)
                    .ThenByDescending(purchaseTransactionLine => purchaseTransactionLine.Discount)
                    .ThenByDescending(purchaseTransactionLine => purchaseTransactionLine.WarehouseID)
                    .ToList();
                var tracker = line.Quantity;

                foreach (var purchase in purchases)
                {
                    var availableQuantity = purchase.Quantity - purchase.SoldOrReturned;
                    var purchaseLineNetTotal = purchase.PurchasePrice - purchase.Discount;

                    if (tracker <= availableQuantity)
                    {
                        purchase.SoldOrReturned += tracker;
                        if (purchaseLineNetTotal == 0) break;
                        var fractionOfTransactionDiscount = tracker*purchaseLineNetTotal/
                                                            purchase.PurchaseTransaction.GrossTotal*
                                                            purchase.PurchaseTransaction.Discount;
                        var fractionOfTransactionTax = tracker*purchaseLineNetTotal/
                                                       purchase.PurchaseTransaction.GrossTotal*
                                                       purchase.PurchaseTransaction.Tax;
                        costOfGoodsSoldAmount += tracker*purchaseLineNetTotal - fractionOfTransactionDiscount +
                                                 fractionOfTransactionTax;
                        break;
                    }

                    if (tracker > availableQuantity)
                    {
                        purchase.SoldOrReturned += availableQuantity;
                        tracker -= availableQuantity;
                        if (purchaseLineNetTotal == 0) continue;
                        var fractionOfTransactionDiscount = availableQuantity*purchaseLineNetTotal/
                                                            purchase.PurchaseTransaction.GrossTotal*
                                                            purchase.PurchaseTransaction.Discount;
                        var fractionOfTransactionTax = availableQuantity*purchaseLineNetTotal/
                                                       purchase.PurchaseTransaction.GrossTotal*
                                                       purchase.PurchaseTransaction.Tax;
                        costOfGoodsSoldAmount += availableQuantity*purchaseLineNetTotal -
                                                 fractionOfTransactionDiscount + fractionOfTransactionTax;
                    }
                }
            }
            context.SaveChanges();
            return costOfGoodsSoldAmount;
        }
    }
}

namespace PUJASM.POS.Utilities
{
    using System;
    using System.Linq;
    using System.Windows;
    using ERP.Models;
    using Models;
    using Models.Ledger;

    public static class LedgerTransactionHelper
    {
        public static bool AddTransactionToDatabase(ERPContext context, LedgerTransaction transaction, DateTime date, string documentation, string description)
        {
            using (var c = new ERPContext())
            {
                var period = c.Ledger_General.First().Period;
                if (period != date.Month)
                {
                    MessageBox.Show("Wrong period.", "Invalid Transaction", MessageBoxButton.OK);
                    return false;
                }

                if (date.Date.Day != UtilityMethods.GetCurrentDate().Day)
                {
                    if (!(date.AddDays(2).Day <= UtilityMethods.GetCurrentDate().Day && date.AddDays(2).Day >= date.Day))
                    {
                        MessageBox.Show("Wrong period.", "Invalid Transaction", MessageBoxButton.OK);
                        return false;
                    }
                }
            }

            transaction.Documentation = documentation;
            transaction.Date = date;
            transaction.Description = description;

            if (Application.Current != null)
            {
                var user = Application.Current.TryFindResource("CurrentUser") as User;
                if (user != null)
                    transaction.User = context.Users.FirstOrDefault(e => e.Username.Equals(user.Username));
            }
            else transaction.User = context.Users.FirstOrDefault(e => e.Username.Equals("EMPTY"));

            context.Ledger_Transactions.Add(transaction);
            return true;
        }

        public static void AddTransactionLineToDatabase(ERPContext context, LedgerTransaction transaction, string accountName, string seq, decimal amount)
        {
            var transactionLine = new LedgerTransactionLine();
            var account = context.Ledger_Accounts.First(e => e.Name.Equals(accountName));
            var ledgerAccount = context.Ledger_General.First(e => e.ID.Equals(account.ID)); 

            transactionLine.LedgerTransaction = transaction;
            transactionLine.LedgerAccount = account;
            transactionLine.Seq = seq;
            transactionLine.Amount = amount;

            // Update the amount of the account
            if (seq == "Debit") ledgerAccount.Debit += amount;
            else if (seq == "Credit") ledgerAccount.Credit += amount;

            context.Ledger_Transaction_Lines.Add(transactionLine);
        }
    }
}

namespace PUJASM.POS.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using ERP.Models;
    using MVVMFramework;

    public class LedgerTransaction : ObservableObject
    { 
        public LedgerTransaction()
        {
            LedgerTransactionLines = new ObservableCollection<LedgerTransactionLine>();
        }

        [Key]
        public int ID { get; set; }

        [Required, Index]
        public DateTime Date { get; set; }

        public string Documentation { get; set; }

        public string Description { get; set; }

        public virtual User User { get; set; }

        public ObservableCollection<LedgerTransactionLine> LedgerTransactionLines { get; set; }
    }
}

namespace PUJASM.POS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using ERP.Models;

    public class SalesTransaction
    {
        public SalesTransaction()
        {
            // ReSharper disable once VirtualMemberCallInContructor
            SalesTransactionLines = new HashSet<SalesTransactionLine>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 0), StringLength(128)]
        public string SalesTransactionID { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual User User { get; set; }

        public decimal GrossTotal { get; set; }
        
        public decimal Discount { get; set; }

        public decimal Total { get; set; }

        [Index]
        public DateTime Date { get; set; }

        public string Notes { get; set; }

        public virtual ICollection<SalesTransactionLine> SalesTransactionLines { get; set; }

        public virtual ICollection<SalesReturnTransaction> SalesReturnTransactions { get; set; }
    }
}

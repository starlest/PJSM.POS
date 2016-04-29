namespace PUJASM.POS.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using MVVMFramework;

    public class LedgerGeneral : ObservableObject
    {
        [Key, ForeignKey("LedgerAccount")]
        public int ID { get; set; }

        [Required]
        public int PeriodYear { get; set; }

        [Required]
        public int Period { get; set; }

        [Required]
        public decimal Debit { get; set; }

        [Required]
        public decimal Credit { get; set; }

        public virtual LedgerAccount LedgerAccount { get; set; }
    }
}

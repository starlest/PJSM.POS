namespace PUJASM.POS.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using MVVMFramework;

    public class LedgerAccountBalance : ObservableObject
    {
        [Key, ForeignKey("LedgerAccount")]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PeriodYear { get; set; }

        [Required]
        public decimal BeginningBalance { get; set; }

        [Required]
        public decimal Balance1 { get; set; }

        [Required]
        public decimal Balance2 { get; set; }

        [Required]
        public decimal Balance3 { get; set; }

        [Required]
        public decimal Balance4 { get; set; }

        [Required]
        public decimal Balance5 { get; set; }

        [Required]
        public decimal Balance6 { get; set; }

        [Required]
        public decimal Balance7 { get; set; }

        [Required]
        public decimal Balance8 { get; set; }

        [Required]
        public decimal Balance9 { get; set; }

        [Required]
        public decimal Balance10 { get; set; }

        [Required]
        public decimal Balance11 { get; set; }

        [Required]
        public decimal Balance12 { get; set; }

        public virtual LedgerAccount LedgerAccount { get; set; }
    }
}

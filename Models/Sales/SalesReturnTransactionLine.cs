namespace PUJASM.POS.Models.Sales
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Item;
    using MVVMFramework;

    public class SalesReturnTransactionLine : ObservableObject
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string SalesReturnTransactionID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ItemID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WarehouseID { get; set; }
        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public decimal SalesPrice { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public decimal Discount { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public decimal ReturnPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required]
        public decimal CostOfGoodsSold { get; set; }

        [ForeignKey("ItemID")]
        public virtual Item Item { get; set; }

        [ForeignKey("WarehouseID")]
        public virtual Warehouse Warehouse { get; set; }

        [ForeignKey("SalesReturnTransactionID")]
        public virtual SalesReturnTransaction SalesReturnTransaction { get; set; }
    }
}

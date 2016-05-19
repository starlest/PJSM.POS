namespace PUJASM.POS.Models.Purchase
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Item;
    using MVVMFramework;

    public class PurchaseTransactionLine : ObservableObject
    {
        [Key]
        [Column(Order = 0)]
        public string PurchaseTransactionID { get; set; }

        [Key]
        [Column(Order = 1)]
        public string ItemID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int WarehouseID { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal PurchasePrice { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal Discount { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Total { get; set; }

        public int SoldOrReturned { get; set; }

        [ForeignKey("ItemID")]
        public virtual Item Item { get; set; }

        [ForeignKey("PurchaseTransactionID")]
        public virtual PurchaseTransaction PurchaseTransaction { get; set; }

        [ForeignKey("WarehouseID")]
        public virtual Warehouse Warehouse { get; set; }
    }
}

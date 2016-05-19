namespace PUJASM.POS.Models.Item
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Sales;

    [Table("Inventory")]
    #pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class Item
    {
        public Item()
        {
            // ReSharper disable once VirtualMemberCallInContructor
            Suppliers = new ObservableCollection<Supplier>();
            IsActive = true;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 0)]
        public string ID { get; set; }

        public virtual ItemCategory ItemCategory { get; set; }

        public string Name { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalesPrice { get; set; }

        public bool IsActive { get; set; }

        public virtual ObservableCollection<Supplier> Suppliers { get; set; }

        public virtual ObservableCollection<Stock> Stocks { get; set; }

        public virtual ICollection<SalesTransactionLine> SalesTransactionLines { get; set; }

        public virtual ICollection<SalesReturnTransactionLine> SalesReturnTransactionLines { get; set; }

        public virtual ObservableCollection<Promotion> Promotions { get; set; } 

        #pragma warning disable 659
        public override bool Equals(object obj)
        {
            var item = obj as Item;
            return item != null && ID.Equals(item.ID);
        }
    }
}

namespace PUJASM.POS.Models
{
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class Supplier
    {
        public Supplier()
        {
            IsActive = true;
        }

        public int ID { get; set; }

        [Required, MaxLength(100), Index(IsUnique = true)]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        public int GSTID { get; set; }

        public decimal PurchaseReturnCredits { get; set; }

        public bool IsActive { get; set; }

        public virtual ObservableCollection<Item.Item> Items { get; set; }

        public override string ToString() { return Name; }
        
        #pragma warning disable 659
        public override bool Equals(object obj)
        {
            var supplier = obj as Supplier;
            return supplier != null && ID.Equals(supplier.ID);
        }
    }
}

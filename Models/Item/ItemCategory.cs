namespace PUJASM.POS.Models.Item
{
    using System.Collections.Generic;
    using MVVMFramework;

    #pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class ItemCategory : ObservableObject
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        #pragma warning disable 659
        public override bool Equals(object obj)
        {
            var category = obj as ItemCategory;
            return category != null && ID.Equals(category.ID);
        }
    }
}

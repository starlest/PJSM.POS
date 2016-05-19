namespace PUJASM.POS.Models.Item
{
    using System;

    public class Promotion
    {
        public int ID { get; set; }
         
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public virtual Item Item { get; set; }

        public int PromotionQuantity { get; set; }

        public decimal Discount { get; set; }
    }
}

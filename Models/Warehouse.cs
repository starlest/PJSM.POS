namespace PUJASM.POS.Models
{
    public class Warehouse
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            var warehouse = obj as Warehouse;
            return warehouse != null && ID.Equals(warehouse.ID);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

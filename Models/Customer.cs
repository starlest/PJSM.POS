namespace PUJASM.POS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Sales;

    [Table("Customers")]
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Telephone { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public DateTime Expiry { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual List<SalesTransaction> SalesTransactions { get; set; }
    }
}

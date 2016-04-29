namespace PUJASM.ERP.Models
{
    using System.ComponentModel.DataAnnotations;

    public partial class User
    {
        [Key]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool IsAdmin { get; set; }
    }
}

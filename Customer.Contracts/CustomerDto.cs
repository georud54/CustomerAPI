using System.ComponentModel.DataAnnotations;

namespace Customer.Contracts
{
    public class CustomerDto
    {
        [Key]
        [Required]
        public int Id { get; set; }
       
        public string FirstName { get; set; }
      
        public string LastName { get; set; }
       
        public DateTime DateOfBirth { get; set; }
    }
}

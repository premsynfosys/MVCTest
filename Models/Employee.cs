using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web;

namespace Test.Models
{
    public class Employee
    {
        public int ID { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLength(3,ErrorMessage ="Length should be morethan 3")]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public decimal Salary { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
        public string Pwd { get; set; }


        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(".+@.+\\..+", ErrorMessage = "Please Enter Correct Email Address")]
      
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter Mobile No")]
        [StringLength(10, ErrorMessage = "The Mobile must contains 10 characters", MinimumLength = 10)]

        public string Mobile { get; set; }
        [Required]
        public string Img { get; set; }
    
        [Required]
        public string Role { get; set; }
    }
}

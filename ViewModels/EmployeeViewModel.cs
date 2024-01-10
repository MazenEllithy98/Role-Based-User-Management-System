using Demo.DAL.Models;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Http;

namespace Demo.PL.ViewModels
{
    public class EmployeeViewModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Name is Required!")]
        [MaxLength(50, ErrorMessage = "Maximum Character Length is 50 Characters!")]
        [MinLength(5, ErrorMessage = "Minimum Character Length is 5 Characters!")]
        public string Name { get; set; }
        [Range(22, 30, ErrorMessage = "Age Range is between 22 and 30!")]
        public int? Age { get; set; }
        [RegularExpression(@"^[0-9]{1,3}-[a-zA-Z]{10,20}-[a-zA-Z]{10,20}-[a-zA-Z]{10,20}$"
         , ErrorMessage = "Address Must be like 123-Street-City-Country")]
        
        public string Address { get; set; }
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
        public IFormFile Image { get; set; }
        public string ImageName { get; set; }

        public bool IsActive { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }

        public int? DepartmentsID { get; set; }

        //Navigational Property (one) 
        public Departments Department { get; set; }
    }
}

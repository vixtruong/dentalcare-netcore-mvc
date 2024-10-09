// Models/EmployeeViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace DentalCare.Models
{
    public class Employee
    {
        public string? id { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public string? Role { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Birthday is required")]
        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Join time is required")]
        public DateTime? JoinTime { get; set; }

        public IFormFile? Avatar { get; set; }

        public string? AvatarPath { get; set; }

        public string? Faculty { get; set; }

        public bool? Fired { get; set; }
    }
}
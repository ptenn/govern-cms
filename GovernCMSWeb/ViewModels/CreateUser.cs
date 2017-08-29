using System;
using System.ComponentModel.DataAnnotations;

namespace GovernCMS.ViewModels
{
    public class CreateUser
    {
        public int UserId { get; set; }

        [Required, EmailAddress(ErrorMessage = "Data entered is not a valid e-mail address.")]
        public string EmailAddr { get; set; }

        [Required, MinLength(8, ErrorMessage = "Password must be at least 8 characters"),
         RegularExpression(@".*(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).*", ErrorMessage = "Password must meet requirements")]
        public string Passwd { get; set; }

        [Compare("Passwd", ErrorMessage = "Confirm password doesn't match, please try again")]
        public string ConfirmPasswd { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public int? OrganizationId { get; set; }

        public string OrganizationName { get; set; }
    }
}
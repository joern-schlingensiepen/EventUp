﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventUpWebApp.Models
{
    public class UserViewModel
    {
        
        
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

                
            public string Name { get; set; }

            public string FamilyName { get; set; }

            public string TelephoneNumber { get; set; }

            public string Role_Admin { get; set; }
            public string Role_Planner { get; set; }
            public string Role_Supplier { get; set; }


    }
   
}
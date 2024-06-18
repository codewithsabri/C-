using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace userProject.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Updated
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public DateTime Created { get; set; }

        public UserViewModel()
        {
            Created = DateTime.Now;
        }

        public string CreatedInFrenchFormat
        {
            get
            {
                return Created.ToString("dd/MM/yyyy HH:mm:ss", new System.Globalization.CultureInfo("fr-FR"));
            }
        }


    }
}
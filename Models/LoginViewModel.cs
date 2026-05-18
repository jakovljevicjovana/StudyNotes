using System.ComponentModel.DataAnnotations;

namespace StudyNotes.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
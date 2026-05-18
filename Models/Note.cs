using System.ComponentModel.DataAnnotations;

namespace StudyNotes.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naslov beleške je obavezan.")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sadržaj beleške je obavezan.")]
        public string Content { get; set; } = string.Empty;

        public bool IsImportant { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Predmet je obavezan.")]
        public int SubjectId { get; set; }

        public Subject? Subject { get; set; }

        public string? FilePath { get; set; }

        public string? FileName { get; set; }
    }
}
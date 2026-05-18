using System.ComponentModel.DataAnnotations;

namespace StudyNotes.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv predmeta je obavezan.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public List<Note> Notes { get; set; } = new List<Note>();
    }
}
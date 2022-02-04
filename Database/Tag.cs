using Database.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Tag]
        [MaxLength(20)]
        public string Name { get; set; }

        public List<AlbumTag> Albums { get; set; } = new List<AlbumTag>();
    }
}

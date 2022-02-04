using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string BackgroundColor { get; set; }

        public bool Public { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public List<AlbumPicture> Pictures { get; set; } = new List<AlbumPicture>();

        public List<AlbumTag> Tags { get; set; } = new List<AlbumTag>();
    }
}

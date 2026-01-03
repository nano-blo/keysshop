using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeysShop.Models
{
    [Table("Genre")]
    public class Genre
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("жанр")]
        public string жанр { get; set; }

        public virtual ICollection<GameGenre> GameGenres { get; set; }
    }
}

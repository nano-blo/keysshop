using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeysShop.Models
{
    [Table("Games_Genre")]
    public class GameGenre
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("id_игры")]
        public int id_игры { get; set; }

        [Column("id_жанра")]
        public int id_жанра { get; set; }

        [ForeignKey("id_игры")]
        public virtual Game Game { get; set; }

        [ForeignKey("id_жанра")]
        public virtual Genre Genre { get; set; }
    }
}
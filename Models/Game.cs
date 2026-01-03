using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeysShop.Models
{
    [Table("Games")]
    public class Game
    {
        [Key]
        [Column("id_игры")]
        public int id_игры { get; set; }

        [Column("название")]
        public string название { get; set; }

        [Column("год_выпуска")]
        public string год_выпуска { get; set; }

        [Column("описание")]
        public string описание { get; set; }

        [Column("id_разработчика")]
        public int? id_разработчика { get; set; }

        [Column("изображение")]
        public string изображение { get; set; }

        // ИСПРАВЛЕНО: Указываем тип для decimal!
        [Column("цена", TypeName = "decimal(18,2)")]
        public decimal? цена { get; set; }

        [ForeignKey("id_разработчика")]
        public virtual Developer Developer { get; set; }

        public virtual ICollection<GameGenre> GameGenres { get; set; }
        public virtual ICollection<Key> Keys { get; set; }

        [NotMapped]
        public string ImageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(изображение))
                    return "/images/default-game.png";
                return $"/images/games/{изображение}";
            }
        }
    }
}
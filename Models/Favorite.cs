using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeysShop.Models
{
    [Table("Favorites")]
    public class Favorite
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("Game")]
        [Column("id_игры")]
        public int GameId { get; set; }

        [ForeignKey("User")]
        [Column("id_пользователя")]
        public int UserId { get; set; }

        // Навигационные свойства
        public virtual Game Game { get; set; }
        public virtual User User { get; set; }
    }
}

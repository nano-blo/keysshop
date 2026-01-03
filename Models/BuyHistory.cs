using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeysShop.Models
{
    [Table("Buy_History")]
    public class BuyHistory
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("User")]
        [Column("id_пользователь")]
        public int UserId { get; set; }

        [ForeignKey("Key")]
        [Column("id_ключ")]
        public int KeyId { get; set; }

        [Display(Name = "Время покупки")]
        [Column("время_покупки")]
        public DateTime? PurchaseTime { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public virtual User User { get; set; }
        public virtual Key Key { get; set; }
        public virtual Game Game { get; set; } // Через Key → Game
    }
}

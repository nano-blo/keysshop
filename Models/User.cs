using Azure.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeysShop.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(100, ErrorMessage = "Имя не может быть длиннее 100 символов")]
        [Column("имя")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        [Column("email")]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Пароль обязателен")]
        [Column("пароль")]
        public string Password { get; set; }

        [Display(Name = "Роль")]
        [Column("роль")]
        public string Role { get; set; } = "User"; // По умолчанию обычный пользователь

        // Навигационные свойства
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<BuyHistory> BuyHistory { get; set; }
    }
}
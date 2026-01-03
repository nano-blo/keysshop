using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeysShop.Models
{
    [Table("Developer")]
    public class Developer
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("разработчик")]
        public string разработчик { get; set; }

        public virtual ICollection<Game> Games { get; set; }
    }
}
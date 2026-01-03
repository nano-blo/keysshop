using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeysShop.Models
{
    [Table("Keys")]
    public class Key
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("id_игры")]
        public int? id_игры { get; set; }

        [Column("ключ")]
        public string ключ { get; set; }

        [Column("продан")]
        public bool? продан { get; set; }

        [ForeignKey("id_игры")]
        public virtual Game Game { get; set; }
    }
}

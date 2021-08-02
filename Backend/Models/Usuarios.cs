using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("USUARIOS")]
    public class Usuarios
    {
        [Key]
        [Column("CPF")]
        public string CPF { get; set; }

        [Column("NOME")]
        public string Nome { get; set; }

        [Column("DATA_CRIACAO")]
        public DateTime DataCriacao { get; set; }

        [Column("DATA_ALTERACAO")]
        public DateTime DataAlteracao { get; set; }

        [Column("ATIVO")]
        public bool Ativo { get; set; }
    }
}

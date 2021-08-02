using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("PERMISSOES")]
    public class Permissoes
    {
        [Key]
        [Column("CODIGO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Column("NOME")]
        public string Nome { get; set; }

        [Column("CRIAR")]
        public bool Criar { get; set; }

        [Column("LER")]
        public bool Ler { get; set; }

        [Column("ATUALIZAR")]
        public bool Atualizar { get; set; }

        [Column("DELETAR")]
        public bool Deletar { get; set; }

        [Column("ADMINISTRADOR")]
        public bool Administrador { get; set; }

        [Column("DATA_CRIACAO")]
        public DateTime DataCriacao { get; set; }

        [Column("DATA_ALTERACAO")]
        public DateTime DataAlteracao { get; set; }
    }
}

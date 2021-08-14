using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    [Table("CADASTRADORES")]
    public class Cadastradores
    {
        [Key]
        [Column("USUARIO")]
        public string Usuario { get; set; }

        [Column("SENHA")]
        public string Senha { get; set; }

        [Column("ATIVO")]
        public bool Ativo { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(Permissao))]
        [Column("COD_PERMISSAO")]
        public int? CodPermissao { get; set; }

        public Permissoes Permissao { get; set; }

        [Column("DATA_CRIACAO")]
        public DateTime DataCriacao { get; set; }

        [Column("DATA_ALTERACAO")]
        public DateTime DataAlteracao { get; set; }
    }
}
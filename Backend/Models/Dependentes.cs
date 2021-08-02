using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    [Table("DEPENDENTES")]
    public class Dependentes
    {
        [Key]
        [Column("CODIGO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Column("TITULAR")]
        [ForeignKey(nameof(Titular))]
        [JsonPropertyName("titular")]
        public string TitularCPF { get; set; }

        [JsonIgnore]
        public virtual Usuarios Titular { get; set; }

        [Column("USUARIO")]
        [ForeignKey(nameof(Dependente))]
        [JsonPropertyName("dependente")]
        public string DependenteCPF { get; set; }

        [JsonIgnore]
        public virtual Usuarios Dependente { get; set; }
    }
}

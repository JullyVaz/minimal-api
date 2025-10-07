using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace MinimalApi.Dominio.Entidades;

public class Veiculo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;
    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = default!;
    [Required]
    [StringLength(100)]
    public string Marca { get; set; } = default!;
    [Required]
    [Range(1886, 2100, ErrorMessage = "Ano inválido")]
    public int Ano { get; set; } = default!;
}
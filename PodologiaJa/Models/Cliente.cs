using System.ComponentModel.DataAnnotations;

namespace PodologiaJa.Models
{
    public class Cliente
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public string Nome_completo { get; set; } = string.Empty;


        [Required(ErrorMessage = "O celular é obrigatório.")]
        [RegularExpression(@"\(\d{2}\)\d{5}-\d{4}", ErrorMessage = "O celular deve estar no formato (XX)XXXXX-XXXX")]
        public string Celular { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O Email deve ser valido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data de agendandamento é obrigatória .")]
        public DateOnly Data_Agendamento { get; set; }

        public TimeOnly Hora_Agendamento { get; set; }

        public string Descricao { get; set; } = string.Empty;
    }
}

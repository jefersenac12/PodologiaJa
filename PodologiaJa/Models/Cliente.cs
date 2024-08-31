namespace PodologiaJa.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome_completo { get; set; }= string.Empty;
        public string Celular { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Data_Agendamento { get; set; } = string.Empty;
        public string Hora_Agendamento { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }
}

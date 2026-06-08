namespace GerenciamentoConferencias.Server.Dto
{
    public class PalestraAgendadaDto
    {
        public required string Horario { get; set; }
        public required string Titulo { get; set; }
        public int DuracaoMinutos { get; set; }
        public string FormatoDuracao => DuracaoMinutos == 5 ? "relâmpago" : $"{DuracaoMinutos}min";
    }
}
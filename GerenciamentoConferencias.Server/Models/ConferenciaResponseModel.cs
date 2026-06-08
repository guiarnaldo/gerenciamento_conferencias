using GerenciamentoConferencias.Server.Dto;

namespace GerenciamentoConferencias.Server.Models
{
    public class ConferenciaResponseModel
    {
        public required bool Sucesso { get; set; }
        public string? FormatoTexto { get; set; }
        public ConferenciaDto? Dados { get; set; }
        public string? Erro { get; set; }
    }
}

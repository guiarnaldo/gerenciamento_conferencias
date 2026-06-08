using GerenciamentoConferencias.Server.Dto;

namespace GerenciamentoConferencias.Server.Models
{
    public class ConferenciaRequestModel
    {
        public required List<PalestraEntradaDto> Palestras { get; set; }
    }
}

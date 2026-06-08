namespace GerenciamentoConferencias.Server.Dto
{
    public class SessaoDto
    {
        public required string Nome { get; set; }
        public required List<PalestraAgendadaDto> Palestras { get; set; }
        public int DuracaoTotal { get; set; }
    }
}

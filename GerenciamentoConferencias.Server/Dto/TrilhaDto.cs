namespace GerenciamentoConferencias.Server.Dto
{
    public class TrilhaDto
    {
        public int Numero { get; set; }
        public required SessaoDto SessaoManha { get; set; }
        public required SessaoDto SessaoTarde { get; set; }
    }
}

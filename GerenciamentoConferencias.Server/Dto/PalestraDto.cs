namespace GerenciamentoConferencias.Server.Dto
{
    public record PalestraDto(string Titulo, int DuracaoMinutos)
    {
        public bool IsRelampago => DuracaoMinutos == 5;
    }
}
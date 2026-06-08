namespace GerenciamentoConferencias.Server.Dto
{
    public class PalestraEntradaDto
    {
        /// <summary>
        /// Nome/título da palestra
        /// </summary>
        public required string Nome { get; set; }

        /// <summary>
        /// Tempo em minutos (ignorado se IsRelampago for true)
        /// </summary>
        public int Tempo { get; set; }

        /// <summary>
        /// Indica se é uma palestra relâmpago (5 minutos)
        /// </summary>
        public bool IsRelampago { get; set; }

        /// <summary>
        /// Retorna a duração real em minutos
        /// </summary>
        public int DuracaoMinutos => IsRelampago ? 5 : Tempo;

        /// <summary>
        /// Retorna o formato de exibição da duração
        /// </summary>
        public string FormatoDuracao => IsRelampago ? "relâmpago" : $"{Tempo}min";
    }
}
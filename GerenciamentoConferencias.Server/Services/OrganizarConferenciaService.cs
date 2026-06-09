using GerenciamentoConferencias.Server.Dto;

namespace GerenciamentoConferencias.Server.Services
{
    public class OrganizarConferenciaService
    {
        // Constantes dos horários
        private const int DURACAO_MANHA = 180; // 9h às 12h = 180 minutos
        private const int DURACAO_TARDE_MIN = 180; // 13h às 16h = 180 minutos (mínimo)
        private const int DURACAO_TARDE_MAX = 240; // 13h às 17h = 240 minutos (máximo)
        private const int HORA_INICIO_MANHA = 9;
        private const int HORA_INICIO_TARDE = 13;
        private const int HORA_NETWORKING_MIN = 16;
        private const int HORA_NETWORKING_MAX = 17;
        private const int DURACAO_MAXIMA = 240;

        public static PalestraDto ConverterEntrada(PalestraEntradaDto entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada.Nome))
                throw new ArgumentException("Nome da palestra não pode ser vazio");

            var duracao = entrada.IsRelampago ? 5 : entrada.Tempo;

            if (duracao <= 0)
                throw new ArgumentException($"Duração inválida para palestra: {entrada.Nome}");

            return new PalestraDto(entrada.Nome, duracao);
        }

        /// <summary>
        /// Organiza as palestras nas trilhas
        /// </summary>
        public ConferenciaDto Organizar(List<PalestraDto> palestras)
        {
            if (palestras == null || palestras.Count == 0)
                throw new ArgumentException("Lista de palestras não pode ser vazia");

            // Ordena por duração decrescente
            var palestrasOrdenadas = palestras
            .OrderByDescending(p => p.DuracaoMinutos)
            .ToList();

            var trilhas = new List<TrilhaDto>();
            var trilhaAtual = 1;

            while (palestrasOrdenadas.Count > 0)
            {
                var trilha = CriarTrilha(palestrasOrdenadas, trilhaAtual);
                trilhas.Add(trilha);
                trilhaAtual++;
            }

            return new ConferenciaDto { Trilhas = trilhas };
        }

        private TrilhaDto CriarTrilha(List<PalestraDto> palestrasDisponiveis, int numeroTrilha)
        {
            // Sessão da manhã: exatamente 180 minutos
            var (palestrasManha, _) = PreencherSessao(
                palestrasDisponiveis,
                DURACAO_MANHA,
                exato: true
            );

            // Remove as palestras já alocadas
            foreach (var p in palestrasManha)
            {
                palestrasDisponiveis.Remove(p);
            }

            // Sessão da tarde: entre 180 e 240 minutos
            var (palestrasTarde, _) = PreencherSessao(
                palestrasDisponiveis,
                DURACAO_TARDE_MAX,
                minimo: DURACAO_TARDE_MIN,
                exato: false
            );

            foreach (var p in palestrasTarde)
            {
                palestrasDisponiveis.Remove(p);
            }

            // Agenda os horários
            var sessaoManha = AgendarSessao("Sessão da Manhã", palestrasManha, HORA_INICIO_MANHA);
            var sessaoTarde = AgendarSessao("Sessão da Tarde", palestrasTarde, HORA_INICIO_TARDE);

            return new TrilhaDto
            {
                Numero = numeroTrilha,
                SessaoManha = sessaoManha,
                SessaoTarde = sessaoTarde
            };
        }

        /// <summary>
        /// Preenche uma sessão com palestras
        /// </summary>
        private (List<PalestraDto> selecionadas, List<PalestraDto> restantes) PreencherSessao(
            List<PalestraDto> palestras,
            int capacidadeMaxima,
            int minimo = 0,
            bool exato = false)
        {
            var selecionadas = new List<PalestraDto>();
            var capacidadeRestante = capacidadeMaxima;

            for (int i = 0;i < palestras.Count;i++)
            {
                var palestra = palestras[i];
                if (palestra.DuracaoMinutos <= capacidadeRestante)
                {
                    selecionadas.Add(palestra);
                    capacidadeRestante -= palestra.DuracaoMinutos;
                }

                if (exato && capacidadeRestante == 0)
                    break;
            }

            // Se precisa ser exato e não preencheu, tenta encontrar combinação exata
            if (exato && capacidadeRestante > 0 && palestras.Count > selecionadas.Count)
            {
                var melhor = EncontrarCombinacaoExata(palestras, capacidadeMaxima);
                if (melhor != null)
                    return (melhor, palestras.Except(melhor).ToList());
            }

            // Verifica se atende o mínimo para sessões não-exatas
            var duracaoTotal = selecionadas.Sum(p => p.DuracaoMinutos);
            if (minimo > 0 && duracaoTotal < minimo && palestras.Count > selecionadas.Count)
            {
                var restantes = palestras.Except(selecionadas).ToList();
                foreach (var p in restantes.ToList())
                {
                    if (p.DuracaoMinutos <= capacidadeRestante)
                    {
                        selecionadas.Add(p);
                        capacidadeRestante -= p.DuracaoMinutos;
                    }
                }
            }

            return (selecionadas, palestras.Except(selecionadas).ToList());
        }

        /// <summary>
        /// Encontra uma combinação exata usando programação dinâmica
        /// </summary>
        private static List<PalestraDto>? EncontrarCombinacaoExata(List<PalestraDto> palestras, int alvo)
        {
            var dp = new bool[alvo + 1];
            var parent = new Dictionary<int, (int valor, PalestraDto palestra)>();
            dp[0] = true;

            foreach (var palestra in palestras)
            {
                for (int j = alvo;j >= palestra.DuracaoMinutos;j--)
                {
                    if (dp[j - palestra.DuracaoMinutos] && !dp[j])
                    {
                        dp[j] = true;
                        parent[j] = (j - palestra.DuracaoMinutos, palestra);
                    }
                }
            }

            if (!dp[alvo])
                return null;

            var resultado = new List<PalestraDto>();
            var atual = alvo;
            while (atual > 0)
            {
                var (anterior, palestra) = parent[atual];
                resultado.Add(palestra);
                atual = anterior;
            }

            return resultado;
        }

        private static SessaoDto AgendarSessao(string nome, List<PalestraDto> palestras, int horaInicio)
        {
            var agendadas = new List<PalestraAgendadaDto>();
            var horaAtual = horaInicio;
            var minutoAtual = 0;
            var duracaoTotal = 0;

            foreach (var palestra in palestras)
            {
                var horario = $"{horaAtual:D2}:{minutoAtual:D2}H";
                agendadas.Add(new PalestraAgendadaDto
                {
                    Horario = horario,
                    Titulo = palestra.Titulo,
                    DuracaoMinutos = palestra.DuracaoMinutos
                });

                minutoAtual += palestra.DuracaoMinutos;
                while (minutoAtual >= 60)
                {
                    horaAtual++;
                    minutoAtual -= 60;
                }
                duracaoTotal += palestra.DuracaoMinutos;
            }

            return new SessaoDto
            {
                Nome = nome,
                Palestras = agendadas,
                DuracaoTotal = duracaoTotal
            };
        }

        /// <summary>
        /// Formata a conferência para saída em texto
        /// </summary>
        public string FormatarSaida(ConferenciaDto conferencia)
        {
            var linhas = new List<string>();

            foreach (var trilha in conferencia.Trilhas)
            {
                linhas.Add($"Trilha {trilha.Numero}:");

                // Sessão da manhã
                foreach (var palestra in trilha.SessaoManha.Palestras)
                {
                    linhas.Add($"{palestra.Horario} {palestra.Titulo} {palestra.FormatoDuracao}");
                }

                // Almoço
                linhas.Add($"12:00H Almoço");

                // Sessão da tarde
                foreach (var palestra in trilha.SessaoTarde.Palestras)
                {
                    linhas.Add($"{palestra.Horario} {palestra.Titulo} {palestra.FormatoDuracao}");
                }

                // Networking
                var horaNetworking = CalcularHoraNetworking(trilha.SessaoTarde);
                linhas.Add($"{horaNetworking} Networking Event");

                linhas.Add("");
            }

            return string.Join("\n", linhas).TrimEnd();
        }

        private static string CalcularHoraNetworking(SessaoDto sessaoTarde)
        {
            var ultimaPalestra = sessaoTarde.Palestras.LastOrDefault();
            if (ultimaPalestra == null)
                return "16:00H";

            var partes = ultimaPalestra.Horario.Replace("H", "").Split(':');
            var hora = int.Parse(partes[0]);
            var minuto = int.Parse(partes[1]);

            minuto += ultimaPalestra.DuracaoMinutos;
            while (minuto >= 60)
            {
                hora++;
                minuto -= 60;
            }

            var horaNetworking = Math.Max(hora, HORA_NETWORKING_MIN);
            var minutoNetworking = horaNetworking == hora ? minuto : 0;

            if (horaNetworking > HORA_NETWORKING_MAX)
            {
                horaNetworking = HORA_NETWORKING_MAX;
                minutoNetworking = 0;
            }

            return $"{horaNetworking:D2}:{minutoNetworking:D2}H";
        }

        // PARSERs
        private static PalestraEntradaDto ParseLinha(string linha)
        {
            if (string.IsNullOrWhiteSpace(linha))
                throw new ArgumentException("Linha vazia");

            linha = linha.Trim();

            // Relâmpago
            if (linha.EndsWith("relâmpago", StringComparison.OrdinalIgnoreCase))
            {
                var nome = linha[..^"relâmpago".Length].Trim();
                if (string.IsNullOrWhiteSpace(nome))
                    throw new ArgumentException($"Nome inválido: {linha}");

                return new PalestraEntradaDto { Nome = nome, Tempo = 5, IsRelampago = true };
            }

            // Padrão "XXmin"
            var partes = linha.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length < 2)
                throw new ArgumentException($"Formato inválido: {linha}");

            var ultima = partes[^1];
            if (!ultima.EndsWith("min", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Esperado 'XXmin' ou 'relâmpago'. Recebido: {ultima}");

            if (!int.TryParse(ultima[..^"min".Length], out var tempo) || tempo <= 0)
                throw new ArgumentException($"Duração inválida: {ultima}");

            if (tempo > DURACAO_MAXIMA)
                throw new ArgumentException($"Duração máxima por palestra é {DURACAO_MAXIMA}min. Recebido: {tempo}min");

            return new PalestraEntradaDto
            {
                Nome = string.Join(" ", partes[..^1]),
                Tempo = tempo,
                IsRelampago = false
            };
        }

        private static List<PalestraEntradaDto> ParseLinhas(List<string> linhas)
        {
            if (linhas == null || linhas.Count == 0)
                throw new ArgumentException("Lista vazia");

            var resultado = new List<PalestraEntradaDto>();
            var erros = new List<string>();

            for (int i = 0;i < linhas.Count;i++)
            {
                try
                { resultado.Add(ParseLinha(linhas[i])); }
                catch (Exception ex) { erros.Add($"Linha {i + 1}: {ex.Message}"); }
            }

            if (erros.Count > 0)
                throw new ArgumentException(string.Join("\n", erros));

            return resultado;
        }

        public static List<PalestraEntradaDto> ParseTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                throw new ArgumentException("Texto vazio");

            var linhas = texto
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

            return ParseLinhas(linhas);
        }

        public static PalestraDto ConverterParaPalestra(PalestraEntradaDto entrada) =>
            new(entrada.Nome, entrada.IsRelampago ? 5 : entrada.Tempo);
    }
}

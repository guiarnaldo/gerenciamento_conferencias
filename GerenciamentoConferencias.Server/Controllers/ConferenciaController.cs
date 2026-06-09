using GerenciamentoConferencias.Server.Dto;
using GerenciamentoConferencias.Server.Models;
using GerenciamentoConferencias.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoConferencias.Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    // Limita todas as requests a 250kb
    [RequestSizeLimit(250_000)]
    public class ConferenciaController(OrganizarConferenciaService organizador): ControllerBase
    {
        private readonly OrganizarConferenciaService _organizador = organizador;

        /// <summary>
        /// Organiza palestras a partir de uma string
        /// </summary>
        [HttpPost("organizar")]
        public IActionResult ParseEOrganizar([FromBody] ParserRequestModel request)
        {
            if (request.Texto == string.Empty)
                return BadRequest(new { sucesso = false, erro = "Texto de entrada vazio!" });

            try
            {
                var entradas = OrganizarConferenciaService.ParseTexto(request.Texto);

                // Converte e organiza usando o serviço existente
                var palestras = entradas
                .Select(OrganizarConferenciaService.ConverterParaPalestra)
                .ToList();

                var conferencia = _organizador.Organizar(palestras);
                var saida = _organizador.FormatarSaida(conferencia);

                return Ok(new ConferenciaResponseModel
                {
                    Sucesso = true,
                    FormatoTexto = saida,
                    Dados = conferencia
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, erro = ex.Message });
            }
        }

        /// <summary>
        /// Organiza palestras a partir de objetos estruturados com Nome, Tempo e IsRelampago
        /// </summary>
        [HttpPost("organizar-json")]
        public IActionResult Organizar([FromBody] ConferenciaRequestModel request)
        {
            try
            {
                if (request.Palestras == null || request.Palestras.Count == 0)
                    return BadRequest(new ConferenciaResponseModel
                    {
                        Sucesso = false,
                        Erro = "Lista de palestras não pode ser vazia"
                    });

                // Converte entradas para modelo interno
                var palestras = request.Palestras
                .Select(OrganizarConferenciaService.ConverterEntrada)
                .ToList();

                var conferencia = _organizador.Organizar(palestras);
                var saida = _organizador.FormatarSaida(conferencia);

                return Ok(new ConferenciaResponseModel
                {
                    Sucesso = true,
                    FormatoTexto = saida,
                    Dados = conferencia
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ConferenciaResponseModel
                {
                    Sucesso = false,
                    Erro = ex.Message
                });
            }
        }

        [HttpGet("exemplo")]
        public IActionResult ObterExemplo()
        {
            var exemplo = new ConferenciaRequestModel
            {
                Palestras =
                [
                    new() { Nome = "Writing Fast Tests Against Enterprise .Net", Tempo = 60, IsRelampago = false },
                    new() { Nome = "Overdoing it in Python", Tempo = 45, IsRelampago = false },
                    new() { Nome = "Lua for the Masses", Tempo = 30, IsRelampago = false },
                    new() { Nome = ".Net Errors from Mismatched Nuget Versions", Tempo = 45, IsRelampago = false },
                    new() { Nome = "Common .Net Errors", Tempo = 45, IsRelampago = false },
                    new() { Nome = "Python for .Net Developers", Tempo = 0, IsRelampago = true },
                    new() { Nome = "Communicating Over Distance", Tempo = 60, IsRelampago = false },
                    new() { Nome = "Accounting-Driven Development", Tempo = 45, IsRelampago = false },
                    new() { Nome = "Woah", Tempo = 30, IsRelampago = false },
                    new() { Nome = "Sit Down and Write", Tempo = 30, IsRelampago = false },
                    new() { Nome = "Pair Programming vs Noise", Tempo = 45, IsRelampago = false },
                    new() { Nome = ".Net Magic", Tempo = 60, IsRelampago = false },
                    new() { Nome = ".Net Core: Why We Should Move On", Tempo = 60, IsRelampago = false },
                    new() { Nome = "Clojure Ate Scala (on my project)", Tempo = 45, IsRelampago = false },
                    new() { Nome = "Programming in the Boondocks of Seattle", Tempo = 30, IsRelampago = false },
                    new() { Nome = ".Net vs. Clojure for Back-End Development", Tempo = 30, IsRelampago = false },
                    new() { Nome = ".Net Core Legacy App Maintenance", Tempo = 60, IsRelampago = false },
                    new() { Nome = "A World Without HackerNews", Tempo = 30, IsRelampago = false },
                    new() { Nome = "User Interface CSS in .Net Apps", Tempo = 30, IsRelampago = false }
                ]
            };

            return Ok(exemplo);
        }

        [HttpPost("organizar-exemplo")]
        public IActionResult OrganizarExemplo()
        {
            try
            {
                var palestras = new List<PalestraEntradaDto> {
                    new() { Nome = "Writing Fast Tests Against Enterprise .Net", Tempo = 60, IsRelampago = false },
                    new() { Nome = "Overdoing it in Python", Tempo = 45, IsRelampago = false },
                    new() { Nome = "Lua for the Masses", Tempo = 30, IsRelampago = false },
                    new() { Nome = ".Net Errors from Mismatched Nuget Versions", Tempo = 45, IsRelampago = false },
                    new() { Nome = "Common .Net Errors", Tempo = 45, IsRelampago = false },
                    new() { Nome = "Python for .Net Developers", Tempo = 0, IsRelampago = true },
                    new() { Nome = "Communicating Over Distance", Tempo = 60, IsRelampago = false },
                    new() { Nome = "Accounting-Driven Development", Tempo = 45, IsRelampago = false },
                    new() { Nome = "Woah", Tempo = 30, IsRelampago = false },
                    new() { Nome = "Sit Down and Write", Tempo = 30, IsRelampago = false },
                    new() { Nome = "Pair Programming vs Noise", Tempo = 45, IsRelampago = false },
                    new() { Nome = ".Net Magic", Tempo = 60, IsRelampago = false },
                    new() { Nome = ".Net Core: Why We Should Move On", Tempo = 60, IsRelampago = false },
                    new() { Nome = "Clojure Ate Scala (on my project)", Tempo = 45, IsRelampago = false },
                    new() { Nome = "Programming in the Boondocks of Seattle", Tempo = 30, IsRelampago = false },
                    new() { Nome = ".Net vs. Clojure for Back-End Development", Tempo = 30, IsRelampago = false },
                    new() { Nome = ".Net Core Legacy App Maintenance", Tempo = 60, IsRelampago = false },
                    new() { Nome = "A World Without HackerNews", Tempo = 30, IsRelampago = false },
                    new() { Nome = "User Interface CSS in .Net Apps", Tempo = 30, IsRelampago = false }
            }
            .Select(OrganizarConferenciaService.ConverterEntrada)
            .ToList();

                var conferencia = _organizador.Organizar(palestras);
                var saida = _organizador.FormatarSaida(conferencia);

                return Ok(new ConferenciaResponseModel
                {
                    Sucesso = true,
                    FormatoTexto = saida,
                    Dados = conferencia
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ConferenciaResponseModel
                {
                    Sucesso = false,
                    Erro = ex.Message
                });
            }
        }
    }
}
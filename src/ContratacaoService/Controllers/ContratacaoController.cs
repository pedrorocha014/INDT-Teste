using Microsoft.AspNetCore.Mvc;
using MediatR;
using ContratacaoService.Dtos;
using Applications.UseCases.Contratacoes.Commands;

namespace ContratacaoService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContratacaoController(IMediator mediator, ILogger<ContratacaoController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<ContratacaoController> _logger = logger;

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            _logger.LogInformation("ContratacaoService está funcionando!");
            
            return Ok(new { message = "ContratacaoService está online!", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no ContratacaoService");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ContratarProposta([FromBody] ContratarPropostaRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando contratação da proposta {PropostaId}", request.PropostaId);

            var command = new ContratarPropostaCommand
            {
                PropostaId = request.PropostaId
            };

            await _mediator.Send(command);

            _logger.LogInformation("Proposta {PropostaId} contratada com sucesso", request.PropostaId);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao contratar proposta {PropostaId}", request.PropostaId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
} 
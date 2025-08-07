using Microsoft.AspNetCore.Mvc;
using MediatR;
using ContratacaoService.Dtos;
using Applications.UseCases.Contratacoes.Commands;
using Applications.UseCases.Contratacoes.Queries;
using Ardalis.Result;
namespace ContratacaoService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContratacaoController(IMediator mediator, ILogger<ContratacaoController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<ContratacaoController> _logger = logger;

    [HttpGet()]
    public async Task<IActionResult> List()
    {
        try
        {
            _logger.LogInformation("Iniciando listagem de contratos");

            var query = new ListContratosQuery();
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao listar contratos: {Error}", result.Errors.FirstOrDefault());
                return StatusCode(500, new { error = result.Errors.FirstOrDefault() });
            }

            var response = result.Value.Select(c => new ContratoResponse
            {
                Id = c.Id,
                CreatedAt = c.CreatedAt,
                PropostaId = c.PropostaId,
                PropostaSeguro = c.PropostaSeguro != null ? new PropostaResponse
                {
                    Id = c.PropostaSeguro.Id,
                    Name = c.PropostaSeguro.Name,
                    Cpf = c.PropostaSeguro.Cpf,
                    Status = c.PropostaSeguro.Status.ToString(),
                    CreatedAt = c.PropostaSeguro.CreatedAt,
                    UpdatedAt = c.PropostaSeguro.UpdatedAt
                } : null
            });

            _logger.LogInformation("Listagem de contratos concluída. Total: {TotalContratos}", response.Count());

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar contratos");
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

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao contratar proposta {PropostaId}: {Error}", request.PropostaId, result.Errors.FirstOrDefault());
                
                if (result.Status == ResultStatus.NotFound)
                {
                    return NotFound(new { error = result.Errors.FirstOrDefault() });
                }
                
                return BadRequest(new { error = result.Errors.FirstOrDefault() });
            }

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
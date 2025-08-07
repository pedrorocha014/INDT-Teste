using Microsoft.AspNetCore.Mvc;
using MediatR;
using Applications.UseCases.Propostas.Commands;
using Applications.UseCases.Propostas.Queries;
using PropostaService.Dtos;
using Core.PropostaAggregate.Enums;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PropostaService.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class PropostasController(IMediator mediator, ILogger<PropostasController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<PropostasController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropostaRequest request)
    {
        try
        {
            var command = new CreatePropostaCommand
            {
                Name = request.Name,
                Cpf = request.Cpf
            };

            var proposta = await _mediator.Send(command);

            _logger.LogInformation("Proposta criada com sucesso: {PropostaId}", proposta.Id);

            return CreatedAtAction(nameof(Create), new { id = proposta.Id }, proposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar proposta");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ListPropostasRequest request)
    {
        try
        {
            var query = new ListPropostasQuery
            {
                Name = request.Name,
                Cpf = request.Cpf,
                Status = !string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<StatusProposta>(request.Status, true, out var status) 
                    ? status 
                    : null
            };

            var propostas = await _mediator.Send(query);

            var response = propostas.Select(p => new PropostaResponse
            {
                Id = p.Id,
                Name = p.Name,
                Cpf = p.Cpf,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });

            _logger.LogInformation("Listadas {Count} propostas", response.Count());

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar propostas");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var query = new GetPropostaByIdQuery
            {
                Id = id
            };

            var proposta = await _mediator.Send(query);

            if (proposta == null)
            {
                _logger.LogWarning("Proposta com ID {PropostaId} não encontrada", id);
                return NotFound();
            }

            var response = new PropostaResponse
            {
                Id = proposta.Id,
                Name = proposta.Name,
                Cpf = proposta.Cpf,
                Status = proposta.Status,
                CreatedAt = proposta.CreatedAt,
                UpdatedAt = proposta.UpdatedAt
            };

            _logger.LogInformation("Proposta {PropostaId} encontrada", id);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar proposta {PropostaId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPatch("{propostaId}")]
    public async Task<IActionResult> Update(int propostaId, [FromBody] UpdatePropostaStatusRequest request)
    {
        try
        {
            var command = new UpdatePropostaCommand
            {
                PropostaId = propostaId,
                Status = !string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<StatusProposta>(request.Status, true, out var status)
                    ? status
                    : throw new BadHttpRequestException("Status inválido")
            };

            var proposta = await _mediator.Send(command);

            if (proposta == null)
                return NotFound();

            return NoContent();
        } 
        catch (BadHttpRequestException ex)
        {
            _logger.LogError(ex, "Status inválido");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar propostas");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}
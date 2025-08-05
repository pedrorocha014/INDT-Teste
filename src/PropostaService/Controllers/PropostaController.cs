using Microsoft.AspNetCore.Mvc;
using MediatR;
using Applications.UseCases.Propostas.Commands;
using PropostaService.Dtos;

namespace PropostaService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropostaController(IMediator mediator, ILogger<PropostaController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<PropostaController> _logger = logger;

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
}
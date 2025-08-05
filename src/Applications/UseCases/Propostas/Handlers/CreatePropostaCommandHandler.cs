using MediatR;
using Core.PropostaAggregate;
using Core.PropostaAggregate.Enums;
using Applications.UseCases.Propostas.Commands;
using Infrastructure.Context;

namespace Applications.UseCases.Propostas.Handlers;

public class CreatePropostaCommandHandler(PostgresDbContext postgresDbContext) : IRequestHandler<CreatePropostaCommand, PropostaSeguro>
{
    private readonly PostgresDbContext _postgresDbContext = postgresDbContext;

    public async Task<PropostaSeguro> Handle(CreatePropostaCommand request, CancellationToken cancellationToken)
    {
        var proposta = new PropostaSeguro
        {
            Name = request.Name,
            Cpf = request.Cpf,
            Status = StatusProposta.EmAnalise,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _postgresDbContext.PropostasSeguro.AddAsync(proposta, cancellationToken);
        await _postgresDbContext.SaveChangesAsync();

        return proposta;
    }
} 
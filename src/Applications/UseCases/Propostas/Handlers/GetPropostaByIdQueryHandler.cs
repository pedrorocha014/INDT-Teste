using Applications.UseCases.Propostas.Queries;
using Core.PropostaAggregate;
using Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.UseCases.Propostas.Handlers;

public class GetPropostaByIdQueryHandler : IRequestHandler<GetPropostaByIdQuery, PropostaSeguro?>
{
    private readonly PostgresDbContext _postgresDbContext;

    public GetPropostaByIdQueryHandler(PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
    }

    public async Task<PropostaSeguro?> Handle(GetPropostaByIdQuery request, CancellationToken cancellationToken)
    {
        return await _postgresDbContext.PropostasSeguro
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
    }
} 
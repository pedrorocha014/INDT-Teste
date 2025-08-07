using Applications.UseCases.Contratacoes.Queries;
using Core.PropostaAggregate;
using Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.UseCases.Contratacoes.Handlers;

public class ListContratosQueryHandler : IRequestHandler<ListContratosQuery, IEnumerable<Contratacao>>
{
    private readonly PostgresDbContext _postgresDbContext;

    public ListContratosQueryHandler(PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
    }

    public async Task<IEnumerable<Contratacao>> Handle(ListContratosQuery request, CancellationToken cancellationToken)
    {
        var query = _postgresDbContext.Contratacao
            .Include(c => c.PropostaSeguro)
            .AsQueryable();

        query = query.OrderByDescending(c => c.CreatedAt);

        return await query.ToListAsync(cancellationToken);
    }
} 
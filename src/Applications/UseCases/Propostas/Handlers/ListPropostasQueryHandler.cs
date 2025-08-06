using Applications.UseCases.Propostas.Queries;
using Core.PropostaAggregate;
using Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.UseCases.Propostas.Handlers;

public class ListPropostasQueryHandler : IRequestHandler<ListPropostasQuery, IEnumerable<PropostaSeguro>>
{
    private readonly PostgresDbContext _postgresDbContext;

    public ListPropostasQueryHandler(PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
    }

    public async Task<IEnumerable<PropostaSeguro>> Handle(ListPropostasQuery request, CancellationToken cancellationToken)
    {
        var query = _postgresDbContext.PropostasSeguro.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Cpf))
        {
            query = query.Where(p => p.Cpf == request.Cpf);
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(p => p.Name.Contains(request.Name));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status.Value);
        }

        query = query.OrderByDescending(p => p.CreatedAt);

        return await query.ToListAsync(cancellationToken);
    }
} 
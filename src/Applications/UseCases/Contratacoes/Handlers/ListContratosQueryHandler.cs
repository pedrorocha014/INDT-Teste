using Applications.UseCases.Contratacoes.Queries;
using Core.PropostaAggregate;
using Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ardalis.Result;

namespace Applications.UseCases.Contratacoes.Handlers;

public class ListContratosQueryHandler : IRequestHandler<ListContratosQuery, Result<IEnumerable<Contratacao>>>
{
    private readonly PostgresDbContext _postgresDbContext;

    public ListContratosQueryHandler(PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
    }

    public async Task<Result<IEnumerable<Contratacao>>> Handle(ListContratosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _postgresDbContext.Contratacao
                .Include(c => c.PropostaSeguro)
                .AsQueryable();

            query = query.OrderByDescending(c => c.CreatedAt);

            var contratos = await query.ToListAsync(cancellationToken);

            return Result<IEnumerable<Contratacao>>.Success(contratos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Contratacao>>.Error($"Erro ao listar contratos: {ex.Message}");
        }
    }
} 
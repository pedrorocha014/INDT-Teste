using Applications.UseCases.Contratacoes.Commands;
using Applications.UseCases.Propostas.Commands;
using Core.PropostaAggregate.Enums;
using Core.PropostaAggregate;
using Infrastructure.Context;
using MediatR;
namespace Applications.UseCases.Contratacoes.Handlers;

public class CreateContratoCommandHandler(PostgresDbContext postgresDbContext) : IRequestHandler<CreateContratoCommand, bool>
{
    private readonly PostgresDbContext _postgresDbContext = postgresDbContext;

    public async Task<bool> Handle(CreateContratoCommand request, CancellationToken cancellationToken)
    {
        var contratacao = new Contratacao
        {
            PropostaId = request.PropostaId,
            CreatedAt = request.CreatedAt
        };

        try
        {
            await _postgresDbContext.Contratacao.AddAsync(contratacao, cancellationToken);
            await _postgresDbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            var a = e;
            throw;
        }


        return true;
    }
}
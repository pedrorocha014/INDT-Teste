using Applications.UseCases.Contratacoes.Commands;
using Applications.UseCases.Propostas.Commands;
using Core.PropostaAggregate.Enums;
using Core.PropostaAggregate;
using Infrastructure.Context;
using MediatR;
using Ardalis.Result;
namespace Applications.UseCases.Contratacoes.Handlers;

public class CreateContratoCommandHandler(PostgresDbContext postgresDbContext) : IRequestHandler<CreateContratoCommand, Result>
{
    private readonly PostgresDbContext _postgresDbContext = postgresDbContext;

    public async Task<Result> Handle(CreateContratoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contratacao = new Contratacao
            {
                PropostaId = request.PropostaId,
                CreatedAt = request.CreatedAt
            };

            await _postgresDbContext.Contratacao.AddAsync(contratacao, cancellationToken);
            await _postgresDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error($"Erro ao criar contrato: {ex.Message}");
        }
    }
}
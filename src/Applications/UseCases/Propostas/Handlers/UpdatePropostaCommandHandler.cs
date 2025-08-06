using Applications.UseCases.Propostas.Commands;
using Core.PropostaAggregate;
using Infrastructure.Context;
using MediatR;

namespace Applications.UseCases.Propostas.Handlers;

public class UpdatePropostaCommandHandler : IRequestHandler<UpdatePropostaCommand, PropostaSeguro?>
{
    private readonly PostgresDbContext _postgresDbContext;

    public UpdatePropostaCommandHandler(PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
    }

    public async Task<PropostaSeguro?> Handle(UpdatePropostaCommand request, CancellationToken cancellationToken)
    {
        var proposta = await _postgresDbContext.PropostasSeguro.FindAsync(request.PropostaId);

        if (proposta == null)
            return null;

        proposta.Status = request.Status;

        await _postgresDbContext.SaveChangesAsync(cancellationToken);

        return proposta;
    }
}
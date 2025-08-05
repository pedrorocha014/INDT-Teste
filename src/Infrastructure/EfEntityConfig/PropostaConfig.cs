using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.PropostaAggregate;
using Infrastructure;

namespace Infrastructure.EfEntityConfig;

public class PropostaConfig : IEntityTypeConfiguration<PropostaSeguro>
{
    public void Configure(EntityTypeBuilder<PropostaSeguro> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(e => e.Cpf)
            .IsRequired()
            .HasMaxLength(14);
            
        builder.Property(e => e.Status)
            .IsRequired()
            .HasEnumType();
            
        builder.Property(e => e.CreatedAt)
            .IsRequired();
            
        builder.Property(e => e.UpdatedAt)
            .IsRequired();
    }
}
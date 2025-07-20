using Domain.Cadastros.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("clientes");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                   .HasColumnName("id");

            builder.OwnsOne(c => c.Cpf, cpf =>
            {
                cpf.Property(p => p.Valor)
                   .HasColumnName("cpf")
                   .IsRequired()
                   .HasMaxLength(11);
            });

            builder.OwnsOne(c => c.Nome, nome =>
            {
                nome.Property(p => p.Valor)
                    .HasColumnName("nome")
                    .IsRequired()
                    .HasMaxLength(200);
            });
        }
    }
}

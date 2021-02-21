using IntroducaoEFC.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntroducaoEFC.Data.Configurations
{
    class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");
            builder.HasKey(p => p.Id);
            //ABAIXO vemos a função GETDATE() da linguagem SQL sendo atribuida ao add o pedido para o compao IniciadoEm
            builder.Property(p => p.IniciadoEm).HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();
            builder.Property(p => p.StatusPedido).HasConversion<string>();
            builder.Property(p => p.TipoFrete).HasConversion<int>();
            builder.Property(p => p.Observacao).HasColumnType("VARCHAR(512)");

            builder.HasMany(p => p.Itens) //muitos itens
            .WithOne(p => p.Pedido) //para um pedido
            .OnDelete(DeleteBehavior.Cascade); //quando excluir um pedido serão excluidos os itens do pedido.
        }
    }
}

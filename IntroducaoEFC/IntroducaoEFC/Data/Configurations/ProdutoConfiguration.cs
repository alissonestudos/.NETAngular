using IntroducaoEFC.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntroducaoEFC.Data.Configurations
{
    class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.CodigoBarras).HasColumnType("VARCHAR(14)").IsRequired();
            builder.Property(p => p.Descricao).HasColumnType("VARCHAR(60)");
            builder.Property(p => p.Valor).IsRequired();
            builder.Property(p => p.TipoProduto).HasConversion<string>(); //o tipoProduto é um Enum... dessa forma salvamos na base de
                                                                    //dados como string (NOME), poderia ser inteiro (NUMERO) por ser enum.
        }
    }
}

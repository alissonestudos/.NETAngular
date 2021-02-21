using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using IntroducaoEFC.Data.Configurations;
using IntroducaoEFC.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IntroducaoEFC.Data
{
    public class ApplicationContext : DbContext
    {
        //criação da instância do looger que será usado para escrever no console
        private static readonly ILoggerFactory _logger = LoggerFactory.Create(p=>p.AddConsole());

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        //Aqui temos a primeira forma de especificar para nosso EFC nosso modelo de dados. Toda entidade que for colocada em uma propriedade
        //do tipo DbSet será automaticamente mapeada. E não será somente ela, serão todas as classes referenciadas dentro dela em um efeito cascata
        //dessa forma teriamos aqui a crição e mapeamento de Pedidos que foi iformado explicitamente, de Cliente e PedidoItem que são propriedades de Pedido
        //e de Produto que é uma propriedade de PedidoItem. Logo seriam criadas 4 tabelas.

        //public DbSet<Pedido> Pedidos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //se estivesse usando outro provider apareceria a opção do
            //provider no lugar de UseSqlServer.
            optionsBuilder
                .UseLoggerFactory(_logger) //informa o logger que será utilizado
                .EnableSensitiveDataLogging() //mostra os valores dos parâmetros
                .UseSqlServer("Data source=(localdb)\\mssqllocaldb;Initial Catalog=IntroducaoEFC;" +
                "Integrated Security=True",
                p=> p.EnableRetryOnFailure(
                    maxRetryCount: 2,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null).MigrationsHistoryTable("TabelaDeMigracao"));  
                                                                                                                                         
        }

        //como criamos os arquivos de configuração, aqui informamos para o EFC onde estão.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //asssim quando a aplicação for executada pela primeira vez o EFC irá procurar por todas as classes que implementam
            //a interface IEntityTypeConfiguration. A assembly é seu binário. No nosso caso nossa aplicação.
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            //assim teriamos que colocar classe por classe
            //modelBuilder.ApplyConfiguration(new ClienteConfiguration());
            //modelBuilder.ApplyConfiguration(new PedidoConfiguration());
            //modelBuilder.ApplyConfiguration(new ProdutoConfiguration());
            //modelBuilder.ApplyConfiguration(new PedidoItemConfiguration());
        }

        //Segunda forma de informar nosso modelo de dados é sobrescrevendo o método OnModelCreate. Também terá efeito cascata

        //toda essa parte do código foi transferida para /Data/Configuration onde se encontram os arquivos de configuração das entidades.
        //aqui foi criado apenas como exemplo.
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    //do parênteses para frente é a configuração atraves do Fluent API
        //    modelBuilder.Entity<Cliente>(p=>
        //    {
        //        p.ToTable("Clientes"); //nome da tabela
        //        p.HasKey(p => p.Id); //chave primária
        //        p.Property(p => p.Nome).HasColumnType("VARCHAR(80)").IsRequired();
        //        p.Property(p => p.Telefone).HasColumnType("CHAR(11)");
        //        p.Property(p => p.CEP).HasColumnType("CHAR(8)").IsRequired();
        //        p.Property(p => p.Estado).HasColumnType("VARCHAR(2)").IsRequired();
        //        p.Property(p => p.Cidade).HasMaxLength(60).IsRequired();

        //        p.HasIndex(i => i.Nome).HasName("idx_cliente_nome"); //criação de um index de procura dentro da tabela (opcional)
        //    });

        //    modelBuilder.Entity<Produto>(p =>
        //    {
        //        p.ToTable("Produtos");
        //        p.HasKey(p => p.Id);
        //        p.Property(p => p.CodigoBarras).HasColumnType("VARCHAR(14)").IsRequired();
        //        p.Property(p => p.Descricao).HasColumnType("VARCHAR(60)");
        //        p.Property(p => p.Valor).IsRequired();
        //        p.Property(p => p.TipoProduto).HasConversion<string>(); //o tipoProduto é um Enum... dessa forma salvamos na base de
        //                                                                //dados como string (NOME), poderia ser inteiro (NUMERO) por ser enum.
        //    });

        //    modelBuilder.Entity<Pedido>(p =>
        //    {
        //        p.ToTable("Pedidos");
        //        p.HasKey(p => p.Id);
        //        //ABAIXO vemos a função GETDATE() da linguagem SQL sendo atribuida ao add o pedido para o compao IniciadoEm
        //        p.Property(p => p.IniciadoEm).HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();
        //        p.Property(p => p.StatusPedido).HasConversion<string>();
        //        p.Property(p => p.TipoFrete).HasConversion<int>();
        //        p.Property(p => p.Observacao).HasColumnType("VARCHAR(512)");

        //        p.HasMany(p => p.Itens) //muitos itens
        //        .WithOne(p => p.Pedido) //para um pedido
        //        .OnDelete(DeleteBehavior.Cascade); //quando excluir um pedido serão excluidos os itens do pedido.
        //    });

        //    modelBuilder.Entity<PedidoItem>(p =>
        //    {
        //        p.ToTable("PedidoItens");
        //        p.HasKey(p => p.Id);
        //        p.Property(p => p.Quantidade).HasDefaultValue(1).IsRequired();
        //        p.Property(p => p.Valor).IsRequired();
        //        p.Property(p => p.Desconto).IsRequired();
        //    });
        //}

        //MAPEAR PROPRIEDADES ESQUECIDAS
        private void MapearPropriedadesEsquecidas(ModelBuilder modelBuilder)
        {
            //aqui buscamos todas as entidades do projeto
            foreach(var entity in modelBuilder.Model.GetEntityTypes())
            {
                //aqui buscamos todas as propriedades do tipo string (pode ser qq regra)
                var properties = entity.GetProperties().Where(p => p.ClrType == typeof(string));

                foreach (var property in properties)
                {
                    if(string.IsNullOrEmpty(property.GetColumnType())
                        && !property.GetMaxLength().HasValue)
                    {
                        property.SetMaxLength(100);//define tamanho do campo
                        property.SetColumnType("Varchar(100)");//define tipo string no BD
                    }
                }
            }
        }

    }

    
}

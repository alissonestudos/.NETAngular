using IntroducaoEFC.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntroducaoEFC
{
    class Program
    {
        public static int Contador = 1;
        static void Main(string[] args)
        {
            //a migração pode ser feita como abaixo... mas não é aconselhável
            //pq caso tenhamos várias aplicações rodando pode gerar problema
            //por causa da concorrência.
            //using var db = new Data.ApplicationContext();
            //db.Database.Migrate();

            //verificando migrações pendentes
            using var db = new Data.ApplicationContext();
            //var existe = db.Database.GetPendingMigrations().Any();

            //if (existe)
            //{
            //    //regras caso existem migrações: Interromper aplicação e
            //    //enviar aviso para executar migrações pendentes por exemplo.
            //}

            InsereDados();
            InsereDadosEmMassa();
            InsereDadosEmMassa2();
            //ConsultaDados();
            //CadastraPedido();
            //ConsultarPedidosCarregamentoAdiantado();
            //AtualizaDado();
            //RemoveRegistro();

            //while (Contador > 0)
            //{
            //    Contador++;
            //}

        }

        private static void InsereDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto 4",
                CodigoBarras = "12345",
                Valor = 10000,
                TipoProduto = ValueObjects.TipoProduto.MercadoriaRevenda,
                Ativo = true
            };
            //instanciando o contexto para manipular o banco.
            using var db = new Data.ApplicationContext();
            //pode ser feito de 4 formas, sendo as 2 primeiras aconselhadas e mais usadas
            //forma 1 - através da propriedade declarada no contexto "Produto"
            db.Produtos.Add(produto);
            //forma 2 - propriedade genérica, dessa forma poderia ser passado
            //qualquer entidade como parâmetro no lugar de Produto.
            //db.Set<Produto>().Add(produto);
            //forma 3 - forçando um rastreamento de objeto "produto" como adicionado
            //db.Entry(produto).State = EntityState.Added;
            //forma 4 - através da propriedade de contexto diretamente, o sistema terá
            //que realizar um discovery para descobri quais dados inserir, mas isso não
            //afeta tanto o desempenho
            //db.Add(produto);

            var registro = db.SaveChanges();
            Console.WriteLine("Total de registro: " + registro);

        }

        private static void InsereDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto Massa 2",
                CodigoBarras = "12345",
                Valor = 10000,
                TipoProduto = ValueObjects.TipoProduto.MercadoriaRevenda,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Alisson Massa",
                CEP = "948484",
                Cidade = "BH",
                Estado = "MG",
                Telefone = "99999999999"
            };

            using var db = new Data.ApplicationContext();
            db.AddRange(produto, cliente);
            var registros = db.SaveChanges();

            Console.WriteLine($"Total de registros: {registros}");

        }

        private static void InsereDadosEmMassa2()
        {
            var produto = new Produto []
            {
                new Produto
                {
                    Descricao = "Produto Massa 5",
                    CodigoBarras = "12345",
                    Valor = 10000,
                    TipoProduto = ValueObjects.TipoProduto.Embalagem,
                    Ativo = true
                },

                new Produto
                {
                    Descricao = "Produto Massa 6",
                    CodigoBarras = "12345",
                    Valor = 10000,
                    TipoProduto = ValueObjects.TipoProduto.Servico,
                    Ativo = true
                }

            };

            using var db = new Data.ApplicationContext();
            db.Set<Produto>().AddRange(produto);
            var registros = db.SaveChanges();

            Console.WriteLine($"Total de registros: {registros}");

        }

        //consultando produto
        private static void ConsultaDados()
        {
            using var db = new Data.ApplicationContext();
            //var consultaPorSintaxe = (from c in db.Clientes where c.Id > 0 select c).ToList();
            //var consultaPorMetodo = db.Clientes.AsNoTracking().Where(p => p.Id > 0).ToList();
            var consultaPorMetodo = db.Produtos.AsNoTracking().Where(p => p.Id > 0)
                .OrderBy(p => p.Id)
                .ToList();

            foreach (var produto in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando produto: {produto.Descricao}");
                //o Find() busca primeiro na memória, se usarmos o AsNoTracking() ele agirá
                //como os outros métodos de busca (FirstOrDefault() por exemplo). Busca o ID.
                db.Clientes.Find(produto.Id);
                //db.Clientes.FirstOrDefault(p => p.Id == cliente.Id);
            }
            Console.WriteLine($"objeto: {consultaPorMetodo.ToString()}");
        }

        private static void CadastraPedido()
        {
            using var db = new Data.ApplicationContext();
            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();
            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "pedido teste 4",
                StatusPedido = ValueObjects.StatusPedido.Entregue,
                TipoFrete = ValueObjects.TipoFrete.FOB,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 3,
                        Valor = produto.Valor,
                    },
                    new PedidoItem
                    {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 4,
                        Valor = produto.Valor,
                    } 
                }
            };
            db.Pedidos.Add(pedido);
            db.SaveChanges();
        }

        private static void ConsultarPedidosCarregamentoAdiantado()
        {
            var db = new Data.ApplicationContext();
            //dessa forma trazemos somente os pedidos sem os itens
            //no momento necessário teríamos que buscas os itens do pedido
            //var pedidos = db.Pedidos.ToList();
            
            //abaixo vemos com lambida, mas pode ser strint Include("Itens")
            var pedidos = db.Pedidos.Include(p => p.Itens)
                .ThenInclude(p=>p.Produto)//buscando o produto referente ao item
                .Include(p=>p.Cliente) //buscando o cliente do pedido
                .ToList();
            
            Console.WriteLine(pedidos);
        }

        private static void AtualizaDado()
        {
            using var db = new Data.ApplicationContext();
            //cliente Id 1 como exemplo.
            var cliente = db.Clientes.Find(3);//o Id vem do front
            //exemplo dos dados que vieram do front
            var clienteDoFront = new
            {
                Id = 3,
                Nome = "teste api",
                Telefone = "8888445"
            };
            //seta os dados que vieram do front
            db.Entry(cliente).CurrentValues.SetValues(clienteDoFront);

            //cliente.Nome = "Artur Miguel";
            //o método Update abaixo sobrescreve todas as propriedades
            //do objeto. Se usarmos apenas o SaveChanges() ele irá atualizar
            //apenas o que mudou. Tracking.
            //db.Clientes.Update(cliente);          
            db.SaveChanges();

        }

        private static void RemoveRegistro()
        {
            using var db = new Data.ApplicationContext();

            var pedido = db.Pedidos.Find(2);
            //opção 1
            db.Pedidos.Remove(pedido);
            //opção 2
            //db.Remove(cliente);
            //opção 3
            //db.Entry(cliente).State = EntityState.Deleted;

            //opção 4 - API
            //var clienteVeioDaAPI = new Cliente { Id = 2 };
            //db.Entry(cliente).State = EntityState.Deleted;

            db.SaveChanges();
        }

    }
}

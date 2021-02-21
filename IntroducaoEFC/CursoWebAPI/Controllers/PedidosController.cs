using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntroducaoEFC.Data;
using IntroducaoEFC.Domain;
using System.Text.Json;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CursoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PedidosController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        //{
        //    return await _context.Pedidos.ToListAsync();
        //}

        //GET: api/Pedidos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            var db = new IntroducaoEFC.Data.ApplicationContext();
            List<Pedido> pedidos = db.Pedidos.Include(p => p.Itens)
                .ThenInclude(p => p.Produto)//buscando o produto referente ao item
                .Include(p => p.Cliente) //buscando o cliente do pedido
                .ToList();

            //para que esse retorno fosse possível instalei o newtonsoft e adicionei o serviço na startup.cs
            //isso permite o retorno de objetos completos, o que não faz parte de boas práticas de desenvolvimento.
            return pedidos;
            //return JsonConvert.SerializeObject(pedidos);
        }

        // GET: api/Pedidos/5
        //método adaptado para retornar os subítens de pedido (cliente e produtos)
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var db = new IntroducaoEFC.Data.ApplicationContext();

            //Usando lista e tratando a lista
            //List<Pedido> pedidos = db.Pedidos.Include(p => p.Itens)
            //    .ThenInclude(p => p.Produto)//buscando o produto referente ao item
            //    .Include(p => p.Cliente) //buscando o cliente do pedido
            //    .ToList();
            //Pedido pedido = pedidos.Find(delegate(Pedido p) { return p.Id == id; });


            Pedido pedido1 = (Pedido)db.Pedidos.Include(p => p.Itens)
                .ThenInclude(p => p.Produto)//buscando o produto referente ao item
                .Include(p => p.Cliente).Where(p => p.Id==id).FirstOrDefault(); //buscando o cliente do pedido;

            

            if (pedido1 == null) return NotFound();

            
            return pedido1;
        }

        // PUT: api/Pedidos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return BadRequest();
            }

            _context.Entry(pedido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Pedidos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);
        }

        // DELETE: api/Pedidos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Pedido>> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return pedido;
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }
    }
}

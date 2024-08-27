using PodologiaJa.Data;
using PodologiaJa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.SqlServer.Server;
using System.Reflection.PortableExecutable;

namespace PodologiaJa.Controllers
{
    public class ClienteController : Controller
    {
        private readonly AulaContext _context;

        public ClienteController(AulaContext context)
        {
            _context = context;
        }

        //metodo pra BuscarCliente todos os Clientes e exibir numa View
        public async Task<IActionResult> BuscarCliente(int pagina = 1)
        {
            var QtdeTClientes = 2;
            var items = await _context.Clientes.ToListAsync();
            //var pagedItems=items.Skip((pagina -1) * QtdeTClientes)
            //    .Take(QtdeTClientes).ToList();
            // passando os dados e informacoes de paginacao para view
            ViewBag.QtdePaginas = (int)Math.Ceiling((double)items.Count() / QtdeTClientes);
            ViewBag.PaginaAtual = pagina;
            ViewBag.QtdeTClientes = QtdeTClientes;


            return View(items);
            // retorna a viewn(await _context.Clentes.TOlistAsync();
        }

        // metodo pra exibir detalhes de um cliente especifico
        public async Task<IActionResult> DetalhesClientes(int Id)
        {

            // retorna uma view com os detalhes do clientes encontrado pelo id
            return View(await _context.Clientes.FindAsync(Id));
        }
        // metodo para cadastro de clientes . pode ser usado para criar ou editar.
        public async Task<IActionResult> CadastroCliente(int? Id)
        {
            // se o id for nulo, retorna uma  view vazia para cadastro de um novo cliente
            if (Id == null)
            {
                return View();
            }
            //se o id nao for nulo, retorna uma view com os dados do cliente para ediçao
            else
            {
                return View(await _context.Clientes.FindAsync(Id));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CadastroCliente([Bind("Id,Nome_completo,Celular,Email,Data_Agedamento,Hora_Agendamento,Descricao")] Cliente cliente)
        {

            // verifica se o modelo e valido.
            if (ModelState.IsValid)
            {

                // se o id do cliente for diferente de zero,atualiza o cliente existente
                if (cliente.Id != 0)
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    TempData["msg"] = "2";
                }
                //se o id do cliente for zero,adiciona um novo cliente ao banco de dados
                else
                {
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();
                    TempData["msg"] = "1";


                }
                // redirediciona para o metodo  buscarClientes apos o cadastro ou atualizaçao
                return RedirectToAction("BuscarCliente");
            }
            // se o modelo nao for valido retorna a view com os dados do cliente para correçao
            return View(cliente);
        }
        // action pra deletar clientes
        public async Task<IActionResult> DeleterCliente(int? Id)
        {
            if (Id != 0)
            {
                var cliente = await _context.Clientes.FindAsync(Id);

                if (cliente != null)
                {
                    _context.Remove<Cliente>(cliente);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("BuscarCliente");
                }
            }
            return RedirectToAction("BuscarCliente");
        }
    }
}



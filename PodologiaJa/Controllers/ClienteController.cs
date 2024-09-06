﻿using PodologiaJa.Data;
using PodologiaJa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.SqlServer.Server;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Data;
using System.Globalization;

namespace PodologiaJa.Controllers
{
    public class ClienteController : Controller
    {
        private readonly AulaContext _context;


        public ClienteController(AulaContext context)
        {
            _context = context;
        }
        public class Formatar
        {
            //// Formatar celular
            public static string
                FormatarCelular(string celular)
            {
                return Regex.Replace(celular, @"(\d{2})(\d{5})(\d{4})", "($1) $2-$3");
            }
            // Validar email
            public static bool
               ValidarEmail(string email)
            {
                try
                {
                    var addr = new
                    System.Net.Mail.MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }
            // Formatar data
            public static string
                FormatarData(DateOnly data)

            {
                return
              data.ToString("dd/MM/yyyy",
              CultureInfo.InvariantCulture);

            }
            // Formatar hora
            public static string
                FormatarHora(TimeOnly hora)
            {
                return
               hora.ToString(@"hh\:mm");

            }


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

        public async Task<IActionResult> CadastroCliente([Bind("Id,Nome_completo,Celular,Email,Data_Agendamento,Hora_Agendamento,Descricao")] Cliente cliente)
        {
            // Formata os campos antes de validar
            cliente.Celular = Formatar.FormatarCelular(cliente.Celular).Trim();
            // o Trim() é usado para garantir que o número de celular não tenha espaços em branco extras antes ou depois do número.
            // Isso ajuda a evitar erros de validação e garante que o número esteja no formato correto.

            // Mensagem de depuração
            Console.WriteLine($"Número de celular formatado: {cliente.Celular}");

            // Valida o formato do celular
            if (!Regex.IsMatch(cliente.Celular, @"\(\d{2}\)\d{5}-\d{4}"))
            {
                ModelState.AddModelError("Celular", "O celular deve estar no formato (XX)XXXXX-XXXX");
            }

            // Verifica se o modelo é válido.
            if (ModelState.IsValid)
            {
                cliente.Data_Agendamento = DateOnly.ParseExact(Formatar.FormatarData(cliente.Data_Agendamento), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cliente.Hora_Agendamento = TimeOnly.ParseExact(Formatar.FormatarHora(cliente.Hora_Agendamento), @"hh\:mm", CultureInfo.InvariantCulture);

                // Se o id do cliente for diferente de zero, atualiza o cliente existente
                if (cliente.Id != 0)
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    TempData["msg"] = "2";
                }
                // Se o id do cliente for zero, adiciona um novo cliente ao banco de dados
                else
                {
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();
                    TempData["msg"] = "1";
                }

                // Redireciona para o método BuscarCliente após o cadastro ou atualização
                return RedirectToAction("BuscarCliente");
            }

            // Se o modelo não for válido, retorna a view com os dados do cliente para correção
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




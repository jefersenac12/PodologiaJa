using PodologiaJa.Data;
using PodologiaJa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            //try-cath Por que? Para capturar exceções inesperadas durante o processo de cadastro, como falhas no banco de dados. Isso evita que o código quebre inesperadamente
            //e exibe uma mensagem de erro amigável ao usuário
            try
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
                //horario de funcionamento : 9h as 18h
                var horarioAbertura = new TimeOnly(9, 0);
                var horarioFechamento = new TimeOnly(18, 0);

                if (cliente.Hora_Agendamento < horarioAbertura || cliente.Hora_Agendamento > horarioFechamento)
                {
                    ModelState.AddModelError("Hora_Agendamento", "os Agendamentos devem ser feitos entre 9h e 18h.");
                }
                // Verifica se o modelo é válido.
                if (ModelState.IsValid)
                {
                    //formata a data e hora de agendamento
                    cliente.Data_Agendamento = DateOnly.ParseExact(Formatar.FormatarData(cliente.Data_Agendamento), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    cliente.Hora_Agendamento = TimeOnly.ParseExact(Formatar.FormatarHora(cliente.Hora_Agendamento), @"hh\:mm", CultureInfo.InvariantCulture);


                    //verifica se o horario e data ja estao agendados por otro cliente
                    var AgendamentoExistente = await _context.Clientes.Where(c => c.Data_Agendamento == cliente.Data_Agendamento && c.Hora_Agendamento == cliente.Hora_Agendamento && c.Id != cliente.Id)
                        .AnyAsync();
                    //FirstOrDefaultAsync: Retorna o primeiro  objeto que coresponde a codiçao, ou null senenhum for encontrado.Util quando voce precisa manipular objeto.
                    //AnyAsync:verificar se existe pelo menos um item que corresponde a uma codiçao.Retorna um valor booleano (true ou false

                    if (AgendamentoExistente != null)
                    {
                        //se existir um agendamento exibe messagem de erro
                        ModelState.AddModelError("Hora_Agendamento", "Este horario ja esta agendado.Por favor,escolha outro horario.");
                        return View(cliente);

                    }

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
            //cath Por que? Para capturar exceções inesperadas durante o processo de cadastro, como falhas no banco de dados. Isso evita que o código quebre inesperadamente
            //e exibe uma mensagem de erro amigável ao usuário
            catch (Exception ex)
            {
                //loga o erro para monitoramento
                Console.WriteLine($"Erro ao cadastrar cliente:{ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocoreu um erro inesperado.Tente novamente.");
                return View(cliente);
            }
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




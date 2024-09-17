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
            var QtdeTClientes = 5;
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
        public async Task<IActionResult> AgendamentoCliente(int? Id)
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
        public async Task<IActionResult> AgendamentoCliente([Bind("Id,Nome_completo,Celular,Email,Data_Agendamento,Hora_Agendamento,Descricao")] Cliente cliente)
        {
            //try-cath Por que? Para capturar exceções inesperadas durante o processo de cadastro, como falhas no banco de dados. Isso evita que o código quebre inesperadamente
            //e exibe uma mensagem de erro amigável ao usuário
            try
            {
                // verificar se apenas um campo foi preenchido
                VerificandoCamposPreenchidos(cliente);
                // Formatar o número de celular antes de validar
                cliente.Celular = Formatar.FormatarCelular(cliente.Celular).Trim();

                // Exibe o número de celular formatado no console para fins de depuração
                Console.WriteLine($"Número de celular formatado: {cliente.Celular}");

                // Valida o formato do celular
                if (!Regex.IsMatch(cliente.Celular, @"\(\d{2}\)\d{5}-\d{4}"))
                {
                    ModelState.AddModelError("Celular", "O celular deve estar no formato (XX)XXXXX-XXXX");
                }

                // Horário de funcionamento: 9h às 18h
                var horarioAbertura = new TimeOnly(9, 0);
                var horarioFechamento = new TimeOnly(18, 0);

                // Verifica se o horário de agendamento está dentro do horário de funcionamento
                if (cliente.Hora_Agendamento < horarioAbertura || cliente.Hora_Agendamento > horarioFechamento)
                {
                    ModelState.AddModelError("Hora_Agendamento", "Os agendamentos devem ser feitos entre 9h e 18h.");
                }

                // Verifica se o modelo é válido
                if (ModelState.IsValid)
                {
                    // Formatar a data e hora de agendamento
                    cliente.Data_Agendamento = DateOnly.ParseExact(Formatar.FormatarData(cliente.Data_Agendamento), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    cliente.Hora_Agendamento = TimeOnly.ParseExact(Formatar.FormatarHora(cliente.Hora_Agendamento), @"hh\:mm", CultureInfo.InvariantCulture);

               
                        //Aqui esta  o intervalo de tempo atual de 30 minutos. para aumentar o intervalo 
                        var intervaloMin = cliente.Hora_Agendamento.AddMinutes(-30); // aumente para -60(1 hora) ou -120(2 horas)
                        var intervaloMax = cliente.Hora_Agendamento.AddMinutes(30); //aumente para 60 (1 hora) ou 120 (2 horas)
                      //verifica se ha agendamento conflitante dentro do intervalo
                        var AgendamentoConflito= await _context.Clientes.
                            Where(c => c.Data_Agendamento == cliente.Data_Agendamento
                            && c.Hora_Agendamento >= intervaloMin
                            && c.Hora_Agendamento <= intervaloMax
                             && c.Id != cliente.Id) //exclui o cliente  atual (em caso de edição)
                            .AnyAsync();
            
                    //// Verifica se o horário e data já estão agendados por outro cliente
                    //var AgendamentoExistente = await _context.Clientes
                    //    .Where(c => c.Data_Agendamento == cliente.Data_Agendamento && c.Hora_Agendamento == cliente.Hora_Agendamento && c.Id != cliente.Id)
                    //    .AnyAsync();
                    //FirstOrDefaultAsync: retorna o primeiro objeto que corresponde à codificação, ou null senenhum for encontrado.Util quando você precisa manipular o objeto.
                    //AnyAsync:verifica se existe pelo menos um item que corresponde a uma codificação.Retorna um valor booleano (true ou false

                    if (AgendamentoConflito)
                    {
                        ModelState.AddModelError("Hora_Agendamento", "Este horário já está ocupaado ou muito proximo de outro agendamento. Por favor, escolha  um horario com pelo menos  30 minutos de intervalo.");
                        return View(cliente);
                    }

                    // Se o cliente já existir, atualiza o cliente existente
                    if (cliente.Id != 0)
                    {
                        _context.Update(cliente);
                        await _context.SaveChangesAsync();
                        TempData["msg"] = "Cliente atualizado com sucesso!";
                    }
                    // Caso contrário, adiciona um novo cliente
                    else
                    {
                        _context.Add(cliente);
                        await _context.SaveChangesAsync();
                        TempData["msg"] = "Cliente cadastrado com sucesso!";
                    }

                    // Redireciona para a página de busca de clientes
                    return RedirectToAction("BuscarCliente");
                }

                // Se o modelo não for válido, retorna a view com os dados do cliente para correção
                return View(cliente);
            }
            //cath Por que? Para capturar exceções inesperadas durante o processo de cadastro, como falhas no banco de dados. Isso evita que o código quebre inesperadamente
            //e exibe uma mensagem de erro amigável ao usuário
            catch (Exception ex)
            {
                // Loga o erro para monitoramento
                Console.WriteLine($"Erro ao cadastrar cliente: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado. Tente novamente.");
                return View(cliente);
            }
        }

        private void VerificandoCamposPreenchidos(Cliente cliente)
        {
            int camposPreenchidos = 0;

            if
           (!string.IsNullOrEmpty(cliente.Nome_completo)) camposPreenchidos++;
            if
           (!string.IsNullOrEmpty(cliente.Celular)) camposPreenchidos++;
            if
           (!string.IsNullOrEmpty(cliente.Email)) camposPreenchidos++;
            if
           (cliente.Data_Agendamento!=default) camposPreenchidos++;
            if
           (cliente.Hora_Agendamento !=default) camposPreenchidos++;
            if
           (!string.IsNullOrEmpty(cliente.Descricao)) camposPreenchidos++;

            if(camposPreenchidos == 1)
            {
                ModelState.AddModelError("", "Por favor, preencha mais de um campo no formulario para que possamos processar seu Cadastro corretamente.");
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
        //metodo que verifica se existe conflito de ageendamento com intervalo de 30 minutos
        //private async Task<bool>ExisteConflitoDeAgendamento(Cliente cliente)
        //{
        //    //Aqui esta  o intervalo de tempo atual de 30 minutos. para aumentar o intervalo 
        //    var intervaloMin = cliente.Hora_Agendamento.AddMinutes(-30); // aumente para -60(1 hora) ou -120(2 horas)
        //    var intervaloMax = cliente.Hora_Agendamento.AddMinutes(30); //aumente para 60 (1 hora) ou 120 (2 horas)

        //    return await _context.Clientes.
        //        Where(c => c.Data_Agendamento == cliente.Data_Agendamento
        //        && c.Hora_Agendamento >= intervaloMin
        //        && c.Hora_Agendamento <= intervaloMax
        //         && c.Id != cliente.Id)
        //        .AnyAsync();
        //}
    }
}




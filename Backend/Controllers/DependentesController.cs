using Backend.Models;
using Backend.Util;
using Ferramentas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Controllers
{
    [ApiController]
    [Route("/dependentes")]
    [Produces("application/json")]
    public class DependentesController : Controller
    {
        private readonly MeuDbContext contexto;
        private readonly ILogger logger;

        /// <summary>
        /// Construtor da classe <see cref="UsuarioController"/>
        /// </summary>
        /// <param name="contexto">Contexto utlizado para comunicação com o banco de dados</param>
        /// <param name="logger">Parâmetro utilizado para gerenciar arquivo de logs</param>
        public DependentesController(MeuDbContext contexto, ILogger<DependentesController> logger)
        {
            this.contexto = contexto;
            this.logger = logger;
        }

        /// <summary>
        /// Listar todos dependentes disponíveis
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista de todos dependentes</response>
        /// <response code="401">Acesso não autorizado</response>
        [HttpGet]
        [AuthorizeByRole(new string[] { Constantes.Administrador, Constantes.Editor, Constantes.Leitor })]
        public IEnumerable<Usuarios> ListarDependentes()
        {
            this.logger.LogDebug("Listando todos dependentes:");
            List<Usuarios> usuarios = this.contexto.Usuarios.ToList();
            List<Usuarios> retorno = new List<Usuarios>();
            foreach (Usuarios usuario in usuarios)
            {
                List<string> dependentes = this.contexto.Usuarios
                    .Join(this.contexto.Dependentes, _ => usuario.CPF, d => d.TitularCPF, (_, d) => d.DependenteCPF).Distinct().ToList();

                if (dependentes.Count > 0)
                {
                    usuario.Dependentes = new List<Usuarios>();
                    foreach (string cpfDepentente in dependentes)
                    {
                        Usuarios dependente = usuarios.First(u => u.CPF.Equals(cpfDepentente));
                        usuario.Dependentes.Add(dependente);
                    }
                    retorno.Add(usuario);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Busca os dependentes de determinado usuário pelo CPF
        /// </summary>
        /// <param name="cpf">CPF do usuário titular</param>
        /// <returns></returns>
        /// <response code="200">Retorna a lista de todos dependentes do usuário</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet]
        [Route("buscar")]
        [AuthorizeByRole(new string[] { Constantes.Administrador, Constantes.Editor, Constantes.Leitor })]
        public IActionResult BuscarDependentes([FromQuery] string cpf)
        {
            bool valido = Checagem.CPF(cpf);
            if (valido)
            {
                Usuarios titular = this.contexto.Usuarios.FirstOrDefault(u => u.CPF.Equals(cpf));
                if (titular != null)
                {
                    List<Usuarios> lista = new List<Usuarios>();
                    List<Dependentes> dependentes = this.contexto.Dependentes.Where(d => d.TitularCPF.Equals(cpf)).ToList();
                    foreach (Dependentes dependente in dependentes)
                    {
                        Usuarios usuario = this.contexto.Usuarios.FirstOrDefault(u => u.CPF.Equals(dependente.DependenteCPF));
                        lista.Add(usuario);
                    }
                    return this.Ok(new { titular = cpf, dependentes = lista });
                }
                else
                {
                    return this.NotFound(new { mensagem = "Usuário não encontrado!" });
                }
            }
            return this.BadRequest(new { mensagem = "CPF Inválido" });
        }

        /// <summary>
        /// Adicionar novo usuário ao sistema
        /// </summary>
        /// <remarks>
        /// Exemplo da requisição:
        /// 
        ///     POST /dependentes/adicionar
        ///     {
        ///         "titular": "123.456.789-00",
        ///         "dependente": "987-654-321-00"
        ///     }
        /// </remarks>
        /// <param name="dependente">Dados da ligação de titular/dependente que será adicionada ao banco de dados</param>
        /// <returns>Adicionar uma nova ligação de titular/dependente ao banco de dados</returns>
        /// <response code="200">Retorna mensagem de sucesso</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        [HttpPost]
        [Route("adicionar")]
        [AuthorizeByRole(new string[] { Constantes.Administrador, Constantes.Editor })]
        public IActionResult AdicionarDependente([FromBody] Dependentes dependente)
        {
            string mensagem;
            try
            {
                bool valido = Checagem.CPF(dependente.TitularCPF) && Checagem.CPF(dependente.DependenteCPF);
                if (valido && dependente.TitularCPF != dependente.DependenteCPF)
                {
                    if (this.contexto.Usuarios.Any(t => t.CPF.Equals(dependente.TitularCPF)))
                    {
                        if (this.contexto.Usuarios.Any(d => d.CPF.Equals(dependente.DependenteCPF)))
                        {
                            this.contexto.Dependentes.Add(dependente);
                            this.contexto.SaveChanges();
                            this.logger.LogInformation("Dependente adicionado com sucesso!");
                            return this.Ok(dependente);
                        }
                        else
                        {
                            mensagem = "Dependente não encontrado";
                        }
                    }
                    else
                    {
                        mensagem = "Titular não encontrado";
                    }
                }
                else
                {
                    mensagem = "CPF(s) inválidos";
                }
                return this.BadRequest(new { mensagem });
            }
            catch (Exception ex)
            {
                mensagem = Logger.TrataMensagemExcecao(ex);
                Logger.GravarLog(mensagem);
                this.logger.LogError(mensagem);
                return this.BadRequest(new { mensagem });
            }
        }
    }
}

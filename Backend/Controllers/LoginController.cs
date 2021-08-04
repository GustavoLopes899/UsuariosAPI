using Backend.Models;
using Backend.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("/login")]
    [Produces("application/json")]
    public class LoginController : Controller
    {
        private readonly MeuDbContext contexto;
        private readonly IConfiguration config;
        private readonly ILogger logger;

        /// <summary>
        /// Construtor da classe <see cref="LoginController"/>
        /// </summary>
        /// <param name="contexto">Contexto utlizado para comunicação com o banco de dados</param>
        /// <param name="config">Configurações da API</param>
        /// <param name="logger">Parâmetro utilizado para gerenciar arquivo de logs</param>
        public LoginController(MeuDbContext contexto, IConfiguration config, ILogger<LoginController> logger)
        {
            this.contexto = contexto;
            this.config = config;
            this.logger = logger;
        }

        /// <summary>
        /// Método utilizado para autenticar o usuário no sistema
        /// </summary>
        /// <remarks>
        /// No body da requisição é necessário apenas passar o usuário e senha. Exemplo da requisição:
        ///
        ///     POST /login/autenticar
        ///     {
        ///        "usuario": "usuario",
        ///        "senha": "senha"
        ///     }
        /// </remarks>
        /// <param name="modelo">Dados do usuário utilizado para fazer a autenticação</param>
        /// <returns>Informações do usuário autenticado</returns>
        /// <response code="200">Retorna informações do usuário autenticado</response>
        /// <response code="404">Usuário não foi autenticado</response>
        [HttpPost]
        [Route("autenticar")]
        public IActionResult Autenticar([FromBody] Cadastradores modelo)
        {
            string chave = this.config["JWT:Secret"];
            Cadastradores autenticador = this.contexto.Cadastradores.Include(c => c.Permissao).FirstOrDefault(a => a.Usuario.Equals(modelo.Usuario) && a.Senha.Equals(modelo.Senha));
            if (autenticador == null)
            {
                string mensagemErro = "Usuário ou senha inválidos";
                this.logger.LogDebug(mensagemErro);
                return this.NotFound(new { mensagem = mensagemErro });
            }
            string token = Token.GerarToken(autenticador, chave);
            this.logger.LogInformation("Login efetuado com sucesso.");
            this.logger.LogDebug($"Token: {token}");
            return this.Ok(new { Token = token });
        }

        /// <summary>
        /// Verifica o status da autenticação
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna as informações de autenticação da sessão</response>
        [HttpGet]
        [Route("status")]
        [AllowAnonymous]
        public IActionResult Status()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                string nomePermissao = this.User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Role))?.Value;
                Permissoes permissao = this.contexto.Permissoes.FirstOrDefault(p => p.Nome.Equals(nomePermissao));
                return this.Ok(new { usuario = this.User.Identity.Name, permissao });
            }
            else
            {
                return this.Ok(new { usuario = Constantes.Anonimo, permissao = Constantes.SemPermissao });
            }
        }
    }
}

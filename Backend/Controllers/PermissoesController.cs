using Backend.Models;
using Backend.Util;
using Ferramentas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Controllers
{
    [ApiController]
    [Route("permissoes")]
    [Produces("application/json")]
    public class PermissoesController : Controller
    {
        private readonly MeuDbContext contexto;
        private readonly ILogger logger;

        /// <summary>
        /// Construtor da classe <see cref="PermissoesController"/>
        /// </summary>
        /// <param name="contexto">Contexto utlizado para comunicação com o banco de dados</param>
        /// <param name="logger">Parâmetro utilizado para gerenciar arquivo de logs</param>
        public PermissoesController(MeuDbContext contexto, ILogger<LoginController> logger)
        {
            this.contexto = contexto;
            this.logger = logger;
        }

        /// <summary>
        /// Listar todas permissões disponíveis
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista de todas permissões</response>
        /// <response code="401">Acesso não autorizado</response>
        [HttpGet]
        [AuthorizeByRole(new string[] { Constantes.Administrador })]
        public IEnumerable<Permissoes> ListarPermissoes()
        {
            this.logger.LogDebug("Listando todas permissões:");
            return this.contexto.Permissoes;
        }

        /// <summary>
        /// Adicionar nova permissão ao sistema
        /// </summary>
        /// <remarks>
        /// Exemplo da requisição:
        /// 
        ///     POST /permissoes/adicionar
        ///     {
        ///         "Nome": "permissao",
        ///         "Criar" : false,
        ///         "Ler" : true,
        ///         "Atualizar" : false,
        ///         "Deletar" : true,
        ///         "Administrador": false
        ///     }
        /// </remarks>
        /// <param name="permissao">Dados da permissão que será adicionada ao banco de dados</param>
        /// <returns>Adicionar uma nova permissão ao banco de dados</returns>
        /// <response code="200">Retorna mensagem de sucesso</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        [HttpPost]
        [Route("adicionar")]
        [AuthorizeByRole(new string[] { Constantes.Administrador })]
        public IActionResult AdicionarPermissao([FromBody] Permissoes permissao)
        {
            try
            {
                if (permissao.Nome != null)
                {
                    permissao.Nome = permissao.Nome.ToUpper();
                    permissao.DataCriacao = DateTime.Now;
                    permissao.DataAlteracao = DateTime.Now;
                    this.contexto.Permissoes.Add(permissao);
                    this.contexto.SaveChanges();
                    this.logger.LogInformation("Permissão adicionada com sucesso!");
                    return this.Ok(permissao);
                }
                return this.BadRequest(new { mensagem = "A permissão inserida é inválida!" });
            }
            catch (Exception ex)
            {
                string mensagem = Logger.TrataMensagemExcecao(ex);
                Logger.GravarLog(mensagem);
                this.logger.LogError(mensagem);
                return this.BadRequest(new { mensagem });
            }
        }

        /// <summary>
        /// Editar permissão existente no sistema
        /// </summary>
        /// <remarks>
        /// Exemplo da requisição:
        /// 
        ///     POST /permissoes/editar?nome=Antiga
        ///     {
        ///         "Nome": "Nova",
        ///         "Criar" : false,
        ///         "Ler" : true,
        ///         "Atualizar" : false,
        ///         "Deletar" : true,
        ///         "Administrador": false
        ///     }
        /// </remarks>
        /// <param name="nome">Nome da permissão que será editada</param>
        /// <param name="editada">Novas informações da permissão que será editada</param>
        /// <returns>Editar uma permissão do sistema</returns>
        /// <response code="200">Retorna mensagem de sucesso</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="404">Permissão não encontrada</response>
        [HttpPost]
        [Route("editar")]
        [AuthorizeByRole(new string[] { Constantes.Administrador })]
        public IActionResult EditarPermissao([FromQuery(Name = "nome")] string nome, [FromBody] Permissoes editada)
        {
            string mensagem;
            try
            {
                Permissoes permissao = this.contexto.Permissoes.FirstOrDefault(u => u.Nome.Equals(nome.ToUpper()));
                if (permissao != null)
                {
                    this.contexto.Entry(permissao).State = EntityState.Modified;
                    permissao.Nome = editada.Nome.ToUpper();
                    permissao.Criar = editada.Criar;
                    permissao.Ler = editada.Ler;
                    permissao.Atualizar = editada.Atualizar;
                    permissao.Deletar = editada.Deletar;
                    permissao.Administrador = editada.Administrador;
                    permissao.DataAlteracao = DateTime.Now;
                    this.contexto.SaveChanges();
                    mensagem = "Permissão atualizada com sucesso!";
                    this.logger.LogInformation(mensagem);
                    return this.Ok(new { mensagem });
                }
                mensagem = "Permissão não encontrada!";
                this.logger.LogWarning(mensagem);
                return this.NotFound(mensagem);
            }
            catch (Exception ex)
            {
                mensagem = Logger.TrataMensagemExcecao(ex);
                Logger.GravarLog(mensagem);
                this.logger.LogError(mensagem);
                return this.BadRequest(new { mensagem });
            }
        }

        /// <summary>
        /// Remover permissão existente no sistema
        /// </summary>
        /// <param name="nome">Nome da permissão para ser removida</param>
        /// <returns>Remover uma permissão do sistema</returns>
        /// <response code="200">Retorna mensagem de sucesso</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="404">Permissão não encontrada</response>
        [HttpDelete]
        [Route("deletar")]
        [AuthorizeByRole(new string[] { Constantes.Administrador })]
        public IActionResult RemoverPermissao([FromQuery(Name = "nome")] string nome)
        {
            string mensagem;
            try
            {
                Permissoes permissao = this.contexto.Permissoes.FirstOrDefault(u => u.Nome.Equals(nome.ToUpper()));
                if (permissao != null)
                {
                    this.contexto.Permissoes.Remove(permissao);
                    this.contexto.SaveChanges();
                    mensagem = "Permissão removida com sucesso!";
                    return this.Ok(new { mensagem });
                }
                mensagem = "Permissão não encontrada!";
                return this.NotFound(new { mensagem });
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

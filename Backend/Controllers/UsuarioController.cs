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
    [Route("usuario")]
    [Produces("application/json")]
    public class UsuarioController : Controller
    {
        private readonly MeuDbContext contexto;
        private readonly ILogger logger;

        /// <summary>
        /// Construtor da classe <see cref="UsuarioController"/>
        /// </summary>
        /// <param name="contexto">Contexto utlizado para comunicação com o banco de dados</param>
        /// <param name="logger">Parâmetro utilizado para gerenciar arquivo de logs</param>
        public UsuarioController(MeuDbContext contexto, ILogger<UsuarioController> logger)
        {
            this.contexto = contexto;
            this.logger = logger;
        }

        /// <summary>
        /// Listar todos usuários disponíveis
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista de todos usuários</response>
        /// <response code="401">Acesso não autorizado</response>
        [HttpGet]
        [AuthorizeByRole(new string[] { Constantes.Administrador, Constantes.Editor, Constantes.Leitor })]
        public IEnumerable<Usuarios> ListarUsuarios()
        {
            this.logger.LogDebug("Listando todos usuários:");
            return this.contexto.Usuarios;
        }

        /// <summary>
        /// Busca um usuário pelo CPF
        /// </summary>
        /// <param name="cpf">CPF do usuário que será buscado</param>
        /// <returns></returns>
        /// <response code="200">Retorna a lista de todos usuários</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet]
        [Route("buscar")]
        [AuthorizeByRole(new string[] { Constantes.Administrador, Constantes.Editor, Constantes.Leitor })]
        public IActionResult BuscarUsuario([FromQuery] string cpf)
        {
            Usuarios usuario = this.contexto.Usuarios.FirstOrDefault(u => u.CPF.Equals(cpf));
            if (usuario != null)
            {
                return this.Ok(usuario);
            }
            else
            {
                return this.NotFound("Usuário não encontrado!");
            }
        }

        /// <summary>
        /// Adicionar novo usuário ao sistema
        /// </summary>
        /// <remarks>
        /// Exemplo da requisição:
        /// 
        ///     POST /usuario/adicionar
        ///     {
        ///         "cpf": "123.456.789-00",
        ///         "nome": "Novo Usuário",
        ///         "ativo": true
        ///     }
        /// </remarks>
        /// <param name="usuario">Dados do usuário que será adicionado ao banco de dados</param>
        /// <returns>Adicionar um novo usuário ao banco de dados</returns>
        /// <response code="200">Retorna mensagem de sucesso</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        [HttpPost]
        [Route("adicionar")]
        [AuthorizeByRole(new string[] { Constantes.Administrador, Constantes.Editor })]
        public IActionResult AdicionarUsuario([FromBody] Usuarios usuario)
        {
            string mensagem;
            try
            {
                bool valido = Checagem.CPF(usuario.CPF);
                if (valido)
                {
                    usuario.Nome = usuario.Nome.ToUpper();
                    usuario.DataCriacao = DateTime.Now;
                    usuario.DataAlteracao = DateTime.Now;
                    this.contexto.Usuarios.Add(usuario);
                    this.contexto.SaveChanges();
                    this.logger.LogInformation("Usuário adicionado com sucesso!");
                    return this.Ok(usuario);
                }
                else
                {
                    mensagem = "CPF inválido";
                    this.logger.LogWarning(mensagem);
                    return this.BadRequest(new { mensagem });
                }
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
        /// Editar permissão existente no sistema
        /// </summary>
        /// <remarks>
        /// É importante ressaltar que não será possível editar o CPF do usuário, porém ele deve ser passado no body da requisição.
        /// 
        /// Não será possível editar também as datas de criação e alteração do usuário. Caso elas sejam passadas no
        /// body da requisição, os dados serão ignorados
        /// 
        /// Exemplo da requisição:
        /// 
        ///     POST /usuario/editar?cpf=111.111.111-11
        ///     {
        ///         "cpf": "111.111.111-11",
        ///         "nome": "usuario",
        ///         "ativo": false
        ///     }
        /// </remarks>
        /// <param name="cpf">CPF do usuário que será editado</param>
        /// <param name="editado">Novas informações do usuário que será editado</param>
        /// <returns>Editar um usuário do sistema</returns>
        /// <response code="200">Retorna mensagem de sucesso</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpPost]
        [Route("editar")]
        [AuthorizeByRole(new string[] { Constantes.Administrador, Constantes.Editor })]
        public IActionResult EditarUsuario([FromQuery(Name = "cpf")] string cpf, [FromBody] Usuarios editado)
        {
            string mensagem;
            try
            {
                Usuarios usuario = this.contexto.Usuarios.FirstOrDefault(u => u.CPF.Equals(cpf));
                if (usuario != null)
                {
                    if (editado.CPF != usuario.CPF)
                    {
                        mensagem = "Não é possível alterar o CPF do usuário!";
                        this.logger.LogWarning(mensagem);
                        return this.BadRequest(new { mensagem });
                    }
                    this.contexto.Entry(usuario).State = EntityState.Modified;
                    usuario.Nome = editado.Nome;
                    usuario.Ativo = editado.Ativo;
                    usuario.DataAlteracao = DateTime.Now;
                    this.contexto.SaveChanges();
                    mensagem = "Usuário atualizado com sucesso!";
                    this.logger.LogInformation(mensagem);
                    return this.Ok(new { mensagem });
                }
                mensagem = "Usuário não encontrado!";
                this.logger.LogWarning(mensagem);
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

        /// <summary>
        /// Remover usuário existente no sistema
        /// </summary>
        /// <param name="cpf">CPF do usuário que será removido</param>
        /// <returns>Remover um usuário do sistema</returns>
        /// <response code="200">Retorna mensagem de sucesso</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpDelete]
        [Route("remover")]
        [AuthorizeByRole(new string[] { Constantes.Administrador, Constantes.Editor })]
        public IActionResult RemoverUsuario([FromQuery(Name = "cpf")] string cpf)
        {
            string mensagem;
            try
            {
                Usuarios usuario = this.contexto.Usuarios.FirstOrDefault(u => u.CPF.Equals(cpf));
                if (usuario != null)
                {
                    this.contexto.Usuarios.Remove(usuario);
                    this.contexto.SaveChanges();
                    mensagem = "Usuário removido com sucesso!";
                    return this.Ok(new { mensagem });
                }
                mensagem = "Usuário não encontrado!";
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

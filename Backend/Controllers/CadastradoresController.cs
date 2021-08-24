using Backend.Models;
using Ferramentas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Controllers
{
    [ApiController]
    [Route("/cadastrador")]
    [Produces("application/json")]
    public class CadastradoresController : Controller
    {
        private readonly MeuDbContext contexto;
        private readonly ILogger logger;

        /// <summary>
        /// Construtor da classe <see cref="CadastradoresController"/>
        /// </summary>
        /// <param name="contexto">Contexto utlizado para comunicação com o banco de dados</param>
        /// <param name="logger">Parâmetro utilizado para gerenciar arquivo de logs</param>
        public CadastradoresController(MeuDbContext contexto, ILogger<DependentesController> logger)
        {
            this.contexto = contexto;
            this.logger = logger;
        }

        /// <summary>
        /// Listar todos usuários cadastradores
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista de todos usuários cadastradores</response>
        /// <response code="401">Acesso não autorizado</response>
        [HttpGet]
        [AllowAnonymous]
        [AuthorizeByRole(new string[] { Constantes.Administrador })]
        public IEnumerable<Cadastradores> ListarCadastradores()
        {
            this.logger.LogDebug("Listando todos usuários cadastradores:");
            List<Cadastradores> cadastradores = this.contexto.Cadastradores.Include(c => c.Permissao).ToList();
            cadastradores.ForEach(c => c.Senha = string.Empty);
            return cadastradores;
        }

        /// <summary>
        /// Busca o cadastrador pelo nome de usuário
        /// </summary>
        /// <param name="usuario">CPF do usuário titular</param>
        /// <returns></returns>
        /// <response code="200">Retorna o cadastrador buscado</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet]
        [AllowAnonymous]
        [Route("buscar")]
        [AuthorizeByRole(new string[] { Constantes.Administrador })]
        public IActionResult BuscarCadastrador([FromQuery] string usuario)
        {
            if (!string.IsNullOrEmpty(usuario))
            {
                Cadastradores cadastrador = this.contexto.Cadastradores.Include(c => c.Permissao).FirstOrDefault(c => c.Usuario.Equals(usuario));
                if (cadastrador != null)
                {
                    cadastrador.Senha = string.Empty;
                    return this.Ok(cadastrador);
                }
                else
                {
                    return this.NotFound(new { mensagem = "Cadastrador não encontrado!" });
                }
            }
            return this.BadRequest(new { mensagem = "Nenhum nome de usuário foi informado" });
        }

        /// <summary>
        /// Adicionar novo cadastrador ao sistema
        /// </summary>
        /// <remarks>
        /// Exemplo da requisição:
        /// 
        ///     POST /cadastrador/adicionar
        ///     {
        ///         "usuario": "Novo",
        ///         "senha": "NovaSenha",
        ///         "ativo": true,
        ///         "permissao": {
        ///             "nome": "Administrador"
        ///         }
        ///     }
        /// </remarks>
        /// <param name="cadastrador">Dados do novo cadastrador que será adicionado ao banco de dados</param>
        /// <returns>Adicionar um novo cadastrador ao banco de dados</returns>
        /// <response code="200">Retorna mensagem de sucesso</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("adicionar")]
        [AuthorizeByRole(new string[] { Constantes.Administrador })]
        public IActionResult AdicionarCadastrador([FromBody] Cadastradores cadastrador)
        {
            string mensagem;
            try
            {
                bool usuarioJaExiste = this.contexto.Cadastradores.Any(c => c.Usuario.Equals(cadastrador.Usuario));
                if (!usuarioJaExiste)
                {
                    if (cadastrador.Senha.Length >= Constantes.TamanhoMinimoSenha)
                    {
                        if (cadastrador.Permissao != null)
                        {
                            Permissoes permissao = this.contexto.Permissoes.FirstOrDefault(p => p.Nome.Equals(cadastrador.Permissao.Nome));
                            if (permissao != null)
                            {
                                cadastrador.Permissao = permissao;
                            }
                        }
                        cadastrador.Senha = MD5Hash.CalculaHash(cadastrador.Senha);
                        cadastrador.DataCriacao = DateTime.Now;
                        cadastrador.DataAlteracao = DateTime.Now;
                        this.contexto.Cadastradores.Add(cadastrador);
                        this.contexto.SaveChanges();
                        cadastrador.Senha = string.Empty;
                        return this.Ok(new { mensagem = "Novo usuário adicionado com sucesso!" });
                    }
                    else
                    {
                        mensagem = "A senha deve conter pelo menos 7 caracteres";
                    }
                }
                else
                {
                    mensagem = "O usuário informado já existe, tente novamente...";
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

        /// <summary>
        /// Editar cadastrador existente no sistema
        /// </summary>
        /// <remarks>
        /// É importante ressaltar que não será possível editar o nome de usuário do cadastrador, porém ele deve ser passado no body da requisição.
        /// 
        /// Não será possível editar também as datas de criação e alteração do cadastrador. Caso elas sejam passadas no
        /// body da requisição, os dados serão ignorados
        /// 
        /// Exemplo da requisição:
        /// 
        ///     POST /cadastrador/editar?usuario=Cadastrador
        ///     {
        ///         "senha": "NovaSenha1",
        ///         "ativo": false,
        ///         "permissao": {
        ///             "nome": "Administrador"
        ///         }
        ///     }
        /// </remarks>
        /// <param name="usuario">Nome de usuário do cadastrador que será editado</param>
        /// <param name="editado">Novas informações do cadastrador que será editado</param>
        /// <returns>Editar um cadastrador do sistema</returns>
        /// <response code="200">Retorna mensagem de sucesso</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="404">Cadastrador não encontrado</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("editar")]
        [AuthorizeByRole(new string[] { Constantes.Administrador })]
        public IActionResult EditarCadastrador([FromQuery(Name = "usuario")] string usuario, [FromBody] Cadastradores editado)
        {
            string mensagem;
            try
            {
                Cadastradores cadastrador = this.contexto.Cadastradores.FirstOrDefault(u => u.Usuario.Equals(usuario));
                if (cadastrador != null)
                {
                    this.contexto.Entry(cadastrador).State = EntityState.Modified;
                    cadastrador.Senha = MD5Hash.CalculaHash(editado.Senha);
                    cadastrador.Ativo = editado.Ativo;
                    if (editado.Permissao != null)
                    {
                        Permissoes permissao = this.contexto.Permissoes.FirstOrDefault(p => p.Nome.Equals(editado.Permissao.Nome));
                        if (permissao != null)
                        {
                            cadastrador.Permissao = permissao;
                            cadastrador.DataAlteracao = DateTime.Now;
                            this.contexto.SaveChanges();
                            mensagem = "Cadastrador atualizado com sucesso!";
                            this.logger.LogInformation(mensagem);
                            return this.Ok(new { mensagem });
                        }
                        else
                        {
                            mensagem = "Permissão inválida";
                        }
                    }
                    else
                    {
                        mensagem = "Permissão não foi informada corretamente";
                    }
                    this.logger.LogWarning(mensagem);
                    return this.BadRequest(new { mensagem });
                }
                else
                {
                    mensagem = "Usuário não encontrado!";
                }
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
        /// Remover cadastrador existente no sistema
        /// </summary>
        /// <param name="usuario">Nome de usuário do cadastrador que será removido</param>
        /// <returns>Remover um cadastrador do sistema</returns>
        /// <response code="200">Retorna mensagem de sucesso</response>
        /// <response code="400">Requisição não foi válida, verifique os logs</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="404">Cadastrador não encontrado</response>
        [HttpDelete]
        [AllowAnonymous]
        [Route("remover")]
        [AuthorizeByRole(new string[] { Constantes.Administrador })]
        public IActionResult RemoverCadastrador([FromQuery(Name = "usuario")] string usuario)
        {
            string mensagem;
            try
            {
                Cadastradores cadastrador = this.contexto.Cadastradores.FirstOrDefault(u => u.Usuario.Equals(usuario));
                if (cadastrador != null)
                {
                    this.contexto.Cadastradores.Remove(cadastrador);
                    this.contexto.SaveChanges();
                    mensagem = "Cadastrador removido com sucesso!";
                    return this.Ok(new { mensagem });
                }
                mensagem = "Cadastrador não encontrado!";
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

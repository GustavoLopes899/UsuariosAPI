using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("")]
    [ApiController]
    [Produces("application/json")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Página inicial da aplicação
        /// </summary>
        /// <returns></returns>
        /// /// <response code="200">Retorna a mensagem da página inicial da API</response>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            string status;
            if (this.User.Identity.IsAuthenticated)
            {
                status = $"Usuário autenticado, seja bem vindo {this.User.Identity.Name}";
            }
            else
            {
                status = $"Usuário não autenticado. É necessário autenticar-se para utilizar a API.";
            }
            return this.Ok(new { info = "Página inicial", status });
        }
    }
}

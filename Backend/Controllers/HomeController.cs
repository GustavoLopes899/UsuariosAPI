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
            return this.Ok("Página inicial");
        }
    }
}

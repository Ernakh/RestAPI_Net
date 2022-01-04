using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI_Net6.Data;
using RestAPI_Net6.Models;

namespace RestAPI_Net6.Controllers
{
    [Route("api")]
    [ApiController]
    public class PessoaController : ControllerBase
    {
        [HttpGet]
        [Route("pessoas")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] Contexto context)
        {
            var pessoas = await context
                .Pessoas
                .AsNoTracking()//Se você está apenas consultando um objeto no EF, ou seja, não vai modificar e gravar, use AsNoTracking() sempre que possível.
                               //Isto não quer dizer que você não possa instanciar outro contexto, mas evitar isto pode lhe dar um ganho de performance!
                .ToListAsync();

            return pessoas == null
                ? NotFound()
                : Ok(pessoas);
        }

        [HttpGet]
        [Route("pessoas/{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] Contexto context,
            [FromRoute] int id)
        {
            var pessoa = await context
                .Pessoas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == id);

            return pessoa == null
                ? NotFound()
                : Ok(pessoa);
        }

        [HttpPost("pessoas")]
        public async Task<IActionResult> PostAsync(
            [FromServices] Contexto context,
            [FromBody] Pessoa pessoa)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model inválida!");

            try
            {
                await context.Pessoas.AddAsync(pessoa);
                await context.SaveChangesAsync();
                return Created($"api/pessoas/{pessoa.id}", pessoa);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("pessoas/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] Contexto context,
            [FromBody] Pessoa pessoa,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var p = await context.Pessoas.FirstOrDefaultAsync(x => x.id == id);

            if (p == null)
                return NotFound();

            try
            {
                p.nome = pessoa.nome;

                context.Pessoas.Update(p);
                await context.SaveChangesAsync();
                return Ok(p);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("pessoas/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] Contexto context,
            [FromRoute] int id)
        {
            var p = await context.Pessoas.FirstOrDefaultAsync(x => x.id == id);

            try
            {
                context.Pessoas.Remove(p);
                await context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

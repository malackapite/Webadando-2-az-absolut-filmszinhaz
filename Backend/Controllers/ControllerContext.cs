using Backend.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    public abstract class ControllerContext(IDbContext context) : ControllerBase()
    {
        protected readonly IDbContext context = context;

        protected async Task<ActionResult<T>> CheckIfModelStateIsValidAsync<T>(Func<Task<ActionResult<T>>> handleRequest) => ModelState.IsValid ? await handleRequest() : BadRequest(ModelState);

        protected async Task<ActionResult<T>> HandleDbUpdateException<T>(Func<Task<ActionResult<T>>> func)
        {
            try
            {
                return await func();
            }
            catch (DbUpdateException e)
            {
                return BadRequest(e.GetInnerMostExceptionMessage());
            }
        }
    }
}

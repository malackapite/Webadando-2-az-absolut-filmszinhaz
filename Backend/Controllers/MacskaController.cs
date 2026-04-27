using Backend.DAL;
using Backend.Models.Felhasznalo;
using Backend.Models.Macska;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Backend.Controllers
{
    [Route("macska"), Authorize(Policy = nameof(Felhasznalo.Engedely.TermekekKezelese))]
    public class MacskaController(IDbContext context) : TableController<int, Macska, MacskaDTO>(context)
    {
        protected override DbSet<Macska> DbSet => context.Macskak;

        [HttpGet("{id}")]
        public override async Task<ActionResult<MacskaDTO>> Get([FromRoute] int id) => await PerformGetAsync(id);

        [HttpPatch]
        public async Task<ActionResult<MacskaDTO>> Patch([FromBody] MacskaPatch macskaPatch) => await CheckIfModelStateIsValidAsync(async () => await PerformPatchAsync(macskaPatch, macskaPatch.Id));

        [HttpDelete("{id}")]
        public override async Task<ActionResult<MacskaDTO>> Delete([FromRoute] int id) => await PerformDeleteAsync(id);

        public class MacskaPatch : IPatchDTO<Macska>
        {
            [Required] public int Id { get; set; }

            [MaxLength(MacskaBase.NevMaxLength)] public string? Nev { get; set; }

            [MaxLength(MacskaBase.SzinMaxLength)] public string? Szin { get; set; }

            public byte? Kor { get; set; }

            public void Patch(Macska macska)
            {
                if (Nev is not null)
                {
                    macska.Nev = Nev;
                }
                if (Szin is not null)
                {
                    macska.Szin = Szin;
                }
                if (Kor.HasValue)
                {
                    macska.Kor = Kor.Value;
                }
            }
        }
    }
}

using Backend.DAL;
using Backend.Models.Felhasznalo;
using Backend.Models.Macska;
using Backend.Models.Rendeles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Route("rendeles"), Authorize(Policy = nameof(Felhasznalo.Engedely.RendelesekKeszitese))]
    public class RendelesController(IDbContext context) : ControllerContext(context)
    {
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RendelesAdatok>>> Get() => await PerformGetAll();

        [HttpGet("{id}"), AllowAnonymous]
        public async Task<ActionResult<RendelesAdatok>> Get([FromRoute] int id)
        {
            var rendelesAdatok = await context.Rendelesek
                .Where((Rendeles rendeles) => rendeles.Id == id)
                .Select(static (Rendeles rendeles) => new {
                    rendeles._Felhasznalo.Email,
                    rendeles.SzallitasiCim,
                    Vegosszeg = rendeles._RendeleshezTartozikok.Sum(static (RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik._Macska.Ar * rendeleshezTartozik.Mennyiseg),
                    rendeles.RendelesIdeje
                })
                .FirstOrDefaultAsync()
            ;
            return rendelesAdatok is null ? NotFound() : new RendelesAdatok {
                Id = id,
                Email = rendelesAdatok.Email,
                SzallitasiCim = rendelesAdatok.SzallitasiCim,
                Vegosszeg = rendelesAdatok.Vegosszeg,
                RendelesIdeje = rendelesAdatok.RendelesIdeje,
                RendeltMacskak = (await context.RendeleshezTartozikok
                    .Where((RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik.Rendeles == id)
                    .Select(static (RendeleshezTartozik rendeleshezTartozik) => new {
                        Macska = rendeleshezTartozik._Macska,
                        rendeleshezTartozik.Mennyiseg
                    })
                    .ToListAsync()
                )
                .ConvertAll(static macskakAdatok => new Item<MacskaDTO> {
                    Macska = macskakAdatok.Macska.Convert(),
                    Mennyiseg = macskakAdatok.Mennyiseg
                })
            };
        }

        [HttpPost]
        public async Task<ActionResult<RendelesAdatok>> Post([FromBody] RendelesData rendelesData)
            => await CheckIfModelStateIsValidAsync(async () => await HandleDbUpdateException<RendelesAdatok>(async () => {
                if (!int.TryParse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int felhasznaloId))
                {
                    return Unauthorized();
                }
                Rendeles rendeles = new Rendeles {
                    Felhasznalo = felhasznaloId,
                    SzallitasiCim = rendelesData.SzallitasiCim,
                    RendelesIdeje = DateTime.UtcNow,
                    _RendeleshezTartozikok = [.. rendelesData.RendeltMacskak
                        .Select(static (Item<int> item) => new RendeleshezTartozik {
                            Macska = item.Macska,
                            Mennyiseg = item.Mennyiseg
                        })
                    ]
                };
                context.Rendelesek.Add(rendeles);
                await context.SaveChangesAsync();
                await context.Entry(rendeles).ReloadAsync();
                var rendelesAdatok = await context.Rendelesek
                    .Where((Rendeles rendelesLocal) => rendelesLocal.Id == rendeles.Id)
                    .Select(static (Rendeles rendeles) => new {
                        rendeles._Felhasznalo.Email,
                        Vegosszeg = rendeles._RendeleshezTartozikok.Sum(static (RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik._Macska.Ar * rendeleshezTartozik.Mennyiseg)
                    })
                    .SingleAsync()
                ;
                return new RendelesAdatok {
                    Id = rendeles.Id,
                    Email = rendelesAdatok.Email,
                    SzallitasiCim = rendeles.SzallitasiCim,
                    Vegosszeg = rendelesAdatok.Vegosszeg,
                    RendelesIdeje = rendeles.RendelesIdeje,
                    RendeltMacskak = (await context.RendeleshezTartozikok
                        .Where((RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik.Rendeles == rendeles.Id)
                        .Select(static (RendeleshezTartozik rendeleshezTartozik) => new {
                            Macska = rendeleshezTartozik._Macska,
                            rendeleshezTartozik.Mennyiseg
                        })
                        .ToListAsync()
                    )
                    .ConvertAll(static macskakAdatok => new Item<MacskaDTO> {
                        Macska = macskakAdatok.Macska.Convert(),
                        Mennyiseg = macskakAdatok.Mennyiseg
                    })
                };
            }))
        ;

        [HttpDelete("{id}")]
        public async Task<ActionResult<RendelesAdatok>> Delete([FromRoute] int id) => await HandleDbUpdateException<RendelesAdatok>(async () => {
            Rendeles? rendeles = await context.Rendelesek.FindAsync(id);
            if (rendeles is null)
            {
                return NotFound();
            }
            var rendelesAdatok = await context.Rendelesek
                .Where((Rendeles rendeles) => rendeles.Id == id)
                .Select(static (Rendeles rendeles) => new {
                    rendeles._Felhasznalo.Email,
                    Vegosszeg = rendeles._RendeleshezTartozikok.Sum(static (RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik._Macska.Ar * rendeleshezTartozik.Mennyiseg),
                })
                .SingleAsync()
            ;
            List<Item<MacskaDTO>> rendeltMacskak = (await context.RendeleshezTartozikok
                .Where((RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik.Rendeles == rendeles.Id)
                .Select(static (RendeleshezTartozik rendeleshezTartozik) => new {
                    Macska = rendeleshezTartozik._Macska,
                    rendeleshezTartozik.Mennyiseg
                })
                .ToListAsync()
            )
            .ConvertAll(static macskakAdatok => new Item<MacskaDTO> {
                Macska = macskakAdatok.Macska.Convert(),
                Mennyiseg = macskakAdatok.Mennyiseg
            });
            context.Rendelesek.Remove(rendeles);
            await context.SaveChangesAsync();
            return new RendelesAdatok {
                Id = rendeles.Id,
                Email = rendelesAdatok.Email,
                SzallitasiCim = rendeles.SzallitasiCim,
                Vegosszeg = rendelesAdatok.Vegosszeg,
                RendelesIdeje = rendeles.RendelesIdeje,
                RendeltMacskak = rendeltMacskak
            };
        });

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<RendelesAdatok>>> Delete() => await HandleDbUpdateException<IEnumerable<RendelesAdatok>>(async () => {
            RendelesAdatok[] rendelesAdatokok = await PerformGetAll();
            await context.Rendelesek.ExecuteDeleteAsync();
            return rendelesAdatokok;
        });

        async Task<RendelesAdatok[]> PerformGetAll()
        {
            var rendelesAdatok = await context.Rendelesek
                .Select(static (Rendeles rendeles) => new {
                    rendeles.Id,
                    rendeles._Felhasznalo.Email,
                    rendeles.SzallitasiCim,
                    Vegosszeg = rendeles._RendeleshezTartozikok.Sum(static (RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik._Macska.Ar * rendeleshezTartozik.Mennyiseg),
                    rendeles.RendelesIdeje
                })
                .ToListAsync()
            ;
            RendelesAdatok[] rendelesAdatokok = new RendelesAdatok[rendelesAdatok.Count];
            for (int i = 0; i < rendelesAdatok.Count; i++)
            {
                var currentRendelesAdatok = rendelesAdatok[i];
                rendelesAdatokok[i] = new RendelesAdatok {
                    Id = currentRendelesAdatok.Id,
                    Email = currentRendelesAdatok.Email,
                    SzallitasiCim = currentRendelesAdatok.SzallitasiCim,
                    Vegosszeg = currentRendelesAdatok.Vegosszeg,
                    RendelesIdeje = currentRendelesAdatok.RendelesIdeje,
                    RendeltMacskak = (await context.RendeleshezTartozikok
                        .Where((RendeleshezTartozik rendeleshezTartozik) => rendeleshezTartozik.Rendeles == currentRendelesAdatok.Id)
                        .Select(static (RendeleshezTartozik rendeleshezTartozik) => new {
                            Macska = rendeleshezTartozik._Macska,
                            rendeleshezTartozik.Mennyiseg
                        })
                        .ToListAsync()
                    )
                    .ConvertAll(static macskaAdatok => new Item<MacskaDTO> {
                        Macska = macskaAdatok.Macska.Convert(),
                        Mennyiseg = macskaAdatok.Mennyiseg
                    })
                };
            }
            return rendelesAdatokok;
        }

        public class RendelesData
        {
#pragma warning disable CS8618
            [Required] public string SzallitasiCim { get; set; }

            [Required] public List<Item<int>> RendeltMacskak { get; set; }
#pragma warning restore CS8618
        }

        public class RendelesAdatok
        {
            public int Id { get; set; }

            public required string Email { get; set; }

            public required string SzallitasiCim { get; set; }

            public int Vegosszeg { get; set; }

            public DateTime RendelesIdeje { get; set; }

            public required List<Item<MacskaDTO>> RendeltMacskak { get; set; }
        }

        public class Item<T>
        {
            public required T Macska { get; set; }

            public int Mennyiseg { get; set; }
        }
    }
}

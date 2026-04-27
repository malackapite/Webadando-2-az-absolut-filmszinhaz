using Backend.DAL;
using Backend.Models.Felhasznalo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Controllers
{
    [Route("felhasznalo")]
    public class FelhasznaloController(IDbContext context, IConfiguration config) : TableController<int, Felhasznalo, FelhasznaloDTO>(context)
    {
        const char delimiter = ';';

        readonly IConfiguration config = config;

        protected override DbSet<Felhasznalo> DbSet => context.Felhasznalok;

        [HttpGet("engedelyek")]
        public ActionResult<IEnumerable<string>> Engedelyek() => Enum.GetNames<Felhasznalo.Engedely>();

        [HttpGet("{id}")]
        public override async Task<ActionResult<FelhasznaloDTO>> Get([FromRoute] int id)
        {
            Felhasznalo? felhasznalo = await DbSet
                .Select(static (Felhasznalo felhasznalo) => new Felhasznalo {
                    Id = felhasznalo.Id,
                    Email = felhasznalo.Email,
                    Jelszo = "",
                    Engedelyek = felhasznalo.Engedelyek
                })
                .FirstOrDefaultAsync((Felhasznalo felhasznalo) => felhasznalo.Id == id)
            ;
            return felhasznalo is not null ? felhasznalo.Convert() : NotFound();
        }

        public override async Task<ActionResult<IEnumerable<FelhasznaloDTO>>> Get() => await DbSet
            .Select(static (Felhasznalo felhasznalo) => new FelhasznaloDTO {
                Id = felhasznalo.Id,
                Email = felhasznalo.Email,
                Jelszo = "",
                Engedelyek = felhasznalo.Engedelyek.ToEngedelyekStringList()
            })
            .ToListAsync()
        ;

        public override async Task<ActionResult<FelhasznaloDTO>> Post([FromBody] FelhasznaloDTO felhasznaloDTO) => await CheckIfModelStateIsValidAsync(async () => {
            Felhasznalo felhasznalo = felhasznaloDTO.Convert();
            felhasznalo.Jelszo = EncryptPassword(felhasznalo.Jelszo);
            return await HandlePostAsync(felhasznalo);
        });

        [HttpPatch]
        public async Task<ActionResult<FelhasznaloDTO>> Patch([FromBody] FelhasznaloPatch felhasznaloPatch)
            => await CheckIfModelStateIsValidAsync(async () => await PerformPatchAsync(felhasznaloPatch, felhasznaloPatch.Id))
        ;

        [HttpDelete("{id}")]
        public override async Task<ActionResult<FelhasznaloDTO>> Delete([FromRoute] int id) => await PerformDeleteAsync(id);

        [HttpPost("login")]
        public async Task<ActionResult<TokenData>> Login([FromBody] LoginData loginData) => await CheckIfModelStateIsValidAsync<TokenData>(async () => {
            Felhasznalo? felhasznalo = await DbSet.FirstOrDefaultAsync((Felhasznalo felhasznalo) => felhasznalo.Email == loginData.Email);
            if (felhasznalo is null || !PasswordMatches(felhasznalo, loginData))
            {
                return BadRequest("Helytelen email, vagy jelszó.");
            }
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            DateTime expirationTime = DateTime.UtcNow.AddHours(1);
            return new TokenData {
                Token = tokenHandler.WriteToken(tokenHandler.CreateToken(new SecurityTokenDescriptor {
                    Subject = new ClaimsIdentity([
                        new Claim(ClaimTypes.NameIdentifier, felhasznalo.Id.ToString()),
                        .. Enum.GetValues<Felhasznalo.Engedely>()
                            .Where((Felhasznalo.Engedely engedely) => (felhasznalo.Engedelyek & (byte)engedely) != 0)
                            .Select(static (Felhasznalo.Engedely engedely) => new Claim(engedely.ToString(), "true"))
                    ]),
                    Expires = expirationTime,
                    Issuer = config["Jwt:Issuer"]!,
                    Audience = config["Jwt:Audience"]!,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["Jwt:Key"]!)), SecurityAlgorithms.HmacSha256Signature)
                })),
                LejaratiIdopont = expirationTime
            };
        });

        static bool PasswordMatches(Felhasznalo felhasznalo, LoginData loginData)
        {
            string[] elements = felhasznalo.Jelszo.Split(delimiter);
            return CryptographicOperations.FixedTimeEquals(Convert.FromBase64String(elements[1]), Pbkdf2(loginData.Password, Convert.FromBase64String(elements[0])));
        }

        static string EncryptPassword(string password)
        {
            const int saltSize = 1 << 4;
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            return string.Join(delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(Pbkdf2(password, salt)));
        }

        static byte[] Pbkdf2(string password, byte[] salt)
        {
            const int keySize = 1 << 5;
            const int iterations = 1 << 13;
            return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, keySize);
        }

        public class FelhasznaloPatch : IPatchDTO<Felhasznalo>
        {
            [Required] public int Id { get; set; }

            [EmailAddress] public string? Email { get; set; }

            [PersonalData] public string? Jelszo { get; set; }

            public List<string>? Engedelyek { get; set; }

            public void Patch(Felhasznalo felhasznalo)
            {
                if (Email is not null)
                {
                    felhasznalo.Email = Email;
                }
                if (Jelszo is not null)
                {
                    felhasznalo.Jelszo = EncryptPassword(Jelszo);
                }
                if (Engedelyek is not null)
                {
                    felhasznalo.Engedelyek = Engedelyek.ToEngedelyekBitmask();
                }
            }
        }

        public class LoginData
        {
            [Required] public required string Email { get; set; }
            [Required] public required string Password { get; set; }
        }

        public class TokenData
        {
            public required string Token { get; set; }
            public DateTime LejaratiIdopont { get; set; }
        }
    }
}

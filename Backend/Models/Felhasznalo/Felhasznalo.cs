using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Felhasznalo
{
    [Index(nameof(Email), IsUnique = true)]
    public class Felhasznalo : FelhasznaloBase<byte>, IConvertible<FelhasznaloDTO>
    {
        [Flags]
        public enum Engedely : byte
        {
            TermekekKezelese,
            RendelesekKeszitese
        }
#pragma warning disable CS8618
        [Key] public int Id { get; set; }

        [Required, PersonalData] public string Jelszo { get; set; }

        public List<Rendeles.Rendeles> _Rendelesek { get; set; }
#pragma warning restore CS8618
        public FelhasznaloDTO Convert() => new FelhasznaloDTO {
            Id = Id,
            Email = Email,
            Jelszo = Jelszo,
            Engedelyek = Engedelyek.ToEngedelyekStringList()
        };
    }
}

using Backend.Models.Rendeles;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Macska
{
    public class Macska : MacskaBase, IConvertible<MacskaDTO>
    {
#pragma warning disable CS8618
        [Key] public int Id { get; set; }

        public List<RendeleshezTartozik> _RendeleshezTartozikok { get; set; }
#pragma warning restore CS8618
        public MacskaDTO Convert() => new MacskaDTO {
            Id = Id,
            Nev = Nev,
            Szin = Szin,
            Kor = Kor,
            Ar = Ar
        };
    }
}

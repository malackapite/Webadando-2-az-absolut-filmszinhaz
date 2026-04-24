using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Macska
{
    public class Macska : MacskaBase, IConvertible<MacskaDTO>
    {
        [Key] public int Id { get; set; }

        public MacskaDTO Convert() => new MacskaDTO {
            Id = Id,
            Nev = Nev,
            Szin = Szin,
            Kor = Kor
        };
    }
}

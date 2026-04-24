using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Macska
{
    public abstract class MacskaBase
    {
        public const int NevMaxLength = 32;
        public const int SzinMaxLength = 16;

        [Required, MaxLength(NevMaxLength)] public required string Nev { get; set; }

        [Required, MaxLength(SzinMaxLength)] public required string Szin { get; set; }

        [Required] public byte Kor { get; set; }
    }
}

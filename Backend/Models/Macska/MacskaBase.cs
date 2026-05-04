using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Macska
{
    public abstract class MacskaBase
    {
        public const int NevMaxLength = 32;
        public const int SzinMaxLength = 16;
#pragma warning disable CS8618
        [Required, MaxLength(NevMaxLength)] public string Nev { get; set; }

        [Required, MaxLength(SzinMaxLength)] public string Szin { get; set; }

        [Required] public byte Kor { get; set; }

        [Required] public int Ar { get; set; }
#pragma warning restore CS8618
    }
}

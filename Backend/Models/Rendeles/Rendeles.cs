using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Rendeles
{
    public class Rendeles
    {
        public const int SzallitasiCimMaxLength = 128;
#pragma warning disable CS8618
        [Key] public int Id { get; set; }

        [Required] public int Felhasznalo { get; set; }

        [Required, MaxLength(SzallitasiCimMaxLength)] public string SzallitasiCim { get; set; }

        [Required] public DateTime RendelesIdeje { get; set; }

        [ForeignKey(nameof(Felhasznalo))] public Felhasznalo.Felhasznalo _Felhasznalo { get; set; }

        public List<RendeleshezTartozik> _RendeleshezTartozikok { get; set; }
#pragma warning restore CS8618
    }
}

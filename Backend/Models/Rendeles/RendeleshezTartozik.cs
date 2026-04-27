using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Rendeles
{
    [PrimaryKey(nameof(Rendeles), nameof(Macska))]
    public class RendeleshezTartozik
    {
#pragma warning disable CS8618
        public int Rendeles { get; set; }

        public int Macska { get; set; }

        [Required] public int Mennyiseg { get; set; }

        [ForeignKey(nameof(Rendeles))] public Rendeles _Rendeles { get; set; }

        [ForeignKey(nameof(Models.Macska.Macska))] public Macska.Macska _Macska { get; set; }
#pragma warning restore CS8618
    }
}

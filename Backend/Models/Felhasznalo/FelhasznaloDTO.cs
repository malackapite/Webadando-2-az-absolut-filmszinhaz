using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Felhasznalo
{
    public class FelhasznaloDTO : FelhasznaloBase<List<string>>, IConvertible<Felhasznalo>
    {
#pragma warning disable CS8618
        public int Id { get; set; }

        [Required] public string Jelszo { get; set; }
#pragma warning restore CS8618
        public Felhasznalo Convert() => new Felhasznalo {
            Id = Id,
            Email = Email,
            Jelszo = Jelszo,
            Engedelyek = Engedelyek.ToEngedelyekBitmask()
        };
    }
}

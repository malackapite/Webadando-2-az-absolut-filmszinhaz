using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Felhasznalo
{
    public abstract class FelhasznaloBase<TEngedelyek>
    {
#pragma warning disable CS8618
        [Required, EmailAddress] public string Email { get; set; }

        [Required] public TEngedelyek Engedelyek { get; set; }
#pragma warning restore CS8618
    }
}

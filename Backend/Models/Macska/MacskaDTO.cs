namespace Backend.Models.Macska
{
    public class MacskaDTO : MacskaBase, IConvertible<Macska>
    {
        public int Id { get; set; }

        public Macska Convert() => new Macska {
            Id = Id,
            Nev = Nev,
            Szin = Szin,
            Kor = Kor,
            Ar = Ar
        };
    }
}

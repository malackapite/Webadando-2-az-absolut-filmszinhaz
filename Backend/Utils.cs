using Backend.Models.Felhasznalo;

namespace Backend
{
    public static class Utils
    {
        public static string GetInnerMostExceptionMessage(this Exception exception) => exception.InnerException?.GetInnerMostExceptionMessageInternal() ?? exception.Message;

        static string GetInnerMostExceptionMessageInternal(this Exception innerException)
        {
            while (innerException.InnerException is not null)
            {
                innerException = innerException.InnerException;
            }
            return innerException.Message;
        }

        public static List<string> ToEngedelyekStringList(this byte engedelyekBitmask) => [.. Enum.GetValues<Felhasznalo.Engedely>()
            .Where((Felhasznalo.Engedely engedely) => (engedelyekBitmask & (byte)engedely) != 0)
            .Select(static (Felhasznalo.Engedely engedely) => engedely.ToString())
        ];

        public static byte ToEngedelyekBitmask(this List<string> engedelyek)
        {
            byte bitMask = 0;
            for (int i = 0; i < engedelyek.Count; i++)
            {
                if (Enum.TryParse(engedelyek[i], true, out Felhasznalo.Engedely engedely))
                {
                    bitMask |= (byte)engedely;
                }
            }
            return bitMask;
        }
    }
}

namespace Backend
{
    public interface IConvertible<out T>
    {
        T Convert();
    }
}

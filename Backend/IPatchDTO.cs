namespace Backend
{
    public interface IPatchDTO<in TRecord> where TRecord : class
    {
        void Patch(TRecord record);
    }
}

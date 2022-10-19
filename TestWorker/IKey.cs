namespace TestWorker
{
    public interface IKey<out T>
    {
        public T Key { get; }
    }
}

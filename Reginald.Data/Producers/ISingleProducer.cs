namespace Reginald.Data.Producers
{
    public interface ISingleProducer<T>
    {
        bool Check(string input);

        T Produce();
    }
}

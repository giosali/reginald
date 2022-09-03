namespace Reginald.Models.Producers
{
    public interface ISingleProducer<T>
    {
        bool Check(string input);

        T Produce();
    }
}

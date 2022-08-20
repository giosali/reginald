namespace Reginald.Data.Producers
{
    public interface IMultipleProducer<T>
    {
        bool Check(string input);

        T[] Produce();
    }
}

namespace Reginald.Models.Producers
{
    internal interface ISingleProducer<T>
    {
        bool Check(string input);

        T Produce();
    }
}

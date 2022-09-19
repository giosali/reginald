namespace Reginald.Models.Producers
{
    using System.Threading;

    internal interface IMultipleProducer<T>
    {
        bool Check(string input);

        T[] Produce(CancellationToken token = default);
    }
}

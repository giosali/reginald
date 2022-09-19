using System.Threading;

namespace Reginald.Models.Producers
{
    internal interface IMultipleProducer<T>
    {
        bool Check(string input);

        T[] Produce(CancellationToken token = default);
    }
}

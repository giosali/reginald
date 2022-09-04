﻿namespace Reginald.Models.Producers
{
    public interface IMultipleProducer<T>
    {
        bool Check(string input);

        T[] Produce();
    }
}
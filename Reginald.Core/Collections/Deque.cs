namespace Reginald.Core.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Deque<T>
    {
        public Deque(int maxLength)
        {
            List = new();
            if (maxLength < 0)
            {
                throw new ArgumentException($"{maxLength} is not non-negative", nameof(maxLength));
            }

            MaxLength = maxLength;
        }

        public Deque(IEnumerable<T> items)
        {
            List = new(items);
            MaxLength = -1;
        }

        public Deque(IEnumerable<T> items, int maxLength)
        {
            if (maxLength < 0)
            {
                throw new ArgumentException($"{maxLength} is not non-negative", nameof(maxLength));
            }

            List = new(items);
            MaxLength = maxLength;
        }

        public int MaxLength { get; private set; }

        public int Count => List.Count;

        private LinkedList<T> List { get; set; }

        public T this[int index] => List.ElementAt(index);

        public void Append(T item)
        {
            List.AddLast(item);
            if (List.Count > MaxLength)
            {
                List.RemoveFirst();
            }
        }

        public void AppendLeft(T item)
        {
            List.AddFirst(item);
            if (List.Count > MaxLength)
            {
                List.RemoveLast();
            }
        }

        public T Pop()
        {
            T returnValue = List.Last();
            List.RemoveLast();
            return returnValue;
        }

        public T PopLeft()
        {
            T returnValue = List.First();
            List.RemoveFirst();
            return returnValue;
        }
    }
}

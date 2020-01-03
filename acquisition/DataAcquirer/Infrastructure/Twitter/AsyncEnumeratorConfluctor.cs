using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Twitter
{
    public class AsyncEnumeratorConfluctor
    {
        private struct EnumeratorWrapper<T>
        {
            public bool HasMoved { get; }
            public int Hash { get; }
            public IAsyncEnumerator<T> Enumerator { get; }

            public EnumeratorWrapper(IAsyncEnumerator<T> enumerator, bool hasMoved)
            {
                HasMoved = hasMoved;
                Enumerator = enumerator;
                Hash = enumerator.GetHashCode();
            }
        }
        public static async IAsyncEnumerable<T> AggregateEnumerables<T>(
            IEnumerable<IAsyncEnumerable<T>> enumerables)
        {
            var taskDictionary = new Dictionary<int, Task<EnumeratorWrapper<T>>>();

            enumerables
                .Select(r => r.GetAsyncEnumerator())
                .Select(r => new EnumeratorWrapper<T>(r, true))
                .ToList()
                .ForEach(r =>
                    taskDictionary.Add(r.Hash, Task.Run(async () =>
                    {
                        var moved = await r.Enumerator.MoveNextAsync();
                        return new EnumeratorWrapper<T>(r.Enumerator, moved);
                    })));

            while (true)
            {
                if (taskDictionary.Count == 0)
                {
                    break;
                }
                var moveTasks = taskDictionary.Values;
                var first = await Task.WhenAny(moveTasks);

                var enumWrapper = first.Result;
                var enumerator = enumWrapper.Enumerator;
                var hash = enumerator.GetHashCode();
                if (!enumWrapper.HasMoved)
                {
                    taskDictionary.Remove(hash);
                    continue;
                }
                yield return enumerator.Current;

                var nextTask = Task.Run(async () =>
                {
                    var moved = await enumerator.MoveNextAsync();
                    return new EnumeratorWrapper<T>(enumerator, moved);
                });
                taskDictionary[hash] = nextTask;
            }
        }
    }
}

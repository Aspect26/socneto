using System.Collections.Concurrent;
using System.Threading;

public class EventQueue
{
    private readonly ConcurrentQueue<object> _eventQueue;

    public EventQueue()
    {
        _eventQueue = new ConcurrentQueue<object>();
    }

    public void Enqueue(object data)
    {
        _eventQueue.Enqueue(data);
    }

    public object Dequeue(CancellationToken cancellationToken)
    {
        if (_eventQueue.Count == 0)
        {
            return null;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            if (_eventQueue.TryDequeue(out object objEvent))
            {
                return objEvent;
            }
        }
        return null;

    }
}

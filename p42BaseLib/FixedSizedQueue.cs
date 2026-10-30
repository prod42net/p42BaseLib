namespace p42BaseLib;

/// <summary>
/// Represents a queue with a fixed maximum size. When the maximum size is exceeded,
/// the oldest items are automatically removed to make space for new items.
/// </summary>
/// <typeparam name="T">The type of the elements held in the queue.</typeparam>
public class FixedSizeQueue<T>(int maxSize)
{
    object _locker = new object();
    /// <summary>
    /// Represents the internal queue used to store elements in the FixedSizeQueue.
    /// Maintains a fixed maximum size specified at initialization.
    /// </summary>
    readonly Queue<T> _queue = new Queue<T>(maxSize);

     
    /// Adds an item to the queue. If the queue exceeds the predefined maximum size,
    /// the oldest item is removed to maintain the size limit.
    /// <param name="item">The item to be added to the queue.</param>
    public void Enqueue(T item)
    {
        lock (_locker)
        {
            _queue.Enqueue(item);
            while (_queue.Count > maxSize)
            {
               _queue.Dequeue();
            }
        }
    }

    /// <summary>
    /// Gets the number of elements currently contained in the queue.
    /// </summary>
    public long GetCount()
    {
        lock (_locker)
        {
            return _queue.Count;
        }
    }

    public T Dequeue()
    {
        lock (_locker)
        {
            return _queue.Dequeue();
        }
    }

}
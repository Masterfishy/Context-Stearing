using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SourceEvictingQueue extends a Queue data structure. The 
/// SourceEvictingQueue only allows one entry per source in the Queue.
/// It is possible to update a source entry by enqueuing.
/// </summary>
/// <typeparam name="S">They type for identifying a source</typeparam>
/// <typeparam name="T">The type of data to queue</typeparam>
public class SourceEvictingQueue<S, T>
{
    private Dictionary<int, S> m_sourceLookup;
    private Dictionary<S, Queue<T>> m_sourceQueues;
    private int m_evicitionCapacity;
    private int m_dequeueIndex;

    /// <summary>
    /// The total number of elements in each source queue.
    /// </summary>
    public int Count
    {
        get
        {
            int count = 0;

            foreach (Queue<T> q in m_sourceQueues.Values)
            {
                count += q.Count;
            }

            return count;
        }
    }

    /// <summary>
    /// Constructs a new SourceEvictingQueue.
    /// </summary>
    /// <param name="capacity">The maximum size of a source's queue</param>
    public SourceEvictingQueue(int capacity) {
        m_sourceLookup = new Dictionary<int, S>();
        m_sourceQueues = new Dictionary<S, Queue<T>>();

        m_evicitionCapacity = capacity;
        m_dequeueIndex = 0;
    }

    /// <summary>
    /// Enqueues an element to a given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="element"></param>
    public void Enqueue(S source, T element)
    {
        // If source already exists in source queue
        if (m_sourceQueues.TryGetValue(source, out Queue<T> elems))
        {
            // Remove the oldest entry if source queue is at capacity
            if (elems.Count >= m_evicitionCapacity)
            {
                elems.Dequeue();
            }

            elems.Enqueue(element);

            return;
        }

        // If the source needs to be added
        m_sourceLookup.Add(m_sourceQueues.Count, source);                       // Add the source to the lookup table

        m_sourceQueues[source] = new Queue<T>();                                // Create the queue for the source
        m_sourceQueues[source].Enqueue(element);
    }

    /// <summary>
    /// Removes and returns the oldest element in the queue.
    /// </summary>
    /// <returns>The oldest element in the queue</returns>
    public T Dequeue()
    {
        if (m_sourceLookup.Count <= 0)
        {
            throw new System.InvalidOperationException("Cannot dequeue from an empty source eviciting queue!");
        }

        Queue<T> queue;
        int searchAttempts = 0;

        do
        {
            // Get the source's queue at the dequeue index
            S source = m_sourceLookup[m_dequeueIndex];
            queue = m_sourceQueues[source];

            // Increment the dequeue index
            m_dequeueIndex = (m_dequeueIndex + 1) % m_sourceLookup.Keys.Count;

            // Throw an exception if we don't find a non-empty queue
            if (searchAttempts == m_sourceLookup.Count)
            {
                throw new System.InvalidOperationException("Cannot dequeue when all sources have empty queues!");
            }

            // Increment our search attempts
            searchAttempts++;

            // Keep searching until we find a non-empty queue
        } while (queue == null || queue.Count <= 0);

        return queue.Dequeue();
    }
}
